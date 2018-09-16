using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : MenuPanel
{
    [Header("Buttons")]
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private List<CanvasEffect> buttonEffects;
    


    protected override IEnumerator OpenPanel()
    {
        // Turn on button effects
        foreach (CanvasEffect effect in this.buttonEffects)
        {
            effect.TurnOn();
        }

        // Fade in canvas group
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.mainCanvasGroup, this.fadeTime, 1));

        // Set canvas group interactable
        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        // Set canvas group not interactable
        this.mainCanvasGroup.interactable = false;
        this.mainCanvasGroup.blocksRaycasts = false;

        // Turn off button effects
        foreach (CanvasEffect effect in this.buttonEffects)
        {
            effect.TurnOff();
        }

        // Fade out main group
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.mainCanvasGroup, this.fadeTime, 0.0f));

        // Set group alpha
        this.mainCanvasGroup.alpha = 0;

        yield return null;
    }
}
