using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public enum MeleeWeaponTypes { None, Hammer } // Add more weapon types here. * Note: Also add new weapon type to "MeleeWeaponType" global var.
public abstract class MeleeWeaponBase : MonoBehaviour
{
    [Header("Melee Weapon Basic Configuration")]
    [SerializeField] public float ChargeTime; // Must be set
    [SerializeField] protected float NormalAttackCooldown;
    [SerializeField] protected float ChargedAttackCooldown;
    [SerializeField] protected List<GameObject> Hitboxes;

    public List<DamageOnTrigger2D> DamageTriggers { get { return hitboxDamageTriggers; } }
    public MeleeWeaponTypes MeleeWeaponType { 
        get
        {
            if (gameObject.GetComponent<Hammer>() != null) return MeleeWeaponTypes.Hammer;
            return MeleeWeaponTypes.None;
        } 
    }

    SpriteRenderer m_SpriteRenderer;

    private List<Collider2D> hitboxColliders = new List<Collider2D>();
    private List<DamageOnTrigger2D> hitboxDamageTriggers = new List<DamageOnTrigger2D>();
    protected enum AttackType { Normal, Charged }
    protected AttackType currentAttackType = AttackType.Normal;

    protected virtual void Awake()
    {

        for(int i = 0; i < Hitboxes.Count; ++i)
        {
            GameObject hitbox = Hitboxes[i];
            Collider2D hitboxCollider = hitbox.GetComponent<Collider2D>();
            DamageOnTrigger2D hitboxDamageTrigger = hitbox.GetComponent<DamageOnTrigger2D>();

            if (hitboxCollider == null)
            {
                Debug.LogError("Error: Hitbox #" + i + " does not have a Collider2D coponent!");
                return;
            }

            if(hitboxDamageTrigger == null)
            {
                Debug.LogError("Error: Hitbox #" + i + " does not have a DamageOnTrigger2D coponent!");
                return;
            }

            hitboxDamageTrigger.OnDamageCaused.AddListener(ApplyGain);

            hitboxColliders.Add(hitboxCollider);
            hitboxDamageTriggers.Add(hitboxDamageTrigger);
        }

        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        HideWeapon();
    }

    public void NormalAttack()
    {
        StartCoroutine(NormalAttackWrapper());
    }

    public void ChargedAttack()
    {
        StartCoroutine(ChargedAttackWrapper());
    }

    public IEnumerator NormalAttackWrapper()
    {
        ShowWeapon();
        currentAttackType = AttackType.Normal;
        yield return StartCoroutine(NormalAttackRoutine());
        HideWeapon();
    }

    public IEnumerator ChargedAttackWrapper()
    {
        ShowWeapon();
        currentAttackType = AttackType.Charged;
        yield return StartCoroutine(ChargedAttackRoutine());
        HideWeapon();
    }

    /// <summary>
    /// Implement Weapon's Normal Attack Routine.
    /// <para> - Be sure to include any cooldown logic. Input class will call this function whenever a "Melee Attack" input is detected. </para>
    /// </summary>
    protected abstract IEnumerator NormalAttackRoutine();

    /// <summary>
    /// Implement Weapon's Charged Attack Routine.
    /// <para> - Be sure to include any cooldown logic. Input class will call this function whenever a "Charged Attack" input is detected. </para> 
    /// </summary>
    protected abstract IEnumerator ChargedAttackRoutine();

    /// <summary>
    /// Implement the cost of a single attack. Leave blank if there're none.
    /// <para> - Programmer must call this function in Normal/Charged Attack Routine. </para>
    /// <para> - target is the object hit. </para>
    /// </summary>
    public abstract void ApplyCost(bool isNormalAttack);

    /// <summary>
    /// Implement the gain of a single attack. Ex: stamina
    /// <para> - This function will be called by Damage Trigger if something is hit. </para>
    /// <para> - target is the object hit. </para>
    /// </summary>
    public abstract void ApplyGain(GameObject target);

    void HideWeapon()
    {
        Color tmp = m_SpriteRenderer.color;
        tmp.a = 0;
        m_SpriteRenderer.color = tmp;

        foreach(Collider2D _collider in hitboxColliders)
        {
            _collider.enabled = false;
        }
    }

    void ShowWeapon()
    {
        Color tmp = m_SpriteRenderer.color;
        tmp.a = 255;
        m_SpriteRenderer.color = tmp;

        foreach (Collider2D _collider in hitboxColliders)
        {
            _collider.enabled = true;
        }
    }
}
