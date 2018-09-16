using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartPanel : MenuPanel
{
    [Header("Start Button")]
    [SerializeField]
    private CanvasEffect startTextEffect;
    [SerializeField]
    private CanvasGroup buttonCanvasGroup;
    [SerializeField]
    private float buttonFadeTime;

    [Header("Title")]
    [SerializeField]
    private CanvasGroup titleCanvasGroup;
    [SerializeField]
    private float titleFadeTime;
    


    protected override IEnumerator OpenPanel()
    {
        // Set group alphas
        this.mainCanvasGroup.alpha = 1;
        this.buttonCanvasGroup.alpha = 0;

        // Fade in title
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleFadeTime, 1));

        // Turn on start button effect
        this.startTextEffect.TurnOn();

        // Fade in start button group
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.buttonCanvasGroup, this.buttonFadeTime, 1));

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

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

        // Fade out main group
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.mainCanvasGroup, this.titleFadeTime, 0.0f));

        this.buttonCanvasGroup.alpha = 0;
        this.mainCanvasGroup.alpha = 0;

        yield return null;
    }
}
