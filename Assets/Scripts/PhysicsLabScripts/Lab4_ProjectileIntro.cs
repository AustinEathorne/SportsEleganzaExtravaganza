using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lab4_ProjectileIntro : MonoBehaviour 
{
	[Header("Towers")]
	[SerializeField]
	private GameObject towerPrefab;

	private List<Transform> towerTransforms = new List<Transform>();

	[SerializeField]
	private int numberOfTowers = 0;

	[SerializeField]
	private float spawnPosition = 0.0f;
	[SerializeField]
	private float respawnPosition = 0.0f;

	[SerializeField]
	private float towerMinHeight = 0.0f;
	[SerializeField]
	private float towerMaxHeight = 0.0f;
	[SerializeField]
	private float towerHeightModifier = 0.0f;

	[SerializeField]
	private float towerMinSpeed = 0.0f;
	[SerializeField]
	private float towerMaxSpeed = 0.0f;

	[SerializeField]
	private float towerMinDistance = 0.0f;
	[SerializeField]
	private float towerMaxDistance = 0.0f;

	private List<float> towerSpeeds = new List<float>();

	[Header("JumpObject")]
	[SerializeField]
	private Transform jumpObjTransform;
	[SerializeField]
	private Rigidbody jumpObjRigidbody;


	// Simulation
	private int activeTower = 0;
	private float jumpObjDistanceToJumpAt = 0.0f;
	private bool isJumping = false;
	private bool isSimulating = false;

	[Header("UI")]
	[SerializeField]
	private GameObject gameSetupContainer;
	[SerializeField]
	private Slider minHeightSlider;
	[SerializeField]
	private Slider maxHeightSlider;
	[SerializeField]
	private Text minHeightText;
	[SerializeField]
	private Text maxHeightText;
	[SerializeField]
	private Text minHeightTextBG;
	[SerializeField]
	private Text maxHeightTextBG;
	[SerializeField]
	private Slider numTowersSlider;
	[SerializeField]
	private Text numTowersText;
	[SerializeField]
	private Text numTowersTextBG;


	[SerializeField]
	private GameObject startTextContainer;
	[SerializeField]
	private float startFlashTime = 1.5f;

	[SerializeField]
	private Text distanceText;
	[SerializeField]
	private Text distanceTextBG;
	[SerializeField]
	private Text velocityText;
	[SerializeField]
	private Text velocityTextBG;

	[SerializeField]
	private Button exitButton;
	[SerializeField]
	private Button restartButton;


	// Main stuff
	private IEnumerator Start()
	{
		
		yield return this.StartCoroutine(SetupInputSliders());

		// Wait for user input
		float timeCount = 0.0f;
		while(!Input.GetKeyDown(KeyCode.Space))
		{
			timeCount += Time.deltaTime;

			if(timeCount >= this.startFlashTime)
			{
				timeCount = 0.0f;
				this.startTextContainer.SetActive(!this.startTextContainer.activeSelf);
			}

			yield return null;
		}

		// Get Values from sliders
		this.towerMinHeight = this.minHeightSlider.value;
		this.towerMaxHeight = this.maxHeightSlider.value;
		this.numberOfTowers = (int)this.numTowersSlider.value;

		yield return this.StartCoroutine(this.SetupSimulation());

		this.startTextContainer.SetActive(false);
		this.gameSetupContainer.SetActive(false);
		this.isSimulating = true;
	}

	public IEnumerator SetupInputSliders()
	{
		this.minHeightSlider.minValue = this.towerMinHeight - this.towerHeightModifier;
		this.minHeightSlider.maxValue = this.towerMinHeight + this.towerHeightModifier;

		this.maxHeightSlider.minValue = this.towerMaxHeight - this.towerHeightModifier;
		this.maxHeightSlider.maxValue = this.towerMaxHeight + this.towerHeightModifier;

		yield return null;
	}

	public void UpdateSetupSliders()
	{
		this.minHeightText.text = System.Math.Round(this.minHeightSlider.value, 2).ToString();
		this.minHeightTextBG.text = System.Math.Round(this.minHeightSlider.value, 2).ToString();

		this.maxHeightText.text = System.Math.Round(this.maxHeightSlider.value, 2).ToString();
		this.maxHeightTextBG.text = System.Math.Round(this.maxHeightSlider.value, 2).ToString();

		this.numTowersText.text = this.numTowersSlider.value.ToString();
		this.numTowersTextBG.text = this.numTowersSlider.value.ToString();
	}

	private IEnumerator SetupSimulation()
	{
		float rand;
		float distanceCount = 0.0f;

		// Setup towers
		for(int i = 0; i < this.numberOfTowers; i++)
		{
			// Instantiate and get transform reference
			GameObject tower = Instantiate(towerPrefab, new Vector3(0.0f, 0.0f, this.spawnPosition), Quaternion.identity) as GameObject;
			this.towerTransforms.Add(tower.GetComponent<Transform>());

			// Scale towers
			rand = Random.Range(this.towerMinHeight, this.towerMaxHeight);
			this.towerTransforms[i].localScale = new Vector3(this.towerTransforms[i].localScale.x, rand, this.towerTransforms[i].localScale.z);

			// Position towers
			rand = Random.Range(this.towerMinDistance, this.towerMaxDistance);
			this.towerTransforms[i].localPosition = new Vector3(this.towerTransforms[i].localPosition.x, this.towerTransforms[i].localScale.y * 0.5f, distanceCount - rand);
			distanceCount -= rand;

			// Assign tower speeds
			rand = Random.Range(this.towerMinSpeed, this.towerMaxSpeed);
			this.towerSpeeds.Add(rand);
		}

		yield return null;
	}

	private void FixedUpdate()
	{
		this.velocityText.text = "Velocity: " + System.Math.Round(this.jumpObjRigidbody.velocity.y, 2).ToString();
		this.velocityTextBG.text = "Velocity: " + System.Math.Round(this.jumpObjRigidbody.velocity.y, 2).ToString();

		if(this.isSimulating)
		{
			this.ResetJump();
			this.MoveTowers();
			this.MoveObject();
		}
	}

	// Towers
	private void MoveTowers()
	{
		for(int i = 0; i < this.numberOfTowers; i++)
		{
			if(this.IsTowerOutOfBounds(this.towerTransforms[i]))
			{
				this.RespawnTower(this.towerTransforms[i]);
			}

			this.towerTransforms[i].GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, this.towerSpeeds[i]);
		}
	}

	private bool IsTowerOutOfBounds(Transform towerTransform)
	{
		if(towerTransform.localPosition.z >= this.respawnPosition)
		{
			return true;
		}

		return false;
	}

	private void RespawnTower(Transform towerTransform)
	{
		spawnPosition = -(this.numberOfTowers * this.towerMaxDistance);
		towerTransform.localPosition = new Vector3(0.0f, towerTransform.localScale.y * 0.5f, this.spawnPosition);
	}

	// Jump object
	private void MoveObject()
	{
		if(!this.isJumping && this.CheckForJump())
		{
			this.isJumping = true;
			Debug.Log("Jump");

			// Get time until jump obj and tower's position are equal
			float timeToTower = this.GetTimeToActiveTower();
			//Debug.Log("Time to tower: " + timeToTower);

			// Get jump distance
			float distance = (this.towerTransforms[this.activeTower].localScale.y - this.jumpObjTransform.localPosition.y) + 1.5f;
			this.distanceText.text = "Jump height: " + System.Math.Round(distance, 2).ToString();
			this.distanceTextBG.text = "Jump height: " + System.Math.Round(distance, 2).ToString();
			//Debug.Log("Jump distance: " + distance.ToString());

			// Get initial velocity and apply to jump object
			float initialVelocity = this.CalculateInitialVelocity(distance,  Physics.gravity.y, timeToTower);
			Vector3 vel = new Vector3(0.0f, initialVelocity, 0.0f);
			this.jumpObjRigidbody.velocity = vel;

			// Next tower
			this.activeTower++;
			if(this.activeTower >= this.numberOfTowers)
			{
				this.activeTower = 0;
			}
			//Debug.Log("Active tower: " + this.activeTower.ToString());

		}
	}

	private void ResetJump()
	{
		// Check if the jump object is on the ground
		if(this.jumpObjTransform.localPosition.y <= (this.jumpObjTransform.localScale.y * 0.5f) + 0.1f)
		{
			this.isJumping = false;
			this.distanceText.text = "Jump height: 0";
			this.distanceTextBG.text = "Jump height: 0";
			//Debug.Log("Done Jump");
        }
	}

	private bool CheckForJump()
	{
		this.jumpObjDistanceToJumpAt = this.towerSpeeds[this.activeTower];

		if(this.jumpObjTransform.localPosition.z - this.towerTransforms[this.activeTower].localPosition.z <= this.jumpObjDistanceToJumpAt)
		{
			return true;
		}
		return false;
	}

	private float GetTimeToActiveTower()
	{
		// Find distance to next tower
		float distance = this.jumpObjTransform.localPosition.z - this.towerTransforms[this.activeTower].localPosition.z;
		//Debug.Log("Distance to next tower: " + distance.ToString());

		// Find and return time until the jump obj and tower's positions are equal
		return distance / this.towerSpeeds[this.activeTower];
	}

	// Calculate
	private float CalculateTravelTime(float initialVelocity, float finalVelocity, float acceleration)
	{
		float travelTime = (float)((finalVelocity - initialVelocity)/acceleration);
		return travelTime;
	}

	private float CalculateInitialVelocity(float displacement, float acceleration, float time)
	{
		return (displacement - ((0.5f * acceleration) * (time * time))) / time;
	}

	// Button OnClicks
	public void CloseApplication()
	{
		Application.Quit();
	}

	public void RestartSimulation()
	{
		SceneManager.LoadScene(0);
	}
}
