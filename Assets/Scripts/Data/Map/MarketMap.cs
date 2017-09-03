using System;
using UnityEngine;

[Serializable]
public class MarketMap {
	
	[SerializeField]
	private int[] demand;
	[SerializeField]
	private int[] supply;
	[SerializeField]
	private bool[] trends;

	[HideInInspector]
	[SerializeField]
	private int[] totalSupply;

	[NonSerialized]
	private Texture2D[] textures;
	[NonSerialized]
	private bool[] textureUpToDate;
	[NonSerialized]
	private Color[] pixels;

	[NonSerialized]
	private Texture2D[] supplyTextures;
	[NonSerialized]
	private bool[] supplyTextureUpToDate;

	public MarketMap(RegionMapData regionData) {
		demand = new int[Constant.Market.TypeCount * regionData.Width * regionData.Height];
		supply = new int[Constant.Market.TypeCount * regionData.Width * regionData.Height];
		trends = new bool[Constant.Market.TypeCount * regionData.Width * regionData.Height];
		totalSupply = new int[Constant.Market.TypeCount];

		ReInitializeVariables(regionData);

		int index;
		for (int t = 0; t < Constant.Market.TypeCount; t++) {
			for (int x = 0; x < regionData.Width; x++) {
				for (int y = 0; y < regionData.Height; y++) {
					index = GetIndex(t, x, y);
					demand[index] = (int) (Constant.Market.HardMinDemand + (Constant.Market.HardMaxDemand - Constant.Market.HardMinDemand) * Mathf.PerlinNoise(x, y));
				}
			}
		}

		for (int i = 0; i < 100; i++) {
			Simulate();
		}
	}

	public void ReInitializeVariables(RegionMapData regionData) {
		textures = new Texture2D[Constant.Market.TypeCount];
		textureUpToDate = new bool[Constant.Market.TypeCount];

		supplyTextures = new Texture2D[Constant.Market.TypeCount];
		supplyTextureUpToDate = new bool[Constant.Market.TypeCount];

		pixels = new Color[regionData.Width * regionData.Height];

		for (int t = 0; t < Constant.Market.TypeCount; t++) {
			textures[t] = new Texture2D(regionData.Width, regionData.Height,TextureFormat.Alpha8, false, true);
			textures[t].filterMode = FilterMode.Point;

			supplyTextures[t] = new Texture2D(regionData.Width, regionData.Height,TextureFormat.Alpha8, false, true);
			supplyTextures[t].filterMode = FilterMode.Point;
		}
	}

	public void AdjustSupply(MarketType type, OrbitData orbit, int value) {
		if (value == 0) {
			return;
		}

		int t = (int)type;
		int index;
		int total = 0;

		for (int i = 0; i < orbit.Coverage.Length; i++) {
			OrbitData.CoveragePoint point = orbit.Coverage[i];

			for (int x = -point.Radius; x <= point.Radius; x++) {
				for (int y = -point.Radius; y <= point.Radius; y++) {
					LatLong latlong = new LatLong(point.Latitude + x, point.Longitude + y);

					int latDiff = latlong.Latitude - point.Latitude;
					int lonDiff = latlong.Longitude - point.Longitude;
					float distance = Mathf.Sqrt(latDiff * latDiff + lonDiff * lonDiff);

					if (distance <= point.Radius) {
						try {
							index = GetIndex(t, latlong.Latitude - Constant.MinLatitude, latlong.Longitude - Constant.MinLongitude);
							supply[index] += value;
							total += value;
						} catch (Exception exc) {
							Debug.Log("Attempting to set "+t+" "+(latlong.Latitude - Constant.MinLatitude)+", "+(latlong.Longitude - Constant.MinLongitude)+" += "+value);
							throw exc;
						}
					}
				}
			}
		}

		Debug.Log("Added "+value+" -> "+total+" supply to "+type+" market");

		totalSupply[t] += total;
	}

