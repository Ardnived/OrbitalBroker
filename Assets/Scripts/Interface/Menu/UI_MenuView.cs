using UnityEngine;
using UnityEngine.UI;

public class UI_MenuView : MonoBehaviour {

	public Camera menuCamera;
	public GameObject menuContainer;
	public GameObject gameContainer;
	public GameController gameController;

	[Header("Sub Menus")]
	public GameObject mainMenuFrame;
	public GameObject saveGameFrame;
	public GameObject loadGameFrame;
	public GameObject settingsFrame;
	public GameObject loadingFrame;
	public GameObject confirmFrame;
	public GameObject errorFrame;

	public InputField saveFileNameInput;

	[Header("UI Elements")]
	public Image backgroundImage;
	public GameObject loadButtonContainer;
	public GameObject saveLoadButtonsContainer;
	public Text errorText;

	#if UNITY_EDITOR
	[Header("Editor Fields (For Testing")]
	[SerializeField]
	private bool _autoLoad; 
	[SerializeField]
	private string _fileName;
	#endif

	private bool initialMenu = true;
	private bool unsavedChanges = false;
	private string fileName;

	private MenuButton lastPressed = MenuButton.ShowMainMenu;
	private GameObject lastFrame;
	private GameObject visibleFrame;

	#if UNITY_EDITOR
	void Start() {
		if (_autoLoad) {
			if (string.IsNullOrEmpty(_fileName)) {
				Press(MenuButton.NewGame);
			} else {
				SetFileName(_fileName);
				Press(MenuButton.LoadGame);
			}
		}
	}
	#endif

	private void ShowFrame(GameObject frame) {
		if (visibleFrame != null) {
			visibleFrame.SetActive(false);
			lastFrame = visibleFrame;
		}

		frame.SetActive(true);
		visibleFrame = frame;
	}

	private void ShowError(string message) {
		errorText.text = message;
		ShowFrame(errorFrame);
	}

	private void ShowGame() {
		unsavedChanges = true;
		menuContainer.SetActive(false);
		gameContainer.SetActive(true);
		ShowFrame(mainMenuFrame);

		if (initialMenu) {
			menuCamera.gameObject.SetActive(false);
			backgroundImage.gameObject.SetActive(true);
			saveLoadButtonsContainer.SetActive(true);
			loadButtonContainer.SetActive(false);
			initialMenu = false;
		}
	}

	public void Press(string button) {
		Press((MenuButton) System.Enum.Parse(typeof(MenuButton), button));
	}

	public void Press(MenuButton button) {
		switch (button) {
		case MenuButton.NewGame:
			if (unsavedChanges) {
				ShowFrame(confirmFrame);
			} else {
				GameController.Queue(GameController.Action.NewGame);
				ShowGame();
			}
			break;

		case MenuButton.LoadGame:
			if (string.IsNullOrEmpty(fileName)) {
				ShowError("You didn't select a file.");
			} if (unsavedChanges) {
				ShowFrame(confirmFrame);
			} else {
				GameController.Queue(GameController.Action.LoadGame, fileName);
				ShowGame();
			}
			break;

		case MenuButton.SaveGame:
			if (string.IsNullOrEmpty(fileName)) {
				ShowError("You must choose a save name.");
			} else {
				GameController.Queue(GameController.Action.SaveGame, fileName);
				unsavedChanges = false;
				ShowFrame(mainMenuFrame);
			}
			break;

		case MenuButton.ShowMainMenu:
			ShowFrame(mainMenuFrame);
			break;

		case MenuButton.ShowLoadGame:
			ShowFrame(loadGameFrame);
			break;

		case MenuButton.ShowSaveGame:
			SetFileName(saveFileNameInput.text);
			ShowFrame(saveGameFrame);
			break;

		case MenuButton.ShowSettings:
			ShowFrame(settingsFrame);
			break;

		case MenuButton.Exit:
			Application.Quit();
			break;

		case MenuButton.Confirm:
			unsavedChanges = false;
			Press(lastPressed);
			break;

		case MenuButton.Cancel:
			ShowFrame(mainMenuFrame);
			break;

		case MenuButton.Back:
			ShowFrame(lastFrame);
			break;
		}

		lastPressed = button;
	}

	public void SetFileName(string fileName) {
		this.fileName = fileName;
	}



	public enum MenuButton {
		NewGame,
		SaveGame,
		LoadGame,
		ShowMainMenu,
		ShowSaveGame,
		ShowLoadGame,
		ShowSettings,
		SaveSettings,
		CloseMenu,

		Exit,
		Confirm,
		Cancel,
		Back,
	}

}

