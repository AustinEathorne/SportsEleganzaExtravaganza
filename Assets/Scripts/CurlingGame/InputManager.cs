using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	[SerializeField]
	private GameManager gameManager;
	[SerializeField]
	private AudioManager audioManager;

	private bool isWaitingForInput = false;
	private bool isMovingRock = false;

	public IEnumerator GetShotInput()
	{
		this.isWaitingForInput = true;
		Debug.Log("Waiting for key down");
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

		this.isMovingRock = true;

		// Wait for player to release the space key or if the rock has passed the line
		while(!Input.GetKeyUp(KeyCode.Space))
		{
			if(this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
			{
				this.isWaitingForInput = false;
				this.isMovingRock = false;
				yield break;
			}
			yield return null;
		}

		this.isWaitingForInput = false;
		this.isMovingRock = false;
		// Debug.Log("Key released");
	}

	private void Update()
	{
		if(gameManager.GetIsAllowedToPosition())
		{
			// Debug.Log("Can Position");
			this.GetRockMovementInput();
		}

		if(gameManager.GetIsAllowedToSweep() && this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
		{
			this.GetSweepInput();
		}

		this.GetShotPreviewInput();
	}

	private void GetRockMovementInput()
	{
		if(Input.GetKey(KeyCode.A))
		{
			gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.left / 6);
		}
		else if(Input.GetKey(KeyCode.D))
		{
			gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.right / 6);
		}
		else if(Input.GetKey(KeyCode.W)) 
		{
			gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.forward / 6);
		}
		else if(Input.GetKey(KeyCode.S))
		{
			gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.back / 6);
		}
	}

	private void GetSweepInput()
	{
		if(Input.GetKey(KeyCode.Space))
		{
			// Debug.Log("Sweep");
			this.gameManager.DecreaseFrictionCoefficient();
			this.gameManager.StartCoroutine(this.gameManager.GetActiveRock().GetComponent<CurlingRock>().BroomSweep());
		}
	}

	private void GetShotPreviewInput()
	{
		if(Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
		{
			this.gameManager.GetRingPreviewScreen().MoveScreen();
		}
	}

	// Get/Set
	public bool GetIsMovingRock()
	{
		return this.isMovingRock;
	}

	public bool GetIsWaitingForInput()
	{
		return this.isWaitingForInput;
	}
}
