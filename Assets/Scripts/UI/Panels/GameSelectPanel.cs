﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameSelectPanel : MenuPanel
{
    [Header("BG Groups")]
    [SerializeField]
    protected float bgFadeTime;
    [SerializeField]
    protected List<CanvasGroup> bgCanvasGroupList;

    [Header("Title Container")]
    [SerializeField]
    protected float titleGroupFadeTime;
    [SerializeField]
    protected CanvasGroup titleCanvasGroup;
    public CanvasEffect backButtonEffect;

    [Header("Play Button")]
    [SerializeField]
    private float playButtonFadeTime;
    [SerializeField]
    private CanvasGroup playButtonCanvasGroup;
    [SerializeField]
    private CanvasEffect playButtonEffect;


    [Header("Preview Swap Buttons")]
    [SerializeField]
    private float swapButtonFadeTime;
    [SerializeField]
    private CanvasGroup swapButtonCanvasGroup;
    [SerializeField]
    private List<CanvasEffect> previewSwapButtonEffects;

    [Header("Game Preview Panels")]
    [SerializeField]
    private float previewFadeTime;
    [SerializeField]
    private List<CanvasGroup> previewCanvasGroupList;
    [SerializeField]
    private List<VideoPlayer> videoPlayerList;

    [Header("Misc Values")]
    [SerializeField]
    protected float fadeDelay;

    [Header("Debug")]
    [SerializeField]
    private int currentPreviewIndex;

    private IEnumerator changePreviewRoutine;
    


    protected override IEnumerator OpenPanel()
    {
        this.currentPreviewIndex = 0;
        this.playButtonCanvasGroup.alpha = 0;
        this.swapButtonCanvasGroup.alpha = 0;

        // Fade in bg groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeListOverTime(this.bgCanvasGroupList, this.bgFadeTime, 1, true));

        yield return new WaitForSeconds(this.fadeDelay);

        // Fade in text groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleGroupFadeTime, 1));

        yield return new WaitForSeconds(this.fadeDelay);

        // Turn on back button effect
        this.backButtonEffect.TurnOn();

        // Start playing the first preview's video
        this.videoPlayerList[this.currentPreviewIndex].Play();

        // Turn on preview swap button effects
        foreach (CanvasEffect effect in this.previewSwapButtonEffects)
        {
            effect.TurnOn();
        }

        // Fade in preview swap canvas group
        UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.swapButtonCanvasGroup, this.swapButtonFadeTime, 1));

        // Yield preview fade in
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[this.currentPreviewIndex], this.previewFadeTime, 1));

        // Turn on play button effect and fade in
        this.playButtonEffect.TurnOn();
        yield return UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeOverTime(this.playButtonCanvasGroup, this.playButtonFadeTime, 1));

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

        this.changePreviewRoutine = null;

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        this.mainCanvasGroup.interactable = false;
        this.mainCanvasGroup.blocksRaycasts = false;

        // Turn off play button effect and fade out
        this.playButtonEffect.TurnOff();
        yield return UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeOverTime(this.playButtonCanvasGroup, this.playButtonFadeTime, 0));

        // Fade out preview
        UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[this.currentPreviewIndex], this.previewFadeTime, 0));

        // Turn off game selection button effects
        foreach (CanvasEffect effect in this.previewSwapButtonEffects)
        {
            effect.TurnOff();
        }

        // Fade out preview swap canvas group
        UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.swapButtonCanvasGroup, this.swapButtonFadeTime, 0));

        yield return new WaitForSeconds(this.fadeDelay);

        // Turn off back button effect
        this.backButtonEffect.TurnOff();

        // Fade out text groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeOverTime(this.titleCanvasGroup, this.titleGroupFadeTime, 0));

        yield return new WaitForSeconds(this.fadeDelay);

        // Fade out bgs
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeListOverTime(this.bgCanvasGroupList, this.bgFadeTime, 0, true));

        // Stop playing the current preview's video
        this.videoPlayerList[this.currentPreviewIndex].Stop();

        this.playButtonCanvasGroup.alpha = 0;

        yield return null;
    }


    public void OnChangePreviewClick(int _direction)
    {
        this.StartCoroutine(this.AttemptChangePreview(_direction));
    }

    private IEnumerator AttemptChangePreview(int _direction)
    {
        yield return new WaitUntil(() => this.changePreviewRoutine == null);

        this.changePreviewRoutine = this.ChangePreview(this.currentPreviewIndex + _direction);
        this.StartCoroutine(this.changePreviewRoutine);

        yield return null;
    }

    private IEnumerator ChangePreview(int _index)
    {
        // Wrap index
        if (_index < 0)
            _index = this.previewCanvasGroupList.Count - 1;
        else if (_index > this.previewCanvasGroupList.Count - 1)
            _index = 0;

        // Fade out old preview
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[this.currentPreviewIndex], this.previewFadeTime, 0));

        // Stop old preview video and begin the new one
        this.videoPlayerList[this.currentPreviewIndex].Stop();

        yield return new WaitForEndOfFrame();

        this.videoPlayerList[_index].Play();

        // Fade in new preview
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[_index], this.previewFadeTime, 1));

        this.currentPreviewIndex = _index;
        this.changePreviewRoutine = null;

        yield return null;
    }


    public void OnPlayGameClick()
    {
        CanvasManagerMenu.Instance.StartCoroutine(CanvasManagerMenu.Instance.OnGameSelect(this.currentPreviewIndex));
    }
}
