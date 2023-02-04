using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType {
    Slow,
    PLACEHOLD
}

public abstract class BuffSO : ScriptableObject
{
    public float remainingDuration;
    public BuffType buffType;

    //<summary>
    // Implement logic for starting
    //</summary>
    public abstract void Initialize();

    //<summary>
    // Implement logic for activating buff/defbuff
    //</summary>
    public abstract void ActivateBuff(GameObject buffed);

    //<summary>
    // Logic for deactivating applied buff/defbuff
    //</summary>
    public abstract void DeactivateBuff(GameObject buffed);
}
