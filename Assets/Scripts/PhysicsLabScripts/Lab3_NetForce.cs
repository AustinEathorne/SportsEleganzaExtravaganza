using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab3_NetForce : MonoBehaviour 
{
	[Header("Scene References")]
	[SerializeField]
	private Transform m_distanceIndicator;
	[SerializeField]
	private Rigidbody m_rb;

	[Header("Input")]
	[SerializeField]
	private float m_distance = 0.0f;

	[Header("Debug")]
	[SerializeField]
	private float m_netForce = 0.0f;

	private float m_forceTime = 1.0f;

	private float m_dynamicFrictionAcceleration = 0.0f;
	private float m_dynamicFrictionForce = 0.0f;
	private float m_accelerationDistance = 0.0f;

	// Part 1
	private float m_elapsedTime = 0.0f;
	private float m_part1Acceleration = 0.0f;
	private float m_part1Displacement = 0.0f; // displacement until we apply our new force
	private float m_part1InitialVelocity = 0.0f;
	private float m_part1FinalVelocity = 7.5f;

	// Part 2
	private float m_part2Displacement = 0.0f;
	private float m_part2Acceleration = 0.0f;
	private float m_part2Time = 0.0f;

	// bools
	private bool m_isSimulating = false;
	private bool m_isStopping = false;

	private void FixedUpdate()
	{
		if(this.m_isSimulating)
		{
			this.m_elapsedTime += Time.fixedDeltaTime;
			Vector3 force = this.transform.forward * this.m_netForce;
			m_rb.AddForce(force, ForceMode.Force);
		}
		if(this.m_elapsedTime >= this.m_forceTime)
		{
			this.m_isSimulating = false;

			if(!this.m_isStopping)
			{
				this.m_isStopping = true;

				this.StartCoroutine(this.ReCalculateForce());

				Debug.Log("Apply new force");
				Vector3 force = this.transform.forward * this.m_netForce;
				m_rb.AddForce(force, ForceMode.Force);
			}
		}
	}

	private IEnumerator Start()
	{
		yield return new WaitUntil(()=> Input.GetKeyDown(KeyCode.Space));
		yield return this.StartCoroutine(this.SetUpSimulation());
		yield return new WaitUntil(()=> Input.GetKeyDown(KeyCode.Space));
		this.m_isSimulating = true;
	}

	private IEnumerator SetUpSimulation()
	{
		this.m_distanceIndicator.position = new Vector3(0.0f, 0.5f, this.m_distance + 7.0f); // Always stops 7 units past distance

		Collider col = this.GetComponent<Collider>();

		float normalForce = m_rb.mass * Physics.gravity.y * -1.0f;
		float ustaticFriction = col.material.staticFriction;
		float uDynamicFriction = col.material.dynamicFriction;
		float staticFrictionForce = normalForce * ustaticFriction;
		this.m_dynamicFrictionForce = normalForce * uDynamicFriction;

		this.m_dynamicFrictionAcceleration = this.m_dynamicFrictionForce / this.m_rb.mass;

		this.m_accelerationDistance = this.m_distance / 2.0f;

		this.m_part1Acceleration = this.CalculateAverageAcceleration(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_accelerationDistance); // Make up final velocity and distance

		this.m_forceTime = this.CalculateTravelTime(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_part1Acceleration);

		this.m_netForce = ConvertAccelerationToForce(this.m_part1Acceleration);

		this.m_part1Displacement = CalculateDisplacementWithAcceleration(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_part1Acceleration);

		this.m_netForce += this.m_dynamicFrictionForce;

		this.m_part1Acceleration += this.m_dynamicFrictionAcceleration;

		this.m_part1FinalVelocity = this.CalculateFinalVelocity(this.m_part1InitialVelocity, this.m_part1Displacement, this.m_part1Acceleration);

		this.m_part1Acceleration = this.CalculateAverageAcceleration(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_part1Displacement);

		this.m_forceTime = this.CalculateTravelTime(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_part1Acceleration);

		float appliedForce = this.ConvertAccelerationToForce(this.m_part1Acceleration);

		this.m_part1Displacement = CalculateDisplacementWithAcceleration(this.m_part1InitialVelocity, this.m_part1FinalVelocity, this.m_part1Acceleration);

		this.m_netForce = appliedForce + this.m_dynamicFrictionForce;

		this.m_part1Acceleration += this.m_dynamicFrictionAcceleration;

		Debug.Log("part1 displacement: " + this.m_part1Displacement);
		Debug.Log("part1 acceleration: " + this.m_part1Acceleration);
		Debug.Log("part1 force time: " + this.m_forceTime);
		Debug.Log("part1 net force: " + this.m_netForce);

		yield return null;
	}

	private IEnumerator ReCalculateForce()
	{
		this.m_part2Displacement = this.CalculateDisplacementWithAcceleration(this.m_part1FinalVelocity, 0.0f, -this.m_dynamicFrictionAcceleration);

		this.m_part2Time = this.CalculateTravelTime(this.m_part1FinalVelocity, 0.0f, -this.m_dynamicFrictionAcceleration);

		this.CalculateInitialVelocity(this.m_part2Displacement, -m_dynamicFrictionAcceleration, this.m_part2Time);

		Debug.Log(this.m_part2Displacement.ToString() + "m to go");
		Debug.Log("part2 travel time" + this.m_part2Time.ToString());
		Debug.Log("part2 net force: " + this.m_netForce);

		yield return null;
	}

	private float CalculateFinalVelocity(float initialVelocity, float displacement, float acceleration)
	{
		float finalVel = Mathf.Sqrt(Mathf.Pow(initialVelocity, 2) + (2 * acceleration * displacement));
		return finalVel;
	}

	private float CalculateInitialVelocity(float displacement, float acceleration, float time)
	{
		return (displacement - ((0.5f * acceleration) * (time * time))) / time;
	}

	private float CalculateAverageAcceleration(float initialVelocity, float finalVelocity, float distance)
	{
		float acceleration = ((finalVelocity * finalVelocity) - (initialVelocity * initialVelocity))/(2.0f * distance);
		return acceleration;
	}

	private float CalculateDisplacementWithAcceleration(float initialVelocity, float finalVelocity, float acceleration)
	{
		float displacement = ((finalVelocity * finalVelocity) -(initialVelocity * initialVelocity)) / (2 * acceleration);
		return displacement;
	}

	private float CalculateTravelTime(float initialVelocity, float finalVelocity, float acceleration)
	{
		float travelTime = (finalVelocity - initialVelocity) /acceleration;
		return travelTime;
	}

	private float ConvertAccelerationToForce(float acceleration)
	{
		return acceleration * m_rb.mass;
	}
}
