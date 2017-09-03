using System;

public class SatelliteDataCollection : OwnedDataCollection<SatelliteController> {

	public SatelliteDataCollection(int ownerID, On.Event eventChannel) : base(ownerID, eventChannel) {
 		// Do nothing
	}

	public SatelliteDataCollection Reset(SatelliteData[] array, bool asBlueprint = false) {
		Clear();
		this.Deserialize(array, asBlueprint);
		return this;
	}

	public new SatelliteData[] Serialize() {
		SatelliteData[] result = new SatelliteData[data.Count];
		int i = 0;

		foreach (SatelliteController satellite in data.Values) {
			result[i] = satellite.Data;
			i++;
		}

		return result;
	}

	public void Deserialize(SatelliteData[] array, bool asBlueprint = false) {
		SatelliteController[] result = new SatelliteController[array.Length];

		for (int i = 0; i < array.Length; i++) {
			if (asBlueprint) {
				result[i] = SatelliteManager.Instance.CreateBlueprint(data: array[i]);
			} else {
				result[i] = SatelliteManager.Instance.CreateSatellite(data: array[i]);
			}
		}

		base.Deserialize(result);
	}

}
