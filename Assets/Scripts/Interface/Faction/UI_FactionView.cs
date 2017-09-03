using UnityEngine;
using UnityEngine.UI;

public class UI_FactionView : MonoBehaviour {

	[Header("UI Elements")]
	public Image logo;
	public Text title;
	public Text relationshipText;

	public void Set(Faction faction) {
		//logo.sprite = faction.Logo;
		title.text = faction.Name;
		relationshipText.text = "";
	}

}

