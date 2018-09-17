using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRing : MonoBehaviour {

	[SerializeField]
	private int ringId;

	public int GetRingId()
	{
		return this.ringId;
	}
}
