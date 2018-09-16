using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{

	[SerializeField]
	private Lab5_HomeRunDerby labMan;

	void Update()
	{
		if(this.labMan.GetCurrentBall())
		{
			this.transform.LookAt(this.labMan.GetCurrentBall().transform.position);
		}
	}

}
