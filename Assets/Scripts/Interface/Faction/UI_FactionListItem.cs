using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_FactionListItem : UI_FactionList.Item {

	public Image logo;
	public Text title;
	public Text subtitle;

	protected override void Set(Faction faction) {
		//logo.color = faction.Color;
		title.text = faction.Name;
		//subtitle.text = "¥" + System.String.Format("{0:n0}", faction.Funds);
	}

}
