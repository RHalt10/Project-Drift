using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Math;

public class BuffOnCollide : MonoBehaviour
{
    public LayerMask layerMaskToDebuff;
    [SerializeField] public List<BuffSO> debuffsToApply;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(layerMaskToDebuff, collision.gameObject.layer))
        {
            HasBuff charBuffs = collision.gameObject.GetComponent<HasBuff>();
            if(charBuffs == null) return;
            
            foreach (BuffSO dbf in debuffsToApply)
            {
                charBuffs.ApplyBuff(dbf);
            }
        }
    }
}
