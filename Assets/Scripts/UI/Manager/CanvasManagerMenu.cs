using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CanvasManagerMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private MenuStateButtonListDictionary buttonDictionary;

    [Header("Debug")]
    [SerializeField]
    private MenuState currentState;



    public IEnumerator Start()
    {
        yield return this.StartCoroutine(this.TurnOnPanelButtons());
    }


    public IEnumerator TurnOnPanelButtons()
    {
        foreach (CustomTextButton button in this.buttonDictionary[this.currentState])
        {
            button.TurnOn();
        }

        yield return null;
    }



    public void ChangeMenuState(MenuState _state)
    {
        this.currentState = _state;
    }


}

public enum MenuState
{
    Start, MainMenu, GameSelect
}

[Serializable]
public class MenuStateButtonListStorage : SerializableDictionary.Storage<List<CustomTextButton>> { }

[Serializable]
public class MenuStateButtonListDictionary : SerializableDictionary<MenuState, List<CustomTextButton>, MenuStateButtonListStorage> { }