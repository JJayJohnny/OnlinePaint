   M S B A M L   `   `   `  ����F  BPaintClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:PaintClientPaintClient  5   	       PaintClient.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:PaintClient     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��3* �6   �� 5       � RowDefinitions 5   
   �� 5      $��2* �6   �� 5      .+�5   
   �� -   5        $��paintCanvas��5      $��0��6$   $��0��61   $��2��6A   $��Stretch=�6�   $��Stretch=�6�   
���   �6�   �� 5   
   -   5        $��colorPicker��5      $��1��67   $��1��6G   $��Stretch=�6T   $��20,20,20,20q�6r   $��Stretch=�6�   
���  ��6�   �� 5   
   $��IP��6   $��1��6   $��0��6-   $
��Left=�6:   $��	20,10,0,0q�6U   $	��Top=�6h   �� 5   
   -   5        $��	ipTextBox��5      $��1��6#   $��0��63   $
��Left=�6@   $��	45,14,0,0q�6[   $�� ��6n    �� TextWrapping$
 Wrap=�6v   $	��Top=�6�   $	��132��6�   �� 5   
   $
��Port��6   $��1��6   $��0��6/   $
��Left=�6<   $��	20,41,0,0q�6W   $	��Top=�6j   �� 5   
   -   5        $��portTextBox��5      $��1��6%   $��0��65   $
��Left=�6B   $��	57,45,0,0q�6]   $�� ��6p   $
 Wrap=�6x   $	��Top=�6�   $	��120��6�   �� 5   
   -   5        $��connectButton��5      $��Connect��6&   $��1��6M   $��0��6]   $
��Left=�6j   $��	20,72,0,0q�6�   $	��Top=�6�   �� 5   
   -   5        $��disconnectButton��5      ${�False��6)   $��
Disconnect��6;   $��1��6h   $��0��6x   $
��Left=�6�   $��
115,72,0,0q�6�   $	��Top=�6�   �� 5   
   -   5        $��thicknessSlider��5      $��1��6(   $��0��68   $��Center=�6E   $��0,0,0,10q�6b   $��Bottom=�6t   $	��160��6�    � Minimum$ 1Q�6�    � Maximum$ 15Q�6�    � Value$ 4Q�6�   �� 5    
   -   5        $��thicknessLabel��5       $��
Thickness:��6&   $��1��6;   $��0��6K   $
��Left=�6X   $��	20,0,0,25q�6s   $��Bottom=�6�   5$      5%      