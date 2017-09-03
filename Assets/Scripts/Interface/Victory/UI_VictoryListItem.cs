using UnityEngine;
using UnityEngine.UI;

public class UI_VictoryListItem : UI_VictoryList.Item {

	public Image icon;
	public Text title;
	public Text total;

	protected override void Set(Faction faction) {
		title.text = faction.Name;
	}

}
