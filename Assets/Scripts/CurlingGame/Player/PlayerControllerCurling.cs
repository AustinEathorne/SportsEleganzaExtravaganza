using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCurling : MonoBehaviour {

	[SerializeField]
	private GameManagerCurling gameManager;
	[SerializeField]
	private AudioManagerCurling audioManager;

    [SerializeField]
    private LayerMask raycastLayer;

	private bool isWaitingForInput = false;
	private bool isMovingRock = false;

    private Vector3 lastTargetPosition;
    private Vector3 mouseOffset;
    private Plane hitPlane;



    private void Update()
	{
		if(gameManager.GetIsAllowedToPosition())
		{
			// Debug.Log("Can Position");
            if (Application.isEditor)
            {
                if (Input.GetMouseButton(0))
                {
                    this.GetRockMovementInputEditor();
                }
            }
            else
            {

            }
		}

		if(gameManager.GetIsAllowedToSweep() && this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
		{
			this.GetSweepInput();
		}

		this.GetShotPreviewInput();
	}

    private void GetRockMovementInputEditor()
	{
        Ray mouseRay = this.GenerateMouseRay();
        RaycastHit hit;

        if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, Mathf.Infinity, this.raycastLayer))
        {
            if (hit.transform.tag == "Rock")
            {
                this.hitPlane = new Plane(Camera.main.transform.forward * -1, this.gameManager.GetActiveRock().transform.position);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float rayDistance;
                this.hitPlane.Raycast(ray, out rayDistance);
                this.mouseOffset = this.transform.position - ray.GetPoint(rayDistance);
                this.lastTargetPosition = this.transform.position - ray.GetPoint(rayDistance);
                
                Debug.Log("Touching Rock");
                
                if (this.hitPlane.Raycast(ray, out rayDistance))
                {
                    Vector3 point = ray.GetPoint(rayDistance);

                    // Make sure our target destination has moved
                    if (Vector3.Distance(this.lastTargetPosition, point) > 0.0f)
                    {
                        this.gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(new Vector3(point.x, 0.75f, point.z));
                        Debug.Log("Rock position: " + new Vector3(point.x, 1.0f, point.z).ToString());
                    }
                }
            }
        }
    }

    private void GetRockMovementInputMobile()
    {
        if (Input.GetKey(KeyCode.A))
        {
            gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.left / 6);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.right / 6);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.forward / 6);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameManager.GetActiveRock().GetComponent<CurlingRock>().MoveRock(Vector3.back / 6);
        }
    }

    public IEnumerator GetShotInput()
    {
        this.isWaitingForInput = true;
        Debug.Log("Waiting for key down");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        this.isMovingRock = true;

        // Wait for player to release the space key or if the rock has passed the line
        while (!Input.GetKeyUp(KeyCode.Space))
        {
            if (this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
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


    // Raycast
    private Ray GenerateMouseRay()
    {
        Vector3 mousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 mousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 worldPosFar = Camera.main.ScreenToWorldPoint(mousePositionFar);
        Vector3 worldPosNear = Camera.main.ScreenToWorldPoint(mousePositionNear);

        return new Ray(worldPosNear, worldPosFar - worldPosNear);
    }

    private Ray GenerateTouchRay()
    {
        Vector3 touchPositionFar = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.farClipPlane);
        Vector3 touchPositionNear = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane);

        Vector3 worldPosFar = Camera.main.ScreenToWorldPoint(touchPositionFar);
        Vector3 worldPosNear = Camera.main.ScreenToWorldPoint(touchPositionNear);

        return new Ray(worldPosNear, worldPosFar - worldPosNear);
    }
}
