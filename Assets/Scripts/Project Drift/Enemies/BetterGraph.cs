using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Serialization;
using Pathfinding.Util;
using System;

/// <summary>
/// A custom pathfinding graph that only allows nodes on ground tiles
/// and has links on jumpable links.
/// Written by Nikhil Ghosh '24
/// </summary>
[JsonOptIn]
[Preserve]
public class BetterGraph : PointGraph
{
    [JsonMember]
    public LayerMask groundLayerMask;

    [JsonMember]
    public LayerMask obstacleLayerMask;

    [JsonMember]
    public float nodeSize;

    PointNode[,] nodeGrid;
    Vector2 sceneBoundsMin;

    GraphTransform transform;

    public override void GetNodes(Action<GraphNode> action)
    {
        if (nodes == null) return;

        for (int i = 0; i < nodes.Length; i++)
        {
            // Call the delegate
            action(nodes[i]);
        }
    }

    public override NNInfoInternal GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
    {
        Vector2Int index = new Vector2Int((int)((position.x - sceneBoundsMin.x) / nodeSize), 
            (int)((position.y - sceneBoundsMin.y) / nodeSize));
        if (nodeGrid[index.x, index.y] != null)
            return new NNInfoInternal(nodeGrid[index.x, index.y]);

        return base.GetNearest(position, constraint, hint);
    }

    public override NNInfoInternal GetNearestForce(Vector3 position, NNConstraint constraint)
    {
        Vector2Int index = new Vector2Int((int)((position.x - sceneBoundsMin.x) / nodeSize),
            (int)((position.y - sceneBoundsMin.y) / nodeSize));
        if (nodeGrid[index.x, index.y] != null)
            return new NNInfoInternal(nodeGrid[index.x, index.y]);

        return base.GetNearestForce(position, constraint);
    }

    protected override IEnumerable<Progress> ScanInternal()
    {
        Bounds sceneBounds = GetBoundsOfScene();
        Vector2 sceneSize = sceneBounds.size;
        Vector2Int gridDimensions = new Vector2Int(Mathf.CeilToInt(sceneSize.x / nodeSize), Mathf.CeilToInt(sceneSize.y / nodeSize));

        nodeGrid = new PointNode[gridDimensions.x, gridDimensions.y];

        yield return new Progress(0.05f, "?");

        int iterations = 0;
        for(int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                Vector2 position = (Vector2)sceneBounds.min + new Vector2(i * nodeSize, j * nodeSize);
                if (IsValidNodePosition(position))
                    nodeGrid[i, j] = CreateNode(position);

                iterations++;
                if (iterations >= 200)
                {
                    yield return new Progress(0.1f, "?");
                    iterations = 0;
                }
            }
        }

