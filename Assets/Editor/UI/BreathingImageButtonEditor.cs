using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(BreathingImageButton))]
public class BreathingImageButtonEditor : Editor
{
    // Foldout bools
    private bool showState = true;
    private bool showComponents = true;   
    private bool showOn = true;
    private bool showOff = true;
    private bool showIdle = true;
    private bool showPressed = true;
    private bool showExit = true;
    private bool showOnClick = true;

    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 1;

        #region Setup

        BreathingImageButton myTarget = (BreathingImageButton)target;
        GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
        serializedObject.Update();
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;

        #endregion

        #region State - Debug

        Rect rState = EditorGUILayout.BeginVertical(boxStyle);
        this.showState = EditorGUILayout.Foldout(this.showState, "Debug", foldoutStyle);

        if (this.showState)
        {
            Rect rStateA = EditorGUILayout.BeginVertical(boxStyle);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonState"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Components

        Rect rComponent = EditorGUILayout.BeginVertical(boxStyle);
        this.showComponents = EditorGUILayout.Foldout(this.showComponents, "Components", foldoutStyle);

        if (this.showComponents)
        {
            Rect rComponentA = EditorGUILayout.BeginVertical(boxStyle);

            EditorList.Show(serializedObject.FindProperty("transformList"), EditorListOptions.ListLabel | EditorListOptions.Buttons);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            Rect rComponentB = EditorGUILayout.BeginVertical(boxStyle);

            EditorList.Show(serializedObject.FindProperty("imageList"), EditorListOptions.ListLabel | EditorListOptions.Buttons);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region On

        Rect rOn = EditorGUILayout.BeginVertical(boxStyle);
        this.showOn = EditorGUILayout.Foldout(this.showOn, "On", foldoutStyle);

        if (this.showOn)
        {
            Rect rOnA = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.activationScaleTime = EditorGUILayout.FloatField("Scale Up Time", myTarget.activationScaleTime);
            myTarget.activationFadeTime = EditorGUILayout.FloatField("Fade In Time", myTarget.activationFadeTime);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Off

        Rect rOff = EditorGUILayout.BeginVertical(boxStyle);
        this.showOff = EditorGUILayout.Foldout(this.showOff, "Off", foldoutStyle);

        if (this.showOff)
        {
            Rect rOffA = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.pressedScaleAwayTime = EditorGUILayout.FloatField("Scale Down Time", myTarget.pressedScaleAwayTime);
            myTarget.fadeOutTime = EditorGUILayout.FloatField("Fade Out Time", myTarget.fadeOutTime);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Idle

        Rect rIdle = EditorGUILayout.BeginVertical(boxStyle);
        this.showIdle = EditorGUILayout.Foldout(this.showIdle, "Idle", foldoutStyle);

        if (this.showIdle)
        {
            Rect rIdleA = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.scaleDownTime = EditorGUILayout.FloatField("Scale Down Time", myTarget.scaleDownTime);
            myTarget.scaleUpTime = EditorGUILayout.FloatField("Scale Up Time", myTarget.scaleUpTime);

            myTarget.minScale = EditorGUILayout.FloatField("Min Scale", myTarget.minScale);
            myTarget.maxScale = EditorGUILayout.FloatField("Max Scale", myTarget.maxScale);

            EditorGUILayout.EndVertical();

            Rect rIdleB = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.colorShiftTime = EditorGUILayout.FloatField("Shift Time", myTarget.colorShiftTime);

            Rect rIdleB2 = EditorGUILayout.BeginVertical(boxStyle);

            EditorList.Show(serializedObject.FindProperty("idleColours"), EditorListOptions.ListLabel | EditorListOptions.Buttons);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Pressed/Released

        Rect rPressed = EditorGUILayout.BeginVertical(boxStyle);
        this.showPressed = EditorGUILayout.Foldout(this.showPressed, "Pressed/Released", foldoutStyle);

        if (this.showPressed)
        {
            Rect rPressedA = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.pressedScaleTime = EditorGUILayout.FloatField("Scale Down Time", myTarget.pressedScaleTime);
            myTarget.pressedColorShiftTime = EditorGUILayout.FloatField("Shift Time", myTarget.pressedColorShiftTime);

            Rect rPressedB = EditorGUILayout.BeginVertical(boxStyle);

            EditorList.Show(serializedObject.FindProperty("pressedColours"), EditorListOptions.ListLabel | EditorListOptions.Buttons);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Exit

        Rect rExit = EditorGUILayout.BeginVertical(boxStyle);
        this.showExit = EditorGUILayout.Foldout(this.showExit, "Exit", foldoutStyle);

        if (this.showExit)
        {
            Rect rExitA = EditorGUILayout.BeginVertical(boxStyle);

            myTarget.exitReleasedScaleTime = EditorGUILayout.FloatField("Scale Up Time", myTarget.exitReleasedScaleTime);
            myTarget.exitReleasedShiftTime = EditorGUILayout.FloatField("Shift Time", myTarget.exitReleasedShiftTime);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region OnClick

        Rect rOnClick = EditorGUILayout.BeginVertical(boxStyle);
        this.showOnClick = EditorGUILayout.Foldout(this.showOnClick, "Events", foldoutStyle);

        if (this.showOnClick)
        {
            Rect rOnClickA = EditorGUILayout.BeginVertical(boxStyle);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onClick"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion
    }

}
