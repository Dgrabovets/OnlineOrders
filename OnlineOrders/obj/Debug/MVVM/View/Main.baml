   M S B A M L   `   `   `  ����G  COnlineOrders, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:OnlineOrders.MVVM.ViewOnlineOrders.MVVM.View  5   	   !    OnlineOrders.MVVM.View.Main   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:OnlineOrders.MVVM.View     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  6<      MainViewModel  � DataContext#  [  5   	    � Title$# Интернет заказы��5	   	   $
��1080��6!   $
��1920��6/    � WindowStartupLocation$ CenterScreen=�6<    � 	Resources 5       �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow��%)  '��       '�� H     '  �     '��      '      '�� P    �� 5   
   " ��6   .�5      ��  � ��$ 20(�6)   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�69   �� 5       � ��$ 23(�6)   5      �� 5   
   "  6   .�5      ��  � ��$ 23(�6)   5      �� 5   
   " ��6   .�5      ��  � ��$	 200��6&   �� 5       � ��$ 70��6'   �� 5       � ��
 �   �6,   �� 5       � ��$ 30(�6)   �� 5       � ��$ 2q�60   �� 5       � ��$ 20, 0, 20, 0q�6'   5      �� 5   
   "  6   .�5      ��  � ��$ 20(�6)   5       �� 5!   
   " ��6   .�5"      ��  � ��$ 23(�6)   �� 5#       � ��$ 50, 0, 0, 0q�6'   �� 5$       � ��$ Center=�62   5%      5&      .��5(      � .+�5)   
   �� $��2q�6   
���   �$��15q�69   ._�5*      �  � RowDefinitions 5+      �� 5,      $	��10* �6$   �� 5-      $	��80* �6$   �� 5.      $	��10* �6$   .+�50      ��  �� Orientation$ 
Horizontal=�6   $��LeftToRight=�66   .+�51      �� $��0��6   $��Клиент:��6)   �� 52      -   5        	 ��Name$	 ClientComboBox��52   j   
 �� MaxDropDownHeight$	
 150��6   $��Visible=�67   $��Center=�6�   $	��400��6�   $��35��6�   $��30, 0, 0, 0q�6�    �� DisplayMemberPath$ FullName��6  ��6�   �� *	clients+ �� SelectedItem 6�   �� *selectedClient+*  $Microsoft.Xaml.Behaviors.Interaction   Triggers 53      +  %Microsoft.Xaml.Behaviors.EventTrigger  54         	EventName$ SelectionChanged��6-   *  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 55   "   2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command 68   �� *ClientChanged+56      58      �� 59      $��40��6   $��40��6(   $��15, 0, 0, 0q�64   
������ $��0q�6b   ��6v   �� *EditClientCommand+.��5:      �� $#��..\..\Resources\EditImage.png��6    $��30��6G   5;      �� 5<      $��0��6   $��Статус:��6)   �� 5=      $��23(�6   $��Center=�6-   $	��400��6H   $��35��6T   $��30, 0, 0, 0q�6`   ��6u   �� *
statuses+ 6�   �� *selectedStatus+ 5>        5?      $ SelectionChanged��6-   . 5@   "      68   �� *StatusChanged+5A      5C      5D      &   System.Windows.Controls.DataGrid  5F      $��Visible=�6a   $��1��6�   $��15, 15, 15, 0q�6�   
���   �$��2q�6�      AutoGenerateColumns .     CanUserAddRows .  ��6   �� *orders+ 6:   �� *selectedOrder+   Columns 5G      0  *System.Windows.Controls.DataGridTextColumn  5H      ,	  &System.Windows.Controls.DataGridColumn 	  Header$ № Заказа��6D    	  Width5
  /System.Windows.Controls.DataGridLengthConverter$	 10*
 6V   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6-   �� *ID+6V     5I      $ Дата создания��6K   $	 10*
 6b    6-   �� *	CreatedAt+6b     5J      $ 
Сумма��6G   $	 10*
 6V    6-   �� *Total+6V     5K      $ Статус��6M   $	 10*
 6]    6-   �� *OrderStatus+6]     5L      $ Клиент��6P   $	 20*
 6`    6-   �� *ClientFullName+6`     5M      $ Комментарий��6I   $	 20*
 6^    6-   �� *	Comment+6^   5O      �� 5P      $��2��6   $
��Left=�6&   $��Обновить��6A   ��6T   �� *ResetCommand+�� 5Q      $	��720��6   $��2��6)   $��Right=�66   $ 
Horizontal=�6R   .+�5R      �� $��2��6   $��Изменить��6*   ��6=   �� *UpdateCommand+�� 5S      $��2��6   $��Добавить��6*   ��6=   �� *
AddCommand+�� 5T      $��2��6   $��Удалить��6*   ��6<   �� *DeleteCommand+5U      5V      5W      5X      5Y      