        for (int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                AddGridConnectionsToNode(ref nodeGrid, i, j, gridDimensions);

                iterations++;
                if (iterations >= 200)
                {
                    yield return new Progress(0.1f, "?");
                    iterations = 0;
                }
            }
        }

        foreach(JumpableLink link in GameObject.FindObjectsOfType<JumpableLink>())
        {
            AddLinkConnectionsToNodes(link, ref nodeGrid, sceneBounds, gridDimensions);
        }

        List<PointNode> allNodes = new List<PointNode>();
        for (int i = 0; i < gridDimensions.x; i++)
        {
            for(int j = 0; j < gridDimensions.y; j++)
            {
                if (nodeGrid[i, j] != null)
                    allNodes.Add(nodeGrid[i, j]);
            }
        }
        nodes = allNodes.ToArray();
        sceneBoundsMin = sceneBounds.min;
    }

    Bounds GetBoundsOfScene()
    {
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);
        foreach (Collider2D collider in GameObject.FindObjectsOfType(typeof(Collider2D)))
            b.Encapsulate(collider.bounds);

        b.extents = b.extents * 1.1f;
        
        return b;
    }

    bool IsValidNodePosition(Vector2 position)
    {
        Collider2D col = Physics2D.OverlapPoint(position, groundLayerMask);
        if (col == null)
            return false;

        col = Physics2D.OverlapPoint(position, obstacleLayerMask);
        return col == null;
    }

    PointNode CreateNode(Vector2 position)
    {
        PointNode node = new PointNode(active);
        // Node positions are stored as Int3. We can convert a Vector3 to an Int3 like this
        node.position = (Int3)(Vector3)position;
        node.Walkable = true;
        return node;
    }

    void AddGridConnectionsToNode(ref PointNode[,] nodes, int i, int j, Vector2Int gridDimensions)
    {
        uint straightCost = (uint)Mathf.RoundToInt(nodeSize * Int3.Precision);
        uint diagonalCost = (uint)Mathf.RoundToInt(nodeSize * Mathf.Sqrt(2F) * Int3.Precision);

        if (nodes[i, j] != null)
        {
            if (i != 0 && nodes[i - 1, j] != null)
                nodes[i, j].AddConnection(nodes[i - 1, j], straightCost);
            if (i != gridDimensions.x - 1 && nodes[i + 1, j] != null)
                nodes[i, j].AddConnection(nodes[i + 1, j], straightCost);
            if (j != 0 && nodes[i, j - 1] != null)
                nodes[i, j].AddConnection(nodes[i, j - 1], straightCost);
            if (j != gridDimensions.y - 1 && nodes[i, j + 1] != null)
                nodes[i, j].AddConnection(nodes[i, j + 1], straightCost);

            if (i != 0 && j != 0 && nodes[i - 1, j - 1] != null)
                nodes[i, j].AddConnection(nodes[i - 1, j - 1], diagonalCost);
            if (i != 0 && j != gridDimensions.y - 1 && nodes[i - 1, j + 1] != null)
                nodes[i, j].AddConnection(nodes[i - 1, j + 1], diagonalCost);
            if (i != gridDimensions.x - 1 && j != 0 && nodes[i + 1, j - 1] != null)
                nodes[i, j].AddConnection(nodes[i + 1, j - 1], diagonalCost);
            if (i != gridDimensions.x - 1 && j != gridDimensions.y - 1 && nodes[i + 1, j + 1] != null)
                nodes[i, j].AddConnection(nodes[i + 1, j + 1], diagonalCost);
        }
    }

    void AddLinkConnectionsToNodes(JumpableLink link, ref PointNode[,] nodes, Bounds sceneBounds, Vector2Int gridDimensions)
    {
        Vector3 right = Quaternion.AngleAxis(-90f, Vector3.forward) * link.Direction;
        Vector3 startLeftPoint = link.WorldStartPoint - right.normalized * (link.width / 2f);
        Vector3 endLeftPoint = link.WorldEndPoint - right.normalized * (link.width / 2f);

        for (float f = 0; f < link.width; f += nodeSize)
        {
            Vector3 startCurrentPoint = startLeftPoint + right.normalized * f;
            Vector3 endCurrentPoint = endLeftPoint + right.normalized * f;

            Vector2 startCurrentIndex = ((Vector2)startCurrentPoint - (Vector2)sceneBounds.min) / nodeSize;
            Vector2 endCurrentIndex = ((Vector2)endCurrentPoint - (Vector2)sceneBounds.min) / nodeSize;

            PointNode startCurrentNode = nodes[(int)startCurrentIndex.x, (int)startCurrentIndex.y];
            PointNode endCurrentNode = nodes[(int)endCurrentIndex.x, (int)endCurrentIndex.y];
            if (startCurrentNode != null && endCurrentNode != null)
            {
                uint cost = (uint)Mathf.RoundToInt(Vector3.Distance(startCurrentPoint, endCurrentPoint) * Int3.Precision);
                startCurrentNode.AddConnection(endCurrentNode, cost);
                endCurrentNode.AddConnection(startCurrentNode, cost);
            }
        }
    }
}
