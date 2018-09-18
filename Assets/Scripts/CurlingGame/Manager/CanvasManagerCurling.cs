using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManagerCurling : MonoBehaviour {

	[Header("Sliders")]
	[SerializeField]
	private GameObject massSlider;
	[SerializeField]
	private GameObject accelerationSlider;

	[Header("Buttons")]
	[SerializeField]
	private GameObject statSubmitButton;

	[Header("Text")]
	[SerializeField]
	private Text p1_scoreText;
	[SerializeField]
	private Text p1_scoreTextShadow;
	[SerializeField]
	private Text p2_scoreText;
	[SerializeField]
	private Text p2_scoreTextShadow;
	[SerializeField]
	private Text massValueText;
	[SerializeField]
	private Text massValueTextShadow;
	[SerializeField]
	private Text accelerationValueText;
	[SerializeField]
	private Text accelerationValueTextShadow;
	[SerializeField]
	private Text playerTurnText;
	[SerializeField]
	private Text playerTurnTextShadow;
	[SerializeField]
	private Text winnerText;
	[SerializeField]
	private Text winnerTextShadow;

	private float massValue = 0.0f;
	private float accelerationValue = 0.0f;

	// Called from GameManager
	public void UpdatePointText(float p1Score, float p2Score)
	{
		this.p1_scoreText.text = "Player 1: " + p1Score.ToString();
		this.p1_scoreTextShadow.text = "Player 1: " + p1Score.ToString();
		this.p2_scoreText.text = "Player 2: " + p2Score.ToString();
		this.p2_scoreTextShadow.text = "Player 2: " + p2Score.ToString();

	}

	public void UpdateSliderTextValues()
	{
		this.massValueText.text = System.Math.Round(this.GetMassValue(), 2).ToString();
		this.massValueTextShadow.text = System.Math.Round(this.GetMassValue(), 2).ToString();
		this.accelerationValueText.text = System.Math.Round(this.GetAccelerationValue(), 2).ToString();
		this.accelerationValueTextShadow.text = System.Math.Round(this.GetAccelerationValue(), 2).ToString();
	}

	// Set UI element active/inactive
	public void EnableStatSubmitElements(bool value)
	{
		this.massSlider.SetActive(value);
		this.accelerationSlider.SetActive(value);
		this.statSubmitButton.SetActive(value);
	}

	public void EnableWinnerText(bool value)
	{
		this.winnerText.gameObject.SetActive(value);
	}

	// Set Slider Values
	public void SetMassSliderValue(float value)
	{
		this.massSlider.GetComponent<Slider>().value = value;
	}

	public void SetMassSliderMinValue(float value)
	{
		this.massSlider.GetComponent<Slider>().minValue = value;
	}

	public void SetMassSliderMaxValue(float value)
	{
		this.massSlider.GetComponent<Slider>().maxValue = value;
	}

	public void SetAccelerationSliderValue(float value)
	{
		this.accelerationSlider.GetComponent<Slider>().value = value;
	}

	public void SetAccelerationSliderMinValue(float value)
	{
		this.accelerationSlider.GetComponent<Slider>().minValue = value;
	}

	public void SetAccelerationSliderMaxValue(float value)
	{
		this.accelerationSlider.GetComponent<Slider>().maxValue = value;
	}

	public void SetPlayerTurnText(string value)
	{
		this.playerTurnText.text = value;
		this.playerTurnTextShadow.text = value;
	}

	// Get/Set Current Values
	public void SetMassValue() // Set on slider value changed
	{
		this.massValue = this.massSlider.GetComponent<Slider>().value;
	}

	public float GetMassValue() // Called from Game Manager
	{
		return this.massValue;
	}

	public void SetAccelerationValue() // Set on slider value changed
	{
		this.accelerationValue = this.accelerationSlider.GetComponent<Slider>().value;
	}

	public float GetAccelerationValue() // Called from Game Manager
	{
		return this.accelerationValue;
	}

	public void SetWinnertext(string str)
	{
		this.winnerText.text = str + "\n" + "WINNER/GAGNON";
		this.winnerTextShadow.text = str + "\n" + "WINNER/GAGNON";
	}
}
