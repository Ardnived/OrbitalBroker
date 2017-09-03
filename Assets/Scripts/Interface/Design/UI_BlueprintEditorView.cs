using UnityEngine;
using UnityEngine.UI;

public class UI_BlueprintEditorView : MonoBehaviour {

	public BlueprintManager blueprintManager;

	[Header("UI Elements")]
	public InputField nameField;
	public Button saveButton;

	private string blueprintName = "";

	public void Set(SatelliteController blueprint) {
		nameField.text = blueprint.Data.Name;
		SetName(blueprint.Data.Name);
	}

	public void SetName(string name) {
		blueprintName = name;
		saveButton.interactable = !string.IsNullOrEmpty(name);
	}

	public void Save() {
		blueprintManager.Save(blueprintName);
	}

}

