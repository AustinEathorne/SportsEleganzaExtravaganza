using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class BreathingTextButton : CustomButton
{
    // text list
    public List<Text> textList;

    // On
    public float activationScaleTime;
    public float activationFadeTime;

    // Off
    public float fadeOutTime;
    public float pressedScaleAwayTime;

    // Idle
    public float scaleDownTime;
    public float scaleUpTime;

    public float minScale;
    public float maxScale;

    public float colorShiftTime;

    // Pressed/Released
    public float pressedScaleTime;
    public float pressedColorShiftTime;
    public List<Color> pressedColours;

    // Exit Released
    public float exitReleasedScaleTime;
    public float exitReleasedShiftTime;

    #region Base

    protected override IEnumerator Start()
    {
        yield return base.StartCoroutine(base.Start());
    }

    protected override IEnumerator Run()
    {
        yield return base.StartCoroutine(base.Run());
    }

    protected override IEnumerator TurnOn()
    {
        for (int i = 0; i < this.transformList.Count; i++)
        {
            this.transformList[i].transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < this.textList.Count; i++)
        {
            this.textList[i].color = new Color(this.idleColours[i].r, this.idleColours[i].g, this.idleColours[i].b, 0.0f);
        }

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeListOverTime(this.textList, this.activationFadeTime, 1.0f, true));
        yield return UIUtility.Instance.ScaleListOverTime(this.transformList, 1.0f, this.activationScaleTime, true, true);

        this.buttonState = ButtonState.IDLE;
        this.StartCoroutine(this.Run());

        yield return null;
    }

    #endregion

    #region States

    protected override IEnumerator IdleState()
    {
        yield return new WaitUntil(() => this.routineLock == false);
        this.routineLock = true;

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.ShiftListColours(this.textList, this.idleColours,
            this.colorShiftTime, true));

        float elapsedTime = 0.0f;

        Coroutine scale = UIUtility.Instance.StartCoroutine(UIUtility.Instance.ScaleListOverTime(this.transformList,
            this.minScale, this.scaleDownTime, true, true));

        while (elapsedTime < this.scaleDownTime)
        {
            if (this.buttonState != ButtonState.IDLE)
            {
                elapsedTime = this.scaleDownTime;
                UIUtility.Instance.StopCoroutine(scale);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (this.buttonState == ButtonState.IDLE)
        {
            elapsedTime = 0.0f;
            scale = UIUtility.Instance.StartCoroutine(UIUtility.Instance.ScaleListOverTime(this.transformList,
                this.maxScale, this.scaleUpTime, true, true));

            while (elapsedTime < this.scaleUpTime)
            {
                if (this.buttonState != ButtonState.IDLE)
                {
                    elapsedTime = this.scaleUpTime;
                    UIUtility.Instance.StopCoroutine(scale);
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        this.routineLock = false;
        yield return null;
    }

    protected override IEnumerator PressedState()
    {
        yield return new WaitUntil(() => this.routineLock == false);
        this.routineLock = true;

        Coroutine scale = UIUtility.Instance.StartCoroutine(UIUtility.Instance.ScaleListOverTime(this.transformList, this.minScale,
            this.pressedScaleTime, true, true));
        Coroutine shift = UIUtility.Instance.StartCoroutine(UIUtility.Instance.ShiftListColours(this.textList, this.pressedColours,
            this.pressedColorShiftTime, true));

        while (this.buttonState != ButtonState.RELEASED && this.buttonState != ButtonState.EXIT)
        {
            yield return null;
        }

        UIUtility.Instance.StopCoroutine(scale);
        UIUtility.Instance.StopCoroutine(shift);

        this.routineLock = false;
        yield return null;
    }

    protected override IEnumerator ReleasedState()
    {
        yield return new WaitUntil(() => this.routineLock == false);
        this.routineLock = true;

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.ShiftListColours(this.textList, this.pressedColours,
            this.pressedColorShiftTime, true));

        this.onClick.Invoke();

        this.buttonState = ButtonState.OFF;
        this.routineLock = false;

        yield return null;
    }

    protected override IEnumerator OffState()
    {
        UIUtility.Instance.StartCoroutine(UIUtility.Instance.ScaleListOverTime(this.transformList, 0.0f, 
            this.pressedScaleAwayTime, true, true));
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeListOverTime(this.textList, 
            this.fadeOutTime, 0.0f, true));
    }

    protected override IEnumerator ExitReleasedState()
    {
        yield return new WaitUntil(() => this.routineLock == false);
        this.routineLock = true;

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.ShiftListColours(this.textList, this.idleColours,
            this.exitReleasedShiftTime, true));
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.ScaleListOverTime(this.transformList, 1.0f, 
            this.exitReleasedScaleTime, true, true));

        this.buttonState = ButtonState.IDLE;

        this.routineLock = false;
    }

    #endregion
}
