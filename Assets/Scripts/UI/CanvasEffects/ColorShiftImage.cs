using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorShiftImage : CanvasEffect
{
    [Header("Image")]
    [SerializeField]
    private Image image;

    [Header("Colour")]
    [SerializeField]
    private List<Color> colours;
    [SerializeField]
    private bool isUsingImageAlpha;
    private int currentColor;

    [Header("Time")]
    [SerializeField]
    private float shiftTime;

    private IEnumerator colourShiftRoutine;


    public override IEnumerator Run()
    {
        // Stop routines
        if (this.stopRoutine != null)
        {
            this.StopCoroutine(this.stopRoutine);
        }
        if (this.colourShiftRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.colourShiftRoutine);
        }

        while (this.runRoutine != null)
        {
            foreach (Color colour in this.colours)
            {
                this.colourShiftRoutine = UIUtility.Instance.ShiftColour(this.image, this.shiftTime, colour, this.isUsingImageAlpha);
                yield return UIUtility.Instance.StartCoroutine(this.colourShiftRoutine);
            }

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
        if (this.colourShiftRoutine != null)
        {
            UIUtility.Instance.StopCoroutine(this.colourShiftRoutine);
        }

        yield return null;
    }
}
