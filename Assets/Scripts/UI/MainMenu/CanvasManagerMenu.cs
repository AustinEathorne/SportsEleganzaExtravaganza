using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CanvasManagerMenu : MonoBehaviour
{    
    [Header("Buttons")]
    [SerializeField]
    private MenuStatePanelListDictionary panelDictionary;

    [Header("Debug")]
    [SerializeField]
    private MenuState previousState;
    [SerializeField]
    private MenuState currentState;

    // Routines
    private IEnumerator stateChangeRoutine;



    public IEnumerator Start()
    {
        this.stateChangeRoutine = null;

        yield return this.panelDictionary[0].StartCoroutine(
            this.panelDictionary[0].TogglePanel(true));
    }


    public void OnStateChangeClick(int _state)
    {
        this.StartCoroutine(this.StartMenuStateChange((MenuState)_state));
    }

    public IEnumerator StartMenuStateChange(MenuState _state)
    {
        // Wait if we're curerntly changing states
        yield return new WaitUntil(() => this.stateChangeRoutine == null);

        this.stateChangeRoutine = this.ChangeMenuState(_state);
        this.StartCoroutine(this.stateChangeRoutine);
    }

    public IEnumerator ChangeMenuState(MenuState _state)
    {
        // Set our current state
        this.currentState = _state;

        // Turn off previous
        yield return this.panelDictionary[this.previousState].StartCoroutine(
            this.panelDictionary[this.previousState].TogglePanel(false));
        // Turn on current
        yield return this.panelDictionary[this.currentState].StartCoroutine(
            this.panelDictionary[this.currentState].TogglePanel(true));

        // Set our previous state for the next state change
        this.previousState = this.currentState;

        // Set routine to null
        this.stateChangeRoutine = null;

        yield return null;
    }
}

public enum MenuState
{
    Start = 0, MainMenu, GameSelect
}


[Serializable]
public class MenuStatePanelListDictionary : SerializableDictionary<MenuState, MenuPanel> { }
