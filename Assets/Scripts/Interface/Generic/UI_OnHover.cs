using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// TODO: Make this class work....
public class UI_OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public OnChangeEvent OnChange;

	public void OnPointerEnter(PointerEventData data) {
		OnChange.Invoke(true);
	}

	public void OnPointerExit(PointerEventData data) {
		OnChange.Invoke(false);
	}

	[System.Serializable]
	public class OnChangeEvent : UnityEvent<bool> {}
}

