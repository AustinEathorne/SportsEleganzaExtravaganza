using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashPanel : MenuPanel
{
    [Header("Canvas Manager")]
    [SerializeField]
    private CanvasManagerMenu canvasManager;

    [Header("Canvas Groups")]
    [SerializeField]
    private float groupIntroDelayTime;
    [SerializeField]
    private float groupFadeTime;
    [SerializeField]
    private float groupStayTime;
    [SerializeField]
    private List<CanvasGroup> canvasGroups;

    [Header("Fade Image")]
    [SerializeField]
    private float fadeImageFadeTime;
    [SerializeField]
    private Image fadeImage;



    protected override IEnumerator OpenPanel()
    {
        // Set group alphas
        this.mainCanvasGroup.alpha = 1;

        // Delay start
        yield return new WaitForSeconds(this.groupIntroDelayTime);

        // Fade in canvas groups
        foreach (CanvasGroup group in this.canvasGroups)
        {
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(group, this.groupFadeTime, 1));
            yield return new WaitForSeconds(this.groupStayTime);
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(group, this.groupFadeTime, 0));
        }

        // Fade out fade image
        yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.fadeImage, this.fadeImageFadeTime, 0));

        // Move to start panel
        this.canvasManager.OnStateChangeClick(1);

        this.mainCanvasGroup.alpha = 0;

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        yield return null;
    }
}
