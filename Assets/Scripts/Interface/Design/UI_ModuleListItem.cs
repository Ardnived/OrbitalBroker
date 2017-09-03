using UnityEngine;

public class UI_ModuleListItem : UI_ModuleList.Item {

	public UnityEngine.UI.Text title;

	protected override void Set(ModuleDesign moduleDesign) {
		title.text = moduleDesign.Name;
	}

}
