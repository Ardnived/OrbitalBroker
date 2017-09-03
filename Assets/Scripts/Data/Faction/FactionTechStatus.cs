using UnityEngine;
using System;
using System.Collections.Generic;

public struct FactionTechStatus {

	[NonSerialized]
	private Faction master;

	[SerializeField]
	private int[] pointsSpent;
	[SerializeField]
	private List<int> techIDsToAdvance;

	public void Init() {
		pointsSpent = new int[GameController.TechWeb.Techs.Length];
		techIDsToAdvance = new List<int>();
	}

	public void ReInitializeVariables(Faction faction) {
		this.master = faction;
	}

	public void Update() {
		for (int i = 0; i < techIDsToAdvance.Count; i++) {
			int id = techIDsToAdvance[i];
			pointsSpent[id]++;

			TechData tech = GameController.TechWeb.Techs[id];
			TechDifficulty difficulty = GameController.TechWeb.GetDifficulty(tech, master);

			if (difficulty.PointsRequired >= pointsSpent[id]) {
				pointsSpent[id] = Constant.TechStatus.Researched;
				tech.AwardTo(master);
				GameController.Data.Logs.Add(new Log("Researched "+tech.Name));
			}
		}

		techIDsToAdvance.Clear();
	}

	public bool MarkTech(int techID, bool marked) {
		if (marked) {
			if (techIDsToAdvance.Contains(techID)) {
				return false;
			} else {
				techIDsToAdvance.Add(techID);
			}
		} else {
			if (techIDsToAdvance.Contains(techID)) {
				techIDsToAdvance.Remove(techID);
			} else {
				return false;
			}
		}

		return true;
	}

}
