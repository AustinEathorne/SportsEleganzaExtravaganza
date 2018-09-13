using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class CustomTextButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // State
    public enum ButtonState
    {
        IDLE = 0, PRESSED, RELEASED, OFF, EXIT
    }

    public ButtonState buttonState = ButtonState.IDLE;
    protected bool routineLock = false;

    // Components
    public List<RectTransform> transformList;
    public List<Text> textList;

    // Idle
    public List<Color> idleColours;

    // Pressed

    // Released

    // Off

    // OnClick
    public UnityEvent onClick;




    public virtual void TurnOn()
    {
        this.StartCoroutine(this.TurnOnRoutine());
    }

    public virtual void TurnOff()
    {
        this.StartCoroutine(this.OffState());
    }

    protected virtual IEnumerator TurnOnRoutine()
    {
        this.textList[0].color = this.idleColours[0];
        this.textList[1].color = this.idleColours[1];

        this.transformList[0].localScale = Vector3.one;
        this.transformList[1].localScale = Vector3.one;

        this.buttonState = ButtonState.IDLE;
        this.StartCoroutine(this.Run());

        yield return null;
    }


    protected virtual IEnumerator Run()
    {
        switch (this.buttonState)
        {
            case ButtonState.IDLE:
                yield return this.StartCoroutine(this.IdleState());
                break;
            case ButtonState.PRESSED:
                yield return this.StartCoroutine(this.PressedState());
                break;
            case ButtonState.RELEASED:
                yield return this.StartCoroutine(this.ReleasedState());
                break;
            case ButtonState.EXIT:
                yield return this.StartCoroutine(this.ExitReleasedState());
                break;
            default:
                Debug.Log("[CustomButton] Something went very wrong.");
                break;
        }

        if (this.buttonState != ButtonState.OFF)
        {
            this.StartCoroutine(this.Run());
        }

        yield return null;
    }

    protected abstract IEnumerator IdleState();

    protected abstract IEnumerator PressedState();

    protected abstract IEnumerator ReleasedState();

    protected abstract IEnumerator OffState();

    protected abstract IEnumerator ExitReleasedState();


    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("[CustomButton] OnPointerDown");
        this.buttonState = ButtonState.PRESSED;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (this.buttonState == ButtonState.PRESSED)
        {
            //Debug.Log("[CustomButton] OnPointerUp");
            this.buttonState = ButtonState.RELEASED;
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (this.buttonState == ButtonState.PRESSED)
        {
            //Debug.Log("[CustomButton] OnPointerExit");
            this.buttonState = ButtonState.EXIT;
        }
    }
}
