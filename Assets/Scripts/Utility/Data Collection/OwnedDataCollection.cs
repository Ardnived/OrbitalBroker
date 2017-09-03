using System;

public class OwnedDataCollection<T> : DataCollection<T> where T : OwnedDataElement {

	private int ownerID;

	public OwnedDataCollection(int ownerID, On.Event eventChannel) : base(eventChannel) {
		this.ownerID = ownerID;
	}

	public override void Set(T element) {
		element.OwnerID = ownerID;
		base.Set(element);
	}

}