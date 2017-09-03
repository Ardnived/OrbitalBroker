using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TechData {
	public string Name;

	public int ID;

	[TextArea]
	public string Description;

	public int Difficulty;
	public int[] Synergies;

	public Rewards Reward;

	#if UNITY_EDITOR
	public Sprite Icon;
	#endif

	public int GetAdjustedDifficulty(Faction faction) {
		int result = Difficulty;

		for (int i = 0; i < Synergies.Length; i++) {
			int techID = Synergies[i];
			if (faction.ResearchLevels[techID] == Constant.TechStatus.Researched) {
				result--;
			}
		}

		if (GameController.Data.TechStatus[ID].Status != GlobalTechState.TechStatus.Secret) {
			result = Mathf.CeilToInt(result / 2f);
		}

		return result > 1 ? result : 1;
	}

	public void AwardTo(Faction faction) {
		Debug.Log("Awarding Tech #"+ID+" to "+faction.Name);
		faction.ResearchLevels[ID] = Constant.TechStatus.Researched;

		if (Reward.DesignID > 0) {
			faction.AvailableModuleIDs.Add(Reward.DesignID);
			On.UpdateModuleDesigns.Trigger();
		}

		faction.TechFlags |= (int) Reward.Flag;
	}

	[Serializable]
	public struct Rewards {
		public int DesignID;
		public TechFlag Flag;
	}

}

[Flags]
public enum TechFlag {
	None = 0,
	WeightReduction = 1,
	MaintenanceReduction = 2,
	DecommissionRefund = 4,
	GPSImprovement = 8,
	ModuleLimitIncrease = 16,
}
