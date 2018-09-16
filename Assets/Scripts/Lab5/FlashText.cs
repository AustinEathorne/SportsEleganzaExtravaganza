using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashText : MonoBehaviour {

	[SerializeField]
	private float flashSpeed = 5.0f;

	[SerializeField]
	private List<Text> textList = new List<Text>();

	public IEnumerator Flash()
	{
		float alpha = textList[0].color.a;

		while(alpha <= 0.99f)
		{
			for(int i = 0; i < textList.Count; i++)
			{
				alpha = Mathf.MoveTowards(alpha, 1.0f, this.flashSpeed * Time.deltaTime);
				this.textList[i].color = new Color(textList[i].color.r, textList[i].color.g, textList[i].color.b, alpha);
			}
			yield return null;
		}

		while(alpha >= 0.01f)
		{
			for(int i = 0; i < textList.Count; i++)
			{
				alpha = Mathf.MoveTowards(alpha, 0.0f, this.flashSpeed * Time.deltaTime);
				this.textList[i].color = new Color(textList[i].color.r, textList[i].color.g, textList[i].color.b, alpha);
			}
			yield return null;
		}

		this.StartCoroutine(this.Flash());
	}
}
