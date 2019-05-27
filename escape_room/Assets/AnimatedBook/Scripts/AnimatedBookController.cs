using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnimatedBookController : MonoBehaviour {

	public enum BOOK_STATE
	{
		CLOSED,
		OPENING,
		OPENED,
		CLOSING
	}

	public enum PAGES_TRANSITIONS
	{
		NONE,
		TURNING_RIGHT,
		TURNING_LEFT
	}

	[System.Serializable]
	public class PageObjects
	{
		public Transform page;
		public Image RectoImage;
		public Image VersoImage;
		[System.NonSerialized]
		public GameObject UiRecto;		// Instantiated Ui recto for the current page
		[System.NonSerialized]
		public GameObject UiVerso;		// Instantiated Ui verso for the current page
	}

	// Define the UI for Recto and Verso
	[System.Serializable]
	public class Page
	{
		public GameObject UiRecto;		// Panel for recto
		[Tooltip("Leave is empty to use default background image")]
		public Sprite RectoBackground;		// Image for recto background
		public GameObject UiVerso;		// Panel for verso
		[Tooltip("Leave is empty to use default background image")]
		public Sprite VersoBackground;		// Image for recto background
	}

	/// <summary>
	/// Public variables
	/// </summary>
	public Transform rightRotationReference;						// Right orientation reference for pages 
	public Transform leftRotationReference;							// Left orientation reference for pages 
	[Tooltip("The default background image of all pages")]
	public Sprite defaultBackground;								// Default background image for pages
	public Transform pagesParent;									// Reference to the parent GameObject of all pages
	[Tooltip("Defines the pages of this book")]
	public Page[] pagesUi;											// The UI prefabs associated to each pages
	[Tooltip("Adjust quick rotation speed (when double tap is done)")]
	[Range(1f, 10)] public float quickRotationSpeed = 2;			// Speed for quick rotation
	public AudioClip pageTurnSound;									// Sound effect for flipping page

	/// <summary>
	/// Public actions
	/// </summary>
	public System.Action _onBookClose;								// Event triggered when the book is closed
	public System.Action _onBookOpen;								// Event triggered when the book is opened

	/// <summary>
	/// Private variables
	/// </summary>
	private PageObjects[] bookPages;								// List of all 3 page's
	private Quaternion pageUnturnedRotation;						// Local rotation of an unturned page
	private Quaternion pageTurnedRotation;							// Local rotation of a turned page
	private Quaternion pageInitRot;									// Initial rotation of pages
	private BOOK_STATE state = BOOK_STATE.CLOSED;					// Current state in which the book is
	private Animator anim;											// Reference to the animator component
	private Quaternion initGlobalQuat;								// Global rotation of the pages relative to the book
	private Quaternion unturnedLocalQuat;							// Local rotation for a non turned page
	private Quaternion turnedLocalQuat;								// Local rotation for a turned page
	private float t = 0;											// Used for animation progression
	private float speed = 1;										// Speed of turning page animation
	private int currentPage = 0;									// The index of the current page visible
	private PAGES_TRANSITIONS inTransition = PAGES_TRANSITIONS.NONE;// True when a page is being turned


	// Getter and setter for book state
	public void SetOpened() {
		state = BOOK_STATE.OPENED;

		// Notify listeners that the book is opened
		if(_onBookOpen != null) {
			_onBookOpen.Invoke();
		}
	}

	public void SetOpening() {
		state = BOOK_STATE.OPENING;
	}

	public void SetClosing() {
		state = BOOK_STATE.CLOSING;
	}

	public void SetClosed() {
		state = BOOK_STATE.CLOSED;

		// Notify listeners that the book is closed
		if(_onBookClose != null) {
			_onBookClose.Invoke();
		}
	}

	public BOOK_STATE getBookState() {
		return state;
	}

	public PageObjects[] getPageObjects() {
		return bookPages;
	}
		
	// Use this for initialization
	void Start () {

		InitReferences ();

		// Set the turned and unturned rotation values
		pageUnturnedRotation = Quaternion.Euler(0, 89, 0);
		pageTurnedRotation = Quaternion.Euler (0, 271, 0);

		// Deactivate invisible pages 2 and 3 at startup
		bookPages [1].page.gameObject.SetActive (false);
		bookPages [2].page.gameObject.SetActive (false);

		if (pagesUi.Length == 0) {
			// Case where book have no page
			bookPages [0].page.gameObject.SetActive (false);
		} else {
			ActivatePage(0, currentPage);
		}
	}

	// Initialize the variables references
	private void InitReferences() {
		// Get the animator reference
		anim = GetComponent<Animator> ();

		// Get usefull references for all 3 book pages
		bookPages = new PageObjects[3];
		for(int i=0; i<3; i++) {
			PageObjects page = new PageObjects();
			Transform pageTransform = pagesParent.Find("Page" + i);
			page.page = pageTransform;
			page.RectoImage = pageTransform.Find ("Recto").Find ("CanvasRecto").GetComponent<Image>();
			page.VersoImage = pageTransform.Find ("Verso").Find ("CanvasVerso").GetComponent<Image>();
			bookPages[i] = page;
		}
	}

	// Activate the page number i, setting it's sprite and UI
	private void ActivatePage(int i, int pageIndex) {
		// Activate the current page's gameobject
		bookPages [i].page.gameObject.SetActive (true);

		if(bookPages [i].UiRecto != null) {
			// Destroy previous instantiated Ui Recto if exists
			Destroy (bookPages [i].UiRecto);
		}
		// Setting the Recto background Image
		if (pagesUi [pageIndex].RectoBackground != null) {
			bookPages [i].RectoImage.sprite = pagesUi [pageIndex].RectoBackground;
		} else {
			bookPages [i].RectoImage.sprite = defaultBackground;
		}
		// Setting the Ui Recto
		if (pagesUi [pageIndex].UiRecto != null) {
			bookPages [i].UiRecto = Instantiate (pagesUi [pageIndex].UiRecto);
			bookPages [i].UiRecto.transform.SetParent (bookPages [i].RectoImage.transform, false);
		}

		if(bookPages [i].UiVerso != null) {
			// Destroy previous instantiated Ui Verso if exists
			Destroy (bookPages [i].UiVerso);
		}
		// Setting the Verso background Image
		if (pagesUi [pageIndex].VersoBackground != null) {
			bookPages [i].VersoImage.sprite = pagesUi [pageIndex].VersoBackground;
		} else {
			bookPages [i].VersoImage.sprite = defaultBackground;
		}
		// Setting the Ui Verso
		if (pagesUi [pageIndex].UiVerso != null) {
			bookPages [i].UiVerso = Instantiate (pagesUi [pageIndex].UiVerso);
			bookPages [i].UiVerso.transform.SetParent (bookPages [i].VersoImage.transform, false);
		}
	}

	// Deactivate the page number i
	private void DeactivatePage(int i) {
		Destroy (bookPages [i].UiRecto);
		Destroy (bookPages [i].UiVerso);
		bookPages [i].page.gameObject.SetActive (false);
	}
	
	// Used to override pages rotation during Opening and Closing Animation
	void LateUpdate () {
		if (state == BOOK_STATE.OPENING || state == BOOK_STATE.CLOSING) {
			for (int i=0; i< bookPages.Length; i++) {
				//bookPages[i].page.rotation = pageInitRot * transform.rotation;
				if (currentPage == 0 && leftRotationReference != null) {
					bookPages [i].page.rotation = leftRotationReference.rotation;
				} else if(rightRotationReference != null) {
					bookPages [i].page.rotation = rightRotationReference.rotation;
				}
			}
		}
	}

	// Play flip page sound
	private void PlayFlipPageSound() {
		if (pageTurnSound != null) {
			AudioSource.PlayClipAtPoint (pageTurnSound, transform.position);
		}
	}

	public void TurnToNextPage() {
		// If book is closed, open it
		if (state == BOOK_STATE.CLOSED && currentPage == 0) {
			anim.SetTrigger ("OpenBook");
		}
		// If book is opened ...
		else if (state == BOOK_STATE.OPENED) {
			// If we are not in the last page, we turn the page
			if (currentPage < pagesUi.Length) {
				// If we asked another time to turn the page, double the transition speed
				if(inTransition == PAGES_TRANSITIONS.TURNING_RIGHT) {
					ImproveTransitionSpeed();
				} 
				// If we asked to turn the page, or turn the other side while in transition
				else {
					StopAllCoroutines();
					StartCoroutine ("TurnToNextPageTransition");
				}
			}
			// If we are in the last page, we close the book
			else {
				anim.SetTrigger ("CloseBookLeft");
			}
		}
	}

	public void TurnToPreviousPage() {
		// If book is closed, open it
		if (state == BOOK_STATE.CLOSED) {
			anim.SetTrigger ("OpenBook");
		}
		// If book is opened ...
		else if (state == BOOK_STATE.OPENED) {
			if (currentPage >= 0) {
				// If we are in the first page and not in transition, close the book
				if (currentPage == 0 && inTransition == PAGES_TRANSITIONS.NONE) {
					anim.SetTrigger("CloseBookRight");
					return;
				} 
				// Otherwise turn the page
				else if (currentPage > 0 && inTransition == PAGES_TRANSITIONS.NONE) {
					currentPage --;
				}

				// If we asked another time to turn the page, double the transition speed
				if(inTransition == PAGES_TRANSITIONS.TURNING_LEFT) {
					ImproveTransitionSpeed();
				} 
				// If we asked to turn the page, or turn the other side while in transition
				else {
					StopAllCoroutines ();
					StartCoroutine ("TurnToPreviousPageTransition");
				}
			}
		}
	}

	// Improve the transition speed
	public void ImproveTransitionSpeed() {
		speed = quickRotationSpeed;
	}

	// Animation during turning to next page
	IEnumerator TurnToNextPageTransition() {		
		bool hasActivatedNextPage = false;

		// Play flip page sound
		PlayFlipPageSound();

		if (inTransition == PAGES_TRANSITIONS.TURNING_LEFT) {
			t = 1 - t;
		}
		inTransition = PAGES_TRANSITIONS.TURNING_RIGHT;
		while (t < 1) {
			bookPages[currentPage % 3].page.localRotation = Quaternion.Lerp(pageUnturnedRotation, pageTurnedRotation, t);
			t += Time.deltaTime * speed;

			if (t > 0.05f && !hasActivatedNextPage) {
				// Activate the next page just after animation has started to prevent visual overlap
				if (currentPage + 1 < pagesUi.Length) {
					ActivatePage((currentPage + 1) % 3, currentPage + 1);
					bookPages [(currentPage + 1) % 3].page.localRotation = pageUnturnedRotation;
				}
				hasActivatedNextPage = true;
			}

			yield return new WaitForFixedUpdate();
		}
		bookPages [currentPage % 3].page.localRotation = pageTurnedRotation;

		// Deactivate the previous page
		DeactivatePage((currentPage + 2) % 3);

		currentPage ++;
		if(currentPage > pagesUi.Length) {
			currentPage = pagesUi.Length;
		}

		TransitionFinished ();
	}

	// Animation during turning to previous page
	IEnumerator TurnToPreviousPageTransition() {		
		bool hasActivatedPreviousPage = false;
		bool hasDeactivatedNextPage = false;

		// Play flip page sound
		PlayFlipPageSound();

		if (inTransition == PAGES_TRANSITIONS.TURNING_RIGHT) {
			t = 1 - t;
		}
		inTransition = PAGES_TRANSITIONS.TURNING_LEFT;
		while (t < 1) {
			bookPages[currentPage % 3].page.localRotation = Quaternion.Lerp(pageTurnedRotation, pageUnturnedRotation, t);
			t += Time.deltaTime * speed;

			if (t > 0.05f && !hasActivatedPreviousPage) {
				// Activate the previous page just after animation has started to prevent visual overlap
				if (currentPage > 0) {
					ActivatePage ((currentPage + 2) % 3, currentPage - 1);
					bookPages [(currentPage + 2) % 3].page.localRotation = pageTurnedRotation;
				}
				hasActivatedPreviousPage = true;
			}
			if (t > 0.95f && !hasDeactivatedNextPage) {
				// Deactivate the next page just before animation finish to prevent visual overlap
				DeactivatePage((currentPage + 1) % 3);
				hasDeactivatedNextPage = true;
			}

			yield return new WaitForFixedUpdate();
		}
		bookPages [currentPage % 3].page.localRotation = pageUnturnedRotation;

		TransitionFinished ();
	}

	// When transition is finished, reset variables used during transition
	private void TransitionFinished() {
		t = 0;
		inTransition = PAGES_TRANSITIONS.NONE;
		speed = 1;
	}
}
