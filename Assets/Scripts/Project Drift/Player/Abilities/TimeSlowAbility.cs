using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

[CreateAssetMenu(fileName = "New Ability", menuName = "Project Drift/Abilities/TimeSlow")]
public class TimeSlowAbility : PlayerAbilitySO
{
    public float MaxDuration; 
    public float SlowedTime;
    public float NormalTime;

    Subscription<AbilityInterruptEvent> interrupt_event;

    private bool interrupted = false;

    public override void Initialize(PlayerController _controller)
    {
        base.Initialize(_controller);
        interrupt_event = EventBus.Subscribe<AbilityInterruptEvent>(OnInterrupted);
        interrupted = false;
    }

    public override void Activate()
    {
        Debug.Log("(TIME SLOW ABILITY) Player Stamia: " + playerStamina.currentStamina);
        interrupted = false;
        playerStamina.UseStaminaOverTime(MaxDuration, staminaCost * 100);
        controller.StartCoroutine(DoAbility());
    }

    IEnumerator DoAbility()
    {
        EventBus.Publish(new TimeSlowEvent(true));

        Time.timeScale = SlowedTime;

        while(!interrupted && playerStamina.currentStamina > 0)
        {
            yield return null;
        }

        Time.timeScale = NormalTime;
        interrupted = false;

        EventBus.Publish(new TimeSlowEvent(false));
    }

    void OnInterrupted(AbilityInterruptEvent e)
    {
        Debug.Log("(TIME SLOW ABILITY) Ability Interrupted!");
        interrupted = true;
    }
}

public class AbilityInterruptEvent { }

public class TimeSlowEvent {
    public bool status;

    public TimeSlowEvent(bool _status) { status = _status; }
}

