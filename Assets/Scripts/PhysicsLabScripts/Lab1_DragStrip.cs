using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lab1_DragStrip : MonoBehaviour 
{
	[Header("Prefabs")]
	[SerializeField]
	private GameObject m_displacementIndicator;
	[SerializeField]
	private GameObject m_dogeParticle;
	[SerializeField]
	private GameObject m_dogeExplosionParticle;

	[Header("Input")]
	[SerializeField]
	private float m_travelTime = 0.0f; // Seconds
	[SerializeField]
	private float m_finalVelocity = 100.0f; // km/h - needs to be converted to m/s

	[Header("Ending")]
	[SerializeField]
	private float m_flashSpeed = 5.0f;
	[SerializeField]
	private float m_flashSpeedIncrease = 0.25f;
	[SerializeField]
	private float m_flashSpeedLimit = 20.0f;

	[Header("UI")]
	[SerializeField]
	private Text m_velocityText;
	[SerializeField]
	private Text m_accelerationText;

	private float m_acceleration = 0.0f;
	private float m_currentAcceleration = 0.0f;
	private float m_displacement = 0.0f;

	private float m_elapsedTime = 0.0f;

	private bool m_isAccelerating = false;

	private Rigidbody m_rb = null;


	private IEnumerator Start()
	{
		this.m_rb = this.GetComponent<Rigidbody>();

		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

		yield return this.StartCoroutine(this.SetUpSimulation());

		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

		m_isAccelerating = true;
	}

	private void FixedUpdate()
	{
		if(m_isAccelerating)
		{
			this.m_elapsedTime += Time.fixedDeltaTime;
			// Debug.Log("Elapsed Time: " + this.m_elapsedTime);

			// Apply average acceleration
			Vector3 acceleration = this.transform.forward * this.m_acceleration;
			this.m_rb.AddForce(acceleration, ForceMode.Acceleration);

			// Debug current acceleration
			m_currentAcceleration = this.CalculateAverageAcceleration(0.0f, this.m_rb.velocity.z, this.m_elapsedTime);
			Debug.Log("Current Acceleration: " + m_currentAcceleration);
			this.m_accelerationText.text = "Acceleration: " + this.m_currentAcceleration.ToString() + " m/s^2";

			if(this.m_elapsedTime >= m_travelTime)
			{
				this.m_isAccelerating = false;
				this.m_rb.velocity = Vector3.zero;
				this.m_rb.angularVelocity = Vector3.zero;

				this.StartCoroutine(this.SweetEnding());

				Debug.Log("done");
			}
		}
	}

	private IEnumerator SetUpSimulation()
	{
		// Convert from km/h to km/s
		this.m_finalVelocity = this.ConvertToMetersPerSecond(this.m_finalVelocity);

		// Debug.Log("Final Velocity: " + m_finalVelocity + " m/s");

		// Calculate average acceleration and displacement
		this.m_acceleration = this.CalculateAverageAcceleration(0, this.m_finalVelocity, this.m_travelTime);
		this.m_displacement = this.CalculateDisplacement(0, this.m_acceleration, this.m_travelTime);

		// Place reference object at expected displacement
		GameObject displacementBox = GameObject.Instantiate(m_displacementIndicator, this.transform.position, this.transform.rotation) as GameObject;
		displacementBox.name = "DisplacementIndicator";
		displacementBox.transform.position = new Vector3(0.0f, 0.5f, this.m_displacement + 1.0f); // add 0.5*2 for both cube width's

		Debug.Log("Acceleration: " + m_acceleration + ", Displacement: " + m_displacement);
		yield return null;
	}

	private float ConvertToMetersPerSecond(float finalVelocity)
	{
		float tempVel = (finalVelocity * 1000.0f)/3600.0f;
		return tempVel;
	}

	private float CalculateAverageAcceleration(float initialVelocity, float finalVelocity, float travelTime)
	{
		float acceleration = (finalVelocity - initialVelocity) / travelTime;
		return acceleration;
	}

	private float CalculateDisplacement(float initialVelocity, float acceleration, float travelTime)
	{
		float displacement = (float)((initialVelocity*travelTime) + (0.5*(acceleration * Mathf.Pow(travelTime, 2))));
		return displacement;
	}

	private IEnumerator SweetEnding()
	{
		this.m_dogeParticle.SetActive(true);

		Color col = this.gameObject.GetComponent<MeshRenderer>().material.color;
		float tempRed = 1.0f;

		while(tempRed > float.Epsilon)
		{
			tempRed = Mathf.MoveTowards(tempRed, 0.0f, Time.deltaTime * this.m_flashSpeed);	
			col = new Color(tempRed, col.g, col.b, col.a);
			this.gameObject.GetComponent<MeshRenderer>().material.color = col;

			yield return null;
		}

		//tempRed = 0.0f;

		while(tempRed < 0.999f)
		{
			tempRed = Mathf.MoveTowards(tempRed, 1.0f, Time.deltaTime * this.m_flashSpeed);	
			col = new Color(tempRed, col.g, col.b, col.a);
			this.gameObject.GetComponent<MeshRenderer>().material.color = col;

			yield return null;
		}

		this.m_flashSpeed += m_flashSpeedIncrease;

		if(this.m_flashSpeed > this.m_flashSpeedLimit)
			this.StartCoroutine(this.WeDoneNow());
		else
			this.StartCoroutine(this.SweetEnding());
	}

	private IEnumerator WeDoneNow()
	{
		this.GetComponent<MeshFilter>().mesh = null;

		this.m_dogeExplosionParticle.SetActive(true);

		yield return new WaitForSeconds(1.0f);

		this.m_dogeParticle.SetActive(false);

		yield return null;
	}
}
