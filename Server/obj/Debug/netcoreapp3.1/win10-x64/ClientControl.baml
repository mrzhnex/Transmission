   M S B A M L   `   `   `  ����A  =Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ServerServer  5   	   ? ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA,clr-namespace:Core.Application;assembly=CoreCore.Application 5   	       Server.ClientControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )notlocalclr-namespace:Server     5   	   >theme,clr-namespace:Core.Application;assembly=Core     5   	   -   5        $	��100��5   	   $	��200��6     � DataContext  5
   	   ��  �� Path$ DataContext� �� RelativeSource ��  �� AncestorType" �� � 	Resources 5         ConnectionTwo �� 	UriSource �� BitmapScalingMode  Speaker  SpeakerCrossed  
Microphone  MicrophoneCrossed��%  &	        &	 ^     &	 �     &	     &	 p    �� 5   
   $3 -/Server;component/Resources/ConnectionTwo.png?�6,   $ HighQuality=�6f   �� 5   
   $- '/Server;component/Resources/Speaker.png?�6&   $ HighQuality=�6Z   �� 5   
   $4 ./Server;component/Resources/SpeakerCrossed.png?�6-   $ HighQuality=�6h   �� 5   
   $0 */Server;component/Resources/Microphone.png?�6)   $ HighQuality=�6`   �� 5   
   $7 1/Server;component/Resources/MicrophoneCrossed.png?�60   $ HighQuality=�6n   5      .��5      �� $��2q�6d   ��6   �� %  Core.Application.ServerSettings   Theme �� Source# Z $ FourthColor�._�5   
   �  5      	 �� 
TargetType
 �� Property �� Value��%�   '��       �� 5      "	 ��6   .�5      �� 
 � ��$ 2q�6/   �� 5      
 � ��$ Center=�6:   �� 5      
 � ��$ Center=�6<   5      5       � ColumnDefinitions 5      �� 5      $
��auto �6#   62   �� 5       $��* �6#   6/    � RowDefinitions 5"      �� 5#      $
��auto �6    60   �� 5$      $��* �6    6-   �� 5%      $��* �6    6-   .+�5'      � ��6   �� # Z $ 
FirstColor�.+�5(      �� #��[  6   $��12��6@   $��12��6K   $��2q�6W   5)      � 5*      $��1��6   
�������.+�5+      �� #���  6   $��12��6;   $��12��6F   $��2q�6R   5,      � 5-      $��2��6   
�������.+�5.      �� #���  6   $��12��6>   $��12��6I   $��2q�6U   5/      � 51      $��1��6   ��6#   �� # Z $ 
FirstColor� 52      �� 53      $��* �6'   63   �� 54      $��* �6'   63   �� 55      $��* �6'   63   .+�57      �� ��6   �� $ Username��� 58      $��1��6   ��6,   �� $ ConnectionTimeSpan��� 59      $��2��6   ��6,   �� $ Ip�5:      � 5<      $��1��6   $��2��6    
�������$��1��6S   .+�5=         Server.SpectrumControl  -   5          Name$ InputSpectrumControl��5=   +   $��4q�6I   5>      5?      5@      5A      