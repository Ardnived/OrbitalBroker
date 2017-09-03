using UnityEngine;
using UnityEngine.UI;

public class UI_MarketTypeListItem : UI_MarketTypeList.Item {

	public Text title;

	protected override void Set(MarketType demandType) {
		title.text = demandType.ToString();
	}

}
