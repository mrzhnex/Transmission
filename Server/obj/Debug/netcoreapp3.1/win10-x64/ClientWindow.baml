   M S B A M L   `   `   `  ����A  =Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ServerServer  5   	   ? ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA,clr-namespace:Core.Application;assembly=CoreCore.Application 5   	       Server.ClientWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )notlocalclr-namespace:Server     5   	   >theme,clr-namespace:Core.Application;assembly=Core     5   	   -   5          � AllowsTransparency$
  true��5   	    � WindowStyle$
 None=�5   	    � 
ResizeMode$ CanResizeWithGrip=�6   $	��160��6;   $	��400��6K   $��0��6Z   $��0��6e    � 	Resources 5         NonDefaultMouseOverEffect �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers	 P� Property
 P� Value  Minimize �� BitmapScalingMode 
 Deploy  	DeployTwo 	 Close  Settings  ConnectionTwo  
Microphone  MicrophoneCrossed 	 Speaker 
 SpeakerCrossed��%j  &	        &	 �    &	 Y    &	 �    &	 \    &	 �    &	 ]    &	 �    &	 h    &		 �    &	
 t    �� 5   
   " ��62   .�5      ��  �@�OverridesDefaultStyle$
 True��66   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��border��6!   $��0q�6/   
���   �#��z��6W   ._�5      �� $��Center=�6/   $��Center=�6L   5       5      P� 5      	 �@J�IsMouseOver$

 True��6=   . �5   "   ��  �@J�Opacity$	 0.8Q�6<   5      5      5      5      �� 5    
   $.��(/Server;component/Resources/Minimize.png��6!   $��14��6S   $��14��6_   $ HighQuality=�6j   $��3q�6�   �� 5!   
   $,��&/Server;component/Resources/Deploy.png��6   $��14��6O   $��14��6[   $ HighQuality=�6f   $��3q�6�   �� 5"   
   $/��)/Server;component/Resources/DeployTwo.png��6"   $��14��6U   $��14��6a   $ HighQuality=�6l   $��3q�6�   �� 5#   
   $+��%/Server;component/Resources/Close.png��6   $��14��6M   $��14��6Y   $ HighQuality=�6d   $��3q�6�   �� 5$   
   $.��(/Server;component/Resources/Settings.png��6!   $��20��6S   $��20��6_   $ HighQuality=�6j   $��4q�6�   �� 5%   
   $3��-/Server;component/Resources/ConnectionTwo.png��6&   $��20��6]   $��20��6i   $ HighQuality=�6t   $��4q�6�   �� 5'   
   $0��*/Server;component/Resources/Microphone.png��6#   $��16��6W   $��16��6c   $ HighQuality=�6n   $��4q�6�   �� 5(   
   $7��1/Server;component/Resources/MicrophoneCrossed.png��6*   $��16��6e   $��16��6q   $ HighQuality=�6|   $��4q�6�   �� 5)   
   $-��'/Server;component/Resources/Speaker.png��6    $��16��6Q   $��16��6]   $ HighQuality=�6h   $��4q�6�   �� 5*   
   $4��./Server;component/Resources/SpeakerCrossed.png��6'   $��16��6_   $��16��6k   $ HighQuality=�6v   $��4q�6�   5+      .��5,      �� $��2q�6d   ��6   �� %  Core.Application.ServerSettings   Theme �� Source# Z  �� Path$ FourthColor�._�5-   
   �  5.        HorizontalBrush �� 
StartPoint �� EndPoint � Color � Offset  SliderThumbStyle �� RadiusX �� RadiusY � RowDefinitions �Name � ColumnDefinitions �� Padding �� CornerRadius ]� Thumb��%  &	       &	 f     '�� �    7  7  '�� D    �� 5/      $	 0,0&�6>   $	 1,0&�6O   .��50      � $ Black��6#   $	 1.0Q�61   51      �� 52      " n�61   .�53      ��  �@J�SnapsToDevicePixels$
 true��6<   �� 54       �@�OverridesDefaultStyle$ false��6>   �� 55       � ��$ 20��6/   �� 56       � ��$ 15��6.   �� 57       �@J�SnapsToDevicePixels$
 true��6<   �� 58       �@�OverridesDefaultStyle$
 true��6>   �� 59       � �� 5:      �� 5;      " n�6.   .R�5<   "   �� 
�������
���   �$��18��6H   $��8��6T   $��1��6^   $��0q�6r   $ 15��6}   $ 2��6�   5=      5?      5@      �� 5A      " ��6   .�5B      ��  � ��$ 4q�6/   �� 5C       �@�OverridesDefaultStyle$
 true��6>   �� 5D       � ��$ Center=�6:   �� 5E       � ��$ 30��6.   �� 5F       � �� 5G      �� 5H      " ��6.   .R�5I   "   �  5J   &   �� 5K   *   $
��auto �68   �� 5L   *   $	 row��6F   $
��auto �68   6F   �� 5M   *   $
��auto �68    5O   &   �� 5P   *   $
��auto �6;   �� 5Q   *   $ column��6E   $��* �6;   6E   �� 5R   *   $
��auto �6;   .+�5T   &   �� $��PART_Border��6-   
���   �$��1q�5U   1   $	 1.5q�5V      $ 4��5W      $��1��5X      $��1��6*   #��z��5Y      #��z��5Z      8��  $��Stretch=�5\      $��Center=�5]      ]� 5^   &   $��
PART_Track��6,   $��Stretch=�5_       $��Center=�5`       $��1��5a       $��1��6-   #��z��5b       #��z��5c        5d   *   n� 5e   .   8�� 64   5g   '   5h   #   5j      5l      5m      �� 5n      " ��6   .�5o      ��  � ��$ 2q�6/   �� 5p       � ��$ Center=�6:   �� 5q       � ��$ Center=�6<   5r      5s       5t      �� 5u      $
��auto �6#   62   �� 5v      $��* �6#   6/    5x      �� 5y      $
��auto �6    60   �� 5z      $��* �6    6-   �� 5{      $��* �6    6-   .+�5}      � ��6   �� # Z $ 
FirstColor�.+�5~      �� -   5         ��Name$ 
Disconnect��5~      #��[" �6F   $��0q�6�   .��5      ��  �� ResourceKey$ ConnectionTwo��6%   5�      5�      � 5�      $��1��6   
������� 5�      �� 5�      6&   �� 5�      6&   .+�5�      �� -   5        $ OutputMuteStatus��5�      $��2��63   #��[" �6b    � ToolTip$ микрофон��6�   .��5�      C�  C� ResourceKey$ Speaker��6&   5�      �� 5�      -   5         ��Name$ InputVolume��5�      $��1��6.     � Maximum$	  1.0Q�6;   ! � Value$	! 1.0Q�6I   6U   5�      � 5�      $��2��6   
������� 5�      �� 5�      6&   �� 5�      6&   .+�5�      �� -   5        $ InputMuteStatus��5�      $��2��62   #��[" �6`   $ микрофон��6�   .��5�      C� $ 
Microphone��6&   5�      �� 5�      -   5        $ OutputVolume��5�      $��1��6/   $	  1.0Q�6<   $	! 1.0Q�6J   6V   5�      � 5�      $��1��6   ��6#   �� # Z $ 
FirstColor� 5�      �� 5�      $
��auto �6'   66   �� 5�      $
��auto �6'   66   �� 5�      $
��auto �6'   66   �� 5�      $
��auto �6'   66   �� 5�      $
��auto �6'   66   �� 5�      $��* �6'   63   .+�5�      �� -   5        $ Settings��5�      #��[" �6B   $��0q�6}   .��5�      �� $ Settings��6%   5�      �� 5�      -   5        " ��Name$" ClientStatus��5�   ,   $��1��6   6,   �� 5�      -	   5        $" Username��5�   ,   $��2��6   6,   �� 5�      -
   5        $" ConnectionTimeSpan��5�   ,   $��3��6   6,   �� 5�      -   5        $" Ip��5�   ,   $��4��6   6,   � 5�      $��5��6   $��Right=�6'    5�      �� 5�      $
��auto �6+   6:   �� 5�      $
��auto �6+   6:   �� 5�      $
��auto �6+   6:   .+�5�      �� -   5        $ Minimize��5�      #��[" �6F   $��0q�6�   .��5�      �� $ Minimize��6)   5�      �� 5�      -   5        $ Deploy��5�      $��1��6-   #��[" �6R   $��0q�6�   .��5�      C� $ Deploy��6*   5�      �� 5�      -   5        $ CloseButton��5�      $��2��62   #��[" �6V   $��0q�6�   .��5�      �� $ Close��6)   5�      5�      5�      � 5�      $��1��6   $��2��6    $��1��61   
�������.+�5�         Server.SpectrumControl  -   5        #  Name$# InputSpectrumControl��5�   +   $��4q�6I   5�      5�      5�      5�      