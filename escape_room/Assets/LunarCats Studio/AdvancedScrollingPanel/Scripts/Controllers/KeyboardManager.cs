using UnityEngine;
using System.Collections;

public class KeyboardManager : ControllersManager {

	// Reference to buttons GameObjects of Keyboard
	public GameObject buttonQuote;
	public GameObject buttonPlus;
	public GameObject buttonMinus;
	public GameObject button0;
	public GameObject button1;
	public GameObject button2;
	public GameObject button3;
	public GameObject button4;
	public GameObject button5;
	public GameObject button6;
	public GameObject button7;
	public GameObject button8;
	public GameObject button9;
	public GameObject buttonSemiColon;
	public GameObject buttonComa;
	public GameObject buttonPoint;
	public GameObject buttonSlash;
	public GameObject buttonAccoladeOpen;
	public GameObject buttonAccoladeClose;
	public GameObject buttonA;
	public GameObject buttonAlt;
	public GameObject buttonAlt_Gr;
	public GameObject buttonB;
	public GameObject buttonBackspace;
	public GameObject buttonC;
	public GameObject buttonCaps_Lock;
	public GameObject buttonCtrl_Left;
	public GameObject buttonCtrl_Right;
	public GameObject buttonD;
	public GameObject buttonDelete;
	public GameObject buttonDown;
	public GameObject buttonE;
	public GameObject buttonEnd;
	public GameObject buttonEnter;
	public GameObject buttonEsc;
	public GameObject buttonF;
	public GameObject buttonF1;
	public GameObject buttonF2;
	public GameObject buttonF3;
	public GameObject buttonF4;
	public GameObject buttonF5;
	public GameObject buttonF6;
	public GameObject buttonF7;
	public GameObject buttonF8;
	public GameObject buttonF9;
	public GameObject buttonF10;
	public GameObject buttonF11;
	public GameObject buttonF12;
	public GameObject buttonG;
	public GameObject buttonH;
	public GameObject buttonHome;
	public GameObject buttonI;
	public GameObject buttonInsert;
	public GameObject buttonJ;
	public GameObject buttonK;
	public GameObject buttonL;
	public GameObject buttonLeft;
	public GameObject buttonM;
	public GameObject buttonN;
	public GameObject buttonNum_Lock;
	public GameObject buttonO;
	public GameObject buttonP;
	public GameObject buttonPad_num_multiply;
	public GameObject buttonPad_num_plus;
	public GameObject buttonPad_num_minus;
	public GameObject buttonPad_num_point;
	public GameObject buttonPad_num_slash;
	public GameObject buttonPad_num_0;
	public GameObject buttonPad_num_1;
	public GameObject buttonPad_num_2;
	public GameObject buttonPad_num_3;
	public GameObject buttonPad_num_4;
	public GameObject buttonPad_num_5;
	public GameObject buttonPad_num_6;
	public GameObject buttonPad_num_7;
	public GameObject buttonPad_num_8;
	public GameObject buttonPad_num_9;
	public GameObject buttonPad_num_Enter;
	public GameObject buttonPage_Down;
	public GameObject buttonPage_Up;
	public GameObject buttonPause;
	public GameObject buttonPrint_Src;
	public GameObject buttonQ;
	public GameObject buttonR;
	public GameObject buttonRight;
	public GameObject buttonS;
	public GameObject buttonScroll;
	public GameObject buttonScroll_Lock;
	public GameObject buttonShift_Left;
	public GameObject buttonShift_Right;
	public GameObject buttonSpace;
	public GameObject buttonT;
	public GameObject buttonTab;
	public GameObject buttonU;
	public GameObject buttonUp;
	public GameObject buttonV;
	public GameObject buttonW;
	public GameObject buttonWindows_Left;
	public GameObject buttonWindows_Right;
	public GameObject buttonX;
	public GameObject buttonY;
	public GameObject buttonZ;
	public GameObject buttonBackSlash;
	public GameObject buttonTild;
	public GameObject body;

