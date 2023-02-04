using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Math;

public class CanDebuff : MonoBehaviour
{
    public LayerMask layerMaskToDebuff;
    [SerializeField] public List<DebuffInfo> debuffsToApply;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(layerMaskToDebuff, collision.gameObject.layer))
        {
            CharacterDebuff debuff = collision.gameObject.GetComponent<CharacterDebuff>();
            if(debuff == null) return;
            
            foreach (DebuffInfo dbf in debuffsToApply)
            {
                debuff.ApplyDebuff(dbf.debufftype, dbf.remainingDuration);
            }
        }
    }
}
