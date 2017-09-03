using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_FileListItem : UI_FileList.Item {

	public Text title;

	protected override void Set(string fileName) {
		title.text = fileName + ".game";
	}

}