	// Use this for initialization
	void Start () {
		meshs = transform.Find ("Meshs").gameObject;
		
		// Add each buttons to the list
		buttons.Add (buttonQuote);				// 0
		buttons.Add (buttonPlus);				// 1
		buttons.Add (buttonMinus);				// 2
		buttons.Add (button0);					// 3
		buttons.Add (button1);					// 4
		buttons.Add (button2);					// 5
		buttons.Add (button3);					// 6
		buttons.Add (button4);					// 7
		buttons.Add (button5);					// 8
		buttons.Add (button6);					// 9
		buttons.Add (button7);					// 10
		buttons.Add (button8);					// 11
		buttons.Add (button9);					// 12
		buttons.Add (buttonSemiColon);			// 13
		buttons.Add (buttonComa);				// 14
		buttons.Add (buttonPoint);				// 15
		buttons.Add (buttonSlash);				// 16
		buttons.Add (buttonAccoladeOpen);		// 17
		buttons.Add (buttonAccoladeClose);		// 18
		buttons.Add (buttonA);					// 19
		buttons.Add (buttonAlt);				// 20
		buttons.Add (buttonAlt_Gr);				// 21
		buttons.Add (buttonB);					// 22
		buttons.Add (buttonBackspace);			// 23
		buttons.Add (buttonC);					// 24
		buttons.Add (buttonCaps_Lock);			// 25
		buttons.Add (buttonCtrl_Left);			// 26
		buttons.Add (buttonCtrl_Right);			// 27
		buttons.Add (buttonD);					// 28
		buttons.Add (buttonDelete);				// 29
		buttons.Add (buttonDown);				// 30
		buttons.Add (buttonE);					// 31
		buttons.Add (buttonEnd);				// 32
		buttons.Add (buttonEnter);				// 33
		buttons.Add (buttonEsc);				// 34
		buttons.Add (buttonF);					// 35
		buttons.Add (buttonF1);					// 36
		buttons.Add (buttonF2);					// 37
		buttons.Add (buttonF3);					// 38
		buttons.Add (buttonF4);					// 39
		buttons.Add (buttonF5);					// 40
		buttons.Add (buttonF6);					// 41
		buttons.Add (buttonF7);					// 42
		buttons.Add (buttonF8);					// 43
		buttons.Add (buttonF9);					// 44
		buttons.Add (buttonF10);				// 45
		buttons.Add (buttonF11);				// 46
		buttons.Add (buttonF12);				// 47
		buttons.Add (buttonG);					// 48
		buttons.Add (buttonH);					// 49
		buttons.Add (buttonHome);				// 50
		buttons.Add (buttonI);					// 51
		buttons.Add (buttonInsert);				// 52
		buttons.Add (buttonJ);					// 53
		buttons.Add (buttonK);					// 54
		buttons.Add (buttonL);					// 55
		buttons.Add (buttonLeft);				// 56
		buttons.Add (buttonM);					// 57
		buttons.Add (buttonN);					// 58
		buttons.Add (buttonNum_Lock);			// 59
		buttons.Add (buttonO);					// 60
		buttons.Add (buttonP);					// 61
		buttons.Add (buttonPad_num_multiply);	// 62
		buttons.Add (buttonPad_num_plus);		// 63
		buttons.Add (buttonPad_num_minus);		// 64
		buttons.Add (buttonPad_num_point);		// 65
		buttons.Add (buttonPad_num_slash);		// 66
		buttons.Add (buttonPad_num_0);			// 67
		buttons.Add (buttonPad_num_1);			// 68
		buttons.Add (buttonPad_num_2);			// 69
		buttons.Add (buttonPad_num_3);			// 70
		buttons.Add (buttonPad_num_4);			// 71
		buttons.Add (buttonPad_num_5);			// 72
		buttons.Add (buttonPad_num_6);			// 73
		buttons.Add (buttonPad_num_7);			// 74
		buttons.Add (buttonPad_num_8);			// 75
		buttons.Add (buttonPad_num_9);			// 76
		buttons.Add (buttonPad_num_Enter);		// 77
		buttons.Add (buttonPage_Down);			// 78
		buttons.Add (buttonPage_Up);			// 79
		buttons.Add (buttonPause);				// 80
		buttons.Add (buttonPrint_Src);			// 81
		buttons.Add (buttonQ);					// 82
		buttons.Add (buttonR);					// 83
		buttons.Add (buttonRight);				// 84
		buttons.Add (buttonS);					// 85
		buttons.Add (buttonScroll);				// 86
		buttons.Add (buttonScroll_Lock);		// 87
		buttons.Add (buttonShift_Left);			// 88
		buttons.Add (buttonShift_Right);		// 89
		buttons.Add (buttonSpace);				// 90
		buttons.Add (buttonT);					// 91
		buttons.Add (buttonTab);				// 92
		buttons.Add (buttonU);					// 93
		buttons.Add (buttonUp);					// 94
		buttons.Add (buttonV);					// 95
		buttons.Add (buttonW);					// 96
		buttons.Add (buttonWindows_Left);		// 97
		buttons.Add (buttonWindows_Right);		// 98
		buttons.Add (buttonX);					// 99
		buttons.Add (buttonY);					// 100
		buttons.Add (buttonZ);					// 101
		buttons.Add (buttonBackSlash);			// 102
		buttons.Add (buttonTild);				// 103
		buttons.Add (body);						// 104
	}

}
