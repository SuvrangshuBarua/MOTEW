using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine.AI;

namespace StateMachine
{   

public class PanicState : IState
{
    public SpeechBubble SpeechBubble = null;

    private float _timestamp;
    private const float _duration = 4;

    public void Enter(Humon npc)
    {
        npc.Navigation.TogglePanicSpeed();

        if (Random.Range(0f, 1f) < 0.5f)
        {
            string[] texts = {
                "WHAT THE F-",
                "RUUUN",
                "HIDE! HIDE!",
                "CATCH ME IF YOU CAN",
                "I HAVE KIDS",
                "ITS THE END",
                "SPARE ME",
            };

            SpeechBubble = SpeechBubbleManager.Instance
                    .CreateSpeechBubble(npc.transform,
                        texts[Random.Range(0, texts.Length)],
                        null, _duration);
        }

        _timestamp = Time.time + _duration;
    }

    public void Update(Humon npc)
    {
        if (_timestamp <= Time.time)
        {
            npc.StateMachine.ChangeState<RoamState>();
            return;
        }

        if (npc.Navigation.ReachedDestination)
        {
            npc.Navigation.SetDestination(
                    npc.Navigation.GetRandomDestination());
        }
    }

    public void Exit(Humon npc)
    {
        npc.Navigation.TogglePanicSpeed();
        npc.Navigation.StopMoving();

        SpeechBubble = null;
    }

    public State GetState()
    {
        return State.Panic;
    }
}

} // namespace StateMachine

