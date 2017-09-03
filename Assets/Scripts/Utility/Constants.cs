
public static class Constant {

	public const float Kepler = 0.00000000000009905f;
	public const long GravitationalParameter = 398600441800000;

	public const int PlanetRadius = 6371;
	public const int PlanetCircumference = 40030;
	public const float PlanetRotationSpeed = 360f / 24 / 60 / 60; // In degrees per second.
	public const float DefaultTimeScale = 1;//60 * 60; // in game seconds per real second, (therefore: 1 game hour per real second)

	public const int LatitudeCount = 180;
	public const int MaxLatitude = 90;
	public const int MinLatitude = -90;

	public const int LongitudeCount = 360;
	public const int MaxLongitude = 180;
	public const int MinLongitude = -180;

	/*
	public const int MinAltitude = 160;
	public const int MaxAltitude = 35786;
	*/

	public const short CostPerTechPoint = 200;
	public const short CostPerLaunchFacility = 1000;
	public const short CostPerResearchLab = 1200;
	public const short CostPerWeight = 30;
	public const short CostPerCopyrightPoint = 50;
	public const short ValuePerDemandPoint = 1;

	public const int CoreModuleID = 0;
	public const int PlayerFactionID = 0;
	public const int EmptyRegionID = 0;

	public static class Init {
		public const int Funds = 500;
		public const int TechPoints = 5;
		public static readonly int[] Techs = new int[] { 5, 9, 13, 21 };
	}

	public static class Market {
		public const short TypeCount = 4;

		public const short HardMaxDemand = 50;
		public const short SoftMaxDemand = 40;
		public const short SoftMinDemand = 20;
		public const short HardMinDemand = -10;

		public const short MaxUpTrend = 4;
		public const short MaxDownTrend = 4;

		public const short GraphDataPoints = 100;

		public static short[] DemandThresholds = new short[] { -1000, 5, 20, 30, 40 };
	}

	public static class TechStatus {
		public const short Researched = -1;

		public const short MinCopyrightID = 0; // or higher
		public const short PublicDomain = -1;
		public const short Secret = -2;
	}

	public static class Victory {
		public const float MarketShareThreshold = 0.5f;
	}
}
