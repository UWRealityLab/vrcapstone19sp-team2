using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour {

	/// 
	/// Enums definitions
	/// 
	public enum CLOSING_ANIMATION {NONE, FADE_AWAY }
	public enum ALIGNMENT {LEFT_ALIGNMENT, CENTERED, RIGHT_ALIGNMENT }
	public enum VERTICAL_ALIGNMENT {UPPER_ALIGNMENT, MIDDLE, LOWER_ALIGNMENT }
	protected enum COMMAND_TYPE {NONE, KEYBOARD_ICON, MOUSE_ICON, GAMEPAD_ICON, ICON, WAIT, PAUSE }

	/// 
	/// Public variables declaration
	/// 
	public GameObject UIPanel;						// Reference to the UI Panel GameObject
	public GameObject canvas;						// Reference to the canvas child GameObject
	public ALIGNMENT panelHAlignment = ALIGNMENT.CENTERED;		// Horizontal Alignment of the panel
	public VERTICAL_ALIGNMENT panelVAlignment = VERTICAL_ALIGNMENT.MIDDLE;		// Vertical Alignment of the panel
	public ALIGNMENT textAlignment = ALIGNMENT.CENTERED;		// Alignment of the panel
	public bool autoStart = false;					// True to automatically start the display of this message at Start Up
	public int defaultHeight = 170;					// UI Panel default height
	public int defaultWidth = 145;					// UI Panel default width
	[Multiline]
	public List<string> messageList = new List<string>();	// List of all messages for each pages, in case of multiple page
	public int nbrCharactersByLine = 40;			// Number of caracters by line
	[Range(0.1f, 10.0f)]
	public float textScrollingSpeed = 1.0f;			// Speed of scrolling message.
	[Range(0.1f, 10.0f)]
	public float openingSpeed = 1;					// Speed of unrool an apparition animation
	public float messageDuration = 5;				// Duration of the message in seconds
	public bool automaticPageSwitch = true;			// When set to true, transition between each page of message is automatic after messageDuration time. Otherwise, player must click on message to switch.
	public CLOSING_ANIMATION closingAnimation;		// Defines which animation must be played on closing message
	[Range(0.1f, 10.0f)]
	public float closingAnimationSpeed = 1.0f;		// Speed of closing animation

	// Controllers parameters
	public GamePadManager GamePadController;		// Reference to the gamepad controller object
	public KeyboardManager KeyboardController;		// Reference to the keyboard controller object
	public MouseManager MouseController;			// Reference to the mouse controller object
	public bool linkWithController = true;			// True to link gamepad icons with Animated GamePad model
	public bool automaticControllerPosition = true;	// True to link GamePad Position with UI Panel's height
	public int ControllerPadding = 0;				// Padding for GamePad height position relative to Panel
	public Color colorFocused;						// Color of UI Panel background when focused

	// Icons parameters
	public Sprite[] iconsList;						// List of icons
	public Sprite[] iconsGamePad;					// List of icons for GamePad buttons
	public Sprite[] iconsKeyboard;					// List of icons for GamePad buttons
	public Sprite[] iconsMouse;						// List of icons for GamePad buttons
	public Vector2 iconSize = new Vector2(48, 48);	// Size of icons
	public bool automaticIconSize = true;			// True to automatically set the size of icons according to text's font size

	/// 
	/// Protected variables declaration
	/// 
	protected RectTransform UIPanelRect;			// Reference to the Rect transform of UI Panel object
	protected Vector3 UIPanelDefaultPosition;		// Default position of the UI Panel. This value is initialized at startup with current UIPanel local position
	protected GameObject text;						// Reference to the first text object
	protected GameObject textModel;					// Reference to the Model of text
	protected Text text_text;						// Reference to the Text component of text object
	protected RectTransform textRect;				// Reference to the Rect transform of text object
	protected GameObject hiddenText;				// Reference to the hidden text object
	protected Text hiddenText_text;					// Reference to the text component of hidden text 
	protected RectTransform hiddenTextRect;			// Reference to the Rect transform component of hidden text
	protected int currentIndexDisplayed = 0;		// Temporary variable used to run through all characters in the message. It is equal to the index of the current character displayed
	protected string message;						// The message to display
	protected Color normalColor;					// Normal color of UI Panel
	protected List<GameObject> instanciatedIcons = new List<GameObject>();	// Instanciated prefabs of icons
	protected List<float> relativeRightPosition = new List<float>();	// Relative position from right of the text. This parameter is only used in text aligment is right
	protected List<bool> inCurrentLine = new List<bool>();				// Indicates if an icon n°i is in current displaying text's line
	protected List<GameObject> textList = new List<GameObject>();		// List of texts by lines
	protected int lineCharactersCount = 0;			// Number of caracters by line
	protected int linesCount = 0;					// Number of lines
	protected bool isFocused = false;				// True when player looks at the message
	protected int currentPage = 1;					// Index of current page
	protected bool isHidding = false;				// True when message is in process of hidding
	protected float messageWidth;					// Width of the ui message
	protected int waitingDuration = 0;				// Define the time to wait when executing a wait command

	protected static int OFFSET_GAMEPAD_Y_POSITION = 115;		// Offset for Gamepad Y position
	protected static int OFFSET_KEYBOARD_Y_POSITION = 150;		// Offset for Keyboard Y position
	protected static int OFFSET_MOUSE_Y_POSITION = 150;			// Offset for Mouse Y position

	// Use this for initialization
	virtual protected void Start () {
		if (UIPanel == null) {
			UIPanel = transform.Find ("UIPanel").gameObject;
		}
		UIPanelRect	= UIPanel.GetComponent<RectTransform> ();
		UIPanelDefaultPosition = UIPanelRect.localPosition;
	}
			 
	/// <summary>
	/// Set the duration time of the message in seconds
	/// </summary>
	/// <param name="_duration">Duration.</param>
	public void SetDuration(float _duration)
	{
		messageDuration = _duration;
	}

	/// <summary>
	/// Sets the list of message.
	/// </summary>
	/// <param name="messages_p">Messages p.</param>
	public void SetMessageList(List<string> messages_p)
	{
		messageList = messages_p;
	}

	/// <summary>
	/// Display a single the message.
	/// </summary>
	/// <param name="message_p">Message p.</param>
	public void DisplayMessage(string message_p)
	{
		messageList.Clear ();
		messageList.Add (message_p);
		StartMessageDisplay ();
	}
		
	/// <summary>
	/// Closes the message.
	/// </summary>
	public void CloseMessage()
	{
		// Cancel all previous invocations and coroutines
		CancelInvoke();
		StopAllCoroutines ();

		// Play appropriate closing animation
		switch(closingAnimation)
		{
		case CLOSING_ANIMATION.NONE:
			HideLater ();
			break;
		case CLOSING_ANIMATION.FADE_AWAY:
			FadeAway ();
			break;
		}
	}

	// Display next message if there is still more to display, or start fade away transition
	public void OnClick()
	{
		if (currentPage < messageList.Count) {
			// Display next message
			NextMessage();
		} else {
			// Close message
			CloseMessage();
		}
	}

	// Display next message
	void NextMessage()
	{
		// Cancel all previous invocations and coroutines
		CancelInvoke();
		StopAllCoroutines ();

		currentPage ++;
		
		// Hide Controllers
		HideControllers ();

		InitNewMessage ();
		// Start display of the new page
		DisplaySingleMessage (messageList[currentPage-1]);
	}

	// Display the message
	public void StartMessageDisplay()
	{
		// Cancel all previous invocations and coroutines
		CancelInvoke();
		StopAllCoroutines ();
		if (isHidding) {
			HideLater ();
		}

		// Initialize parameters
		InitNewMessage ();
		currentPage = 1;
		if (messageList.Count > 0) {
			DisplaySingleMessage (messageList [currentPage - 1]);
		}
	}

	// Unpause the message
	public void Unpause() {
		waitingDuration = 0;
	}

	// Pause the message
	public void Pause() {
		waitingDuration = -1;
	}

	// Initialize parameters to start display new message
	virtual protected void InitNewMessage()
	{
		// Reset parameters
		for (int i=1; i<textList.Count; i++) {
			Destroy(textList[i]);
		}
		text_text = text.GetComponent<Text> ();
		textRect = text.GetComponent<RectTransform> ();
		text_text.text = "";
		currentIndexDisplayed = 0;
		lineCharactersCount = 0;
		linesCount = 0;
		waitingDuration = 0;

		// Set icon size
		if (automaticIconSize) {
			iconSize.Set(text_text.fontSize, text_text.fontSize);
		}

		// Destroy previous hidden text
		if (hiddenText != null) {
			Destroy(hiddenText);
		}

		// Hide controllers
		HideControllers ();

		// Destroy all previous icons
		if (instanciatedIcons.Count > 0) {
			for (int i=0; i<instanciatedIcons.Count; i++) {
				instanciatedIcons [i].transform.SetParent(null);
				Destroy (instanciatedIcons [i]);
			}
			instanciatedIcons.Clear ();
			relativeRightPosition.Clear();
			inCurrentLine.Clear();
		}

		textList.Clear();
		textList.Add (text);

		text_text.CrossFadeAlpha(1.0f, 1/closingAnimationSpeed, false);

		// Create a hidden text element, used to calculate the size of the message
		hiddenText = Instantiate(text, text.transform.localPosition, text.transform.localRotation) as GameObject;
		hiddenText.name = "hiddenText";
		hiddenText.transform.SetParent (text.transform.parent);
		hiddenText.transform.localScale = text.transform.localScale;
		hiddenText.transform.localRotation = text.transform.localRotation;
		hiddenText.transform.localPosition = text.transform.localPosition;
		hiddenText_text = hiddenText.GetComponent<Text> ();
		hiddenText_text.text = "";
		hiddenText_text.color = new Color(0, 0, 0, 0);
		hiddenTextRect 	= hiddenText.GetComponent<RectTransform> ();
		ContentSizeFitter sizeFitter = hiddenText.GetComponent<ContentSizeFitter> ();
		sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		// Cancel all previous invocations
		CancelInvoke();
	}

	// Display the message
	virtual protected void DisplaySingleMessage(string message_p)
	{
	}

	// Main function where text animation is managed. At each call on this function a new character is displayed.
	virtual protected void ProceedNextCharacter()
	{
		// Process the waiting or pausing command if requested
		if (waitingDuration > 0) {
			waitingDuration--;
			return;
		} else if (waitingDuration == -1) {
			return;
		}

		// Process the next character
		if(message != "")
		{
			bool isACommand = false;
			// Check if a command must be executed
			if(message[currentIndexDisplayed].ToString() == "#" && currentIndexDisplayed+2 < message.Length) {				

				// Get the first character after #. It defines which command to execute
				string commandCharacter = message [currentIndexDisplayed + 1].ToString ();

				// Parse the character to get the corresponding command
				COMMAND_TYPE commandType = parseCommandType(commandCharacter);

				isACommand = true;

				// If command is Keyboard, Mouse or Gamepad icon, increment current index by one
				if (commandType != COMMAND_TYPE.ICON && commandType != COMMAND_TYPE.NONE) {
					currentIndexDisplayed++;
				}

				// Execute command
				if (commandType == COMMAND_TYPE.PAUSE) {
					ExecutePauseCommand ();
				} else {
					// Try to parse the number of the command
					int commandNumber = 0;
					if (!int.TryParse (message.Substring (currentIndexDisplayed + 1, 2), out commandNumber)) {
						currentIndexDisplayed++;
						// If next character is not a number, we cancel the process
						isACommand = false;
						return;
					}

					if (commandType == COMMAND_TYPE.GAMEPAD_ICON || commandType == COMMAND_TYPE.KEYBOARD_ICON || commandType == COMMAND_TYPE.MOUSE_ICON || commandType == COMMAND_TYPE.ICON) {
						if (!ExecuteIconCommand (commandType, commandNumber)) {
							return;	// Icon will be created next time
						}
					} else if (commandType == COMMAND_TYPE.WAIT) {
						ExecuteWaitCommand (commandNumber);
					}
				}

				currentIndexDisplayed += 2;		// Position index after the controller command text			
			} else {
				// Display the next character
				text_text.text = text_text.text + message[currentIndexDisplayed];
				hiddenText_text.text = hiddenText_text.text + message[currentIndexDisplayed];
				lineCharactersCount ++;
				CheckNextWordFit ();					
			}

			// Check on message width to see if it wiil exceed panel's width
			CheckMessageWidth();

			// Management for multiple line message
			if(lineCharactersCount >= nbrCharactersByLine && currentIndexDisplayed+1 < message.Length) {
				text_text.text = text_text.text + "\n";	// Insert '\n'
				AddNewLine ();
			}
			else if (message[currentIndexDisplayed].Equals('\n')) {
				AddNewLine ();
			}

			// If text is in right alignment, we need to adjust icons position
			if (textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
				if (!isACommand) {
					RightAlignmentAdjustIconPosition ();	
				}
			}
		}	

		// Whrn message is finished, wait for the next
		if(currentIndexDisplayed >= message.Length - 1)
		{
			CancelInvoke ("ProceedNextCharacter");
			if (automaticPageSwitch) {
				Invoke ("OnClick", messageDuration);
			}
		} else {
			// Display next character
			currentIndexDisplayed ++;
		}
	}

	protected COMMAND_TYPE parseCommandType(string commandCharacter_p) {
		COMMAND_TYPE parsedCommand = COMMAND_TYPE.NONE;

		// If text is #G, #K or #M we need to display a controller icon
		if (commandCharacter_p == "G") {
			parsedCommand = COMMAND_TYPE.GAMEPAD_ICON;
		} else if (commandCharacter_p == "M") {
			parsedCommand = COMMAND_TYPE.MOUSE_ICON;
		} else if (commandCharacter_p == "K") {
			parsedCommand = COMMAND_TYPE.KEYBOARD_ICON;
		} else if (commandCharacter_p == "W") {
			parsedCommand = COMMAND_TYPE.WAIT;
		} else if (commandCharacter_p == "P") {
			parsedCommand = COMMAND_TYPE.PAUSE;
		} else {
			parsedCommand = COMMAND_TYPE.ICON;
		}

		return parsedCommand;
	}

	// Process a Icon command
	protected bool ExecuteIconCommand(COMMAND_TYPE commandType_p, int iconToShow_p) {
		int spacingIconCount = 3;		// The amount of spacing characters needed to fill the size of an icon

		// Check if the icon's position will be out of the UI Panel's bounds
		if(lineCharactersCount + spacingIconCount > nbrCharactersByLine) {
			// We need to add a new line before creating the icon
			CompleteLineWithSpacing ();
			hiddenText_text.text = " ";
			text_text.text = text_text.text + "\n";	// Insert '\n'
			AddNewLine ();
			return false;	// The icon will be created next time
		}

		// Instanciate and position a new icon
		if(CreateNewIcon (commandType_p, iconToShow_p)) {
			// Add spacing characters to fit icon size
			for(int i=0; i<spacingIconCount; i++) {
				text_text.text = text_text.text + " ";
				hiddenText_text.text = hiddenText_text.text + " ";
			}

			lineCharactersCount += spacingIconCount;
		}

		// Show Controllers
		if (this.gameObject.activeInHierarchy) {
			ShowControllers (commandType_p, iconToShow_p);
		}
		return true;
	}

	// Process a wait command
	protected void ExecuteWaitCommand(int waitTime_p) {
		waitingDuration = waitTime_p;
	}

	// Process a pause command
	protected void ExecutePauseCommand () {
		waitingDuration = -1;
	}
		
		
	// Check if next word will exceed the maximal number of character by line
	// If so, we need to switch to the new line
	void CheckNextWordFit() {
		if (message.Length > currentIndexDisplayed + 1) {
			// If when are at the end of a word and next character is not a start command '#'
			if (message [currentIndexDisplayed].ToString () == " " && message [currentIndexDisplayed + 1].ToString () != "#") {
				// Get next index of spacing element
				int nextSpace = message.IndexOf (" ", currentIndexDisplayed + 1);
				// Get next index of new line element		
				int nextNewLine = message.IndexOf ("\n", currentIndexDisplayed + 1);
				// Take the minimum but positive from those two values, it give the index of the end of the current word.
				int endOfWordIndex = -1;
				if (nextSpace == -1 || nextNewLine == -1) {
					endOfWordIndex = Mathf.Max (nextSpace, nextNewLine);
				} else {
					endOfWordIndex = Mathf.Min (nextSpace, nextNewLine);
				}

				if (endOfWordIndex > -1) {
					// Check if the complete word will exceed panel's width. If so, we need to add a new line before begining to display the word
					if (lineCharactersCount + endOfWordIndex - currentIndexDisplayed > nbrCharactersByLine) {
						text_text.text = text_text.text + "\n";	// Insert '\n'
						AddNewLine ();
					}
				} else if (lineCharactersCount + message.Length - currentIndexDisplayed > nbrCharactersByLine) {
					text_text.text = text_text.text + "\n";	// Insert '\n'
					AddNewLine ();
				}
			}
		}
	}

	// Check if actual line exceeds UI panel width. If so, we need to resize UI panel
	void CheckMessageWidth() {
		if(hiddenTextRect.rect.width > UIPanelRect.rect.width - defaultWidth /*+ textRect.anchoredPosition.x*/ && currentIndexDisplayed > 0) {
			float difference = hiddenTextRect.rect.width - (UIPanelRect.rect.width - defaultWidth);

			// Take into account text's x position when alignment is left
			if (textAlignment == ALIGNMENT.LEFT_ALIGNMENT) {
				difference += textRect.anchoredPosition.x;
			}

			// Resize UI panel
			UIPanelRect.sizeDelta += new Vector2(difference, 0);

			// If Panel alignment is Left or Right, we have to translate it's position
			if (panelHAlignment == ALIGNMENT.LEFT_ALIGNMENT) {
				UIPanelRect.localPosition = UIPanelRect.localPosition + new Vector3(difference/2f, 0, 0);
			} else if(panelHAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
				UIPanelRect.localPosition = UIPanelRect.localPosition - new Vector3(difference/2f, 0, 0);
			}
			// Keep texts's alignment in UI panel
			if (textAlignment == ALIGNMENT.CENTERED || textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
				for (int i=0; i<textList.Count; i++) {
					if (textAlignment == ALIGNMENT.CENTERED) {
						textList [i].GetComponent<Text> ().rectTransform.localPosition = textList [i].GetComponent<Text> ().rectTransform.localPosition + new Vector3 (difference / 2.0f, 0, 0);
					} else if (textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
						textList [i].GetComponent<Text> ().rectTransform.localPosition = textList [i].GetComponent<Text> ().rectTransform.localPosition + new Vector3 (difference, 0, 0);
					}
				}

				// Keep alignment of current text line's icons in UI panel
				if(textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
					if(inCurrentLine.Count > 0) {
						for (int i=0; i<inCurrentLine.Count; i++) {
							if(inCurrentLine[i] == true) {
								instanciatedIcons [i].transform.localPosition = instanciatedIcons [i].transform.localPosition + new Vector3 (difference, 0, 0);
							}
						}
					}
				}
			}
		}
	}

	// Instanciate and position a new icon in the text
	bool CreateNewIcon(COMMAND_TYPE controllerType_p, int index_p) {
		Sprite newIcon;

		// Check if icon exists
		switch (controllerType_p) {
			case COMMAND_TYPE.GAMEPAD_ICON:
				if(index_p >= iconsGamePad.Length) {
					return false;
				}
				newIcon = iconsGamePad[index_p];
				break;

			case COMMAND_TYPE.KEYBOARD_ICON:
				if(index_p >= iconsKeyboard.Length) {
					return false;
				}
				newIcon = iconsKeyboard[index_p];
				break;

			case COMMAND_TYPE.MOUSE_ICON:
				if(index_p >= iconsMouse.Length) {
					return false;
				}
				newIcon = iconsMouse[index_p];
				break;

			case COMMAND_TYPE.NONE:
			default:
				if(index_p >= iconsList.Length) {
					return false;
				}
				newIcon = iconsList[index_p];
				break;
		}

		if (newIcon != null) {
			// Create, position and resize new icon
			GameObject img = new GameObject ("Icon" + index_p);
			instanciatedIcons.Add (img);
			img.AddComponent<Image> ();
			img.GetComponent<Image> ().sprite = Instantiate (newIcon) as Sprite;
			img.GetComponent<Image> ().rectTransform.sizeDelta = iconSize;
			img.transform.SetParent (textList[textList.Count-1].transform);
			img.transform.localScale = new Vector3 (1, 1, 1);
			img.transform.localRotation = Quaternion.identity;
			if (hiddenTextRect.rect.width > 0 && hiddenText_text.text == "") {
				// This manage the case where an icon is the frist element to display on the message
				img.transform.localPosition = new Vector3 (iconSize.x / 2, -(linesCount) * (hiddenTextRect.rect.height / 2.0f) * (text_text.lineSpacing), 0);
			} else {
				if(textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
					// When text alignment is right, icon's position must be adjust evry time the text is updated, because position is relative to the right
					img.transform.localPosition = new Vector3 (-iconSize.x / 2, 0, 0);
					relativeRightPosition.Add(hiddenTextRect.rect.width);
					inCurrentLine.Add(true);
				} else {
					img.transform.localPosition = new Vector3 (hiddenTextRect.rect.width + iconSize.x / 2, 0, 0);
				}
			}
			img.GetComponent<RectTransform> ().anchorMin = new Vector2 (0, 0.5f);
			img.GetComponent<RectTransform> ().anchorMax = new Vector2 (0, 0.5f);
			return true;
		} else {
			return false;
		}
	}

	// Complete the text with spacing caracters to fit exactly the maximal number of characters by line
	void CompleteLineWithSpacing() {
		for (int i = 0; i < nbrCharactersByLine - lineCharactersCount; i++) {
			text_text.text = text_text.text + " ";
		}
	}

	// Add a new line
	void AddNewLine()
	{
		NewLineUIPanelResizing();		// Resize UI panel's height
		lineCharactersCount = 0;
		linesCount ++;
		hiddenText_text.text = "";

		// Create the new line Text GameObject
		GameObject newLineText = Instantiate(textModel, textModel.transform.localPosition, textModel.transform.localRotation) as GameObject;
		newLineText.transform.SetParent (textModel.transform.parent);
		newLineText.transform.localScale = textModel.transform.localScale;
		newLineText.transform.localRotation = textModel.transform.localRotation;
        newLineText.transform.localPosition = Vector3.zero;
        newLineText.GetComponent<RectTransform>().anchoredPosition = textList[textList.Count - 1].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -(hiddenTextRect.rect.height / 2.0f) * text_text.lineSpacing);
		newLineText.name = "TextLine" + linesCount;
		text_text = newLineText.GetComponent<Text> ();
		textList.Add (newLineText);

		// If text alignment is right, all icon's position in current line must not be adjust anymore 
		if(textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
			if(inCurrentLine.Count > 0) {
				for (int i=0; i<inCurrentLine.Count; i++) {
					inCurrentLine[i] = false;
				}
			}
		}
	}

	// Adjust position of icons when text alignment is Right
	void RightAlignmentAdjustIconPosition() {
		if(relativeRightPosition.Count > 0) {
			for (int i=0; i<relativeRightPosition.Count; i++) {
				if(inCurrentLine[i] == true) {
					instanciatedIcons [i].transform.localPosition = new Vector3(relativeRightPosition[i]-hiddenTextRect.rect.width, instanciatedIcons [i].transform.localPosition.y, 0);				
				}
			}
		}
	}

	// Hide all the controllers
	void HideControllers() {
		if (linkWithController) {
			if(GamePadController != null) {
				GamePadController.setActive (false);
			}
			if(KeyboardController != null) {
				KeyboardController.setActive (false);
			}
			if(MouseController != null) {
				MouseController.setActive (false);
			}
		}
	}

	// Show the controller linked with the icon
	void ShowControllers(COMMAND_TYPE controllerType_p, int iconToShow_p) {
		if (linkWithController) {

			// Assign the controller linked to the icon
			ControllersManager controller = null;
			int controllerOffset = 0;

			switch (controllerType_p) {

			case COMMAND_TYPE.GAMEPAD_ICON:
				if (GamePadController != null) {
					controller = GamePadController;
					controllerOffset = OFFSET_GAMEPAD_Y_POSITION;
				}
				break;

			case COMMAND_TYPE.KEYBOARD_ICON:
				if (KeyboardController != null) {
					controller = KeyboardController;
					controllerOffset = OFFSET_KEYBOARD_Y_POSITION;
				}
				break;

			case COMMAND_TYPE.MOUSE_ICON:
				if (MouseController != null) {
					controller = MouseController;
					controllerOffset = OFFSET_MOUSE_Y_POSITION;
				}
				break;
			}

			if (controller != null) {
				controller.setActive (true);
				controller.startFadeIn ();
				controller.HighlightButton (iconToShow_p);
				if (automaticControllerPosition) {
					// Set Y position of the controller relative to Panel's rect and depending on the vertical alignment
					if (panelVAlignment == VERTICAL_ALIGNMENT.UPPER_ALIGNMENT) {
						controller.SetYPosition (UIPanelRect.rect.height + ControllerPadding + controllerOffset);
					} else if (panelVAlignment == VERTICAL_ALIGNMENT.MIDDLE) {
						controller.SetYPosition (UIPanelRect.rect.height / 2.0f + ControllerPadding + controllerOffset);
					} else if (panelVAlignment == VERTICAL_ALIGNMENT.LOWER_ALIGNMENT) {
						controller.SetYPosition (defaultHeight + ControllerPadding + controllerOffset);
					}

					// Set X position of the controller to be centered with Panel's rect
					if (panelHAlignment == ALIGNMENT.LEFT_ALIGNMENT) {
						controller.SetXPosition (UIPanel.transform.position.x + UIPanelRect.rect.width / 2.0f);
					} else if (panelHAlignment == ALIGNMENT.CENTERED) {
						controller.SetXPosition (0);
					} else if (panelHAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
						controller.SetXPosition (UIPanel.transform.position.x - UIPanelRect.rect.width / 2.0f);
					}
				}
			}
		}
	}

	// Start fade out transition for actives controllers
	void FadeOutControllers() {
		if (linkWithController) {
			if(GamePadController != null && GamePadController.isActive) {
				GamePadController.startFadeOut ();
			}
			if(KeyboardController != null && KeyboardController.isActive) {
				KeyboardController.startFadeOut ();
			}
			if(MouseController != null && MouseController.isActive) {
				MouseController.startFadeOut ();
			}
		}
	}

	// Resize UI Panel's height when a new line is added 
	virtual protected void NewLineUIPanelResizing()
	{
		// Translate icons
		if (instanciatedIcons.Count > 0) {
			for (int i=0; i<instanciatedIcons.Count; i++) {
				instanciatedIcons [i].transform.localPosition = instanciatedIcons [i].transform.localPosition + new Vector3(0, (hiddenTextRect.rect.height / 2.0f) * text_text.lineSpacing, 0);
			}
		}
	}

	// Fade away animation
	virtual protected void FadeAway()
	{
		for (int i = 0; i < textList.Count; i++) {
			textList[i].GetComponent<Text>().CrossFadeAlpha (0.0f, 1 / closingAnimationSpeed, false);
		}
		for (int i=0; i<instanciatedIcons.Count; i++) {
			instanciatedIcons [i].GetComponent<Image> ().CrossFadeAlpha (0.0f, 1/closingAnimationSpeed, false);
		}
		if (this.gameObject.activeInHierarchy) {
			FadeOutControllers ();
		}
		Invoke("HideLater", 1/closingAnimationSpeed);
		isHidding = true;
	}

	// Hide the message
	virtual protected void HideLater()
	{
		isHidding = false;
		HideControllers ();
		if (hiddenText != null) {
			Destroy(hiddenText);
		}
		for (int i=0; i<instanciatedIcons.Count; i++) {
			Destroy(instanciatedIcons[i]);
		}
		instanciatedIcons.Clear ();
		relativeRightPosition.Clear ();
		inCurrentLine.Clear ();
		for (int i=1; i<textList.Count; i++) {
			Destroy(textList[i]);
		}
		text_text = text.GetComponent<Text> ();
		textRect = text.GetComponent<RectTransform> ();
		textList.Clear ();
		text_text.text = "";
		textRect.sizeDelta.Set (0, 0);
	}

	// Animation for unroll UI Panel
	IEnumerator OpeningMessage()
	{
		float t = 0f;
		while (t <= 1) {
			AnimationOpenningMessage (t);
			t += openingSpeed * Time.deltaTime;
			yield return null;
		}
		
		AnimationOpenningMessageFinished ();
		
		// Start displaying message
		InvokeRepeating("ProceedNextCharacter", 0.03f / textScrollingSpeed, 0.03f / textScrollingSpeed);
	}
	
	// Animation to open message
	virtual protected void AnimationOpenningMessage(float step_p)
	{		
	}
	
	// Animation to open message
	virtual protected void AnimationOpenningMessageFinished()
	{		
	}

}
