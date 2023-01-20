using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A collection of event classes relating to the Player
 */

/// <summary>
/// Called when the player is damaged
/// </summary>
public class PlayerDamagedEvent
{
    public int amount;

    public PlayerDamagedEvent(int _amount)
    {
        amount = _amount;
    }
}

/// <summary>
/// Called when the player shoots his gun
/// </summary>
public class PlayerShootEvent
{
    public PlayerWeaponSO weapon;

    public PlayerShootEvent(PlayerWeaponSO _weapon)
    {
        weapon = _weapon;
    }
}

/// <summary>
/// Called when the player attacks
/// </summary>
public class PlayerAttackEvent
{
    public PlayerAttackData attack;

    public PlayerAttackEvent(PlayerAttackData _attack)
    {
        attack = _attack;
    }
}

/// <summary>
/// Called when a key is acquired
/// </summary>
public class KeyAcquiredEvent { }

/// <summary>
/// Called when an enemy is defeated
/// </summary>
public class EnemyDefeatedEvent
{
    public string enemyKey;

    public EnemyDefeatedEvent(string _enemyKey)
    {
        enemyKey = _enemyKey;
    }
}