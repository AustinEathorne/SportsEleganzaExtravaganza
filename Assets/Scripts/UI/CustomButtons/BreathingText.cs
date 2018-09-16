using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingText : CanvasEffect
{
    [Header("Text")]
    public List<RectTransform> textTransformList;

    [Header("Scale")]
    [Range(0.5f, 0.9f)]
    public float minScale;
    [Range(0.95f, 1.0f)]
    public float maxScale;

    [Header("Scale Time")]
    public float enableScaleTime;
    public float scaleUpTime;
    public float scaleDownTime;

    // Routines
    private IEnumerator scaleRoutine;



    public override IEnumerator Run()
    {
        // Stop routines
        if (this.stopRoutine != null)
        {
            this.StopCoroutine(this.stopRoutine);
        }
        if (this.scaleRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.scaleRoutine);
        }

        // Set scale to 0
        foreach (RectTransform rt in this.textTransformList)
        {
            rt.localScale = new Vector3(0, 0, 1);
        }

        // OnEnable scale up
        this.scaleRoutine = UIUtility.Instance.ScaleListOverTime(
            this.textTransformList, this.maxScale, this.enableScaleTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleRoutine);

        while (this.runRoutine != null)
        {
            // Scale down
            this.scaleRoutine = UIUtility.Instance.ScaleListOverTime(
                this.textTransformList, this.minScale, this.scaleDownTime, true, true);
            yield return UIUtility.Instance.StartCoroutine(this.scaleRoutine);

            // Scale up
            this.scaleRoutine = UIUtility.Instance.ScaleListOverTime(
                this.textTransformList, this.maxScale, this.scaleUpTime, true, true);
            yield return UIUtility.Instance.StartCoroutine(this.scaleRoutine);

            yield return null;
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
        if (this.scaleRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.scaleRoutine);
        }

        // Scale down
        this.scaleRoutine = UIUtility.Instance.ScaleListOverTime(
               this.textTransformList, this.minScale, this.scaleDownTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleRoutine);

        yield return null;
    }
}
