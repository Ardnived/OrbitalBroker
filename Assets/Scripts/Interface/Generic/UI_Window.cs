using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Window : MonoBehaviour {

	public void ToggleActive() {
		this.gameObject.SetActive(this.gameObject.activeSelf);
	}

}
