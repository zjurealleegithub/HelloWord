   M S B A M L   `   `   `  ����[  WMixModes.Synergy.VisualFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulli9clr-namespace:MixModes.Synergy.VisualFramework.Converters+MixModes.Synergy.VisualFramework.Converters  5      c6clr-namespace:MixModes.Synergy.VisualFramework.Windows(MixModes.Synergy.VisualFramework.Windows  5      g8clr-namespace:MixModes.Synergy.VisualFramework.Behaviors*MixModes.Synergy.VisualFramework.Behaviors  5      c _System.Windows.Interactivity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	      =    7MixModes.Synergy.VisualFramework.Windows.WindowsManager   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      N
converters9clr-namespace:MixModes.Synergy.VisualFramework.Converters    5      Flocal6clr-namespace:MixModes.Synergy.VisualFramework.Windows    5      L	behaviors8clr-namespace:MixModes.Synergy.VisualFramework.Behaviors    5      \iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity    5	      -   5          >�Name$  UserControl��5
         DockIllustrationGridStyle=   7MixModes.Synergy.VisualFramework.Windows.WindowsManager    DockPaneIllustrationStyle# �   5         DockIllustrationContentStyle#   DockIllustrationContentStyle# �  5       � 	Resources 5        DockPointBorder �� 
TargetType �� Property �� Value �� Triggers P� Property	 P� Value  DockIllustrationBrush��%�  &	       &	  =    �� 5      " ��5      .�5      ��  � ��
 ���� 5      �� 5       � ��$ 2q�5      �� 5       � ��$ Center=�5      �� 5       � ��$ Center=�5       5      P� 5   
    �@J�IsMouseOver	 . . �5      ��  � ��
 �   �5       5!      5#      �� 5&      " �5'      .�5(      ��  � ��# �  5)      5*      5,      .��5.      � -   5        
 �Name$
 
LayoutRoot��5.   	   .+�51      ]� ��. .+�54      �� -   5         ��Name$ TopWindowHeaders��55      $	��Top=�54       �� Orientation$ 
Horizontal=�56      �� 59      -   5        $ LeftWindowHeaders��5;      $ Vertical=�59      $
��Left=�5:      5;      �� 5>      -   5        $ RightWindowHeaders��5@      $ Vertical=�5>      $��Right=�5?      5@      �� 5C      -   5        $ BottomWindowHeaders��5E      $ 
Horizontal=�5C      $��Bottom=�5D      5E      � 5I      .+�5J   
   ]� ��. .+�5M      ]� -   5         ]�Name$ LeftPinnedWindows��5M      $
��Left=�5N      ��.  5O      ]� 5R      -   5        $ RightPinnedWindows��5R      $��Right=�5S      ��.  5T      ]� 5W      -	   5        $ TopPinnedWindows��5W      $	��Top=�5X      ��.  5Y      ]� 5\      -
   5        $ BottomPinnedWindows��5\      $��Bottom=�5]      ��.  5^      � 5a      .+�5c      @   :MixModes.Synergy.VisualFramework.Windows.DocumentContainer  -   5          Name$ DocumentContainer��5c   &   ]� 5f      -   5        $ DockingIllustrationPanel��5f      ��.  5g      5i      5j      ]� 5m   
   -   5        $ 	PopupArea��5m      ��.  5n      5o   	   5q      �� 5s      -   5         ��Name$ FloatingPanel��5s      ��.  5t      ]� 5v      -   5        $ DockingPanel��5v      $y�	Collapsed=�5w      ��.  .+�5z      �� $	��Top=�6   #��[ 5{      .  (System.Windows.Interactivity.Interaction   	Behaviors 5|   
   B   <MixModes.Synergy.VisualFramework.Behaviors.DockPointBehavior  5}      ._�5   
   �� $A��;/MixModes.Synergy.Resources;component/Resources/DockTop.png��6   5�   	   �� 5�      $
��Left=�6   #��[ 5�       5�   
     5�      ._�5�   
   �� $B��</MixModes.Synergy.Resources;component/Resources/DockLeft.png��6   5�   	   �� 5�      $��Right=�6   #��[ 5�       5�   
     5�      ._�5�   
   �� $C��=/MixModes.Synergy.Resources;component/Resources/DockRight.png��6   5�   	   �� 5�      $��Bottom=�6   #��[ 5�       5�   
     5�      ._�5�   
   �� $D��>/MixModes.Synergy.Resources;component/Resources/DockBottom.png��6   5�   	   5�      5�      5�      