	public void Simulate() {
		RegionMapData regionData = GameController.Map;
		RegionData region;
		float chanceOfUpTrend;
		int adjustment = 0;
		int index;

		for (int t = 0; t < Constant.Market.TypeCount; t++) {
			for (int x = 0; x < regionData.Width; x++) {
				for (int y = 0; y < regionData.Height; y++) {
					region = regionData.GetRegionData(x, y);

					if (region.ID != Constant.EmptyRegionID) {
						index = GetIndex(t, x, y);

						if (trends[index]) {
							chanceOfUpTrend = 0.6f;
						} else {
							chanceOfUpTrend = 0.4f;
						}

						if (demand[index] > Constant.Market.SoftMaxDemand) {
							chanceOfUpTrend -= (chanceOfUpTrend / Mathf.Abs(demand[index] - Constant.Market.SoftMaxDemand));
						} else if (demand[index] < Constant.Market.SoftMinDemand) {
							chanceOfUpTrend += ((1f - chanceOfUpTrend) / Mathf.Abs(demand[index] - Constant.Market.SoftMinDemand));
						}

						chanceOfUpTrend += regionData.GetMarketTendency(region.Wealth, (MarketType)t);

						if (UnityEngine.Random.Range(0f, 1f) <= chanceOfUpTrend) {
							adjustment = UnityEngine.Random.Range(0, Constant.Market.MaxUpTrend);
							trends[index] = true;
						} else {
							adjustment = -UnityEngine.Random.Range(0, Constant.Market.MaxDownTrend);
							trends[index] = false;
						}

						demand[index] = Mathf.Clamp(demand[index] + adjustment, Constant.Market.HardMinDemand, Constant.Market.HardMaxDemand);
					}
				}
			}

			textureUpToDate[t] = false;
		}
	}

	private void UpdateSupplyTexture(MarketType type) {
		RegionMapData regionData = GameController.Map;
		int t = (int)type;
		int index;
		int value;

		for (int x = 0; x < regionData.Width; x++) {
			for (int y = 0; y < regionData.Height; y++) {
				if (regionData.GetRegionID(x, y) > Constant.EmptyRegionID) {
					index = GetIndex(t, x, y);
					value = Mathf.Clamp(supply[index], 0, Constant.Market.HardMaxDemand);
					pixels[y * regionData.Width + x] = new Color(0, 0, 0, 0.2f + (value / (10 * 1.25f)));
				}
			}
		}

		supplyTextures[t].SetPixels(pixels);
		supplyTextures[t].Apply();
		supplyTextureUpToDate[t] = true;
	}

	private void UpdateTexture(MarketType type) {
		RegionMapData regionData = GameController.Map;
		int t = (int)type;
		int index;
		int value;

		for (int x = 0; x < regionData.Width; x++) {
			for (int y = 0; y < regionData.Height; y++) {
				if (regionData.GetRegionID(x, y) > Constant.EmptyRegionID) {
					index = GetIndex(t, x, y);
					value = Mathf.Clamp(demand[index], 0, Constant.Market.HardMaxDemand);
					pixels[y * regionData.Width + x] = new Color(0, 0, 0, 0.2f + (value / (Constant.Market.HardMaxDemand * 1.25f)));
				}
			}
		}

		textures[t].SetPixels(pixels);
		textures[t].Apply();
		textureUpToDate[t] = true;
	}

	public Texture2D GetSupplyTexture(MarketType type) {
		if (!supplyTextureUpToDate[(int)type]) {
			UpdateSupplyTexture(type);
		}

		return supplyTextures[(int)type];
	}

	public Texture2D GetTexture(MarketType type) {
		if (!textureUpToDate[(int)type]) {
			UpdateTexture(type);
		}

		return textures[(int)type];
	}

	private int GetIndex(int t, int x, int y) {
		return x + GameController.Map.Width * (y + GameController.Map.Height * t);
	}

	public int GetDemand(MarketType type, int x, int y) {
		int index = GetIndex((int)type, x, y);
		return demand[index];
	}

	public int GetSupply(MarketType type, int x, int y) {
		int index = GetIndex((int)type, x, y);
		return supply[index];
	}

	public int GetTotalSupply(MarketType type) {
		return totalSupply[(int)type];
	}

}
