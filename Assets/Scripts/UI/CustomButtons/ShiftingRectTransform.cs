using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftingRectTransform :  CanvasEffect
{
    [Header("Text")]
    public RectTransform rectTransform;

    [Header("Bools")]
    public bool isHorizontalShift;
    public bool isInitialTargetMax;

    [Header("Range")]
    [Range(-10f, 0f)]
    public float minPos;
    [Range(0f, 10f)]
    public float maxPos;

    [Header("Shift Time")]
    public float shiftToMinTime;
    public float shiftToMaxTime;

    [Header("Position")]
    public Vector2 startPosition;
    public Vector2 targetPosition;

    // Routines
    private IEnumerator shiftRoutine;


    public void Awake()
    {
        this.startPosition = this.rectTransform.anchoredPosition;

        if (this.isInitialTargetMax)
        {
            float temp = this.minPos;
            this.minPos = maxPos;
            this.maxPos = temp;
        }
    }

    public override IEnumerator Run()
    {
        // Stop routines
        if (this.stopRoutine != null)
        {
            this.StopCoroutine(this.stopRoutine);
        }
        if (this.shiftRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.shiftRoutine);
        }

        // Shift to start target position
        this.targetPosition = this.isHorizontalShift ? 
            new Vector2(this.startPosition.x + this.minPos, this.startPosition.y) :
            new Vector2(this.startPosition.x, this.startPosition.y + this.minPos);

        this.shiftRoutine = UIUtility.Instance.MoveTransformOverTime(
                this.rectTransform, this.targetPosition, this.shiftToMinTime);
        yield return UIUtility.Instance.StartCoroutine(this.shiftRoutine);

        while (this.runRoutine != null)
        {
            // Shift to max position
            this.targetPosition = this.isHorizontalShift ?
                new Vector2(this.startPosition.x + this.maxPos, this.startPosition.y) :
                new Vector2(this.startPosition.x, this.startPosition.y + this.maxPos);

            this.shiftRoutine = UIUtility.Instance.MoveTransformOverTime(
                this.rectTransform, this.targetPosition, this.shiftToMaxTime);
            yield return UIUtility.Instance.StartCoroutine(this.shiftRoutine);

            // Shift to min position
            this.targetPosition = this.isHorizontalShift ?
                new Vector2(this.startPosition.x + this.minPos, this.startPosition.y) :
                new Vector2(this.startPosition.x, this.startPosition.y + this.minPos);

            this.shiftRoutine = UIUtility.Instance.MoveTransformOverTime(
                this.rectTransform, this.targetPosition, this.shiftToMinTime);
            yield return UIUtility.Instance.StartCoroutine(this.shiftRoutine);
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        // Stop routines
        if (this.runRoutine != null)
        {
            this.StopCoroutine(this.runRoutine);
        }
        if (this.shiftRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.shiftRoutine);
        }

        // Shift to min position
        this.targetPosition = this.isHorizontalShift ?
            new Vector2(this.startPosition.x + this.minPos, this.startPosition.y) :
            new Vector2(this.startPosition.x, this.startPosition.y + this.minPos);

        this.shiftRoutine = UIUtility.Instance.MoveTransformOverTime(
            this.rectTransform, this.targetPosition, this.shiftToMinTime);
        yield return UIUtility.Instance.StartCoroutine(this.shiftRoutine);

        yield return null;
    }
}
