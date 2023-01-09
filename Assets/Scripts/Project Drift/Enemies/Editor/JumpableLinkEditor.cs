using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JumpableLink))]
public class JumpableLinkEditor : Editor
{
    static int selectedID = 0;
    static int selectedPoint = -1;

    static readonly Color HANDLE_COLOR = new Color(255f, 167f, 39f, 210f) / 255;

    public override void OnInspectorGUI()
    {
        JumpableLink link = target as JumpableLink;

        link.startPoint = EditorGUILayout.Vector3Field("Start Point", link.startPoint);
        link.endPoint = EditorGUILayout.Vector3Field("End Point", link.endPoint);

        GUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUIUtility.labelWidth);
        if (GUILayout.Button("Swap"))
        {
            Vector3 temp = link.startPoint;
            link.startPoint = link.endPoint;
            link.endPoint = temp;
            SceneView.RepaintAll();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        link.width = EditorGUILayout.FloatField("Width", link.width);
    }

    static Vector3 CalculateLinkRight(JumpableLink link)
    {
        Vector3 dir = link.endPoint - link.startPoint;
        return (new Vector3(-dir.z, 0.0f, dir.x)).normalized;
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
    static void RenderLinkGizmo(JumpableLink link, GizmoType gizmoType)
    {
        Vector3 right = CalculateLinkRight(link);
        float radius = link.width / 2f;

        Gizmos.DrawLine(link.startPoint - right * radius, link.startPoint + right * radius);
        Gizmos.DrawLine(link.endPoint - right * radius, link.endPoint + right * radius);
        Gizmos.DrawLine(link.startPoint - right * radius, link.endPoint - right * radius);
        Gizmos.DrawLine(link.startPoint + right * radius, link.endPoint + right * radius);
    }

    public void OnSceneGUI()
    {
        JumpableLink link = target as JumpableLink;
  
        if (!link.enabled)
            return;

        Matrix4x4 matrix = Matrix4x4.TRS(link.transform.position, link.transform.rotation, Vector3.one);

        Vector3 startPoint = matrix.MultiplyPoint(link.startPoint);
        Vector3 endPoint = matrix.MultiplyPoint(link.endPoint);
        Vector3 midPoint = Vector3.Lerp(startPoint, endPoint, 0.35f);
        float startSize = HandleUtility.GetHandleSize(startPoint);
        float endSize = HandleUtility.GetHandleSize(endPoint);
        float midSize = HandleUtility.GetHandleSize(midPoint);

        Quaternion zup = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
        Vector3 right = matrix.MultiplyVector(CalculateLinkRight(link));
        float radius = link.width / 2f;

        Color oldColor = Handles.color;
        Handles.color = HANDLE_COLOR;

        Handles.DrawLine(startPoint - right * radius, startPoint + right * radius);
        Handles.DrawLine(endPoint - right * radius, endPoint + right * radius);
        Handles.DrawLine(startPoint - right * radius, endPoint - right * radius);
        Handles.DrawLine(startPoint + right * radius, endPoint + right * radius);

        Vector3 pos;

        if (link.GetInstanceID() == selectedID && selectedPoint == 0)
        {
            EditorGUI.BeginChangeCheck();
            Handles.CubeHandleCap(0, startPoint, zup, 0.1f * startSize, Event.current.type);
            pos = Handles.PositionHandle(startPoint, link.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(link, "Move link point");
                link.startPoint = matrix.inverse.MultiplyPoint(pos);
            }
        }
        else
        {
            if (Handles.Button(startPoint, zup, 0.1f * startSize, 0.1f * startSize, Handles.CubeHandleCap))
            {
                selectedPoint = 0;
                selectedID = link.GetInstanceID();
            }
        }

        if (link.GetInstanceID() == selectedID && selectedPoint == 1)
        {
            EditorGUI.BeginChangeCheck();
            Handles.CubeHandleCap(0, endPoint, zup, 0.1f * startSize, Event.current.type);
            pos = Handles.PositionHandle(endPoint, link.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(link, "Move link point");
                link.endPoint = matrix.inverse.MultiplyPoint(pos);
            }
        }
        else
        {
            if (Handles.Button(endPoint, zup, 0.1f * endSize, 0.1f * endSize, Handles.CubeHandleCap))
            {
                selectedPoint = 1;
                selectedID = link.GetInstanceID();
            }
        }

        EditorGUI.BeginChangeCheck();
        pos = Handles.Slider(midPoint + right * link.width * 0.5f, right, midSize * 0.03f, Handles.DotHandleCap, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(link, "Adjust link width");
            link.width = Mathf.Max(0.0f, 2.0f * Vector3.Dot(right, (pos - midPoint)));
        }

        EditorGUI.BeginChangeCheck();
        pos = Handles.Slider(midPoint - right * link.width * 0.5f, -right, midSize * 0.03f, Handles.DotHandleCap, 0);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(link, "Adjust link width");
            link.width = Mathf.Max(0.0f, 2.0f * Vector3.Dot(-right, (pos - midPoint)));
        }

        Handles.color = oldColor;
    }
}
