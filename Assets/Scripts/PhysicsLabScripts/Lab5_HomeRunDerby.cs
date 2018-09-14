using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lab5_HomeRunDerby : MonoBehaviour 
{
	[Header("Simulation")]
	[SerializeField]
	private int numberOfPlayers = 2;
	private int currentPlayer = 0;
	[SerializeField]
	private int numberOfRounds = 3;
	private int currentRound = 0;
	private List<int> playerPointsList = new List<int>();

	[SerializeField]
	private float minSpeed = 0.0f;
	[SerializeField]
	private float maxSpeed = 100.0f;
	[SerializeField]
	private float minAngle = 330.0f;
	[SerializeField]
	private float maxAngle = 350.0f;
	[SerializeField]
	private float powerMinSweetSpot = 0.0f;
	[SerializeField]
	private float powerMaxSweetSpot = 0.0f;
	[SerializeField]
	private float angleMinSweetSpot = 330.0f;
	[SerializeField]
	private float angleMaxSweetSpot = 350.0f;

	[SerializeField]
	private List<float> powerTickValueList = new List<float>();
	[SerializeField]
	private List<float> angleTickValueList = new List<float>();

	private int difficulty = 0;

	private bool hasStarted = false;
	private bool hasSelectedPlayers = false;
	private bool hasSelectedRounds = false;
	private bool hasSelectedDifficulty = false;

    private bool hasTapped = false;

	[Header("Baseball")]
	[SerializeField]
	private GameObject baseballPrefab;
	private GameObject currentBallObj;
	private Rigidbody currentBallRigidbody;

	[SerializeField]
	private Vector3 ballSpawnPosition = new Vector3(0.0f, 2.0f, 0.0f);


	[Header("Camera")]
	[SerializeField]
	private List<GameObject> cameraList;
	private int currentCamera = 0;
	private bool isUsingBallCamera = true;


	[Header("UI")]
	// Main containers
	[SerializeField]
	private GameObject startContainer;
	[SerializeField]
	private GameObject playerSelectContainer;
	[SerializeField]
	private GameObject roundSelectContainer;
	[SerializeField]
	private GameObject difficultySelectContainer;
	[SerializeField]
	private GameObject gameUIContainer;

	// Meters
	[SerializeField]
	private GameObject powerMeterContainer;
	[SerializeField]
	private Slider powerMeterSlider;
	[SerializeField]
	private Image powerSweetImage;
	[SerializeField]
	private GameObject angleMeterContainer;
	[SerializeField]
	private Slider angleMeterSlider;
	[SerializeField]
	private Image angleSweetImage;

    // Camera
    //[SerializeField]
    //private Text currentCameraText;
    //[SerializeField]
    //private Text currentCameraTextBG;

    // Tap Area
    [SerializeField]
    private Button tapAreaButton;

	//Players
	[SerializeField]
	private Text currentPlayerText;
	[SerializeField]
	private Text currentPlayerTextBg;
	[SerializeField]
	private List<GameObject> scoreTextContainers;
	[SerializeField]
	private List<Text> scoreTextList;
	[SerializeField]
	private List<Text> scoreBgsTextList;

	// Rounds
	[SerializeField]
	private Text currentRoundText;
	[SerializeField]
	private Text currentRoundTextBg;

	// Help
	[SerializeField]
	private Toggle ballCamToggle;
	[SerializeField]
	private GameObject spaceTextContainer;
	[SerializeField]
	private FlashText spaceTextFlash;

	// Game Over
	[SerializeField]
	private GameObject gameOverTextContainer;
	[SerializeField]
	private Text gameOverText;
	[SerializeField]
	private Text gameOverTextBg;


	[Header("Misc.")]
	[SerializeField]
	private Lab5Sounds audioManager;
	[SerializeField]
	private Transform angleIndicator;


	[Header("Debug")]
	[SerializeField]
	private float ballSpeed = 0.0f;
	[SerializeField]
	private float ballAngle = 0.0f;
	[SerializeField]
	private float powerMeterTick = 0.1f;
	[SerializeField]
	private float angleMeterTick = 0.1f;


	// Setup
	private IEnumerator Start()
	{
		yield return this.StartCoroutine(this.SetupSimulation());
		yield return this.StartCoroutine(this.RunSimulation());
		yield return this.StartCoroutine(this.EndGame());
	}

	private IEnumerator SetupSimulation()
	{
		// Game setup UI
		this.startContainer.SetActive(true);
		yield return new WaitUntil(() => this.hasStarted == true);
		this.startContainer.SetActive(false);

		this.playerSelectContainer.SetActive(true);
		yield return new WaitUntil(() => this.hasSelectedPlayers == true);
		this.playerSelectContainer.SetActive(false);

		this.roundSelectContainer.SetActive(true);
		yield return new WaitUntil(() => this.hasSelectedRounds == true);
		this.roundSelectContainer.SetActive(false);

		this.difficultySelectContainer.SetActive(true);
		yield return new WaitUntil(() => this.hasSelectedDifficulty == true);
		this.difficultySelectContainer.SetActive(false);

		// Players
		for(int i = 0; i < this.numberOfPlayers; i++)
		{
			this.playerPointsList.Add(0);

			this.scoreTextList[i].text = "Player " + (i + 1).ToString() + ": 0";
			this.scoreBgsTextList[i].text = "Player " + (i + 1).ToString() + ": 0";
			this.scoreTextContainers[i].SetActive(true);
		}

		// Meters
		this.powerMeterSlider.minValue = this.minSpeed;
		this.powerMeterSlider.maxValue = this.maxSpeed;
		this.powerMeterSlider.value = this.minSpeed;

		this.angleMeterSlider.minValue = this.minAngle;
		this.angleMeterSlider.maxValue = this.maxAngle;
		this.angleMeterSlider.value = this.minAngle;
		this.angleIndicator.localRotation = Quaternion.Euler(this.minAngle, 0.0f, 0.0f);

		this.powerMeterTick = this.powerTickValueList[this.difficulty];
		this.angleMeterTick = this.angleTickValueList[this.difficulty];

		yield return null;
	}


	// Main stuff
	private IEnumerator RunSimulation()
	{
		this.gameUIContainer.SetActive(true);

		// Run round of turns
		for(int i = 0; i < this.numberOfRounds; i++)
		{
			this.currentRound = i;
			Debug.Log("Round: " + (this.currentRound + 1).ToString());

			// Run each player's turn
			for(int j = 0; j < this.numberOfPlayers; j++)
			{
				this.currentPlayer = j;
				Debug.Log("Player " + (this.currentPlayer + 1).ToString() + "'s Turn");

				yield return this.StartCoroutine(this.RunTurn());

				// Next player setup
				this.currentPlayer = this.currentPlayer < (this.numberOfPlayers - 1) ? this.currentPlayer++ : 0;
			}

			// Next round of turns
			this.currentRound++;
		}
		yield return null;
	}

	private IEnumerator RunTurn()
	{
		// Update UI
		this.currentPlayerText.text = "Player " + (this.currentPlayer + 1).ToString();
		this.currentPlayerTextBg.text = "Player " + (this.currentPlayer + 1).ToString();
		this.currentRoundText.text = "Round " + (this.currentRound + 1).ToString();
		this.currentRoundTextBg.text = "Round " + (this.currentRound + 1).ToString();
		this.spaceTextContainer.SetActive(true);
		this.spaceTextFlash.StartCoroutine(this.spaceTextFlash.Flash());

		// Instantiate ball prefab
		this.currentBallObj = Instantiate(this.baseballPrefab, this.ballSpawnPosition, Quaternion.identity) as GameObject;
		this.currentBallObj.name = "Player" + this.currentPlayer + "Ball";
		this.currentBallRigidbody = this.currentBallObj.GetComponent<Rigidbody>();
		this.currentBallObj.GetComponent<Baseball>().SetLabManager(this);

        // Enable tap area
        this.tapAreaButton.interactable = true;

		// Get user input
		yield return this.StartCoroutine(this.RunPowerMeter());
		yield return this.StartCoroutine(this.RunAngleMeter());
		yield return new WaitForSeconds(0.1f);

		// Wait for ball hit
		//yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitUntil(() => this.hasTapped == true);
        this.hasTapped = false;

        // Disable tap area
        this.tapAreaButton.interactable = false;

        // Update UI
        this.spaceTextContainer.SetActive(false);
		this.spaceTextFlash.StopCoroutine(this.spaceTextFlash.Flash());

		// SFX & Camera
		this.ballCamToggle.interactable = false;
		this.audioManager.PlayBatHit();
		this.currentBallObj.GetComponent<Baseball>().TurnOnTrail();
		if(this.isUsingBallCamera)
		{
			this.cameraList[this.currentCamera].SetActive(false);
			this.currentBallObj.GetComponent<Baseball>().GetCamera().SetActive(true);
			this.currentBallObj.GetComponent<Baseball>().StartCoroutine(this.currentBallObj.GetComponent<Baseball>().MoveCamera());
		}

		// Move ball and wait to stop
		yield return this.StartCoroutine(this.MoveBall());

		yield return new WaitForSeconds(1.0f);

		// Reset/End
		if(this.isUsingBallCamera)
		{
			this.currentBallObj.GetComponent<Baseball>().GetCamera().SetActive(false);
			this.cameraList[this.currentCamera].SetActive(false);
			this.cameraList[0].SetActive(true);
		}
		this.ballCamToggle.interactable = true;
		this.currentBallObj.GetComponent<Baseball>().EndLyfe();
	}

	private IEnumerator RunPowerMeter()
	{
		bool goingUp = true;
		this.powerMeterContainer.SetActive(true);

		float value = this.minSpeed;
		this.powerMeterSlider.value = value;

		while(!this.hasTapped)
		{
			if(goingUp)
			{
				value += this.powerMeterTick;
				this.powerMeterSlider.value = value;

				if(value >= this.powerMinSweetSpot && value <= this.powerMaxSweetSpot)
				{
					this.powerSweetImage.color = Color.white;
				}
				else
				{
					this.powerSweetImage.color = Color.black;
				}

				if(value >= this.maxSpeed)
				{
					goingUp = false;
				}
			}
			else
			{
				value -= this.powerMeterTick;
				this.powerMeterSlider.value = value;

				if(value >= this.powerMinSweetSpot && value <= this.powerMaxSweetSpot)
				{
					this.powerSweetImage.color = Color.white;
				}
				else
				{
					this.powerSweetImage.color = Color.black;
				}

				if(value <= this.minSpeed)
				{
					goingUp = true;
				}
			}

			yield return null;
		}

        this.hasTapped = false;
		this.audioManager.PlayInput();

		this.ballSpeed = this.powerMeterSlider.value;
		Debug.Log("Ball Speed: " + this.ballSpeed.ToString());

		yield return new WaitForSeconds(0.75f);
		this.powerMeterContainer.SetActive(false);

	}

	private IEnumerator RunAngleMeter()
	{
		bool goingUp = true;
		this.angleMeterContainer.SetActive(true);
		this.angleIndicator.gameObject.SetActive(true);

		float value = this.minAngle;
		this.angleMeterSlider.value = value;
		this.angleIndicator.localRotation = Quaternion.Euler(this.minAngle, 0.0f, 0.0f);

		while(!this.hasTapped)
		{
			if(goingUp)
			{
				value += this.angleMeterTick;
				this.angleMeterSlider.value = value;
				this.angleIndicator.localRotation = Quaternion.Euler(value, 0.0f, 0.0f);

				if(value >= this.angleMinSweetSpot && value <= this.angleMaxSweetSpot)
				{
					this.angleSweetImage.color = Color.white;
				}
				else
				{
					this.angleSweetImage.color = Color.black;
				}

				if(value >= this.maxAngle)
				{
					goingUp = false;
				}
			}
			else
			{
				value -= this.angleMeterTick;
				this.angleMeterSlider.value = value;
				this.angleIndicator.localRotation = Quaternion.Euler(value, 0.0f, 0.0f);

				if(value >= this.angleMinSweetSpot && value <= this.angleMaxSweetSpot)
				{
					this.angleSweetImage.color = Color.white;
				}
				else
				{
					this.angleSweetImage.color = Color.black;
				}

				if(value <= this.minAngle)
				{
					goingUp = true;
				}
			}

			yield return null;
		}

        this.hasTapped = false;
		this.audioManager.PlayInput();

		this.ballAngle = this.angleMeterSlider.value;
		Debug.Log("Ball Angle: " + this.ballAngle.ToString());

		yield return new WaitForSeconds(0.75f);
		this.angleMeterContainer.SetActive(false);
		this.angleIndicator.gameObject.SetActive(false);
	}

	private IEnumerator MoveBall()
	{
		Vector3 direction = Quaternion.Euler(this.ballAngle, 0.0f, 0.0f) * this.currentBallObj.transform.forward;
		direction.Normalize();

		Debug.DrawRay(this.currentBallObj.transform.position, direction * 100.0f, Color.green);

		this.currentBallRigidbody.velocity = direction * this.ballSpeed;

		// Wait for ball to stop moving
		while(Mathf.Abs(this.currentBallRigidbody.velocity.sqrMagnitude) > Mathf.Epsilon)
		{
			yield return null;
		}
	}

	private IEnumerator EndGame()
	{
		yield return new WaitForSeconds(0.1f);

		int highest = 0;
		List<int> winnerList = new List<int>();
		List<int> winnerScoreList = new List<int>();

		for(int i = 0; i < this.numberOfPlayers; i++)
		{
			if(this.playerPointsList[i] >= highest)
			{
				winnerList.Add(i);
				winnerScoreList.Add(this.playerPointsList[i]);
				highest = this.playerPointsList[i];

				for(int j = 0; j < winnerScoreList.Count; j++)
				{
					if(this.playerPointsList[i] > winnerScoreList[j])
					{
						winnerList.RemoveAt(j);
						winnerScoreList.RemoveAt(j);
					}
				}
			}
		}

		string winnerText = "Player ";

		for(int i = 0; i < winnerList.Count; i++)
		{
			winnerText += (winnerList[i] + 1).ToString();

			if(winnerList.Count - (i + 1) > 0)
			{
				winnerText += " & ";
			}

		}

		this.gameUIContainer.SetActive(false);
		this.gameOverTextContainer.SetActive(true);

		if(winnerList.Count > 1)
		{
			this.gameOverText.text = winnerText + " Win!";
			this.gameOverTextBg.text = winnerText + " Win!";
		}
		else
		{
			this.gameOverText.text = winnerText + " Wins!";
			this.gameOverTextBg.text = winnerText + " Wins!";
		}

		yield return null;
	}


	// Update TODO: switch camera button, remove update
	private void Update()
	{
		if(Input.GetMouseButtonDown(1))
		{
			this.CameraSwitch();
		}	
	}

    // Ball Hit Mobile
    public void OnTapAreaInput()
    {
        this.hasTapped = true;
    }

	// Ball landing
	public void ScoreHomeRun()
	{
		this.audioManager.StartCoroutine(this.audioManager.PlayCheer());

		this.playerPointsList[this.currentPlayer]++;

		this.scoreTextList[this.currentPlayer].text = "Player " + (this.currentPlayer + 1).ToString() + ": " + this.playerPointsList[this.currentPlayer];
		this.scoreBgsTextList[this.currentPlayer].text = "Player " + (this.currentPlayer + 1).ToString() + ": " + this.playerPointsList[this.currentPlayer];

		Debug.Log("Player " + (this.currentPlayer + 1).ToString() + " has " + this.playerPointsList[this.currentPlayer].ToString() + " point(s)");
	}

	public void HitWall()
	{
		this.audioManager.PlaySad();
	}


	// Camera/UI stuff
	private void CameraSwitch()
	{
		this.currentCamera++;
		if(this.currentCamera >= cameraList.Count)
		{
			this.currentCamera = 0;
		}

		for(int i = 0; i < cameraList.Count; i++)
		{
			cameraList[i].SetActive(this.currentCamera == i ? true : false);
		}

		if(this.isUsingBallCamera)
		{
			this.currentBallObj.GetComponent<Baseball>().GetCamera().SetActive(false);
			this.ballCamToggle.isOn = false;
			this.isUsingBallCamera = false;
		}

		//this.currentCameraText.text = "CAM " + (this.currentCamera + 1).ToString();
		//this.currentCameraTextBG.text = "CAM " + (this.currentCamera + 1).ToString();
	}

	public void ToggleUseBallCamera()
	{
		this.isUsingBallCamera = !this.isUsingBallCamera;
	}

	public void RestartApplication()
	{
		SceneManager.LoadScene(1);
	}

	public void CloseApplication()
	{
		SceneManager.LoadScene(0);
	}

	public void SelectPlayers(int value)
	{
		this.numberOfPlayers = value;
		this.hasSelectedPlayers = true;
	}

	public void SelectRounds(int value)
	{
		this.numberOfRounds = value;
		this.hasSelectedRounds = true;
	}

	public void SelectDifficulty(int value)
	{
		this.difficulty = value;
		this.hasSelectedDifficulty = true;
	}

	// Get/Set
	public GameObject GetCurrentBall()
	{
		return this.currentBallObj;
	}

	public void ToggleHasStarted()
	{
		this.hasStarted = !this.hasStarted;
	}
}
