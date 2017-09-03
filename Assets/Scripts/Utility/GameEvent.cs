using System;
using System.Collections.Generic;

public class On {
	
	public static Event GameLoad = new Event("GameLoad");
	public static Event TurnAdvanceEarly = new Event("TurnAdvance|Early");
	public static Event TurnAdvanceMain = new Event("TurnAdvance|Main");
	public static Event TurnAdvanceLate = new Event("TurnAdvance|Late");
	public static Event AfterTurnAdvance = new Event("TurnAdvance|After");

	public static Event UpdateModuleDesigns = new Event("Update|ModuleDesigns");
	public static Event UpdateTechPoints = new Event("Update|TechPoints");
	public static Event UpdateFunds = new Event("Update|Funds");

	public static Event UpdateSatellites = new Event("Update|Satellites");
	public static Event UpdateBlueprints = new Event("Update|Blueprints");
	public static Event UpdateFacilities = new Event("Update|Facilities");

	public class Event {
		public List<Action> listeners = new List<Action>();
		private bool wasTriggered = false;

		#if UNITY_EDITOR
		private string debugLabel;

		public Event(string debugLabel) {
			this.debugLabel = debugLabel;
		}
		#endif

		public void Trigger() {
			wasTriggered = true;

			for (int i = 0; i < listeners.Count; i++) {
				listeners[i].Invoke();
			}

			#if UNITY_EDITOR
			UnityEngine.Debug.Log("Triggered "+debugLabel);
			#endif
		}

		public void Do(Action callback) {
			listeners.Add(callback);

			if (wasTriggered) {
				callback.Invoke();
			}
		}
	}

}

