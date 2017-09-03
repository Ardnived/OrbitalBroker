using System;
using System.Collections.Generic;

[Serializable]
public class ModuleData {
	public int DesignerID;
	public int DesignID;
	public int Repair;
	public ModuleMode EnergyMode;
	public List<ModuleData.Child> Children = new List<Child>();

	public ModuleData(ModuleDesign design) {
		//this.DesignerID = design.OwnerID;
		this.DesignID = design.ID;
	}

	public ModuleDesign GetDesign() {
		//return GameController.Data.GetDesign(DesignerID, DesignID);
		return GameController.ModuleDesigns[DesignID];
	}

	public void SetDesign(ModuleDesign design) {
		//DesignerID = design.OwnerID;
		DesignID = design.ID;
	}

	[Serializable]
	public struct Child {
		public string AnchorID;
		public int ModuleIndex;

		public Child(string anchorID, int moduleIndex) {
			this.AnchorID = anchorID;
			this.ModuleIndex = moduleIndex;
		}
	}
}

public enum ModuleMode {
	Shutdown,
	MinimalConsumption,
	EfficientConsumption,
	HighPower,
}
