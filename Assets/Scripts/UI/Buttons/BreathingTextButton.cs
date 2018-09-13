using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingTextButton : CustomButton
{
    [Space(20)]
    [Header("Text")]
    [SerializeField]
    private List<Text> textList;
    [SerializeField]
    private List<RectTransform> textTransformList;

    [Header("Values")]
    [Header("Colours")]
    public List<Color> idleColours;
    public List<Color> pressedColours;
    [Header("Scale")]
    public float minScale;
    public float maxScale;
    [Header("Turning On")]
    public float onScaleTime;
    public float onFadeTime;
    [Header("Turning Off")]
    public float offScaleTime;
    public float offFadeTime;
    [Header("Idle")]
    public float idleScaleDownTime;
    public float idleScaleUpTime;
    public float idleColourShiftTime;
    [Header("Pressed/Released")]
    public float pressedScaleTime;
    public float pressedColourShiftTime;
    [Header("Exit")]
    public float exitScaleTime;
    public float exitColourShiftTime;

    // Routines
    protected IEnumerator fadeListRoutine;
    protected IEnumerator scaleListRoutine;
    protected IEnumerator shiftColourListRoutine;



    protected override IEnumerator Start()
    {
        // Scale transforms down
        for (int i = 0; i < this.textList.Count; i++)
        {
            this.textTransformList[i].transform.localScale = Vector3.zero;
        }

        yield return base.StartCoroutine(base.Start());

        this.fadeListRoutine = null;
        this.scaleListRoutine = null;
        this.shiftColourListRoutine = null;
    }

    protected override IEnumerator ChangeStates(ButtonState _state)
    {
        if (this.fadeListRoutine != null)
            this.StopCoroutine(this.fadeListRoutine);
        if (this.scaleListRoutine != null)
            this.StopCoroutine(this.scaleListRoutine);
        if (this.shiftColourListRoutine != null)
            this.StopCoroutine(this.shiftColourListRoutine);

        yield return base.StartCoroutine(base.ChangeStates(_state));
    }


    protected override IEnumerator TurnOn()
    {
        // Set text colours to idle
        for(int i = 0; i < this.textList.Count; i++)
        {
            this.textList[i].color = new Color(this.idleColours[i].r, this.idleColours[i].g, this.idleColours[i].b, 0.0f);
        }

        // Start and yield fade/scale routines
        this.fadeListRoutine = UIUtility.Instance.FadeListOverTime(this.textList, this.onFadeTime, 1.0f, true);
        UIUtility.Instance.StartCoroutine(this.fadeListRoutine);

        this.scaleListRoutine = UIUtility.Instance.ScaleListOverTime(this.textTransformList, 1.0f, this.onScaleTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleListRoutine);

        // Start Idle
        this.StartCoroutine(this.AttemptStateChange(ButtonState.Idle));
    }

    protected override IEnumerator TurnOff()
    {
        // Start and yield fade/scale routines
        this.fadeListRoutine = UIUtility.Instance.FadeListOverTime(this.textList, this.offFadeTime, 0.0f, true);
        UIUtility.Instance.StartCoroutine(this.fadeListRoutine);

        this.scaleListRoutine = UIUtility.Instance.ScaleListOverTime(this.textTransformList, 0.0f, this.offScaleTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleListRoutine);
    }

    protected override IEnumerator Idle()
    {
        // Yield scale down
        this.scaleListRoutine = UIUtility.Instance.ScaleListOverTime(this.textTransformList, this.minScale,
            this.idleScaleDownTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleListRoutine);

        // Yield scale up
        this.scaleListRoutine = UIUtility.Instance.ScaleListOverTime(this.textTransformList, this.maxScale,
            this.idleScaleUpTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleListRoutine);

        // Continue running idle routine
        this.stateRoutineDictionary[ButtonState.Idle] = this.Idle();
        this.StartCoroutine(this.stateRoutineDictionary[ButtonState.Idle]);

        yield return null;
    }

    protected override IEnumerator Pressed()
    {
        // Scale to min scale
        this.scaleListRoutine = UIUtility.Instance.ScaleListOverTime(this.textTransformList, this.minScale,
            this.pressedScaleTime, true, true);
        UIUtility.Instance.StartCoroutine(this.scaleListRoutine);

        // Yield shift to pressed colours
        this.shiftColourListRoutine = UIUtility.Instance.ShiftListColours(this.textList, this.pressedColours,
            this.pressedColourShiftTime, true);
        yield return UIUtility.Instance.StartCoroutine(this.shiftColourListRoutine);
    }

    protected override IEnumerator Released()
    {
        // yield shift to pressed colours
        this.shiftColourListRoutine = UIUtility.Instance.ShiftListColours(this.textList, this.pressedColours,
            this.pressedColourShiftTime, true);
        yield return UIUtility.Instance.StartCoroutine(this.shiftColourListRoutine);

        // Invoke onClick event
        this.onClick.Invoke();
    }

    protected override IEnumerator Exit()
    {
        // Shift to idle colours
        this.shiftColourListRoutine = UIUtility.Instance.ShiftListColours(this.textList, this.idleColours, this.idleColourShiftTime, true);
        UIUtility.Instance.StartCoroutine(this.shiftColourListRoutine);

        // Yield scale to 1
        UIUtility.Instance.ScaleListOverTime(this.textTransformList, 1.0f, this.exitScaleTime, true, true);
        yield return UIUtility.Instance.StartCoroutine(this.scaleListRoutine);

        // Start Idle
        this.StartCoroutine(this.AttemptStateChange(ButtonState.Idle));
    }
}
