using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Attach to give object ability to be buffed/debuffed for duration given by BuffSO.
/// </summary> 
public class HasBuff : MonoBehaviour
{
    private readonly Dictionary<BuffSO, float> _buffs = new Dictionary<BuffSO, float>();
    private GroundCharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<GroundCharacterController>();
    }

    private void Update() {
        // Update remaining time for each buff applied.
        foreach (BuffSO buff in _buffs.Keys.ToList())
        {
            if(_buffs[buff] > 0) {
                _buffs[buff] -= Time.deltaTime; 
            } else {
                buff.DeactivateBuff(gameObject);
                _buffs.Remove(buff);
            }
        }
    }

    /// <summary>
    /// Public method for applying a buff to character this is attached to.
    /// </summary> 
    public void ApplyBuff(BuffSO debuff)
    {
        debuff.ActivateBuff(gameObject);
        if(_buffs.ContainsKey(debuff))
        {
            _buffs[debuff] = debuff.buffDuration;
        }
        else
        {
            _buffs.Add(debuff, debuff.buffDuration);
        }
    }
}
