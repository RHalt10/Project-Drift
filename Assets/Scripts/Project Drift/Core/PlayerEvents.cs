using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagedEvent
{
    public int amount;

    public PlayerDamagedEvent(int _amount)
    {
        amount = _amount;
    }
}

public class PlayerShootEvent
{
    public PlayerWeaponSO weapon;

    public PlayerShootEvent(PlayerWeaponSO _weapon)
    {
        weapon = _weapon;
    }
}

public class PlayerAttackEvent
{
    public PlayerAttackData attack;

    public PlayerAttackEvent(PlayerAttackData _attack)
    {
        attack = _attack;
    }
}

public class KeyAcquiredEvent { }

public class EnemyDefeatedEvent
{
    public string enemyKey;

    public EnemyDefeatedEvent(string _enemyKey)
    {
        enemyKey = _enemyKey;
    }
}