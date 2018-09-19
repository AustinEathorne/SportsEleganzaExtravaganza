using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerCurling : MonoBehaviour {

	[Header("Prefabs")]
	[SerializeField]
	private GameObject rockPrefab;

	[Header("Materials")]
	[SerializeField]
	private Material p1_material;
	[SerializeField]
	private Material p2_material;

	[Header("Managers")]
	[SerializeField]
	private PlayerControllerCurling playerController;
	[SerializeField]
	private AudioManagerCurling audioManager;
	[SerializeField]
	private CanvasManagerCurling canvasManager;

	[Header("Scene References")]
	[SerializeField]
	private Transform spawnTransform;
	[SerializeField]
	private GameObject distanceIndicator;
	[SerializeField]
	private RingPreview shotPreviewScreen;
	[SerializeField]
	private BoxCollider releaseLineCollider;
	[SerializeField]
	private BoxCollider releaseLineTrigger;

	[Header("Game Set Up")]
	[SerializeField]
	private int numberOfTurns;

	private int currentTurn = 0;

	[Header("Point Values")]
	[SerializeField]
	private float[] ringValues = new float[4];

	private float p1_totalScore = 0.0f;
	private float p2_totalScore = 0.0f;

	[Header("Min/Max")]
	[SerializeField]
	private float maxVelocity = 0.0f;
	[SerializeField]
	private float minAcceleration = 0.0f;
	[SerializeField]
	private float maxAcceleration = 0.0f;
	[SerializeField]
	private float minMass = 0.0f;
	[SerializeField]
	private float maxMass = 0.0f;

	[Header("HURRY HARD")]
	[SerializeField]
	private MeshCollider icePlane;
	[SerializeField]
	private float frictionCoefficientDecrease = 0.005f;
	[SerializeField]
	private float minSweepProximity = 0.5f;
	[SerializeField]
	private float minCoefficient = 0.0f;

	private float startCoefficient = 0.0f;
	private float lastHighestVel = 0.0f;
	private bool hasDecreased = false;

	// Player Input
	private float p1_acceleration = 10.0f;
	private float p1_mass = 10.0f;
	private float p2_acceleration = 20.0f;
	private float p2_mass = 10.0f;

	// Force
	private float currentForce = 0.0f;
	private float uDynamicFriction = 0.0f;

	// Lists
	private List<GameObject> activeRockList = new List<GameObject>();

	// Bools
	private bool isPlayerOneTurn = false;
    private bool isReadyForShot = false;
    private bool isAddingForceToRock = false;



	private IEnumerator Start()
	{
        Screen.orientation = ScreenOrientation.LandscapeLeft;
		yield return this.StartCoroutine(this.SetUpGame());
		yield return this.StartCoroutine(this.RunGame());
        yield return this.StartCoroutine(this.EndGame());
    }

	private IEnumerator SetUpGame()
	{
		// Get friction coefficient
		this.startCoefficient = icePlane.GetComponent<Collider>().material.dynamicFriction;
		this.uDynamicFriction = rockPrefab.GetComponent<Collider>().material.dynamicFriction;

		// Set Slider Max values
		this.canvasManager.SetMassSliderMaxValue(this.maxMass);
		this.canvasManager.SetMassSliderMinValue(this.minMass);
		this.canvasManager.SetMassSliderValue(this.minMass + ((this.maxMass - this.minMass) * 0.5f));

		this.canvasManager.SetAccelerationSliderMaxValue(this.maxAcceleration);
		this.canvasManager.SetAccelerationSliderMinValue(this.minAcceleration);
		this.canvasManager.SetAccelerationSliderValue(this.minAcceleration + ((this.maxAcceleration - this.minAcceleration) * 0.5f));

		yield return null;
	}

	private IEnumerator RunGame()
	{
		while(currentTurn != (this.numberOfTurns * 2))
		{
			// Re-Calculate Point Values & update point text
			this.CalculatePointTotal();
			this.canvasManager.UpdatePointText(this.p1_totalScore, this.p2_totalScore);

			// Increase turn counter
			currentTurn++;

			// Find which player's turn it is
			this.isPlayerOneTurn = this.currentTurn % 2 == 0 ? false : true;

			// Debug
			if(this.isPlayerOneTurn)
				Debug.Log("Player 1 Turn");
			else
				Debug.Log("Player 2 Turn");

			Debug.Log("Current Turn: " + this.currentTurn.ToString() + " / " + (this.numberOfTurns * 2).ToString());

			// Enable UI elements
			this.canvasManager.EnableStatSubmitElements(true);

			// Start turn
			yield return this.StartCoroutine(this.RunTurn());

			// Reset
			this.ResetTurnValues();

			// Re-Calculate Point Values & update point text
			this.CalculatePointTotal();
			this.canvasManager.UpdatePointText(this.p1_totalScore, this.p2_totalScore);
		}
	}

	private IEnumerator RunTurn()
	{
		// Update UI Sliders
		if(this.currentTurn > 2) // Only set the sliders back to player values if they have been set (on their previous turn)
		{
			this.canvasManager.SetMassSliderValue(this.isPlayerOneTurn ? this.p1_mass : this.p2_mass);
			this.canvasManager.SetAccelerationSliderValue(this.isPlayerOneTurn ? this.p1_acceleration : this.p2_acceleration);
		}

        this.canvasManager.UpdateSliderTextValues();
		this.canvasManager.SetPlayerTurnText(this.isPlayerOneTurn ? "Player 1's Turn" : "Player 2's Turn");

		// Instantiate rock
		GameObject obj = Instantiate(rockPrefab, this.spawnTransform.position, Quaternion.identity) as GameObject;

		// Reset friction coefficient
		obj.GetComponent<Collider>().material.dynamicFriction = this.startCoefficient;
		this.icePlane.material.dynamicFriction = this.startCoefficient;

		// Name & set material
		if(this.isPlayerOneTurn)
		{
			obj.name = "Player1 Rock";
			obj.GetComponent<MeshRenderer>().sharedMaterial = this.p1_material;
		}
		else
		{
			obj.name = "Player2 Rock";
			obj.GetComponent<MeshRenderer>().sharedMaterial = this.p2_material;
		}

		// Add to the active list
		this.activeRockList.Add(obj);

        // Update help text
        this.canvasManager.EnableHelpText(0);

        // Wait for confirmation of stats (mass & acceleration) + rock positioning
        while (this.isReadyForShot != true)
        {
            // Rock positioning
            this.playerController.StartCoroutine(this.playerController.PositionRock());
            yield return null;
        }
        this.isReadyForShot = false;

		// Disable
		this.activeRockList[this.currentTurn - 1].GetComponent<CurlingRock>().GetShotIndicator().SetActive(false);
		this.distanceIndicator.SetActive(false);
		this.releaseLineCollider.enabled = false;
		this.releaseLineTrigger.enabled = true;

		// Calculate force
		this.currentForce = this.CalculateForce();

        // Update help text
        this.canvasManager.EnableHelpText(1);

        // Start getting input and wait until is has been released - Force is applied in fixed update
        yield return this.playerController.StartCoroutine(this.playerController.AddForceToRock( _isAddingForce => {
            if (_isAddingForce)
                this.isAddingForceToRock = true;
        }));
        this.isAddingForceToRock = false;

        // Update help text
        this.canvasManager.EnableHelpText(2);

        // Play shot audio
        this.audioManager.StartCoroutine(this.audioManager.PlayGlideSfx());

		// Move in shot preview screen
		this.shotPreviewScreen.StartCoroutine(this.shotPreviewScreen.MoveScreenDown());

		// Predict initial rock displacement
		float displacement = this.CalculateDisplacement();

		// Set distance indicator position to the far end board if the displacement is off the map
		if(displacement > 140.0f) // off map
		{
			this.distanceIndicator.SetActive(true);
			this.distanceIndicator.transform.position = new Vector3(this.activeRockList[this.currentTurn - 1].transform.position.x, 0.0f, 143.5f);
		}
		else
		{
			this.distanceIndicator.SetActive(true);
			this.distanceIndicator.transform.position = new Vector3(this.activeRockList[this.currentTurn - 1].transform.position.x, 0.0f, displacement);
		}

        // Start getting sweep input
        this.playerController.StartCoroutine(this.playerController.SweepRock());

		// While rock is moving
		float rockPos = this.activeRockList[this.currentTurn - 1].transform.position.z;
		while(Vector3.Distance(this.activeRockList[this.currentTurn - 1].GetComponent<Rigidbody>().velocity, Vector3.zero) >= Mathf.Epsilon)
		{
			// check if friction coefficient has decreased and if velocity has increased (re-calculate displacement & place indicator)
			if(this.CheckHasDecreased() || this.HasVelocityIncreased())
			{
				// Check displacement
				displacement = this.CalculateDisplacement();

				// Set distance indicator position to the far end board if the displacement is off the map
				if(displacement > 143.0f) // off map
				{
					this.distanceIndicator.SetActive(true);
					this.distanceIndicator.transform.position = new Vector3(this.activeRockList[this.currentTurn - 1].transform.position.x, 0.0f, 143f);
				}
				else
				{
					this.distanceIndicator.SetActive(true);
					this.distanceIndicator.transform.position = new Vector3(this.activeRockList[this.currentTurn - 1].transform.position.x, 0.0f, displacement);
				}
			}

			// Break if were close to the displacement
			if(this.distanceIndicator.transform.localPosition.z - this.activeRockList[this.currentTurn - 1].transform.localPosition.z <= this.minSweepProximity)
			{
                //Debug.Log("Stop - Too Close" + (this.activeRockList[this.currentTurn - 1].transform.localPosition.z - this.distanceIndicator.transform.localPosition.z).ToString());
                this.playerController.SetIsSweeping(false);
			}
			yield return null;
		}

        this.canvasManager.DisableAllHelpText();

		// Reset
		this.ResetFrictionCoEfficient();
        this.playerController.SetIsSweeping(false);
        this.shotPreviewScreen.StartCoroutine(this.shotPreviewScreen.MoveScreenUp());
		this.releaseLineCollider.enabled = true;
		this.releaseLineTrigger.enabled = false;

		Debug.Log("Done Turn");
	}

	private IEnumerator EndGame()
	{
		// Check Score
		string str = "";

		if(this.p1_totalScore > this.p2_totalScore)
		{
			str = "PLAYER 1";
		}
		else if(this.p1_totalScore < this.p2_totalScore)
		{
			str = "PLAYER 2";
		}
		else
		{
			str = "PLAYER 1 & PLAYER 2";
		}

		// Display Winner Text
		this.canvasManager.SetWinnertext(str);
		this.canvasManager.EnableWinnerText(true);

		this.audioManager.StartCoroutine(this.audioManager.PlayWinnerGagnant());

		// Wait for escape key
		while(!Input.GetKeyDown(KeyCode.Escape))
		{
			yield return null;
		}

		// Reload Scene
		SceneManager.LoadScene(0);
	}


	private void FixedUpdate()
	{
		if(this.isAddingForceToRock)
		{
			// Debug.Log("moving rock at index: " + (this.currentTurn - 1).ToString());
			this.activeRockList[this.currentTurn - 1].GetComponent<CurlingRock>().AddForceToRock(this.currentForce);
		}
	}


	public void ResetFrictionCoEfficient()
	{
		this.activeRockList[this.currentTurn - 1].GetComponent<Collider>().material.dynamicFriction = this.startCoefficient;
		this.icePlane.material.dynamicFriction = this.startCoefficient;
	}

	public void DecreaseFrictionCoefficient()
	{
		if(this.icePlane.material.dynamicFriction >= this.minCoefficient && this.icePlane.material.dynamicFriction > 0 + this.frictionCoefficientDecrease)
		{
			//Debug.Log("Coefficient: " + this.icePlane.material.dynamicFriction.ToString());
			this.hasDecreased = true;
			this.activeRockList[this.currentTurn - 1].GetComponent<Collider>().material.dynamicFriction -= this.frictionCoefficientDecrease;
			this.icePlane.material.dynamicFriction -= this.frictionCoefficientDecrease;
		}
	}


	private void ResetTurnValues()
	{
		this.currentForce = 0.0f;
	}


	private bool CheckHasDecreased()
	{
		if(this.hasDecreased)
		{
			hasDecreased = false;
			return true;
		}
		return false;
	}

	private bool HasVelocityIncreased()
	{
		if(this.activeRockList[this.currentTurn - 1].GetComponent<Rigidbody>().velocity.z > this.lastHighestVel)
		{
			this.lastHighestVel = this.activeRockList[this.currentTurn - 1].GetComponent<Rigidbody>().velocity.z;
			return true;
		}
		return false;
	}

    // Clamp passed vec3 and update rock position
    public void UpdateRockPosition(Vector3 _position)
    {
        Vector3 pos = new Vector3(Mathf.Clamp(_position.x, -19, 19), 0.75f, Mathf.Clamp(_position.z, -5, 18));
        this.GetActiveRock().transform.position = pos;
    }


	// Get/Set
	public GameObject GetActiveRock()
	{
		return this.activeRockList[this.currentTurn - 1];
	}

	public RingPreview GetRingPreviewScreen()
	{
		return this.shotPreviewScreen;
	}

	public void SetHasDecreased(bool value)
	{
		this.hasDecreased = value;
	}

    public void SetReadyForShot(bool _isReady)
    {
        this.isReadyForShot = _isReady;
    }

	// Calculate/Convert
	private float CalculateForce() // Check player & calculate force
	{
		// Get Input Values
		if(this.isPlayerOneTurn)
		{
			this.p1_mass = this.canvasManager.GetMassValue();
			this.p1_acceleration = this.canvasManager.GetAccelerationValue();
		}
		else
		{
			this.p2_mass = this.canvasManager.GetMassValue();
			this.p2_acceleration = this.canvasManager.GetAccelerationValue();
		}

		// Mass & Acceleration
		float mass = this.isPlayerOneTurn ? this.p1_mass : this.p2_mass;
		this.activeRockList[this.currentTurn - 1].GetComponent<Rigidbody>().mass = mass;
		float acceleration = this.isPlayerOneTurn ? this.p1_acceleration : this.p2_acceleration;

		// Calculate
		float normalForce = mass * Physics.gravity.y * -1.0f;
		float dynamicFrictionForce = normalForce * this.uDynamicFriction;
		float dynamicFrictionAcceleration = dynamicFrictionForce / mass;

		Debug.Log("mass: " + mass.ToString());

		float appliedForce = acceleration * mass;
		Debug.Log("Applied force: " + appliedForce.ToString());

		float netForce = appliedForce + dynamicFrictionForce;
		Debug.Log("Net force: " + netForce.ToString());

		return netForce;
	}

	private float CalculateDisplacement() // Calculate displacement for active rock
	{
		this.uDynamicFriction = icePlane.GetComponent<Collider>().material.dynamicFriction;
		float normalForce = this.p1_mass * Physics.gravity.y * -1.0f;
		float dynamicFrictionForce = normalForce * this.uDynamicFriction;
		float dynamicFrictionAcceleration = dynamicFrictionForce / this.p1_mass;

		float initialDisplacement = this.activeRockList[this.currentTurn - 1].transform.position.z;
		//Debug.Log("Initial Displacement: " + initialDisplacement.ToString());

		float initialVelocity = this.activeRockList[this.currentTurn - 1].GetComponent<Rigidbody>().velocity.z;
		//Debug.Log("Initial Velocity: " + initialVelocity.ToString());

		float finalVelocity = 0.0f;

		float travelTime = this.CalculateTravelTime(initialVelocity, finalVelocity, -dynamicFrictionAcceleration);
		//Debug.Log("Travel Time: " + travelTime);

		float displacement = (this.CalculateDisplacement(initialVelocity, -dynamicFrictionAcceleration, travelTime) + initialDisplacement);

		return displacement;
	}

	private float ConvertAccelerationToForce(float acceleration, float mass)
	{
		return acceleration * mass;
	}
		
	private float CalculateDisplacement(float initialVelocity, float acceleration, float travelTime)
	{
		float displacement = (float)((initialVelocity*travelTime) + (0.5*(acceleration * Mathf.Pow(travelTime, 2))));
		return displacement;
	}

	private float CalculateTravelTime(float initialVelocity, float finalVelocity, float acceleration)
	{
		float travelTime = (finalVelocity - initialVelocity) /acceleration;
		return travelTime;
	}

	private void CalculatePointTotal()
	{
		this.p1_totalScore = 0.0f;
		this.p2_totalScore = 0.0f;

		// Traverse all active rocks
		for(int i = 0; i < this.activeRockList.Count; i++)
		{
			//check whether they currently have a current ring (!= -1)
			if(this.activeRockList[i].GetComponent<CurlingRock>().GetCurrentRingId() != -1)
			{
				// Give points to the correct player
				if(i % 2 == 0)
				{
					this.p1_totalScore += this.ringValues[this.activeRockList[i].GetComponent<CurlingRock>().GetCurrentRingId()];
				}
				else
				{
					this.p2_totalScore += this.ringValues[this.activeRockList[i].GetComponent<CurlingRock>().GetCurrentRingId()];
				}
			}
		}
	}
}
