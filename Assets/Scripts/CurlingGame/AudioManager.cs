using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	[SerializeField]
	private AudioClip glideSfx;

	[SerializeField]
	private List<AudioClip> hardSfxList;

	[SerializeField]
	private AudioClip winnerGagnantSfx;

	[SerializeField]
	private AudioSource hardSfxSource;

	[SerializeField]
	private AudioSource glideSfxSource;


	private bool isPlayingClip = false;

	public IEnumerator PlayRandomHardClip(int rand)
	{
		if(isPlayingClip)
		{
			yield break;
		}

		this.isPlayingClip = true;

		switch(rand)
		{
		case 0: // Hard
			this.hardSfxSource.volume = 0.75f;
			this.hardSfxSource.clip = this.hardSfxList[0];
			this.hardSfxSource.Play();
			yield return new WaitForSeconds(this.hardSfxList[0].length + 0.25f);
			this.isPlayingClip = false;

			yield break;

		case 1: // HardHardHard
			this.hardSfxSource.volume = 0.6f;
			this.hardSfxSource.clip = this.hardSfxList[1];
			this.hardSfxSource.Play();
			yield return new WaitForSeconds(this.hardSfxList[1].length + 0.25f);
			this.isPlayingClip = false;

			yield break;

		case 2: // YeahYeahYeah
			this.hardSfxSource.volume = 0.5f;
			this.hardSfxSource.clip = this.hardSfxList[2];
			this.hardSfxSource.Play();
			yield return new WaitForSeconds(this.hardSfxList[2].length + 0.25f);
			this.isPlayingClip = false;

			yield break;

		default:
			this.isPlayingClip = false;
			yield break;
		}
	}


	public IEnumerator PlayGlideSfx()
	{
		// Debug.Log("Play Glide");

		this.glideSfxSource.clip = this.glideSfx;
		this.glideSfxSource.Play();

		yield return new WaitForSeconds(this.glideSfx.length);
	}


	public IEnumerator PlayWinnerGagnant()
	{
		this.hardSfxSource.volume = 0.1f;
		this.hardSfxSource.clip = this.winnerGagnantSfx;
		this.hardSfxSource.Play();

		yield return null;
	}
}
