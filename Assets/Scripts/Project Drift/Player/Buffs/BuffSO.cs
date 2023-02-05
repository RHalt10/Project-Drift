using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff Scriptable Object parent class.
/// Defines methods for activating and deactivating buff.
/// </summary>  
public abstract class BuffSO : ScriptableObject
{
    public float buffDuration;

    //<summary>
    // Implement logic for activating buff/defbuff
    //</summary>
    public abstract void ActivateBuff(GameObject charToBuff);

    //<summary>
    // Logic for deactivating applied buff/defbuff
    //</summary>
    public abstract void DeactivateBuff(GameObject charToBuff);
}
