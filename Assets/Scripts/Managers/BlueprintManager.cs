using UnityEngine;
using System.Collections.Generic;

public class BlueprintManager : MonoBehaviour, SatelliteMouseHandler {

	public Shader highlightShader;
	public Shader deleteShader;
	public Shader defaultShader;

	//private SatelliteController draftBlueprint;
	private SatelliteController activeBlueprint;

	private Dictionary<string, ModuleLink> candidatePool;
	private ModuleLink candidateLink;
	private ModuleDesign candidateDesign;

	private bool isNew = false;

	// We use overloading here, for the sake of Unity's editor interface
	public void Create() {
		Create(null);
	}

	public void Create(SatelliteController duplicate) {
		SatelliteController blueprint;

		if (duplicate == null) {
			blueprint = SatelliteManager.Instance.CreateBlueprint();

			ModuleDesign design = GameController.ModuleDesigns[Constant.CoreModuleID];
			ModuleLink link = SatelliteManager.Instance.CreateModule(design.Model);
			ModuleData data = new ModuleData(design);

			blueprint.AddModuleData(link, data);
			link.transform.SetParent(blueprint.transform, false);
			link.SetController(blueprint);
			link.EnableAnchors();
		} else {
			// TODO: We have to clone the duplicate.Data object, because it is a reference type
			//blueprint = SatelliteManager.Instance.CreateBlueprint(duplicate.Data);
			blueprint = SatelliteManager.Instance.CreateBlueprint();
		}

		Show(blueprint);
		isNew = true;
	}

	public void Show(SatelliteController blueprint = null) {
		Hide();
		this.activeBlueprint = blueprint;
		this.activeBlueprint.SetHandler(this); // TODO: We should probably only enable this when we are editing, not just when the blueprint is being shown
		this.activeBlueprint.gameObject.SetActive(true);
	}

	private void Hide() {
		if (this.activeBlueprint != null) {
			this.activeBlueprint.gameObject.SetActive(false);
			this.activeBlueprint.SetHandler(null);
		}
	}

	public void Delete(SatelliteController blueprint) {
		// TODO: Implement this
		Hide();
	}

	public void Save(string name) {
		#if UNITY_EDITOR
		activeBlueprint.name = name;
		#endif
		activeBlueprint.Data.Name = name;

		activeBlueprint.Data.CalculateProperties();

		if (isNew) {
			GameController.Data.PlayerFaction.Blueprints.Set(activeBlueprint);
			isNew = false;
		}
	}

	public void Cancel() {
		if (isNew) {
			Destroy(activeBlueprint);
			isNew = false;
		}
	}

	// ------------------------------

	public void OnAnchorHover(Anchor anchor) {
		if (candidateLink != null) {
			candidateLink.SetParentAnchor(anchor);
			candidateLink.gameObject.SetActive(true);
		}
	}

	public void OnAnchorSelected(Anchor selectedAnchor) {
		Debug.Log ("On Select Anchor");
		if (candidateLink != null) {
			ModuleData data = new ModuleData(candidateDesign);
			activeBlueprint.AddModuleData(candidateLink, data, selectedAnchor);

			SetCandidateHighlighted(false);
			candidateLink.EnableAnchors();
			candidateLink = null;
		}
	}

	public void OnModuleHover(ModuleData data, ModuleLink link, bool hover) {
		//SetCandidateHighlighted(hover, HighlightMode.Delete);
	}

	public void OnModuleSelected(ModuleData data, ModuleLink link) {
		// Do something
	}

	public void SetCandidateDesign(ModuleDesign moduleDesign) {
		if (activeBlueprint == null) {
			Debug.LogError("TODO: You shouldn't be able to select a module when there is no blueprint to attach it to.");
			return;
		}

		if (candidateLink != null) {
			// TODO: Consider keeping a candidate for each module on hand, and then only instantiating when needed.
			Destroy(candidateLink.gameObject);
		}

		candidateDesign = moduleDesign;
		candidateLink = SatelliteManager.Instance.CreateModule(moduleDesign.Model);
		candidateLink.SetController(activeBlueprint);

		/*
		if (activeBlueprint.Data.Modules.Count == 0) {
			ModuleData data = new ModuleData(candidateDesign);
			activeBlueprint.AddModuleData(candidateLink, data);

			candidateLink.transform.SetParent(activeBlueprint.transform, false);
			candidateLink.EnableAnchors();
			candidateLink = null;
		} else {
		*/
		SetCandidateHighlighted(true);
		candidateLink.gameObject.SetActive(false);
		//}
	}

	private void SetCandidateHighlighted(bool highlight, HighlightMode mode = HighlightMode.Create) {
		if (candidateLink != null) {
			Shader shader = highlightShader;

			if (highlight == false) {
				shader = defaultShader;
			} else if (mode == HighlightMode.Delete) {
				shader = deleteShader;
			}

			foreach (Renderer renderer in candidateLink.GetComponentsInChildren<Renderer>()) {
				renderer.material.shader = shader;
			}
		}
	}

	private enum HighlightMode {
		Create,
		Delete
	}

}