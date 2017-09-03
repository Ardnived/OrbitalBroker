using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ActivityListItem : UI_ActivityList.Item {

	public Image icon;
	public Text title;
	public Text subtitle;
	public Image progress;

	//private bool expired;

	protected new UI_ActivityList master;

	public override void SetMaster(UI_List<Activity> master) {
		this.master = master as UI_ActivityList;
		base.SetMaster(master);
	}

	protected override void Set(Activity evt) {
		title.text = evt.Title;
		subtitle.text = evt.Subtitle;
		icon.sprite = master.icons[(int)evt.Type];
		icon.color = Color.white;

		//expired = false;
	}

	void LateUpdate() {
		/*
		float value = 1f - (element.Expires - GameController.Data.GameTime) / element.Duration;
		progress.fillAmount = value;
		progress.color = master.progressGradient.Evaluate(value);

		if (expired && value >= 1.2) {
			element.OnPassedExpired();
		} else if (value >= 1) {
			element.OnExpired();
			expired = true;

			icon.color = master.expiredIconColor;
		}
		*/
	}

	public override void OnSelect(BaseEventData eventData) {
		Debug.Log("On click");
		element.OnClick();
	}

}

