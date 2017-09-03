using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_MarketView : MonoBehaviour {

	public GameObject mapFrame;

	[SerializeField]
	private Material mapMaterial;

	[Header("UI Elements")]
	public Text contentTitle;

	private MarketType type;
	private bool supplyMode = false;

	public void Start() {
		On.GameLoad.Do(UpdateDisplay);
		On.AfterTurnAdvance.Do(UpdateDisplay);
		ShowDemand(MarketType.Broadcast);
	}

	public void ShowDemand(MarketType type) {
		this.type = type;
		contentTitle.text = "MAP OF " + type.ToString().ToUpper() + " DEMAND";
		UpdateDisplay();
	}

	public void ShowSupply(MarketType type) {
		this.type = type;
		contentTitle.text = "MAP OF " + type.ToString().ToUpper() + " SUPPLY";
		supplyMode = true;
		UpdateDisplay();
	}

	private void UpdateDisplay() {
		if (supplyMode) {
			mapMaterial.SetTexture("_CellMap", GameController.Data.Markets.GetSupplyTexture(type));
		} else {
			mapMaterial.SetTexture("_CellMap", GameController.Data.Markets.GetTexture(type));
		}
	}

}

