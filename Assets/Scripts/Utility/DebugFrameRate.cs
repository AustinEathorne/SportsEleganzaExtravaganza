using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFrameRate : MonoBehaviour
{
    GUIStyle style;

    // Average frame rate
    [Range(0.1f, 5)]
    public float frameCountUpdateInterval = 1.0f;

    private int frameCount = 0;
    private float timeCount = 0.0f;
    private float averageFramesPerSecond = 0.0f;

    // Total runtime average frame rate
    private int totalFrameCount = 0;
    private float totalTimeCount = 0.0f;
    private float totalAverageFramesPerSecond = 0.0f;


    private void Awake()
    {
        this.style = new GUIStyle();
        style.fontSize = 42;
        style.normal.textColor = Color.green;
    }

    private void Update()
    {
        if (this.timeCount < this.frameCountUpdateInterval)
        {
            this.timeCount += Time.deltaTime;
            this.frameCount++;
        }
        else
        {
            this.totalTimeCount += this.timeCount;
            this.totalFrameCount += this.frameCount;
            this.totalAverageFramesPerSecond = (float)this.totalFrameCount / this.totalTimeCount;

            this.averageFramesPerSecond = (float)this.frameCount / this.timeCount;
            this.frameCount = 0;
            this.timeCount = 0.0f;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 50),
            " " + this.frameCountUpdateInterval.ToString() + " second average: " + this.averageFramesPerSecond.ToString() + " FPS",
            this.style);

        GUI.Label(new Rect(0, 50, 200, 50),
            " Runtime average: " + this.totalAverageFramesPerSecond.ToString() + " FPS",
            this.style);
    }
}
