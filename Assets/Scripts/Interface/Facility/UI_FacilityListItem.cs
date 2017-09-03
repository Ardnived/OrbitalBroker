using UnityEngine;
using UnityEngine.UI;

public class UI_FacilityListItem : UI_FacilityList.Item {

	public Text title;
	public Image icon;
	public Text turns;

	protected new UI_FacilityList master;

	public override void SetMaster(UI_List<Facility> master) {
		this.master = master as UI_FacilityList;
		base.SetMaster(master);
	}

	protected override void Set(Facility facility) {
		int turnCount = facility.TurnsLeft;
		title.text = facility.Name;

		if (turnCount > 0 || facility.Status == FacilityStatus.PreConstruction || facility.Status == FacilityStatus.ActiveConstruction) {
			if (facility.Status == FacilityStatus.PreConstruction || facility.Status == FacilityStatus.ActiveConstruction) {
				icon.sprite = master.constructionIcon;
			} else {
				icon.sprite = master.icons[(int)facility.Mode];
			}

			if (turnCount < 0) {
				turns.text = "-";
			} else {
				turns.text = turnCount.ToString();
			}

			icon.gameObject.SetActive(true);
		} else {
			turns.text = "";
			icon.gameObject.SetActive(false);
		}
	}

}
