   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  H DDesignPattern, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:DesignPatternDesignPattern | xSystem.Windows.Controls.DataVisualization.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35�rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization �{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit2System.Windows.Controls.DataVisualization.Charting    DesignPattern.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6sys&clr-namespace:System;assembly=mscorlib     ` \ICSharpCode.AvalonEdit, Version=4.3.1.9429, Culture=neutral, PublicKeyToken=9cc39be672370310?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  'localclr-namespace:DesignPattern  {DVrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit  �DVC{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit  -     � Title$  
MainWindow��$	��600��$
��1000�� � WindowStartupLocation$ CenterScreen=� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$
 Save�� � 	Resources    GlassButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius J� RenderTransformOrigin � RowDefinitions � Color � Offset �� Triggers P� Property P� Value �� 
TargetName �� ScaleX �� ScaleY��%�  &	        �� " ��.���  � ��
 �������  �@�Cursor$
 Hand����  � �� �� " ��.R��� $	 ButtonBorder��$
 15��$��2q����	#AA000000���	#99FFFFFF$ 0.5,0.5&�._��  �� $��1* ��� $��1* �.+��� $��0��$
 5������ .��� $ 	#08FFFFFF��$ 0Q�� $ 	#88FFFFFF��$ 1Q��� $��content��$��2��$��Center=�$��Center=� P�  � �� . . ��� $ ButtonBorder�� � z� V� .~��� $
 0.99Q�$
 0.99Q�.���� .@��� -   $��firstTabItem��$��概要分析结果��.���  �� $��* ��� $��* ��� $	��10* � � ColumnDefinitions �� $��* ��� $��2* ��� $��* �.+��� $��0��$��0��$��工程文件夹路径��$��Right=�$��10q��� -   $��projectFolderTextBox��$��0��$��1�� �� 
IsReadOnly . $��40,10,40,10q��� -   $ ��projectFolderConfirmButton��$��0��$��2��$��确定��$��10,10,120,10q��� $��1��$��0��$��xml文件路径��$��Right=�$��10q��� -   $��xmlPathTextBox��$��1��$��1�� . $��40,10,40,10q��� -   $��ConfirmButton��$��1��$��2��$��确定��$��10,10,120,10q�� $��2��$��0��$��2��.+�R� -   $��
trTreeview��  VerticalTreeStyle#��[ ����  	 nodes �� Source# [ � $��2��$��2�� �� $��6* ��� $
��2.5* ��� $
��1.5* �.+��� -	   $��funtionsListBox��$��0�� �� SelectionMode$ Multiple=�$��10,0,5,5q��� -
   $��unusedListBox��$��1��
�������$��10,5,5,5q��� -   $��functionsConfirmButton��$��查看生成子树��$��2��$��60,15,60,15q��� $��详细分析结果��.���  �� $��* ��� $��* � �� $��* ��� $��* �.+�>  8System.Windows.Controls.DataVisualization.Charting.Chart  -   $��InvokeChart��$��0��$��1��$��2q�   Title$ 调用比例��
����İ�   Series B  <System.Windows.Controls.DataVisualization.Charting.PieSeries  H  BSystem.Windows.Controls.DataVisualization.Charting.DataPointSeries   IndependentValueBinding ��  �� Path$	 Key�   DependentValueBinding �� $ Value�  -   $��BeInvokedChart��$��1��$��1��$��2q�$ 被调用比例��
����İ�    �� $	 Key� �� $ Value��� -   $��funcGeneralListView��$��0��$��0�� �� View �� .P��� $��	函数名�� �� DisplayMemberBinding �� $ FunctionName��� $��出现次数�� �� $ AppearTimes��� -   $��strongRelyListView��$��1��$��0�� �� .P��� $��目标函数�� �� $ TargFunc��� $��依赖函数�� �� $ DepFunc��� $��次数�� �� $ Times��� $��	百分比�� �� $
 Perc��� -   $��Coding��$��代码编辑��.���  �� $��* ��� $	��19* � �� $
��1.5* ��� $��6* ��� $��3* �.+�� $��1��$��0��.+�R� -     R�Name$  projectTree��   FolderContext! �� 	StaysOpen  FileContext" �� Orientation# �� RelativeSource$ �� Mode% �� AncestorType, &DesignPattern.ProjectTreeIconConverter&   Instance' �� 	Converter��%L  &	       &	 :     'Q� m     �� ! . .@�m� $��NewFile��-   $��添加文件���� ! . .@�m� $��Delete��-   $��删除���� " Q�.���  � �� �� .R��� $" 
Horizontal=�.+��� $	��img��$��20��$��20��$
��Fill=����� # �� $$ FindAncestor=�"% Q�$ Header�#' Z& �� $	��5,0q����� � $��0��$��1��.+�`� .�c� .@��� $
��Save��.���� $/��)pack://application:,,,/Resources/save.gif���� $
��Copy��.���� $/��)pack://application:,,,/Resources/copy.gif���� $��Paste��.���� $0��*pack://application:,,,/Resources/paste.gif���� $
��Undo��.���� $/��)pack://application:,,,/Resources/undo.gif���� $
��Redo��.���� $/��)pack://application:,,,/Resources/redo.gif��c� .@��� -   $��	RunButton��.���� $.��(pack://application:,,,/Resources/run.gif��� $��1��$��1��.+�'  !ICSharpCode.AvalonEdit.TextEditor  -   $��txtCode��$��Consolas)�$
��12pt(�� $��1��$��2�� �� $��* ��� $��* ��� $��* ��� $��* �.+�� $��0��
������� �� $��* ��� $��* ��� $��* � �� $��3* ��� $��* ��� $��2* �.+��� $��0��$��0��$��添加Aspect��$��Bottom=�$��10q��� -   $��aspectTextBox��$��1��$��0��$��24��$��10q��� -   $��aspectCheckBox��$��1��$��1��$��Center=�$��Right=�$��5q��� $��1��$��2��$��添加构造函数��$
��Left=�$��Center=�$��5q��� -   $��aspectButton��$��2��$��0��$��添加��$��10q�� $��1��
������� �� $��* ��� $��* ��� $��* � �� $��3* ��� $��2* ��� $��* �.+��� $��0��$��0��$��添加Advice��$��Bottom=�$��	10,0,0,10q��� $��1��$��0��$��10q�.@��� ( �� 
IsSelected( . $
��call���� $��	execution���� $��construction���� $��destruction���� $��1��$��1��$��10q�.@��� ( . $��before���� $��after���� $��around���� -   $	��add��$��2��$��0��$��添加��$��10q�� $��2��
������� �� $��* ��� $��* ��� $��* � �� $��3* ��� $��* ��� $��2* �.+��� $��0��$��0��$��添加pointcut��$��Bottom=�$��	10,0,0,10q��� $��1��$��0��$��24��$��10q��� $��1��$��1��$��Right=�$��Center=�$��5q��� $��1��$��2��$��	虚切点��$
��Left=�$��Center=�$��5q��� $��2��$��0��$��添加��$��10q�� $��3��
������� �� $��* ��� $��* ��� $��* � �� $
��2.5* ��� $��2* ��� $
��1.5* �.+��� $��0��$��0��$��添加Join Point��$��Bottom=�$��	10,0,0,10q��� -   $��joinPointComboBox��$��1��$��0��$��10q�.@��� ( . $��cflow���� $��within���� $
��that���� $��target���� $��result���� $
��args���� $��1��$��1��$��Center=�$��5q��� $��添加参数��$��1��$��2��$��Center=�$
��Left=�$��5q��� $��2��$��0��$��添加��$��10q��� -   $��	outputTab��$��编译器输出��.���� -   $��debugRichTextBox�� . 