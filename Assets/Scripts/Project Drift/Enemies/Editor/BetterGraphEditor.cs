using UnityEditor;
using Pathfinding;

[CustomGraphEditor(typeof(BetterGraph), "Better Graph")]
public class BetterGraphEditor : GraphEditor
{
    public override void OnInspectorGUI(NavGraph target)
    {
        BetterGraph graph = target as BetterGraph;

        graph.groundLayerMask = EditorGUILayoutx.LayerMaskField("Ground Layer Mask", graph.groundLayerMask);
        graph.obstacleLayerMask = EditorGUILayoutx.LayerMaskField("Obstacle Layer Mask", graph.obstacleLayerMask);

        graph.nodeSize = EditorGUILayout.FloatField("Node Size", graph.nodeSize);
    }
}
