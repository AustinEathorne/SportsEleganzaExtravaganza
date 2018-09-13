using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonBase : MonoBehaviour
{
    [Header("Image")]
    public Image image;

    [Header("Colour")]
    public Color baseColor;
    public Color regularColor;
    public Color selectedColor;

    public float colorShiftTime;

    public IEnumerator shiftInCoroutine = null;
    public IEnumerator shiftOutCoroutine = null;

    [Header("Info")]
    public bool isSelected;


    public virtual void OnClick()
    {
        
    }

    public virtual void Select(bool _isSelected)
    {
        this.isSelected = _isSelected;

        if (this.isSelected) // select
        {
            if (this.shiftOutCoroutine != null)
            {
                UIUtility.Instance.StopCoroutine(this.shiftOutCoroutine);
                this.shiftOutCoroutine = null;
            }

            this.shiftInCoroutine = UIUtility.Instance.ShiftColour(this.image, this.colorShiftTime,
                this.baseColor * this.selectedColor);
            UIUtility.Instance.StartCoroutine(this.shiftInCoroutine);
        }
        else // deselect
        {
            if (this.shiftInCoroutine != null)
            {
                UIUtility.Instance.StopCoroutine(this.shiftInCoroutine);
                this.shiftInCoroutine = null;
            }

            this.shiftOutCoroutine = UIUtility.Instance.ShiftColour(this.image, this.colorShiftTime,
                this.baseColor * this.regularColor);
            UIUtility.Instance.StartCoroutine(this.shiftOutCoroutine);
        }
    }


    public virtual void SetButtonColour(Color _imageColour)
    {
        this.image.color = _imageColour;
        this.baseColor = this.image.color;
    }
}
