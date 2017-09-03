using UnityEngine;
using UnityEngine.UI;

public class UI_ScientificVictoryListItem : UI_VictoryListItem {

	public Image[] objectiveFrames;

	protected override void Set(Faction faction) {
		base.Set(faction);

		FactionVictoryStatus.ScientificVictoryStatus status = faction.VictoryStatus.Scientific;
		total.text = status.TotalStepsCompleted + "/" + status.Steps.Length;

		for (int i = 0; i < status.Steps.Length; i++) {
			objectiveFrames[i].fillAmount = status.Steps[i];
		}
	}

}
