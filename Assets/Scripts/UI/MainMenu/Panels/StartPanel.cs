using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartPanel : MenuPanel
{
    [Header("Start Button")]
    [SerializeField]
    private CanvasEffect startTextEffect;

    [Header("Title Text")]
    [SerializeField]
    private CanvasGroup titleCanvasGroup;
    [SerializeField]
    private float titleFadeTime;
    [SerializeField]
    private CanvasGroup buttonCanvasGroup;
    [SerializeField]
    private float buttonFadeTime;



    protected override IEnumerator OpenPanel()
    {
        this.buttonCanvasGroup.alpha = 0;
        this.mainCanvasGroup.alpha = 1;

        // Fade in title
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleFadeTime, 1));

        // Turn on start button effect
        this.startTextEffect.TurnOn();

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

        // Fade in start button
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.buttonCanvasGroup, this.buttonFadeTime, 1));

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        this.mainCanvasGroup.interactable = false;
        this.mainCanvasGroup.blocksRaycasts = false;

        // Turn off start button effect
        this.startTextEffect.TurnOff();

        // Fade out title
        UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleFadeTime, 0));

        // Fade out group
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.mainCanvasGroup, this.titleFadeTime, 0.0f));

        this.buttonCanvasGroup.alpha = 0;
        this.mainCanvasGroup.alpha = 0;

        yield return null;
    }
}
