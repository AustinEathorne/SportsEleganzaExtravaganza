using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baseball : MonoBehaviour 
{
	[SerializeField]
	private GameObject trailObj;
	[SerializeField]
	private GameObject particleObj;

	[SerializeField]
	private GameObject cameraObj;
	[SerializeField]
	private Vector3 camPos;
	[SerializeField]
	private Vector3 camRot;
	private Vector3 origPos;
	[SerializeField]
	private float camSpeed = 1.0f;

	private Lab5_HomeRunDerby labManager;



	private void Awake()
	{
		this.origPos = this.cameraObj.transform.localPosition;
	}

	public void OnTriggerEnter(Collider col)
	{
		if(col.tag == "SimManager")
		{
			Debug.Log("Home Run!");
			this.labManager.ScoreHomeRun();
		}
	}


	public void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Wall")
		{
			Debug.Log("So Close!");
			this.labManager.HitWall();
		}
	}

	public GameObject GetCamera()
	{
		return this.cameraObj;
	}

	public void SetLabManager(Lab5_HomeRunDerby manager)
	{
		this.labManager = manager;
	}

	public void TurnOnTrail()
	{
		this.trailObj.SetActive(true);
	}

	public void EndLyfe()
	{
		//this.particleObj.SetActive(true);
		Destroy(this.gameObject, 10.0f);
	}

	public IEnumerator MoveCamera()
	{
		while(Vector3.Distance(this.cameraObj.transform.localPosition, this.camPos) >= 0.5f)
		{
			this.cameraObj.transform.localPosition = Vector3.Lerp(this.cameraObj.transform.localPosition, this.camPos, this.camSpeed * Time.deltaTime);
			this.cameraObj.transform.localRotation = Quaternion.Lerp(this.cameraObj.transform.rotation, Quaternion.Euler(this.camRot), this.camSpeed * Time.deltaTime);

			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		while(Vector3.Distance(this.cameraObj.transform.localPosition, this.origPos) >= 0.5f)
		{
			this.cameraObj.transform.localPosition = Vector3.Lerp(this.cameraObj.transform.localPosition, this.origPos, this.camSpeed * Time.deltaTime);
			this.cameraObj.transform.localRotation = Quaternion.Lerp(this.cameraObj.transform.rotation, Quaternion.Euler(0.0f, -16.0f, 0.0f), this.camSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
