using UnityEngine;
using UnityEngine.UI;

public class UI_BlueprintSelectionView : MonoBehaviour {

	public BlueprintManager blueprintManager;

	[Header("UI Elements")]
	public Button createButton;
	public Button editButton;
	public Button duplicateButton;
	public Button deleteButton;

	private SatelliteController selectedBlueprint;

	public void Select(SatelliteController blueprint) {
		this.selectedBlueprint = blueprint;

		blueprintManager.Show(selectedBlueprint);
		editButton.interactable = (blueprint != null);
		//duplicateButton.interactable = (blueprint != null);
		//deleteButton.interactable = (blueprint != null);
	}

	public void Edit() {
		//blueprintManager.Show(selectedBlueprint);
	}

	public void Duplicate() {
		blueprintManager.Create(selectedBlueprint);
	}

	public void Delete() {
		blueprintManager.Delete(selectedBlueprint);
	}

}

