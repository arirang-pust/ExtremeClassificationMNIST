   M S B A M L   `   `   `  ���� "  ExtremeClassificationMNISTDemo=    7Mine.Apps.OCR.ExtremeClassificationMNISTDemo.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$)  #Extreme Classification - MNIST Demo��$	��700��$
��1000�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=�.��� -   $��
m_gridMain������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ White�� � Offset$ 0Q�� $ 	#FF5EA91C��$ 1Q�.+��� -   $��label1��$)��#Extreme Classification - MNIST Demo��$
��Auto��$
��Left=�$��	30,30,0,0q�$	��Top=�$��40(�$��Rosewood Std)����	#FF227609$��Normal$�$��Normal"��� -   $��label2��$g��aCopyright 2016. Kang Hyon-Il, Lee Yong-Hae, Chun Jin-Hong, Yoon Gwang-Min @ PUST, Graduate School��$
��Auto��$��Right=�$��0,0,10,5q�$��Bottom=�$��16(�
�������$��Imprint MT Shadow)��� -   $��
rectangle1��$��10��$��0,90,0,0q�
������ $	��Top=����� $ 1,0.5&�$ 0,0.5&�.��� $ 	#FF347909��$ 0Q�� $ Transparent��$ 1Q��� -   $��label4��$��	Digit OCR��$
��Auto��$��Right=�$��	0,63,20,0q�$	��Top=�$
��Auto��$��20(�$��Garamond)�$��Italic$�$
��Bold"����	#FF227609�� -   $��label3��$��Data Points��$��28��$
��Left=�$��
13,111,0,0q�$	��Top=����	#FF347909$��Cooper)�$��14(��� -   $��m_textBoxDp��$��23��$
��Left=�$��112,111,0,0q�$	��Top=�$	��108��$��Center=��� -	   $��label5��$��	Iteration��$��Cooper)����	#FF347909$��28��$
��Left=�$��235,111,0,0q�$	��Top=�$��14(�$��74���� -
   $��m_textBoxIter��$��23��$
��Left=�$��315,111,0,0q�$	��Top=�$	��126��$��Center=�� -   $��grid2��$
��Left=�$��14,142,0,30q�$	��206��.+�� -   $��grid1��$��29��$
��Left=�$��0,23,0,0q�$	��Top=�$	��206�����	#FF98F587$��1��.+��� -   $��label7��$��Image��$
��Left=�$	��115��
�������$��Center=�$��Center=�$��14(�$��Cooper)����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF006000��$ 0Q�� $ 	#FF14E214��$ 1Q�� $ 	#FF5CEF5C��$ 0.772Q��� -   $��label8��$��Digit��$��Cooper)�$��14(�
�������$��Right=�$��Center=�$��Center=�$��90������ $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF006000��$ 0Q�� $ 	#FF14E214��$ 1Q�� $ 	#FF5CEF5C��$ 0.772Q��� -   $��label6��$��Training Data��$��Cooper)����	#FF347909$��28��$
��Left=�$	��Top=�$��14(��� -   $��m_listBoxTrain��$
��Left=�$��0,53,0,0q�$	��206��� -   $��grid3��$
��Left=�$��235,142,0,30q�$	��206��.+�� -   $��grid4�����	#FF98F587$��1��$��29��$
��Left=�$��0,23,0,0q�$	��Top=�$	��206��.+��� -   $��label9��$��Image��$��Cooper)�$��14(�
�������$
��Left=�$��Center=�$��Center=�$	��115������ $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF006000��$ 0Q�� $ 	#FF14E214��$ 1Q�� $ 	#FF5CEF5C��$ 0.772Q��� -   $��label10��$��Digit��$��Cooper)�$��14(�
�������$��Right=�$��Center=�$��Center=�$��90������ $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF006000��$ 0Q�� $ 	#FF14E214��$ 1Q�� $ 	#FF5CEF5C��$ 0.772Q��� -   $��label11��$��Validation Data��$��Cooper)�$��14(����	#FF347909$��28��$
��Left=�$	��Top=��� -   $��m_listBoxVal��$
��Left=�$��0,53,0,0q�$	��206���� -   $��m_buttonInit��$��
Initialize��$��23��$
��Left=�$��456,111,0,0q�$	��Top=�$	��121��
����������	#004EBE42$��0q���
� .��� $ White��$ 0Q�� $ 	#FF4BBA1A��$ 1Q��� -   $��m_buttonTrain�����	#004EBE42$��Train��
�������$��23��$
��Left=�$��593,111,0,0q�$	��Top=�$	��121��$��0q���
� .��� $ White��$ 0Q�� $ 	#FF4BBA1A��$ 1Q�� -   $��grid5��$��456,142,14,100q�.+��� -   $��label14��$��Created Classifiers��$��Cooper)�$��14(����	#FF347909$��28��$
��Left=�$	��Top=��� -   $��m_listBoxclf��$��0,23,0,0q�$��14(��� -   $��m_labelState��$��60��$
��Left=�$��457,570,0,0q�$	��Top=�$	��206��$��14(�$��Center=�
�������� -   $��m_progressBar��$��36��$
��Left=�$��669,585,0,0q�$	��Top=�$	��295���� -   $��m_buttonWrite�����	#004EBE42$��0q�$��
Write Data��
�������$��23��$��729,111,14,0q�$	��Top=���
� .��� $ White��$ 0Q�� $ 	#FF4BBA1A��$ 1Q��� -   $��
rectangle2��$��0,0,0,7q�$��22��$��Bottom=� K� ZIndex$
 -100������ $ 1,0.5&�$ 0,0.5&�.��� $ Transparent��$ 0Q�� $ 	#FF265821��$ 1Q����� 