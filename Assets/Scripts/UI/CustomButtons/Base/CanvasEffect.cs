using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CanvasEffect : MonoBehaviour
{
    [Header("Bools")]
    public bool playOnEnable;

    protected IEnumerator runRoutine;
    protected IEnumerator stopRoutine;
    


    public virtual void OnEnable()
    {
        if (this.playOnEnable)
        {
            this.runRoutine = this.Run();
            this.StartCoroutine(this.runRoutine);
        }
    }


    public virtual void TurnOn()
    {
        this.runRoutine = this.Run();
        this.StartCoroutine(this.runRoutine);
    }

    public virtual void TurnOff()
    {
        this.stopRoutine = this.Stop();
        this.StartCoroutine(this.stopRoutine);
    }


    public abstract IEnumerator Run();
    public abstract IEnumerator Stop();
}
