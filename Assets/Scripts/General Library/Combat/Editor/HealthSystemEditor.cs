using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WSoft.Combat;

[CustomEditor(typeof(HealthSystem), true)]
public class HealthSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDefaultInspector();

        HealthSystem health = (HealthSystem)target;
        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;

        EditorGUILayout.LabelField("Debug Info:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(health.GetDebugData(), textStyle);
    }
}
