using System;

[Serializable]
public struct Log {

	public string Message;
	public Action Callback;

	public Log(string message, Action callback = null) {
		this.Message = message;
		this.Callback = callback;
	}

}

