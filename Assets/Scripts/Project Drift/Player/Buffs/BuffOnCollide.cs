using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Math;

public class BuffOnCollide : MonoBehaviour
{
    public LayerMask layerMaskToBuff;
    [Tooltip("List of buffs to apply on contact. Drag BuffSO in.")]
    [SerializeField] public List<BuffSO> buffsToApply;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(layerMaskToBuff, collision.gameObject.layer))
        {
            HasBuff charBuffs = collision.gameObject.GetComponent<HasBuff>();
            if(charBuffs == null) {
                Debug.LogWarning("Hit object " + collision.gameObject.name + 
                                 " that cannot recieve buffs/debuffs");
                return;
            }

            foreach (BuffSO dbf in buffsToApply)
            {
                charBuffs.ApplyBuff(dbf);
            }
        }
    }
}
