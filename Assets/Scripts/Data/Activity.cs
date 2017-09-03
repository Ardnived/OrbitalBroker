using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Activity : DataElement {

	public static void Create(ActivityType type, object subject) {
		//Activity activity = new Activity(type, subject);
		//GameController.Data.Activities.Set(activity);
	}

	private static ActivityDelegate[] delegates = new ActivityDelegate[] {
	
	};

	[SerializeField]
	private int _ID;
	public int ID { get { return _ID; } set { _ID = value; } }

	public ActivityType Type;
	public string Title;
	public string Subtitle;
	public int Duration;
	public float Expires;

	public int SubjectID;
	public object Subject;

	private ActivityDelegate del;

	private Activity(ActivityType type, object subject) {
		this.Type = type;
		this.Subject = subject;
		delegates[(int) Type].Setup(this);
	}

	public void ResetDuration(int duration) {
		this.Duration = duration;
		this.Expires = Time.time + duration;
	}

	public virtual void OnClick() {
		delegates[(int) Type].OnClick(this);
	}

	public virtual void OnExpired() {
		delegates[(int) Type].OnClick(this);
	}

	public void OnPassedExpired() {
		//GameController.Data.Activities.Remove(this);
	}

	public void OnDeserialize() {
		this.Subject = delegates[(int) Type].GetSubject(SubjectID);
	}
}

public enum ActivityType {
	Launch,
	Research,
}
