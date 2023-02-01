using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

[CreateAssetMenu(fileName = "New Ability", menuName = "Project Drift/Abilities/EnergyBurst")]
public class EnergyBurstAbility : PlayerAbilitySO
{
    public float time;
    public float radius;
    public LayerMask enemyLayer;
    public LayerMask projectileLayer;
    public float destroyProjectilesInterval;
    public float knockbackForce;
    public float knockbackTime;

    public override void Activate()
    {
        controller.StartCoroutine(DoAbility());
    }

    IEnumerator DoAbility()
    {
        playerStamina.UseStamina(staminaCost);
        DestroyEnemyProjectiles();
        KnockEnemiesBack();
        
        for (float f = 0; f < time; f += destroyProjectilesInterval)
        {
            yield return new WaitForSeconds(destroyProjectilesInterval);
            DestroyEnemyProjectiles();
        }
    }

    void DestroyEnemyProjectiles()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(controller.transform.position, radius, projectileLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<ProjectileMovement2D>() != null)
                Destroy(colliders[i].gameObject);
        }
    }

    void KnockEnemiesBack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(controller.transform.position, radius, enemyLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CanBeKnockbacked knockback = colliders[i].GetComponent<CanBeKnockbacked>();
            if (knockback != null)
            {
                Vector3 direction = colliders[i].transform.position - controller.transform.position;
                knockback.Knockback(direction.normalized * knockbackForce, knockbackTime);
            }
        }
    }
}
