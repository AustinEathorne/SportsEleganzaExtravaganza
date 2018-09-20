using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCurling : MonoBehaviour
{
	[SerializeField]
	private GameManagerCurling gameManager;
	[SerializeField]
	private AudioManagerCurling audioManager;

    [SerializeField]
    private LayerMask raycastLayer;

    private bool isTouchingRock = false;
	private bool isMovingRock = false;
    private bool isSweepingRock = false;
    private bool isInitialized = false;

    private Vector3 lastTargetPosition;
    private Vector3 mouseOffset;
    private Plane hitPlane;



    #region Main

    public IEnumerator PositionRock()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
                this.StartCoroutine(this.OnRockTouch());
            if (Input.GetMouseButton(0))
                this.StartCoroutine(this.OnRockMove());
            if (Input.GetMouseButtonUp(0))
                this.StartCoroutine(this.OnRockRelease());
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                    this.StartCoroutine(this.OnRockTouch());
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    this.StartCoroutine(this.OnRockMove());
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    this.StartCoroutine(this.OnRockRelease());
            }
        }
        

        yield return null;
    }

    private IEnumerator OnRockTouch()
    {
        Ray inputRay = Application.isMobilePlatform ? this.GenerateTouchRay() : this.GenerateMouseRay();
        RaycastHit hit;

        if (Physics.Raycast(inputRay.origin, inputRay.direction, out hit, Mathf.Infinity, this.raycastLayer))
        {
            if (hit.transform.tag == "Rock")
            {
                this.hitPlane = new Plane(Camera.main.transform.forward * -1, this.gameManager.GetActiveRock().transform.position);

                Ray ray = Application.isMobilePlatform ? Camera.main.ScreenPointToRay(Input.GetTouch(0).position) : 
                    Camera.main.ScreenPointToRay(Input.mousePosition)
                    ;
                float rayDistance;
                this.hitPlane.Raycast(ray, out rayDistance);
                this.mouseOffset = this.gameManager.GetActiveRock().transform.position - ray.GetPoint(rayDistance);

                Debug.Log("Touching Rock");

                this.lastTargetPosition = this.gameManager.GetActiveRock().transform.position - ray.GetPoint(rayDistance);
                this.isTouchingRock = true;
            }
            else
            {
                Debug.Log("Touching Nothing");
            }
        }

        yield return null;
    }
    private IEnumerator OnRockMove()
    {
        if (this.isTouchingRock || this.isMovingRock)
        {
            this.isMovingRock = true;

            this.hitPlane = new Plane(Camera.main.transform.forward * -1, this.gameManager.GetActiveRock().transform.position);
            Ray ray = Application.isMobilePlatform ? Camera.main.ScreenPointToRay(Input.touches[0].position) :
                Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayDistance;
            if (this.hitPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);

                // Make sure our target destination has moved
                if (Vector3.Distance(this.gameManager.GetActiveRock().transform.position, point) > 0.0f)
                {
                    this.gameManager.UpdateRockPosition(point);
                }
            }
        }

        yield return null;
    }
    private IEnumerator OnRockRelease()
    {
        this.isTouchingRock = false;
        this.isMovingRock = false;

        yield return null;
    }


    public IEnumerator AddForceToRock(System.Action<bool> _isAddingForce)
    {
        if (!Application.isMobilePlatform)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            _isAddingForce(true);

            // Wait for player to release the space key or if the rock has passed the line
            while (!Input.GetMouseButtonUp(0))
            {
                if (this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
                {
                    yield break;
                }

                yield return null;
            }
        }
        else
        {
            yield return new WaitUntil(() => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

            _isAddingForce(true);

            // Wait for player to release the space key or if the rock has passed the line
            while (Input.GetTouch(0).phase != TouchPhase.Ended)
            {
                if (this.gameManager.GetActiveRock().GetComponent<CurlingRock>().GetHasPassedReleaseLine())
                {
                    yield break;
                }

                yield return null;
            }
        }

        _isAddingForce(false);

        yield return null;
    }

    public void SetIsSweeping(bool _isSweeping)
    {
        this.isSweepingRock = _isSweeping;
    }
    public IEnumerator SweepRock()
    {
        this.isSweepingRock = true;

        if (!Application.isMobilePlatform)
        {
            while (this.isSweepingRock)
            {
                if (Input.GetMouseButton(0))
                {
                    this.gameManager.DecreaseFrictionCoefficient();
                    this.gameManager.StartCoroutine(this.gameManager.GetActiveRock().GetComponent<CurlingRock>().BroomSweep());

                    this.audioManager.StartCoroutine(this.audioManager.PlayRandomHardClip(Random.Range(0, 3)));
                }

                yield return null;
            }
        }
        else
        {
            while (this.isSweepingRock)
            {
                if (Input.touchCount > 0)
                {
                    this.gameManager.DecreaseFrictionCoefficient();
                    this.gameManager.StartCoroutine(this.gameManager.GetActiveRock().GetComponent<CurlingRock>().BroomSweep());

                    this.audioManager.StartCoroutine(this.audioManager.PlayRandomHardClip(Random.Range(0, 3)));
                }

                yield return null;
            }
        }

        yield return null;
    }

    #endregion

    #region RayCasts

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

    #endregion
}
