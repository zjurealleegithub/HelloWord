   M S B A M L   `   `   `  ����
  Synegy#    Synegy.Views.NewProjectWindow   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      U QMixModes.Synergy.Resources, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5	Resources%http://synergy.mixmodes.com/resources  5      [ WMixModes.Synergy.VisualFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+visualFxhttp://mixmodes.com/visualFx  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      +Controlshttp://mixmodes.com/controls  5      3  -MixModes.Synergy.Resources.Resources.Language    NewProjectWindowTitle � Title# Z  5
      $	��400��5      $	��600��5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��64��6   $��0��6   ��5      �� 5   	    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6.   .��5   
   �  � Color$ 	#FF293955��6    � Offset$ 0Q�6)   � 5   
   $ 	#FF334F80��6   $ 1Q�6)   5   
   .+�5      ]� $��0��6   .+�5       �� $D��>/MixModes.Synergy.Resources;component/Resources/NewProject.png��6   $
��Left=�5!      $
��Left=�5"      $��Center=�5#      $��64��5$      $��64��5%      $��UniformToFill=�5&      $
��16,0q�5'      �� 5(         NewProjectHeadline#��Z 6      HeadlineText#���   5)      $	��Top=�5*      $��0,8,0,0q�6-   ���	#FFD9E1EA6>   �� 5+         NewProjectDescription#��Z 6   $	��Top=�5,      $��0,4,0,0q�5-      ���	#FFD9E1EA6)   5.      5/      � 51   
   $��1��6   $��
64,16,64,0q�52      	 � ColumnDefinitions	 53      �� 54      $
��Auto �6#   �� 55      $��* �6#    57      �� 58      $
��Auto �6    �� 59      $��* �6    .+�5;      �� $��0��6   $��0��5<      
   ProjectName#��Z
 5=      $��Center=�5>      $��0,0,16,0q�5?      �� 5@      -   5         ��Name$ ProjectName��5B      $��1��5@      $��0��5A      $	��350��5C      �� 5D      $��0��6   $��1��5E         Description#��Z 5F      $	��Top=�5G      $��	0,16,16,0q�5H      �� 5I      -   5        $ ProjectDescription��5L      $��1��5I      $��1��5J      $
��0,16q�5K       �� TextWrapping$
 Wrap=�5M       �� AcceptsReturn . $	��350��5O      $	��175��5P      5Q      �� 5T   
   $��2��6     DialogFooterBorder#���  5U      ._�5V      ]� .+�5W      �� $��Right=�6      Cancel#��Z 5X      $��Right=�5Y      $��4,8,8,8q�5Z       �� IsCancel . $��80��5[   )   �� 5\      $��Right=�6   	   OK#��Z 5]      $��Right=�5^      $	��4,8q�5_       �� 	IsDefault . $��80��5`   *   5a      5b      5c      5d      