   M S B A M L   `   `   `  ����  OnlineUpdater     OnlineUpdater.UpdateWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Update Window��5   	   $	��400��6   $	��450��6+   $	��400��68   $	��450��6G    � Icon$ refresh.ico��6W    � WindowStartupLocation$ CenterScreen=�6j   .��5      �  � RowDefinitions 5   
   �� 5      $	��110 �6   6+   �� 5      6   �� 5	      $��50 �6   6*   .+�5   
   �� -   5        $��	textBlock��5      $��0��6%   $
��Wrap=�62   $
��10,5q�6F   6V   (  "System.Windows.Controls.WebBrowser  5   
   -   5        $��
webBrowser��5      $��1��6'   6W   �� 5   
   -   5        $��2��5      $
��Left=�6   $��Center=�69   $	��110��6T   $��Не сейчас��6`   $��10,0,0,0q�6t    �� IsCancel . 6�   �� 5   
   $��2��6   $��Right=�6   $��Center=�6:   $	��110��6U   $#��Напомнить позже��6a   $��	0,0,130,0q�6{   $y�	Collapsed=�6�   6�   �� 5   
   -   5        $��2��5      $��Right=�6   $��Center=�6:   $��Обновить��6U   $	��110��6h   $��0,0,10,0q�6t   ���	#FFB2ECBA6�   5      5      