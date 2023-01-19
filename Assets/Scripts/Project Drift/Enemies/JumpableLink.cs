using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special type of link that tells our pathfinding system to create a jumpable link between
/// two places
/// </summary>
public class JumpableLink : MonoBehaviour
{
    public Vector3 startPoint = new Vector3(0.0f, 0.0f, -2.5f);
    public Vector3 endPoint = new Vector3(0.0f, 0.0f, 2.5f);
    public float width;

    public Vector3 Direction => (endPoint - startPoint).normalized;
    public Vector3 WorldStartPoint => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint(startPoint);
    public Vector3 WorldEndPoint => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint(endPoint);

    public Rect GetRect()
    {
        Bounds bounds = GetBounds();
        return new Rect(bounds.min, bounds.size);
    }

    public Bounds GetBounds()
    {
        Vector3 right = new Vector3(-Direction.z, 0.0f, Direction.x);
        float radius = width / 2f;

        Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Vector3 realStartPoint = matrix.MultiplyPoint(startPoint);
        Vector3 realEndPoint = matrix.MultiplyPoint(endPoint);

        Vector3 bottomLeftPoint = realStartPoint - right * radius;
        Vector3 bottomRightPoint = realStartPoint + right * radius;
        Vector3 topLeftPoint = realEndPoint - right * radius;
        Vector3 topRightPoint = realEndPoint + right * radius;

        Bounds bounds = new Bounds();
        bounds.Encapsulate(bottomLeftPoint);
        bounds.Encapsulate(bottomRightPoint);
        bounds.Encapsulate(topLeftPoint);
        bounds.Encapsulate(topRightPoint);

        return bounds;
    }
}
