using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_LogListItem : UI_LogList.Item {

	public Button button;
	public Text title;

	protected override void Set(Log log) {
		title.text = log.Message;
		button.interactable = (log.Callback != null);
	}

	public override void OnSelect(BaseEventData eventData) {
		Debug.Log("Invoked select");
		element.Callback.Invoke();
	}

}

