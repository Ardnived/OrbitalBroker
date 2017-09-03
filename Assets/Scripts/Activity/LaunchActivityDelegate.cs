
public class LaunchActivityDelegate : ActivityDelegate {

	public void Setup(Activity activity) {
		SatelliteController satellite = activity.Subject as SatelliteController;
		activity.SubjectID = satellite.Data.ID;
		activity.Title = "Launching "+satellite.Data.Name;
		activity.Subtitle = "";
		activity.ResetDuration(30);
	}

	public object GetSubject(int ID) {
		return GameController.Data.PlayerFaction.Satellites.Get(ID);
	}

	public void OnClick(Activity activity) {
		SatelliteController satellite = activity.Subject as SatelliteController;
		ViewManager.Instance.Show(satellite);
	}

	public void OnExpired(Activity activity) {
		//SatelliteController satellite = activity.Subject as SatelliteController;
		// TODO: Implement
	}

}
