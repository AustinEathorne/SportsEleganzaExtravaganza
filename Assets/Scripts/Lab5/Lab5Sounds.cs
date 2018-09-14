using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab5Sounds : MonoBehaviour {

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> batHitClips;

	[SerializeField]
	private AudioClip cheerClip;

	[SerializeField]
	private AudioClip sadClip;

	[SerializeField]
	private AudioClip barkClip;

	[SerializeField]
	private AudioClip inputClip;

	public void PlayBatHit()
	{
		int rand  = Random.Range(0, this.batHitClips.Count);
		this.audioSource.clip = batHitClips[rand];
		this.audioSource.Play();
	}

	public IEnumerator PlayCheer()
	{
		this.audioSource.clip = this.cheerClip;
		this.audioSource.Play();

		while(this.audioSource.isPlaying)
		{
			yield return null;
		}

		yield return new WaitForSeconds(0.25f);
		this.audioSource.Play();

		while(this.audioSource.isPlaying)
		{
			yield return null;
		}

		this.audioSource.Play();

		while(this.audioSource.isPlaying)
		{
			yield return null;
		}

		yield return new WaitForSeconds(0.15f);
		this.audioSource.Play();
	}

	public void PlaySad()
	{
		this.audioSource.clip = this.sadClip;
		this.audioSource.Play();
	}

	public void PlayBark()
	{
		this.audioSource.clip = this.barkClip;
		this.audioSource.Play();
	}

	public void PlayInput()
	{
		this.audioSource.clip = this.inputClip;
		this.audioSource.Play();
	}
}
