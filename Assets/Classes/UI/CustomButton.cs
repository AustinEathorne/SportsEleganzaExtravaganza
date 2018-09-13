using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public abstract class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("OnClick")]
    public UnityEvent onClick;

    [Header("Debug")]
    [SerializeField]
    private ButtonState previousState;
    [SerializeField]
    private ButtonState currentState;

    // Routines
    protected IEnumerator stateChangeRoutine;
    protected Dictionary<ButtonState, IEnumerator> stateRoutineDictionary;



    protected abstract IEnumerator TurnOn();
    protected abstract IEnumerator TurnOff();
    protected abstract IEnumerator Idle();
    protected abstract IEnumerator Pressed();
    protected abstract IEnumerator Released();
    protected abstract IEnumerator Exit();


    public virtual IEnumerator Enable(bool _isEnabled)
    {
        // Wait for attempt to finish
        if (_isEnabled)
        {
            // Start state change
            yield return this.StartCoroutine(this.AttemptStateChange(ButtonState.TurningOn));

            // Wait for state change to be finished
            yield return new WaitUntil(() => stateRoutineDictionary[ButtonState.TurningOn] ==  null);
            Debug.Log("[CustomButton] Finished turning on");
        }
        else
        {
            yield return this.StartCoroutine(this.AttemptStateChange(ButtonState.TurningOff));

            // Wait for state change to be finished
            yield return new WaitUntil(() => stateRoutineDictionary[ButtonState.TurningOff] == null);
            Debug.Log("[CustomButton] Finished turning off");
        }

        yield return null;
    }


    protected virtual IEnumerator Start()
    {
        this.stateRoutineDictionary = new Dictionary<ButtonState, IEnumerator>();

        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            this.stateRoutineDictionary.Add(state, null);
        }

        this.currentState = ButtonState.Off;
        this.previousState = ButtonState.Off;

        yield return null;
    }

    protected virtual IEnumerator AttemptStateChange(ButtonState _state)
    {
        yield return new WaitUntil(() => this.stateChangeRoutine == null);

        Debug.Log("[CustomButton] Changing to " + _state.ToString());

        this.stateChangeRoutine = this.ChangeStates(_state);
        yield return this.StartCoroutine(this.stateChangeRoutine);

        this.stateChangeRoutine = null;
    }

    protected virtual IEnumerator ChangeStates(ButtonState _state)
    {
        // Stop previous state's routine
        if (this.stateRoutineDictionary[this.previousState] != null)
        {
            this.StopCoroutine(this.stateRoutineDictionary[this.previousState]);
            this.stateRoutineDictionary[this.previousState] = null;
        }

        // Update state
        this.currentState = _state;

        // Start corresponding routine
        switch (_state)
        {
            case ButtonState.TurningOn:
                this.stateRoutineDictionary[_state] = this.TurnOn();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.TurningOff:
                this.stateRoutineDictionary[_state] = this.TurnOff();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.Idle:
                this.stateRoutineDictionary[_state] = this.Idle();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.Pressed:
                this.stateRoutineDictionary[_state] = this.Pressed();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.Released:
                this.stateRoutineDictionary[_state] = this.Released();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.Exit:
                this.stateRoutineDictionary[_state] = this.Exit();
                this.StartCoroutine(this.stateRoutineDictionary[_state]);
                break;

            case ButtonState.Off:
                Debug.Log("This shouldn't ever be getting run");
                break;

            default:
                break;
        }

        // Update previous state
        this.previousState = _state;

        Debug.Log("[CustomButton] Finished changing to " + _state.ToString());

        yield return null;
    }


    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("[CustomButton] OnPointerDown");
        this.StartCoroutine(this.AttemptStateChange(ButtonState.Pressed));
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (this.currentState == ButtonState.Pressed)
        {
            Debug.Log("[CustomButton] OnPointerUp");
            this.StartCoroutine(this.AttemptStateChange(ButtonState.Released));
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (this.currentState == ButtonState.Pressed)
        {
            Debug.Log("[CustomButton] OnPointerExit");
            this.StartCoroutine(this.AttemptStateChange(ButtonState.Exit));
        }
    }
}

public enum ButtonState
{
    TurningOn = 0, TurningOff, Idle, Pressed, Released, Exit, Off
}