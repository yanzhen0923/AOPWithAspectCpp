using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesignPattern
{
    /// <summary>
    /// CreateNewFile.xaml 的交互逻辑
    /// </summary>
    public partial class CreateNewFile : Window
    {
        public delegate void MyDelegate(string text);

        public event MyDelegate MyEvent;

        public CreateNewFile()
        {
            InitializeComponent();

        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            //触发事件，并将修改后的文本回传  
            MyEvent(fileNameTextBox.Text);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
