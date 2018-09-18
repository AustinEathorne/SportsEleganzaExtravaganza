using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurlingRock : MonoBehaviour 
{
	private Rigidbody m_rb;

	private int currentRingId = -1;
	private List<int> ringIdList = new List<int>();

	[SerializeField]
	private GameObject shotIndicator;
	[SerializeField]
	private float broomSpeed = 6.0f;
	[SerializeField]
	private GameObject BroomContainer;
	[SerializeField]
	private Transform broomTransform1;
	[SerializeField]
	private Transform broomTransform2;
	[SerializeField]
	private Vector3 broomStart1;
	[SerializeField]
	private Vector3 broomStart2;
	[SerializeField]
	private Vector3 broomTarget1;
	[SerializeField]
	private Vector3 broomTarget2;

	[SerializeField]
	private float maxVelocity = 250.0f;

	private bool hasPassedReleaseLine = false;
	private bool isSweeping = false;



	private void Awake()
	{
		this.m_rb = this.GetComponent<Rigidbody>();
	}

	public IEnumerator BroomSweep()
	{
		if(this.isSweeping)
		{
			yield break;
		}

		this.BroomContainer.SetActive(true);

		this.isSweeping = true;
		this.broomTransform1.localPosition = this.broomStart1;
		this.broomTransform2.localPosition = this.broomStart2;

		while(Vector3.Distance(this.broomTransform1.localPosition, this.broomTarget1) >= 0.1f)
		{
			this.broomTransform1.localPosition = Vector3.MoveTowards(this.broomTransform1.localPosition, this.broomTarget1, this.broomSpeed * Time.deltaTime);
			this.broomTransform2.localPosition = Vector3.MoveTowards(this.broomTransform2.localPosition, this.broomTarget2, this.broomSpeed * Time.deltaTime);
			yield return null;
		}

		while(Vector3.Distance(this.broomTransform1.localPosition, this.broomStart1) >= 0.1f)
		{
			this.broomTransform1.localPosition = Vector3.MoveTowards(this.broomTransform1.localPosition, this.broomStart1, this.broomSpeed * Time.deltaTime);
			this.broomTransform2.localPosition = Vector3.MoveTowards(this.broomTransform2.localPosition, this.broomStart2, this.broomSpeed * Time.deltaTime);
			yield return null;
		}

		this.BroomContainer.SetActive(false);
		this.isSweeping = false;
		yield return null;
	}

	// Force movement
	public void AddForceToRock(float netForce)
	{
		Vector3 force = this.transform.forward * netForce;
		this.m_rb.AddForce(force, ForceMode.Force);

		this.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, Mathf.Clamp(this.GetComponent<Rigidbody>().velocity.z, 0.0f, this.maxVelocity));

		// Debug.Log("Vel: " + this.GetComponent<Rigidbody>().velocity.z.ToString());
	}

	// Positioning movement
	public void MoveRock(Vector3 direction)
	{
		this.transform.position += direction;
	}
		

	// Enter - Add int value to list
	// Exit  - Remove int value from list
	// Check - find lowest value in the list
	public void OnTriggerEnter(Collider col)
	{
		if(col.transform.GetComponent<PointRing>())
		{
			// Add ring id to the list if it doesn't exist
			if(!this.ringIdList.Contains(col.transform.GetComponent<PointRing>().GetRingId()))
			{
				this.ringIdList.Add(col.transform.GetComponent<PointRing>().GetRingId());
			}
		}
		else if (col.transform.gameObject.tag == "ReleaseLine")
		{
			this.hasPassedReleaseLine = true;
		}
	}

	public void OnTriggerExit(Collider col)
	{
		if(col.transform.GetComponent<PointRing>())
		{
			// Remove ring value from the list
			if(this.ringIdList.Contains(col.transform.GetComponent<PointRing>().GetRingId()))
			{
				this.ringIdList.Remove(col.transform.GetComponent<PointRing>().GetRingId());
			}
		}
	}

	public void OnCollisionEnter(Collision col)
	{
		if(col.transform.tag == "Rock")
		{
			Debug.Log("HitRock");
			GameObject.FindObjectOfType<GameManagerCurling>().SetHasDecreased(true); // shouldve made another function/ renamed this..
		}
	}

	private int CheckMostInnerRing()
	{
		int lowest = 4; // outer most ring id

		// traverse ring id list
		for(int i = 0; i < this.ringIdList.Count; i++)
		{
			// check if ring id is the lower than the current (center ring is 0)
			if(this.ringIdList[i] < lowest)
			{
				lowest = this.ringIdList[i];
			}
		}

		return lowest;
	}



	// Get/Set
	public int GetCurrentRingId()
	{
		return this.CheckMostInnerRing();
	}

	public bool GetHasPassedReleaseLine()
	{
		return this.hasPassedReleaseLine;
	}

	public GameObject GetShotIndicator()
	{
		return this.shotIndicator;
	}

	public GameObject GetBroomContainer()
	{
		return this.BroomContainer;
	}
}
