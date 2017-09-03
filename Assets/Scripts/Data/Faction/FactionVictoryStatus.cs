using System;
using UnityEngine;

public struct FactionVictoryStatus {

	public DiplomaticVictoryStatus Diplomatic;
	public EconomicVictoryStatus Economic;
	public ScientificVictoryStatus Scientific;

	public void Init() {
		Diplomatic.Delegates = new int[GameController.Map.Regions.Length];
		Economic.MarketShares = new float[Constant.Market.TypeCount];
		Scientific.Steps = new float[3];
	}

	public void UpdateAll(Faction faction) {
		Diplomatic.Update(faction);
		Economic.Update(faction);
		Scientific.Update(faction);
	}


	public struct DiplomaticVictoryStatus {
		public bool IsComplete;

		public int[] Delegates;
		public float TotalPercentage;

		public void Update(Faction faction) {

		}
	}

	public struct EconomicVictoryStatus {
		public bool IsComplete;

		public float[] MarketShares;
		public int TotalMajorities;

		public void Update(Faction faction) {
			MarketMap market = GameController.Data.Markets;
			int count = 0;

			for (int t = 0; t < Constant.Market.TypeCount; t++) {
				int totalMarketShare = market.GetTotalSupply((MarketType)t);

				if (totalMarketShare > 0) {
					float marketShare = faction.TotalMarketSupply[t] / totalMarketShare;
					MarketShares[t] = marketShare;

					if (marketShare >= Constant.Victory.MarketShareThreshold) {
						count++;
					}
				}
			}

			TotalMajorities = count;
			IsComplete = (count >= Constant.Market.TypeCount);
		}
	}

	public struct ScientificVictoryStatus {
		public bool IsComplete;

		public float[] Steps;
		public int TotalStepsCompleted;

		public void Update(Faction faction) {
			Steps[0] = 0;

			if (faction.ResearchLevels[1] == Constant.TechStatus.Researched) {
				Steps[0] += 0.25f;
			}

			if (faction.ResearchLevels[2] == Constant.TechStatus.Researched) {
				Steps[0] += 0.25f;
			}

			if (faction.ResearchLevels[3] == Constant.TechStatus.Researched) {
				Steps[0] += 0.25f;
			}

			if (faction.ResearchLevels[4] == Constant.TechStatus.Researched) {
				Steps[0] += 0.25f;
			}

			if (faction.ResearchLevels[0] == Constant.TechStatus.Researched) {
				Steps[1] = 1f;
			} else {
				Steps[1] = 0f;
			}

		}
	}

}

