using UnityEngine;
using UnityEngine.UI;

public class UI_EconomicVictoryListItem : UI_VictoryListItem {

	public Image[] marketFills;

	protected override void Set(Faction faction) {
		base.Set(faction);

		FactionVictoryStatus.EconomicVictoryStatus status = faction.VictoryStatus.Economic;
		total.text = status.TotalMajorities + "/" + Constant.Market.TypeCount;;

		for (int t = 0; t < Constant.Market.TypeCount; t++) {
			marketFills[t].fillAmount = status.MarketShares[t];
		}
	}

}
