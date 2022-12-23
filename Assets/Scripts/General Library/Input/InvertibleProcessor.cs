using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class InvertibleProcessor : InputProcessor<Vector2>
{
    public bool invertX;
    public bool invertY;

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        if (invertX)
            value.x = -value.x;
        if (invertY)
            value.y = -value.y;

        return value;
    }

#if UNITY_EDITOR
    static InvertibleProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<InvertibleProcessor>();
    }
}
