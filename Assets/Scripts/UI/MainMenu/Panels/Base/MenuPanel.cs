using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuPanel : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField]
    protected CanvasGroup mainCanvasGroup;



    public virtual IEnumerator TogglePanel(bool _isActive)
    {
        if (_isActive)
            yield return this.StartCoroutine(this.OpenPanel());
        else
            yield return this.StartCoroutine(this.ClosePanel());
    }


    protected abstract IEnumerator OpenPanel();

    protected abstract IEnumerator ClosePanel();
}
