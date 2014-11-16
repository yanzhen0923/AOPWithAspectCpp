using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Controls.DataVisualization.Charting;

namespace DesignPattern
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region 局部变量
        TreeViewItem item;

        //读取调用比例和被调用比例文件列表
        List<string> beInvokedConditon = new List<string>();
        List<string> invokeConditon = new List<string>();

        //用来检查文件是否被更改
        bool isModified;

        //用来解析xml数据的对象
        XmlDataProvider nodes = new XmlDataProvider();

        //trie数树对象
        Trie trie;

        string dir;
        string editingFilePath = "";
        string tempWord = "";
        string filesToComplie = "";

        Process runProcess = new Process();
        StreamWriter runStreamWriter;

        CompletionWindow completionWindow;
        #endregion

        #region 窗体初始化
        /// <summary>
        /// 主窗体构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            isModified = false;

            //初始化trie树
            trie = Trie.getTrieInstance();
            trie.Bulid(txtCode.Text);

            //添加文本编辑框事件和高亮风格 并显示行号
            txtCode.TextArea.TextEntering += txtCode_TextArea_TextEntering;
            txtCode.TextArea.TextEntered += txtCode_TextArea_TextEntered;
            txtCode.TextChanged += txtCode_TextChanged;
            txtCode.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C++");
            txtCode.ShowLineNumbers = true;


            //工程文件目录树初始化
            item = new TreeViewItem();
            item.Header = "工程目录树";
            item.FontWeight = FontWeights.Normal;
            item.ContextMenu = projectTree.Resources["FolderContext"] as ContextMenu;

            //添加到树形显示控件
            projectTree.Items.Add(item);
        }

        /// <summary>
        /// 窗体加载后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            runProcess.StartInfo.FileName = "cmd.exe";
            runProcess.StartInfo.UseShellExecute = false;
            runProcess.StartInfo.RedirectStandardInput = true;
            runProcess.StartInfo.RedirectStandardOutput = true;
            runProcess.StartInfo.RedirectStandardError = true;
            runProcess.StartInfo.CreateNoWindow = true;
            runProcess.OutputDataReceived += (s, em) =>
            {
               Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                      {
                          debugRichTextBox.AppendText(em.Data + "\r\n");
                          
                      }));
            };
            runProcess.ErrorDataReceived += (s, em) =>
            {
               Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                      {
                          debugRichTextBox.AppendText(em.Data + "\r\n");
                          
                      }));
            };
            runProcess.Start();
            runProcess.BeginOutputReadLine();
            runStreamWriter = runProcess.StandardInput;//标准输入流



            //List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            //valueList.Add(new KeyValuePair<string, int>("Apple", 101));
            //valueList.Add(new KeyValuePair<string, int>("Banana", 201));
            //valueList.Add(new KeyValuePair<string, int>("Cake", 20));
            //valueList.Add(new KeyValuePair<string, int>("Others", 200));


            //InvokeChart.Title == 
            //((PieSeries)mcChart.Series[0]).ItemsSource = valueList.ToArray();

        }
        #endregion

        #region 通用函数
        /// <summary>
        /// 获取工程目录列表 将每个文件名以空格分隔添加到filesToCompile字符串
        /// </summary>
        /// <param name="Path">工程父文件夹</param>
        private void GetHierarchy(string Path)
        {
            try
            {
                string[] SubPath = Directory.GetFileSystemEntries((string)Path);

                foreach (string SinglePath in SubPath)
                {
                    string tmpName = SinglePath.Substring(dir.Length);
                    if (Directory.Exists(SinglePath))
                    {
                        //若是文件夹则递归查找下一层
                        GetHierarchy(SinglePath);
                    }
                    else
                    {
                        if (tmpName.EndsWith(".c") || tmpName.EndsWith(".cc") || tmpName.EndsWith(".cpp") || tmpName.EndsWith(".h"))
                        {
                            //添加到要编译的文件
                            if (!tmpName.EndsWith(".h"))
                                filesToComplie += (tmpName + " ");
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取工程目录列表 建立工程目录树
        /// </summary>
        /// <param name="Path">工程父文件夹</param>
        /// <param name="t">本层树节点</param>
        private void GetProjectTree(string Path, TreeViewItem t)
        {
            try
            {
                string[] SubPath = Directory.GetFileSystemEntries((string)Path);

                foreach (string SinglePath in SubPath)
                {
                    string tmpName = SinglePath.Substring(dir.Length);
                    if (Directory.Exists(SinglePath))
                    {
                        //添加文件夹节点并递归查找下一层
                        GetProjectTree(SinglePath, addTreeViewItem(t, tmpName, false));
                    }
                    else
                    {
                        if (tmpName.EndsWith(".c") || tmpName.EndsWith(".cc") || tmpName.EndsWith(".cpp") || tmpName.EndsWith(".h"))
                        {
                            //添加文件节点
                            addTreeViewItem(t, tmpName, true);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 查看目录是否存在 即是否已导入工程
        /// </summary>
        /// <returns></returns>
        bool DirExists()
        {
            if (Directory.Exists(dir))
                return true;
            return false;
        }

        /// <summary>
        /// 编译执行工程并输出监控结果
        /// </summary>
        void CompileAndRunAndMonitor()
        {
            filesToComplie = string.Empty;

            if (!DirExists())
            {
                MessageBox.Show("请先导入工程");
                return;
            }

            //获取目录结构
            GetHierarchy(dir);

            //删除缓存ah文件
            string[] filestToDelete = Directory.GetFiles(dir, "*.ah");
            foreach (string filePath in filestToDelete)
                File.Delete(filePath);

            //复制依赖文件
            File.WriteAllBytes(dir + Variables.cutTheTree, Properties.Resources.cutTheTree);
            File.WriteAllBytes(dir + Variables.coverage, Properties.Resources.coverage);
            File.WriteAllBytes(dir + Variables.mycoverage, Properties.Resources.mycoverage);
            File.WriteAllText(dir + Variables.clear, Properties.Resources.clear);

            //runStreamWriter.Write("cd /d " + dir + Environment.NewLine);
            //runStreamWriter.Write("ag++ -o main.exe " + filesToComplie + Environment.NewLine);
            //runStreamWriter.Write("main.exe" + Environment.NewLine);
            //runStreamWriter.Write("cutTheTree.exe" + Environment.NewLine);

            //runProcess.WaitForExit(3000);

            //编译
            Command.Execute("cmd.exe", "cd /d " + dir + " & "
                + "ag++ -o main.exe " + filesToComplie);

            //执行并输出xml
            Command.Execute("cmd.exe", "cd /d " + dir + " & "
                + "main.exe & cutTheTree.exe");

            try
            {
            //把xml转为调用关系树
            LoadXML(dir + Variables.res);

            
                //添加已使用的函数名列表
                string[] functions = File.ReadAllLines(dir + Variables.funcions);
                funtionsListBox.Items.Clear();
                foreach (string s in functions)
                {
                    funtionsListBox.Items.Add(s);
                }

                //添加未使用的函数名列表
                string[] uFunctions = File.ReadAllLines(dir + Variables.nouse);
                unusedListBox.Items.Clear();
                foreach (string s in uFunctions)
                {
                    unusedListBox.Items.Add(s);
                }

                //添加要查看的函数到listview里
                string[] functionAppeartimes = File.ReadAllLines(dir + Variables.appeartimes);
                funcGeneralListView.Items.Clear();
                foreach (string singleFunc in functionAppeartimes)
                {
                    string[] items = singleFunc.Split("$".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    funcGeneralListView.Items.Add(new FuncNameAndApprTm(items[0], items[1]));
                }

                invokeConditon = File.ReadAllLines(dir + Variables.invokedconditon).ToList();
                beInvokedConditon = File.ReadAllLines(dir + Variables.beinvokedconditon).ToList();

                string[] relies = File.ReadAllLines(dir + Variables.strongre);
                strongRelyListView.Items.Clear();
                foreach (string line in relies)
                {
                    string[] detail = line.Split("$".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    strongRelyListView.Items.Add(new StrongRely(detail[0], detail[1], detail[2], detail[3]));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //double a  = 100;

        }

        /// <summary>
        /// 直接执行编译好的函数
        /// </summary>
        void CompileAndRunWithOutMonitor()
        {
            filesToComplie = string.Empty;

            if (!DirExists())
            {
                MessageBox.Show("请先导入工程");
                return;
            }

            //获取目录结构
            GetHierarchy(dir);

            //编译
            Command.Execute("cmd.exe", "cd /d " + dir + " & "
                + "ag++ -o main.exe " + filesToComplie);

            //执行并输出xml
            Command.Execute("cmd.exe", "cd /d " + dir + " & "
                + "main.exe & pause");
        }
        #endregion

        #region 第一页
        /// <summary>
        /// 用于以xml文件为资源生成树状图的函数
        /// </summary>
        /// <param name="xmlPath">带完整路径的xml文件名</param>
        void LoadXML(string xmlPath)
        {
            try
            {
                //如果文件不存在则一直找
                while (!File.Exists(xmlPath))
                {
                    Thread.Sleep(100);
                }

                //获取xml数据 由于treeview中的资源名称为nodes 所以此处直接将此资源指向局部变量nodes
                nodes = FindResource("nodes") as XmlDataProvider;

                //用输入的xml文件内容填充xml文件对象
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);

                //把xml文件赋给资源nodes
                nodes.Document = xmlDocument;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 向xml文件框里拖文件前的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xmlPathTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            //若为文件格式
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// xml文件框完成拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xmlPathTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            // Get data object
            var dataObject = e.Data as DataObject;

            // Check for file list
            if (dataObject.ContainsFileDropList())
            {

                // Process file names
                StringCollection fileNames = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();
                foreach (var fileName in fileNames)
                {
                    bd.Append(fileName + "\n");
                }

                // Set text
                xmlPathTextBox.Text = bd.ToString();
            }
        }

        /// <summary>
        /// 拖放之后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xmlPathTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// xml文本框确认按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string tmp = xmlPathTextBox.Text;

            //若文件存在
            if (File.Exists(tmp))
            {
                ///载入xml文件并显示树状图
                LoadXML(tmp);
            }
        }

        /// <summary>
        /// 拖拽之后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectFolderTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// projecFolder文本框拖放后处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectFolderTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            // Get data object
            var dataObject = e.Data as DataObject;

            // Check for file list
            if (dataObject.ContainsFileDropList())
            {

                // Process file names
                StringCollection fileNames = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();
                foreach (var fileName in fileNames)
                {
                    bd.Append(fileName);
                }

                // Set text
                projectFolderTextBox.Text = bd.ToString() + "\\";
            }
        }

        /// <summary>
        /// projectFolder文本框确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectFolderConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            dir = projectFolderTextBox.Text;

            //建立工程目录树
            item.Items.Clear();
            GetProjectTree(dir, item);

            //编译执行并查看结果
            CompileAndRunAndMonitor();
        }

        /// <summary>
        /// 查看生成字数确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void functionsConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            foreach (string s in funtionsListBox.SelectedItems)
            {
                items.Add(s);
            }

            File.Delete(dir + Variables.aims);
            File.Delete(dir + Variables.res);

            File.WriteAllLines(dir + Variables.aims, items.ToArray());

            while (!File.Exists(dir + Variables.aims))
            {
                Thread.Sleep(100);
            }

            Command.Execute("cmd.exe", "cd /d " + dir + " & "
                + Variables.cutTheTree);

            //载入新的子xml文件
            LoadXML(dir + Variables.res);
        }
        #endregion

        #region 第二页
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FuncNameAndApprTm item = (FuncNameAndApprTm)funcGeneralListView.SelectedItem;
            if (item != null)
            {

                InvokeChart.Title = item.FunctionName + " 调用比例";
                BeInvokedChart.Title = item.FunctionName + " 被调用比例";

                //要向饼状图添加的数据
                List<KeyValuePair<string, double>> valueList = new List<KeyValuePair<string, double>>();

                //读取文件的起始行号索引
                int index;

                //调用关系图
                index = invokeConditon.IndexOf("#" + item.FunctionName);
                for (int i = index + 1; i < invokeConditon.Count; ++i)
                {
                    if (!invokeConditon[i].StartsWith("#"))
                    {
                        string[] detail = invokeConditon[i].Split("$".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        valueList.Add(new KeyValuePair<string, double>(detail[0] + " -- " + detail[1] + "\r\n" + detail[2] + "%", Convert.ToDouble(detail[2])));
                    }
                    else
                        break;
                }
                
                ((PieSeries)InvokeChart.Series[0]).ItemsSource = valueList.ToArray();

                //被调用关系图
                valueList = new List<KeyValuePair<string, double>>();
                index = beInvokedConditon.IndexOf("#" + item.FunctionName);
                for (int i = index + 1; i < beInvokedConditon.Count; ++i)
                {
                    if (!beInvokedConditon[i].StartsWith("#"))
                    {
                        string[] detail = beInvokedConditon[i].Split("$".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        valueList.Add(new KeyValuePair<string, double>(detail[0] + " -- " + detail[1] + "\r\n" + detail[2] + "%", Convert.ToDouble(detail[2])));
                    }
                    else
                        break;
                }
                ((PieSeries)BeInvokedChart.Series[0]).ItemsSource = valueList.ToArray();

                
            }
        }
        #endregion

        #region 第三页

        #region 工程目录树
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="t">父节点</param>
        /// <param name="content">节点名称</param>
        /// <param name="option">true代表文件 false代表文件夹</param>
        TreeViewItem addTreeViewItem(TreeViewItem t, string content,bool option)
        {
            TreeViewItem subItem = new TreeViewItem();
            subItem.Header = content;
            subItem.FontWeight = FontWeights.Normal;
            if (option)
            {
                subItem.ContextMenu = projectTree.Resources["FileContext"] as ContextMenu;
                subItem.MouseDoubleClick += subItem_MouseDoubleClick;
            }
            else
                subItem.ContextMenu = projectTree.Resources["FolderContext"] as ContextMenu;
            t.Items.Add(subItem);
            return subItem;
        }

        /// <summary>
        /// create new file 创建新文件事件
        /// </summary>
        /// <param name="text">要创建的文件名</param>
        void CNF_MyEvent(string text)
        {
            //若长度不为零
            if(text.Length  > 0)
            {
                //设置路径名
                string subPath = string.Empty;
                TreeViewItem father = (TreeViewItem)projectTree.SelectedItem;
                if (father != item)
                    subPath += (father.Header.ToString() + "\\");
                subPath += text;

                //先添加条目
                addTreeViewItem(father, subPath, true);

                //更改编辑对象
                editingFilePath = dir + subPath;

                //写入空文件
                File.WriteAllText(editingFilePath, string.Empty);

                //新建了一个空文件
                txtCode.Text = string.Empty;
            }

        }

        /// <summary>
        /// 创建新窗口的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            CreateNewFile cnf = new CreateNewFile();
            cnf.MyEvent += new CreateNewFile.MyDelegate(CNF_MyEvent);
            cnf.Show();
            cnf.Focus();
        }

        /// <summary>
        /// 目录树条目双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void subItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (isModified)
            {
                MessageBoxResult mbr = MessageBox.Show("放弃对文件\r\n" + editingFilePath + "\r\n的更改?","提示",MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.Cancel)
                    return;
            }

            try
            {
                //先找到点击的条目
                TreeViewItem t = (TreeViewItem)projectTree.SelectedItem;

                //更改正在编辑的文件
                editingFilePath = dir + t.Header.ToString();

                //读取选中的文件
                txtCode.Text = File.ReadAllText(editingFilePath,Encoding.GetEncoding("GB2312"));

                //载入新文件后 重设更改标识为未更改
                isModified = false;
            }
            catch { }
        }

        /// <summary>
        /// 删除文件及条目的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            TreeViewItem t = (TreeViewItem)projectTree.SelectedItem;
            TreeViewItem pt = (TreeViewItem)t.Parent;
            pt.Items.Remove(t);

            string toDeleteFileName = dir + t.Header.ToString();

            //删除文件
            File.Delete(toDeleteFileName);

            //若正在编辑此文件则清屏
            if (editingFilePath == toDeleteFileName)
                txtCode.Clear();
        }
        #endregion

        #region 工具栏
        /// <summary>
        /// 保存命令绑定 点击后操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                File.WriteAllText(editingFilePath, txtCode.Text,Encoding.GetEncoding("GB2312"));
                isModified = false;
            }
            catch{}
            
            return;
        }

        /// <summary>
        /// 是否可执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// 好像有问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            CompileAndRunWithOutMonitor();
        }
        #endregion

        #region 代码编辑器
        void txtCode_TextChanged(object sender,EventArgs e)
        {
            isModified = true;
        }

        /// <summary>
        /// 文本编辑框正在键入字符事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtCode_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        /// <summary>
        /// 获取最大数字
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int MAX(int a, int b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// 字符键入完成 开始代码提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtCode_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            
            if (e.Text == " " || e.Text[0] == 10 || e.Text == "\t")
            {
                trie.Bulid(txtCode.Text);
            }
            
            else
            {
                string toAnalyze = txtCode.Text.Substring(0, txtCode.CaretOffset - 1);
                int start = MAX(MAX(toAnalyze.LastIndexOf(' '),toAnalyze.LastIndexOf('\n')), toAnalyze.LastIndexOf('\t'));
                tempWord = txtCode.Text.Substring(start + 1, txtCode.CaretOffset - start - 1);             
                
                string toSplit = trie.Search(tempWord);
                string[] hints = toSplit.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                completionWindow = new CompletionWindow(txtCode.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (string s in hints)
                {
                    string temp = s.Remove(0, tempWord.Length).Trim();
                    if (temp.Length == 0)
                        continue;
                    data.Add(new MyCompletionData(temp));
                }
                if(data.Count > 0)
                    completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }

        }
        #endregion

        #region 自动添加代码
        private void aspectButton_Click(object sender, RoutedEventArgs e)
        {
            string toAdd = "aspect " + aspectTextBox.Text + "{\r\n";
            if (aspectCheckBox.IsChecked == true)
            {
                toAdd += "  public " + aspectTextBox.Text + "()\r\n    {\r\n    }\r\n";
            }
            toAdd += "};";
            txtCode.Document.Insert(txtCode.CaretOffset, toAdd);
        }
        #endregion

        #endregion
    }
}
