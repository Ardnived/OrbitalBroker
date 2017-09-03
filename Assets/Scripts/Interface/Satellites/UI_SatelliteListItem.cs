using UnityEngine;
using UnityEngine.UI;

public class UI_SatelliteListItem : UI_SatelliteList.Item {

	public Text title;

	protected override void Set(SatelliteController satellite) {
		title.text = satellite.Data.Name;
	}

}
