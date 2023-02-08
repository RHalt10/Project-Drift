using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

[CreateAssetMenu(fileName = "New Ability", menuName = "Project Drift/Abilities/TimeSlow")]
public class TimeSlowAbility : PlayerAbilitySO
{
    public float drainRate;
    public float slowedTimeScale;

    Subscription<AbilityInterruptEvent> interrupt_event;

    private bool interrupted = false;
    private bool ongoing = false;

    public override void Initialize(PlayerController _controller)
    {
        base.Initialize(_controller);
        interrupt_event = EventBus.Subscribe<AbilityInterruptEvent>(OnInterrupted);
        interrupted = false;
    }

    public override bool CanBeActivated()
    {
        return playerStamina.currentStamina > 0.01f;
    }

    public override void Activate()
    {
        if (ongoing)
        {
            interrupted = true;
            playerStamina.StopStaminaDrain();
        }
        else
        {
            interrupted = false;
            float duration = playerStamina.currentStamina / drainRate;
            playerStamina.UseStaminaOverTime(duration, drainRate);
            controller.StartCoroutine(DoAbility());
        }
    }

    IEnumerator DoAbility()
    {
        ongoing = true;
        EventBus.Publish(new TimeSlowEvent(true));

        Time.timeScale = slowedTimeScale;

        while (!interrupted && playerStamina.currentStamina > 0)
        {
            yield return null;
        }

        Time.timeScale = 1f;
        interrupted = false;

        EventBus.Publish(new TimeSlowEvent(false));
        ongoing = false;
    }

    void OnInterrupted(AbilityInterruptEvent e)
    {
        interrupted = true;
    }
}

public class AbilityInterruptEvent { }

public class TimeSlowEvent {
    public bool status;

    public TimeSlowEvent(bool _status) { status = _status; }
}

