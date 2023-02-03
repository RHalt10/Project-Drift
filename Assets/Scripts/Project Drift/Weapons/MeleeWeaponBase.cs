using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public enum MeleeWeaponTypes { None, Hammer, Sword, Spear } // Add more weapon types here. * Note: Also add new weapon type to "MeleeWeaponType" global var.
public abstract class MeleeWeaponBase : MonoBehaviour
{
    [Header("Melee Weapon Basic Configuration")]
    public MeleeWeaponTypes MeleeWeaponType;
    [SerializeField] public int MaxComboCount; 
    [SerializeField] public float ChargeTime; // Must be set
    [SerializeField] protected float NormalAttackCooldown;
    [SerializeField] protected float ChargedAttackCooldown;
    [SerializeField] protected List<GameObject> Hitboxes;

    [Header("Components References")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Melee Weapon Basic Gains")]
    [SerializeField] public float normalRechargePercentage;
    [SerializeField] public float destructibleRechargePercentage;
    
    public List<DamageOnTrigger2D> DamageTriggers { get { return hitboxDamageTriggers; } }

    [HideInInspector] public PlayerInventoryManager owningInventoryManager;

    // The owning inventory manager may not necessarily be initialized on Awake
    // so we initialize it here
    PlayerGun m_playerGun;
    protected PlayerGun playerGun
    {
        get
        {
            if (m_playerGun == null)
                m_playerGun = owningInventoryManager.GetComponent<PlayerGun>();

            return m_playerGun;
        }
    }

    protected Animator animator;

    // Hit Boxes
    private List<Collider2D> hitboxColliders = new List<Collider2D>();
    private List<DamageOnTrigger2D> hitboxDamageTriggers = new List<DamageOnTrigger2D>();

    // Normal Attack
    public bool NormalAttackDone = true;
    public int ComboCounter = 0;
    private float NormalCooldownTimer = 0;
    public int NormalAttackQueue; 

    // Charged Attack
    private float ChargedCooldownTimer = 0;
    private bool ChargedAttackDone = true; 

    // State
    protected enum AttackType { Normal, Charged }
    protected AttackType currentAttackType = AttackType.Normal;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

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

        HideWeapon();
    }

    protected virtual void Update()
    {
        if (NormalAttackDone && NormalAttackQueue > 0)
        {
            StartCoroutine(NormalAttackWrapper());
            NormalAttackQueue--;
        }

        if (NormalAttackDone) NormalCooldownTimer += Time.fixedDeltaTime;
        if (ChargedAttackDone) ChargedCooldownTimer += Time.fixedDeltaTime; 
    }

    public void NormalAttack()
    {
        if (NormalCooldownTimer < NormalAttackCooldown && ComboCounter + NormalAttackQueue >= MaxComboCount)
        {
            return; 
        }

        if (NormalCooldownTimer >= NormalAttackCooldown)
        {
            ComboCounter = 0;
        }

        NormalAttackQueue++;

        NormalCooldownTimer = 0;

    }

    public void ChargedAttack()
    {
        if (ChargedCooldownTimer < ChargedAttackCooldown) return;

        ChargedCooldownTimer = 0; 

        StartCoroutine(ChargedAttackWrapper());
    }

    public IEnumerator NormalAttackWrapper()
    {
        NormalAttackDone = false;
        NormalCooldownTimer = 0;
        ComboCounter++;
        ShowWeapon();
        currentAttackType = AttackType.Normal;
        yield return StartCoroutine(NormalAttackRoutine(ComboCounter));
        HideWeapon();
        yield return new WaitForSeconds(0.1f); 
        NormalAttackDone = true;
    }

    public IEnumerator ChargedAttackWrapper()
    {
        ChargedAttackDone = false;
        ShowWeapon();
        currentAttackType = AttackType.Charged;
        yield return StartCoroutine(ChargedAttackRoutine());
        HideWeapon();
        ChargedAttackDone = true; 
    }

    /// <summary>
    /// Implement Weapon's Normal Attack Routine.
    /// <para> - Be sure to include any cooldown logic. Input class will call this function whenever a "Melee Attack" input is detected. </para>
    /// </summary>
    protected abstract IEnumerator NormalAttackRoutine(int ComboCount);

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
        Color tmp = spriteRenderer.color;
        tmp.a = 0;
        spriteRenderer.color = tmp;

        foreach(Collider2D _collider in hitboxColliders)
        {
            _collider.enabled = false;
        }
    }

    void ShowWeapon()
    {
        Color tmp = spriteRenderer.color;
        tmp.a = 255;
        spriteRenderer.color = tmp;

        foreach (Collider2D _collider in hitboxColliders)
        {
            _collider.enabled = true;
        }
    }
}
