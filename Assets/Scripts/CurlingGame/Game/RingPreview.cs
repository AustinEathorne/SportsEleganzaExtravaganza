using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPreview : MonoBehaviour {

	[SerializeField]
	private float upSpeed;
	[SerializeField]
	private float downSpeed;

	[SerializeField]
	private Vector3 startPosition;
	[SerializeField]
	private Vector3 targetPosition;

	private bool isMoving = false;
	private bool isMovingUp = false;



	public void MoveScreen()
	{
		if(!this.isMoving)
		{
			this.StartCoroutine(this.isMovingUp ? this.MoveScreenDown() : this.MoveScreenUp());
		}
	}

	public void ScreenPreview()
	{
		if(!this.isMoving)
		{
			this.StartCoroutine(this.MoveScreenDown());
		}
	}

	public IEnumerator MoveScreenDown()
	{
		this.isMoving = true;
		isMovingUp = false;
		this.transform.localPosition = this.startPosition;

		while(Vector3.Distance(this.transform.localPosition, this.targetPosition) >= 0.1f)
		{
			if(this.isMovingUp)
			{
				yield break;
			}

			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, this.targetPosition, this.downSpeed * Time.deltaTime);
			yield return null;
		}
		this.isMoving = false;
	}

	public IEnumerator MoveScreenUp()
	{
		this.isMoving = true;
		this.isMovingUp = true;

		while(Vector3.Distance(this.transform.localPosition, this.startPosition) >= 0.1f)
		{
			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, this.startPosition, this.upSpeed * Time.deltaTime);
			yield return null;
		}

		this.isMoving = false;
	}
}
