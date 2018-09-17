using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControllerDingerTime : MonoBehaviour
{
    public RectTransform touchArea;

    public UnityEvent touchEvent;



    public IEnumerator Start()
    {
        if (Application.isEditor)
        {
            this.StartCoroutine(this.RunEditor());
        }
        else
        {
            this.StartCoroutine(this.RunMobile());
        }

        yield return null;
    }

    private IEnumerator RunEditor()
    {
        while (Application.isPlaying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(this.touchArea, Input.mousePosition))
                {
                    this.touchEvent.Invoke();
                }
            }

            yield return null;
        }

        yield return null;
    }

    private IEnumerator RunMobile()
    {
        while (Application.isPlaying)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(this.touchArea, Input.GetTouch(0).position))
                    {
                        this.touchEvent.Invoke();
                    }                    
                }
            }

            yield return null;
        }

        yield return null;
    }
}