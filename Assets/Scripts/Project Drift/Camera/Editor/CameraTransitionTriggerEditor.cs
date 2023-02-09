using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraTransitionTrigger))]
public class CameraTransitionTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        CameraTransitionTrigger trigger = (CameraTransitionTrigger)target;
        if (trigger.shouldTeleport)
        {
            Handles.color = Color.green;
            trigger.teleportPlayerTo = Handles.PositionHandle(trigger.teleportPlayerTo, Quaternion.identity);
        }
    }
}
