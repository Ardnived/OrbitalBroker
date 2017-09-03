using UnityEngine;
using UnityEngine.UI;

public class UI_ListHeader : MonoBehaviour {

	public Text title;

	public void SetTitle(string text) {
		title.text = text.ToUpper();
	}

}

