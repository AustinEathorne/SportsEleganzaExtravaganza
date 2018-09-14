using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartPanel : MenuPanel
{
    [Header("Start Button")]
    [SerializeField]
    private CustomButton startButton;

    [Header("Title Text")]
    [SerializeField]
    private CanvasGroup titleCanvasGroup;
    [SerializeField]
    private float titleFadeTime;



    protected override IEnumerator OpenPanel()
    {
        // Fade in title
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleFadeTime, 1));

        // Turn on start button
        this.startButton.CallTurnOn();

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        this.mainCanvasGroup.interactable = false;
        this.mainCanvasGroup.blocksRaycasts = false;

        // Turn on start button
        this.startButton.CallTurnOff();

        // Fade out title
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleFadeTime, 0));

        yield return null;
    }
}
