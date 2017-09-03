
public interface ActivityDelegate {
	void Setup(Activity activity);
	object GetSubject(int ID);
	void OnClick(Activity activity);
	void OnExpired(Activity activity);
}
