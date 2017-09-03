using UnityEngine;
using UnityEngine.UI;

public class UI_EndGameView : MonoBehaviour {

	public UI_DiplomaticVictoryListItem diplomaticVictoryDetails;
	public UI_EconomicVictoryListItem economicVictoryDetails;
	public UI_ScientificVictoryListItem scientificVictoryDetails;

	[Header("UI Elements")]
	public Text title;

	public void ShowDiplomaticVictory(Faction faction) {
		diplomaticVictoryDetails.gameObject.SetActive(true);
		economicVictoryDetails.gameObject.SetActive(false);
		scientificVictoryDetails.gameObject.SetActive(false);

		diplomaticVictoryDetails.SetElement(faction);
	}

	public void ShowEconomicVictory(Faction faction) {
		diplomaticVictoryDetails.gameObject.SetActive(false);
		economicVictoryDetails.gameObject.SetActive(true);
		scientificVictoryDetails.gameObject.SetActive(false);

		economicVictoryDetails.SetElement(faction);
	}

	public void ShowScientificVictory(Faction faction) {
		diplomaticVictoryDetails.gameObject.SetActive(false);
		economicVictoryDetails.gameObject.SetActive(false);
		scientificVictoryDetails.gameObject.SetActive(true);

		scientificVictoryDetails.SetElement(faction);
	}

	private void ShowVictory(Faction faction) {
		if (faction.ID == Constant.PlayerFactionID) {
			title.text = "VICTORY";
		} else {
			title.text = "DEFEAT";
		}
	}

}

