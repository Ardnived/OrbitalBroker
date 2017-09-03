using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviour {

	public static GameData Data {
		get { return instance.gameData; }
	}

	public static RegionMapData Map {
		get { return instance.mapData; }
	}

	public static TechWeb TechWeb {
		get { return instance.techWeb; }
	}

	public static ModuleDesign[] ModuleDesigns {
		get { return instance.moduleSet.ModuleDesigns; }
	}

	private static GameController instance;
	private static Action action = Action.None;
	private static string fileName = null;

	public static void Queue(Action action, string fileName = null) {
		GameController.action = action;
		GameController.fileName = fileName;
	}



	[SerializeField]
	private GameData gameData = null;

	[SerializeField]
	private RegionMapData mapData;
	[SerializeField]
	private TechWeb techWeb;
	[SerializeField]
	private FactionSet factionSet;
	[SerializeField]
	private ModuleSet moduleSet;

	void Awake() {
		#if UNITY_EDITOR
		if (instance != null) {
			Debug.LogError("There cannot be two GameControllers in the scene. Make sure there is only one.");
			Application.Quit();
		}
		#endif

		instance = this;
		Time.timeScale = Constant.DefaultTimeScale;
	}

	void Update() {
		if (action != Action.None) {
			try {
				Do(action);
				action = Action.None;
			} catch (System.Exception exception) {
				action = Action.None;
				throw exception;
			}
		}
	}

	private void Do(Action action) {
		switch (action) {
		case Action.NewGame:
			gameData = GameData.New(factionSet);
			On.GameLoad.Trigger();
			break;

		case Action.LoadGame:
			GameData loadedData = GameData.Load(fileName);

			if (loadedData != null) {
				gameData = loadedData;
				SatelliteManager.Instance.Clear();
				gameData.HandleDataFromLoad();
			} else {
				Debug.LogError("Could not load data file "+fileName);
			}

			On.GameLoad.Trigger();
			break;

		case Action.SaveGame:
			gameData.HandleDataForSave();
			gameData.Save(fileName);
			break;

		case Action.AdvanceTurn:
			Data.CurrentTurn++;
			Data.Logs.Clear();

			On.TurnAdvanceEarly.Trigger();
			On.TurnAdvanceMain.Trigger();
			On.TurnAdvanceLate.Trigger();
			On.AfterTurnAdvance.Trigger();

			List<Faction> victoriousFactions = new List<Faction>();

			for (int i = 0; i < Data.Factions.Length; i++) {
				Faction faction = Data.Factions[i];

				if (faction.VictoryStatus.Diplomatic.IsComplete || faction.VictoryStatus.Economic.IsComplete || faction.VictoryStatus.Scientific.IsComplete) {
					victoriousFactions.Add(faction);
				}
			}

			if (victoriousFactions.Count > 0) {
				// Show the end game screen and all that.
			}
			break;
		}
	}

	public void NextTurn() {
		Queue(Action.AdvanceTurn);
	}

	public enum Action {
		None,
		NewGame,
		LoadGame,
		SaveGame,
		AdvanceTurn,
	}


	#if UNITY_EDITOR
	/*
	[CustomEditor(typeof(GameController))]
	public class GameControllerEditor : Editor {
		public override void OnInspectorGUI() {
			GameController gameController = (GameController)this.target;

			if (GUILayout.Button("Load")) {
				gameController.LoadGame(gameController.FileName);
			}

			if (GUILayout.Button("Save")) {
				gameController.SaveGame(gameController.FileName);
			}

			if (GUILayout.Button("Serialize")) {
				gameController.gameData.Serialize();
			}

			if (GUILayout.Button("Deserialize")) {
				SatelliteManager.Instance.Clear();
				gameController.gameData.Deserialize();
			}

			DrawDefaultInspector();
		}

	}
	*/
	#endif

}

public enum CursorStyle {
	Default,
	ResizeDiagonalLeft,
	ResizeDiagonalRight,
	Reposition,
}

