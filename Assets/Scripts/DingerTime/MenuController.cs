using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	[Header("Camera")]
	// Camera Objs
	[SerializeField]
	private List<Transform> cameraTransforms = new List<Transform>();
	[SerializeField]
	private List<Vector3> cameraStartPositions = new List<Vector3>();
	[SerializeField]
	private List<Vector3> cameraEndPositions = new List<Vector3>();
	[SerializeField]
	private float cameraSpeed = 5.0f;
	// Camera Transitions
	[SerializeField]
	private RectTransform transitionTransform;
	[SerializeField]
	private float transitionSpeed = 3.0f;
	[SerializeField]
	private float transitionStartSize = 0.1f;
	[SerializeField]
	private float transitionEndSize = 5.0f;
	[SerializeField]
	private float transitionWaitTime = 1.5f;
	[SerializeField]
	private float transitionStartDistance = 1.0f;
	[SerializeField]
	private float transitionRotationTick = 1.0f;


	[Header("Level Transition")]
	[SerializeField]
	private RectTransform levelTransitionTransform;
	[SerializeField]
	private RectTransform levelTransitionDogeTransform;
	[SerializeField]
	private Vector3 levelTransitionEndPosition;
	[SerializeField]
	private float levelTransitionSpeed = 10.0f;
	[SerializeField]
	private float levelTransitionRotationTick = 90.0f;

	private bool isTransitioning = false;
	private bool isSpinningRight = false;


	[Header("Lab Buttons")]
	[SerializeField]
	private List<RectTransform> labButtonTransforms =  new List<RectTransform>();
	[SerializeField]
	private List<Vector3> labButtonStartPositions = new List<Vector3>();
	[SerializeField]
	private List<Vector3> labButtonEndPositions = new List<Vector3>();
	[SerializeField]
	private float labButtonSpeed = 5.0f;
	[SerializeField]
	private float labButtonStartDelay = 1.0f;
	[SerializeField]
	private float labButtonMoveDelay = 0.5f;

	private bool isMovingButtonsIn = false;

	// Misc.
	private bool isLoadingLevel = false;
	private bool hasSwitchedToNewCam = false;

	[Header("Debug")]
	[SerializeField]
	private float transitionRotation = 0.0f;
	[SerializeField]
	private float levelTransitionRotation = 0.0f;


	private IEnumerator Start()
	{		
		this.StartCoroutine(this.CameraMovement());
		this.StartCoroutine(this.MoveLabButtonsIn());
		yield return null;
	}

	// Controls the flow of camera operations
	private IEnumerator CameraMovement()
	{
		// traverse all cameras
		for(int i = 0; i < this.cameraTransforms.Count; i++)
		{
			// Set start position
			this.cameraTransforms[i].localPosition = this.cameraStartPositions[i];

			// Only set back to true if we're not loading the level
			if(!this.isLoadingLevel)
			{
				this.hasSwitchedToNewCam = true;
			}

			// Set camera active and move
			this.cameraTransforms[i].gameObject.SetActive(true);
			yield return this.StartCoroutine(this.MoveCameraToPosition(this.cameraTransforms[i], this.cameraEndPositions[i]));
			this.cameraTransforms[i].localPosition = this.cameraEndPositions[i];

			// Set camera inactive 
			this.hasSwitchedToNewCam = false;
			this.cameraTransforms[i].gameObject.SetActive(false);
		}

		// Restart
		this.StartCoroutine(this.CameraMovement());
		yield return null;
	}

	// Move camera transform to position over time, start camera transition at x distance
	private IEnumerator MoveCameraToPosition(Transform transform, Vector3 targetPos)
	{
		while(Vector3.Distance(transform.localPosition, targetPos) >= 0.01f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, this.cameraSpeed * Time.deltaTime);

			if(Vector3.Distance(transform.localPosition, targetPos) <= this.transitionStartDistance && !this.isTransitioning && !this.isLoadingLevel)
			{ 
				this.StartCoroutine(this.CameraTransition());
			}

			yield return null;
		}
		yield return null;
	}

	// Scales transition image
	private IEnumerator CameraTransition()
	{
		this.isTransitioning = true;
		this.isSpinningRight = !this.isSpinningRight;

		this.transitionTransform.localScale = new Vector2(this.transitionStartSize, this.transitionStartSize);
		this.transitionTransform.gameObject.SetActive(true);

		float size = this.transitionStartSize;

		while(size < this.transitionEndSize)
		{
			size = Mathf.MoveTowards(size, this.transitionEndSize, this.transitionSpeed * Time.deltaTime);
			this.transitionTransform.localScale = new Vector2(size, size);
			yield return null;
		}

		size = this.transitionEndSize;
		this.isSpinningRight = !this.isSpinningRight;
		yield return new WaitForSeconds(this.transitionWaitTime);

		while(size > this.transitionStartSize)
		{
			size = Mathf.MoveTowards(size, this.transitionStartSize, this.transitionSpeed * Time.deltaTime);
			this.transitionTransform.localScale = new Vector2(size, size);
			yield return null;
		}

		this.isSpinningRight = !this.isSpinningRight;
		this.transitionTransform.gameObject.SetActive(false);
		this.isTransitioning = false;
		yield return null;
	}

	// Move lab buttons in
	private IEnumerator MoveLabButtonsIn()
	{
		yield return new WaitForSeconds(this.labButtonStartDelay);
		this.isMovingButtonsIn = true;

		for(int i = 0; i < this.labButtonTransforms.Count; i++)
		{
			this.StartCoroutine(this.MoveUI(this.labButtonTransforms[i], this.labButtonEndPositions[i], this.labButtonSpeed, false));
			yield return new WaitForSeconds(this.labButtonMoveDelay);
			this.labButtonTransforms[i].GetComponent<Button>().interactable = true;
		}

		yield return new WaitForSeconds(0.5f);
		this.isMovingButtonsIn = false;
	}

	// Move lab buttons out
	private IEnumerator MoveLabButtonsOut()
	{
		for(int i = this.labButtonTransforms.Count - 1; i > -1; i--)
		{
			this.StartCoroutine(this.MoveUI(this.labButtonTransforms[i], this.labButtonStartPositions[i], this.labButtonSpeed, false));
			yield return new WaitForSeconds(this.labButtonMoveDelay);
			this.labButtonTransforms[i].GetComponent<Button>().interactable = false;
		}
	}

	// Move UI element to desired position
	private IEnumerator MoveUI(RectTransform rt, Vector3 target, float speed, bool isLerp)
	{
		while(Vector3.Distance(rt.anchoredPosition, target) > 0.5f)
		{
			if(isLerp)
			{
				rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, target, speed * Time.deltaTime);
			}
			else
			{
				rt.anchoredPosition = Vector3.MoveTowards(rt.anchoredPosition, target, speed * Time.deltaTime);
			}
			yield return null;
		}
	}

	// Called on buttonClick, stops all coroutines to ensure UI doesnt get stuck between in/out
	public void StartLevelCo(int index)
	{
		this.StartCoroutine(this.StartLevel(index));
	}

	// UI stuff + load level
	private IEnumerator StartLevel(int index)
	{
		this.cameraSpeed *= 0.5f;
		Debug.Log(this.cameraSpeed.ToString());
		this.isLoadingLevel = true;

		// wait for buttons to finish moving in
		if(this.isMovingButtonsIn)
		{
			Debug.Log("Still moveing in");
			yield return new WaitUntil(() => this.isMovingButtonsIn == false);
		}

		// Move all buttons out except for the selected one
		for(int i = this.labButtonTransforms.Count - 1; i > -1; i--)
		{
			if(i != (index - 1))
			{
				this.StartCoroutine(this.MoveUI(this.labButtonTransforms[i], this.labButtonStartPositions[i], this.labButtonSpeed, false));
				yield return new WaitForSeconds(this.labButtonMoveDelay);
			}
		}

		// Wait for one moe camera transition & then begin level transition & load the scene
		if(this.isTransitioning)
		{
			yield return new WaitUntil(() => this.isTransitioning == false);
		}

		//yield return new WaitUntil(() => this.hasSwitchedToNewCam == false);
		yield return new WaitForSeconds(0.25f);
		yield return this.StartCoroutine(this.MoveUI(this.levelTransitionTransform, this.levelTransitionEndPosition, this.levelTransitionSpeed, false));
		SceneManager.LoadScene(index);
	}
	
	// Transitioning check
	private void Update()
	{
		if(this.isTransitioning)
		{
			this.SpinTransitionImage(this.transitionTransform, this.isSpinningRight, true);
		}

		if(this.isLoadingLevel)
		{
			this.SpinTransitionImage(this.levelTransitionDogeTransform, false, false);
		}
	}

	// Spin when isTransitioning == true
	private void SpinTransitionImage(RectTransform rt, bool isRight, bool isCameraTransition)
	{
		float rot;
		if(isCameraTransition)
		{
			rot = this.transitionRotation;
		}
		else
		{
			rot = this.levelTransitionRotation;
		}

		if(isRight)
		{
			rot -= this.transitionRotationTick * Time.deltaTime;
		}
		else
		{
			rot += this.transitionRotationTick * Time.deltaTime;
		}

		if(isCameraTransition)
		{
			this.transitionRotation = rot;
		}
		else
		{
			this.levelTransitionRotation = rot;
		}

		rt.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, rot));
	}
}
