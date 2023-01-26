using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

[RequireComponent(typeof(DamageOnTrigger2D))]
[RequireComponent(typeof(TriggerOnCollision2D))]
public abstract class MeleeWeaponBase : MonoBehaviour
{
    [Header("Melee Weapon Basic Configuration")]
    [SerializeField] protected float BaseDamage;
    [SerializeField] public float ChargeTime; // Must be set
    [SerializeField] protected float NormalAttackCooldown;
    [SerializeField] protected float ChargedAttackCooldown;
    [SerializeField] protected PlayerAttackData AttackData;

    public DamageOnTrigger2D DamageTrigger { get { return damageTrigger; } }
    private DamageOnTrigger2D damageTrigger;
    public TriggerOnCollision2D AffordanceTrigger { get { return affordanceTrigger; } }
    private TriggerOnCollision2D affordanceTrigger;

    SpriteRenderer m_SpriteRenderer;
    Collider2D m_Collider;

    protected virtual void Awake()
    {
        damageTrigger = GetComponent<DamageOnTrigger2D>();
        affordanceTrigger = GetComponent<TriggerOnCollision2D>();

        damageTrigger.OnDamageCaused.AddListener(ApplyCost);
        damageTrigger.OnDamageCaused.AddListener(ApplyGain);

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<Collider2D>();

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
        yield return StartCoroutine(NormalAttackRoutine());
        HideWeapon();
    }

    public IEnumerator ChargedAttackWrapper()
    {
        ShowWeapon();
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
    /// <para> - This function will be called by Damage Trigger if something is hit. </para>
    /// <para> - target is the object hit. </para>
    /// </summary>
    public abstract void ApplyCost(GameObject target);

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

        m_Collider.enabled = false;
    }

    void ShowWeapon()
    {
        Color tmp = m_SpriteRenderer.color;
        tmp.a = 255;
        m_SpriteRenderer.color = tmp;

        m_Collider.enabled = true;
    }
}
