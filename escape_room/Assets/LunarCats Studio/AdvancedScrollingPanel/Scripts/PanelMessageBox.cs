using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelMessageBox : MessageManager {

	/// 
	/// Private variables declaration
	/// 
	private Image leftBorderImage;						// Reference to the Image component of LeftBorder object
	private Image centerImage;							// Reference to the Image component of Center object
	private Image rightBorderImage;						// Reference to the Image component of RightBorder object
	private RectTransform centerImageRect;				// Reference to the Rect Transform component of the Center object
	private float lineHeightFactor = 1.0f;				// Multiplication factor for panel's height resizing when adding a new line
	private float originTextYPosition = 0;				// Default text's y position.

	// Use this for initialization
	override protected void Start () {
		base.Start ();

		// Storing usefull references
		leftBorderImage = UIPanel.transform.Find("LeftBorder").GetComponent<Image>();
		centerImage = UIPanel.transform.Find("Center").GetComponent<Image>();
		rightBorderImage = UIPanel.transform.Find("RightBorder").GetComponent<Image>();
		text = UIPanel.transform.Find("Center").Find ("Text").gameObject;
		text_text = text.GetComponent<Text> ();
		textRect = text.GetComponent<RectTransform> ();
		textModel = Instantiate(text, text.transform.localPosition, text.transform.localRotation) as GameObject;
		textModel.transform.SetParent (text.transform.parent);
		textModel.transform.localPosition = text.transform.localPosition;
		textModel.transform.localRotation = text.transform.localRotation;
		textModel.transform.localScale = text.transform.localScale;
		textModel.name = "ModelText";
		textModel.GetComponent<Text> ().text = "";
		centerImageRect = centerImage.GetComponent<RectTransform>();
		normalColor = leftBorderImage.color;

		originTextYPosition = textRect.anchoredPosition.y;

		// Display message when this GameObject is activating is autoStart is checked
		if (autoStart) {
			StartMessageDisplay ();
		}
	}

	override protected void AnimationOpenningMessage(float step_p)
	{
		// We gradually resize UI Panel
		UIPanelRect.sizeDelta = Vector2.Lerp(new Vector2(0, defaultHeight + (hiddenTextRect.rect.height/2.0f) * text_text.lineSpacing), new Vector2(hiddenTextRect.rect.width + defaultWidth, defaultHeight + (hiddenTextRect.rect.height/2.0f) * text_text.lineSpacing), step_p);

		// If Panel alignment is Left or Right, we have to translate it's position
		if (panelHAlignment == ALIGNMENT.LEFT_ALIGNMENT) {
			UIPanelRect.localPosition = Vector3.Lerp (UIPanelDefaultPosition, new Vector3 ((hiddenTextRect.rect.width + defaultWidth) / 2f, UIPanelDefaultPosition.y, UIPanelDefaultPosition.z), step_p);
		} else if (panelHAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
			UIPanelRect.localPosition = Vector3.Lerp (UIPanelDefaultPosition, new Vector3 (-(hiddenTextRect.rect.width + defaultWidth) / 2f, UIPanelDefaultPosition.y, UIPanelDefaultPosition.z), step_p);
		}
	}

	override protected void AnimationOpenningMessageFinished()
	{
		// Set text object's parameters according to alignment
		if (textAlignment == ALIGNMENT.CENTERED) {
			textRect.anchoredPosition = new Vector2 (centerImageRect.rect.width / 2.0f, textRect.anchoredPosition.y);
			text.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			text.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			text_text.alignment = TextAnchor.MiddleCenter;
			textModel.GetComponent<RectTransform>().anchoredPosition = textRect.anchoredPosition;
			textModel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			textModel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			textModel.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		} else if (textAlignment == ALIGNMENT.LEFT_ALIGNMENT) {
			textRect.anchoredPosition = new Vector2 (leftBorderImage.rectTransform.rect.width, textRect.anchoredPosition.y);
			text.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			text.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			text_text.alignment = TextAnchor.MiddleLeft;
			textModel.GetComponent<RectTransform>().anchoredPosition = textRect.anchoredPosition;
			textModel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			textModel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			textModel.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
		} else if (textAlignment == ALIGNMENT.RIGHT_ALIGNMENT) {
			textRect.anchoredPosition = new Vector2 (centerImageRect.rect.width - (UIPanelRect.rect.width-hiddenTextRect.rect.width)/2.0f, textRect.anchoredPosition.y);
			text.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			text.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			text_text.alignment = TextAnchor.MiddleRight;
			textModel.GetComponent<RectTransform>().anchoredPosition = textRect.anchoredPosition;
			textModel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			textModel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			textModel.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
		}
		hiddenText_text.text = "";
	}

	// Initialize parameters to start displaying the new message
	override protected void InitNewMessage()
	{
		base.InitNewMessage ();

		textRect.anchoredPosition = new Vector2 (0, originTextYPosition);

		leftBorderImage.CrossFadeAlpha (1.0f, 1/closingAnimationSpeed, false);
		centerImage.CrossFadeAlpha (1.0f, 1/closingAnimationSpeed, false);
		rightBorderImage.CrossFadeAlpha (1.0f, 1/closingAnimationSpeed, false);

		// Activate the canvas
		canvas.SetActive(false);
		canvas.SetActive(true);
		UIPanelRect.sizeDelta = new Vector2(0, defaultHeight + (hiddenTextRect.rect.height/2.0f) * text_text.lineSpacing);
		UIPanelRect.localPosition = new Vector3(0, 0, 0);
	}

	// Display the message
	override protected void DisplaySingleMessage(string message_p)
	{
		message = message_p;

		// Calculate the width of the UI panel's message testing each line's length
		string[] lines = message.Split ('\n');
		for (int i=0; i<lines.Length; i++) {
			// Calculate the number characters used for commands inside message. These characters are not displayed and must not be taken into account in the total width of the panel
			string[] commands = lines[i].Split('#');
			int commandCharactersCount = 0;
			for (int j=0; j<commands.Length; j++) {
				if(commands[j].Length>1) {
					COMMAND_TYPE commandType = parseCommandType(commands[j][0].ToString());				

					if (commandType == COMMAND_TYPE.PAUSE) {
						commandCharactersCount++;
					} else if (commandType == COMMAND_TYPE.WAIT) {
						commandCharactersCount += 3;
					}
					// If command is Keyboard, Mouse or Gamepad icon, increment current index by one
					else if (commandType == COMMAND_TYPE.GAMEPAD_ICON || commandType == COMMAND_TYPE.KEYBOARD_ICON || commandType == COMMAND_TYPE.MOUSE_ICON) {
						int commandNumber = 0;
						if (int.TryParse (commands [j].ToString ().Substring (1, 2), out commandNumber)) {
							commandCharactersCount += 3;
						} else {
							commandCharactersCount++;
						}
					} else if (commandType == COMMAND_TYPE.ICON) {
						commandCharactersCount+=2;
					}
					commandCharactersCount++;
				}
			}			
				
			if (lines[i].Length - commandCharactersCount < nbrCharactersByLine) {
				if(lines[i].Length - commandCharactersCount > hiddenText_text.text.Length) {
					hiddenText_text.text = lines[i].Substring(0, lines[i].Length - commandCharactersCount);
				}
			} else {
				hiddenText_text.text = lines[i].Substring(0, nbrCharactersByLine);
			}
		}			

		// Start displaying text
		StartCoroutine ("OpeningMessage");
	}

	// Resize UI Panel's height when a new line is added 
	override protected void NewLineUIPanelResizing()
	{
		base.NewLineUIPanelResizing ();
		float UIPanelHeightAugmentation = hiddenTextRect.rect.height * lineHeightFactor * text_text.lineSpacing;
		float controllerTranslation = UIPanelHeightAugmentation/2;

		UIPanelRect.sizeDelta = UIPanelRect.sizeDelta + new Vector2 (0, UIPanelHeightAugmentation);
		if (panelVAlignment == VERTICAL_ALIGNMENT.UPPER_ALIGNMENT) {
			UIPanelRect.anchoredPosition = UIPanelRect.anchoredPosition + new Vector2(0, UIPanelHeightAugmentation/2.0f);
			controllerTranslation = UIPanelHeightAugmentation;
		} else if (panelVAlignment == VERTICAL_ALIGNMENT.LOWER_ALIGNMENT) {
			UIPanelRect.anchoredPosition = UIPanelRect.anchoredPosition - new Vector2(0, UIPanelHeightAugmentation/2.0f);
			controllerTranslation = 0;
		}

		// Move controller on Y axis
		if(automaticControllerPosition && linkWithController) {
			if(GamePadController != null && GamePadController.isActive) {
				GamePadController.TranslatePositionY (controllerTranslation);
			}
			if(KeyboardController != null && KeyboardController.isActive) {
				KeyboardController.TranslatePositionY (controllerTranslation);
			}
			if(MouseController != null && MouseController.isActive) {
				MouseController.TranslatePositionY (controllerTranslation);
			}
		}
			
		for (int i=0; i<textList.Count; i++) {
			if(linesCount != i) {
				if (textList [i] != null) {
					textList [i].GetComponent<Text> ().text = textList [i].GetComponent<Text> ().text + '\n';
				}
			}
			// Reposition texts GameObject when origin Y position is not 0
			if(originTextYPosition != 0) {
				if (textList [i] != null) {
					textList [i].GetComponent<RectTransform> ().anchoredPosition = textList [i].GetComponent<RectTransform> ().anchoredPosition + new Vector2 (0, originTextYPosition / 2.0f);
				}
			}
		}
	}

	// Fade away transition
	override protected void FadeAway()
	{
		leftBorderImage.CrossFadeAlpha (0.0f, 1/closingAnimationSpeed, false);
		centerImage.CrossFadeAlpha (0.0f, 1/closingAnimationSpeed, false);
		rightBorderImage.CrossFadeAlpha (0.0f, 1/closingAnimationSpeed, false);
		base.FadeAway ();
	}

	// Hide the message
	override protected void HideLater()
	{
		base.HideLater ();
		// Deactivate the canvas
		canvas.SetActive(false);
	}

	// Change background color when object is focused
	public void SetFocus(bool isFocused_p) 
	{
		isFocused = isFocused_p;
		if (isFocused) {
			leftBorderImage.color = colorFocused;
			centerImage.color = colorFocused;
			rightBorderImage.color = colorFocused;
		} else {
			leftBorderImage.color = normalColor;
			centerImage.color = normalColor;
			rightBorderImage.color = normalColor;
		}
	}
}
