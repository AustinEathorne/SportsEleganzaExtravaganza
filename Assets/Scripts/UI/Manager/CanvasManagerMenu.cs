﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CanvasManagerMenu : MonoBehaviour
{
    
    [Header("Containers")]
    [SerializeField]
    private MenuStateGameObjectDictionary containerDictionary;
    
    [Header("Buttons")]
    [SerializeField]
    private MenuStateButtonListDictionary buttonDictionary;

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
        yield return this.StartCoroutine(this.ToggleContainer(MenuState.Start, true));
        yield return this.StartCoroutine(this.TogglePanelButtons(MenuState.Start, true));
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

        UIUtility.Instance.StopAllCoroutines();

        // Turn off previous
        yield return this.StartCoroutine(this.TogglePanelButtons(this.previousState, false));
        yield return this.StartCoroutine(this.ToggleContainer(this.previousState, false));

        // Turn on current
        yield return this.StartCoroutine(this.ToggleContainer(this.currentState, true));
        yield return this.StartCoroutine(this.TogglePanelButtons(this.currentState, true));

        // Set our previous state for the next state change
        this.previousState = this.currentState;

        // Set routine to null
        this.stateChangeRoutine = null;

        yield return null;
    }


    public IEnumerator TogglePanelButtons(MenuState _state, bool _isActive)
    {
        for (int i = 0; i < this.buttonDictionary[_state].Count; i++)
        {
            // Yield last element
            if (i != (this.buttonDictionary[_state].Count - 1))
            {
                this.buttonDictionary[_state][i].StartCoroutine(this.buttonDictionary[_state][i].Enable(_isActive));
            }
            else
            {
                yield return this.buttonDictionary[_state][i].StartCoroutine(this.buttonDictionary[_state][i].Enable(_isActive));
            }
        }

        yield return null;
    }

    public IEnumerator ToggleContainer(MenuState _state, bool _isActive)
    {
        this.containerDictionary[_state].SetActive(_isActive);
        yield return null;
    }
}

public enum MenuState
{
    Start = 0, MainMenu, GameSelect
}


[Serializable]
public class MenuStateButtonListStorage : SerializableDictionary.Storage<List<CustomButton>> { }

[Serializable]
public class MenuStateButtonListDictionary : SerializableDictionary<MenuState, List<CustomButton>, MenuStateButtonListStorage> { }

[Serializable]
public class MenuStateGameObjectDictionary : SerializableDictionary<MenuState, GameObject> { }
