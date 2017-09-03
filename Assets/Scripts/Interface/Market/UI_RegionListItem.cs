using UnityEngine;
using UnityEngine.UI;

public class UI_RegionListItem : UI_RegionList.Item {

	public Text title;

	protected override void Set(RegionData regionData) {
		title.text = regionData.Name;
	}

}
