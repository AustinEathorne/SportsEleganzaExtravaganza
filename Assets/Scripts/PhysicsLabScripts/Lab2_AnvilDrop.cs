using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab2_AnvilDrop : MonoBehaviour {

	[Header("Prefabs")]
	[SerializeField]
	private GameObject anvilPrefab;
	[SerializeField]
	private GameObject roadblockPrefab;

	[Header("Variables")]
	[SerializeField]
	private float anvilDropHeight = 10.0f;
	[SerializeField]
	private float anvilDistance = 10.0f;
	[SerializeField]
	private float roadblockDistance = 30.0f;
	[SerializeField]
	private float roadblockStoppingDistance = 1.0f;

	[Header("Doge")]
	[SerializeField]
	private GameObject doge; // particle

	private float elapsedTime = 0.0f;

	// vehicle
	private Rigidbody vehiclerb;
	private float vehicleAcceleration = 0.0f;

	// anvil
	private Rigidbody anvilrb;
	private float anvilFinalVelocity = 0.0f;
	private float anvilTravelTime = 0.0f;

	// roadblock
	private Transform roadblockTransform;

	private bool isSimulating = false;
	private bool isAccelerating = true;
	
	private IEnumerator Start()
	{
		yield return new WaitUntil(()=> Input.GetKeyDown(KeyCode.Space));

		yield return this.StartCoroutine(this.SetUpSimulation());

		yield return new WaitUntil(()=> Input.GetKeyDown(KeyCode.Space));

		this.StartCoroutine(this.StartSimulation());
	}

	private IEnumerator SetUpSimulation()
	{
		// Instantiate roadblock prefab
		GameObject roadblockObj = GameObject.Instantiate(this.roadblockPrefab, this.transform.position, this.transform.rotation) as GameObject;
		roadblockObj.name = "Roadblock";
		roadblockObj.transform.position = new Vector3(0.0f, 0.5f, this.roadblockDistance);

		// Get the transform component
		this.roadblockTransform = roadblockObj.transform;

		// Instantiate anvil prefab
		GameObject anvilObj = GameObject.Instantiate(this.anvilPrefab, this.transform.position, this.transform.rotation) as GameObject;
		anvilObj.name = "Anvil";
		anvilObj.transform.position = new Vector3(0.0f, this.anvilDropHeight + (anvilObj.transform.localScale.y * 0.5f), this.anvilDistance);

		// Get the rigidbody component
		this.anvilrb = anvilObj.GetComponent<Rigidbody>();

		// Calculate the anvil's final velocity and travel time
		this.anvilFinalVelocity = this.CalculateFinalVelocity(0.0f, this.anvilDropHeight, Physics.gravity.y);
		this.anvilTravelTime = this.CalculateTravelTime(0.0f, this.anvilFinalVelocity, Physics.gravity.y);
		Debug.Log("Anvil final velocity: " + this.anvilFinalVelocity.ToString() + "\nTravel time: " + this.anvilTravelTime.ToString());

		// Get vehicle rigidbody
		this.vehiclerb = this.GetComponent<Rigidbody>();

		// Calculate vehicle acceleration
		this.vehicleAcceleration = this.CalculateAverageAcceleration(this.anvilDistance + 2.5f, this.anvilTravelTime);
		Debug.Log("Vehicle acceleration: " + this.vehicleAcceleration.ToString());

		yield return null;
	}

	private IEnumerator StartSimulation()
	{
		this.isSimulating = true;
		anvilrb.useGravity = true;
		yield return null;
	}

	private void RecalculateVehicleAcceleration()
	{
		Vector3 pos = new Vector3(this.roadblockTransform.position.x, this.roadblockTransform.position.y,
			this.roadblockTransform.position.z - this.roadblockStoppingDistance);
		float distance = Vector3.Distance(this.transform.position, pos);

		this.vehicleAcceleration = this.CalculateAverageAcceleration(this.vehiclerb.velocity.z, 0.0f, distance);
	}

	private void FixedUpdate()
	{
		if(this.isSimulating)
		{
			elapsedTime += Time.fixedDeltaTime;

			Vector3 acceleration = this.transform.forward * this.vehicleAcceleration;
			vehiclerb.AddForce(acceleration, ForceMode.Acceleration);

			if(elapsedTime >= anvilTravelTime && isAccelerating)
			{
				this.isAccelerating = false;
				this.RecalculateVehicleAcceleration();
				// Debug.Log("STOPPPPPPPPP");
			}

			if(this.vehiclerb.velocity.z <= 0.0f && !isAccelerating)
			{
				this.vehicleAcceleration = 0.0f;
				this.vehiclerb.velocity = Vector3.zero;

				this.StartCoroutine(this.Doge());
			}
		}
	}

	private float CalculateAverageAcceleration(float distance, float travelTime)
	{
		float acceleration = (2*distance)/Mathf.Pow(travelTime, 2);
		return acceleration;
	}

	private float CalculateAverageAcceleration(float initialVelocity, float finalVelocity, float distance)
	{
		float acceleration = ((finalVelocity * finalVelocity) - (initialVelocity * initialVelocity))/(2.0f * distance);
		return acceleration;
	}

	private float CalculateFinalVelocity(float initialVelocity, float displacement, float acceleration)
	{
		float finalVel = Mathf.Pow(initialVelocity, 2) + (2 * acceleration * displacement);
		finalVel = Mathf.Sqrt(finalVel * -1) * -1; // beautiful..
		return finalVel;
	}

	private float CalculateDisplacement(float initialVelocity, float acceleration, float travelTime)
	{
		float displacement = (float)((initialVelocity*travelTime) + (0.5*(acceleration * Mathf.Pow(travelTime, 2))));
		return displacement;
	}

	private float CalculateTravelTime(float initialVelocity, float finalVelocity, float acceleration)
	{
		float travelTime = (float)((finalVelocity - initialVelocity)/acceleration);
		return travelTime;
	}

	private IEnumerator Doge()
	{
		yield return new WaitForSeconds(1.0f);

		this.doge.SetActive(true);
		this.GetComponent<MeshFilter>().mesh = null;
	}
}
