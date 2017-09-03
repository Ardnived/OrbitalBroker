using UnityEngine;
using UnityEngine.UI;

public class UI_DiplomaticVictoryListItem : UI_VictoryListItem {
	
	public Text firstDelegateList;
	public Text secondDelegateList;

	protected override void Set(Faction faction) {
		base.Set(faction);

		FactionVictoryStatus.DiplomaticVictoryStatus status = faction.VictoryStatus.Diplomatic;
		total.text = (int)(status.TotalPercentage * 100) + "%";
		firstDelegateList.text = status.Delegates[0] + "\r\n" + status.Delegates[1] + "\r\n" + status.Delegates[2] + "\r\n" + status.Delegates[3] + "\r\n" + status.Delegates[4];
		secondDelegateList.text = status.Delegates[5] + "\r\n" + status.Delegates[6] + "\r\n" + status.Delegates[7] + "\r\n" + status.Delegates[8] + "\r\n" + status.Delegates[9];
	}

}
