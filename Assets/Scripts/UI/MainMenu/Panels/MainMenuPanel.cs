using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : MenuPanel
{
    [Header("Buttons")]
    [SerializeField]
    private List<CustomButton> buttonList;


    protected override IEnumerator OpenPanel()
    {
        // Turn on buttons
        for (int i = 0; i < this.buttonList.Count; i++)
        {
            this.buttonList[i].CallTurnOn();
        }

        this.mainCanvasGroup.interactable = true;
        this.mainCanvasGroup.blocksRaycasts = true;

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

        yield return null;
    }    
}
