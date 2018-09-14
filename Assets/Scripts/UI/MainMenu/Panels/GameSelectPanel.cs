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

    [Header("Text Groups")]
    [SerializeField]
    protected float textFadeTime;
    [SerializeField]
    protected List<CanvasGroup> textCanvasGroupList;

    [Header("Buttons")]
    [SerializeField]
    private List<CustomButton> buttonList;

    [Header("Values")]
    [SerializeField]
    protected float fadeDelay;

    [Header("GamePreviewPanels")]
    [SerializeField]
    private float previewFadeTime;
    [SerializeField]
    private List<CanvasGroup> previewCanvasGroupList;
    [SerializeField]
    private List<VideoPlayer> videoPlayerList;
    [SerializeField]
    private int currentPreviewIndex;

    private IEnumerator changePreviewRoutine;
    


    protected override IEnumerator OpenPanel()
    {
        this.currentPreviewIndex = 0;

        // Fade in bg groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeListOverTime(this.bgCanvasGroupList, this.bgFadeTime, 1, true));

        yield return new WaitForSeconds(this.fadeDelay);

        // Fade in text groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeListOverTime(this.textCanvasGroupList, this.textFadeTime, 1, true));

        yield return new WaitForSeconds(this.fadeDelay);

        // Turn on buttons
        for (int i = 0; i < this.buttonList.Count; i++)
        {
            this.buttonList[i].CallTurnOn();
        }

        // Start playing the first preview's video
        this.videoPlayerList[this.currentPreviewIndex].Play();

        // Yield preview fade in
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[this.currentPreviewIndex], this.previewFadeTime, 1));

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

        this.changePreviewRoutine = null;

        yield return null;
    }

    protected override IEnumerator ClosePanel()
    {
        this.mainCanvasGroup.interactable = false;
        this.mainCanvasGroup.blocksRaycasts = false;

        // Turn off buttons
        for (int i = 0; i < this.buttonList.Count; i++)
        {
            this.buttonList[i].CallTurnOff();
        }

        // Fade out preview
        UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeOverTime(this.previewCanvasGroupList[this.currentPreviewIndex], this.previewFadeTime, 0));

        yield return new WaitForSeconds(this.fadeDelay);

        // Fade out text groups
        UIUtility.Instance.StartCoroutine(
            UIUtility.Instance.FadeListOverTime(this.textCanvasGroupList, this.textFadeTime, 0, true));

        yield return new WaitForSeconds(this.fadeDelay);

        // Fade out bgs
        yield return UIUtility.Instance.StartCoroutine(
           UIUtility.Instance.FadeListOverTime(this.bgCanvasGroupList, this.bgFadeTime, 0, true));

        // Stop playing the current preview's video
        this.videoPlayerList[this.currentPreviewIndex].Stop();

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
}