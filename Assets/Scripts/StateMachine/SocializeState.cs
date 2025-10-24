using UnityEngine;
using UnityEngine.Assertions;

namespace StateMachine
{

public class SocializeState : IState
{
    public Humon Other = null;
    public bool Speak = false;

    public void Enter(Humon npc)
    {
        _timestamp = 0f;

        var dir = Other.transform.position - npc.transform.position;
        var dest = npc.transform.position + dir * 0.35f;

        npc.Navigation.SetDestination(dest);
    }

    public void Update(Humon npc)
    {
        if (npc.Navigation.ReachedDestination)
        {
            if (_timestamp == 0f)
            {
                _timestamp = Time.time;
            }

            if (Speak)
            {
                string[] texts = {
                    "Hejsan!",
                    "Hej hej!",
                    "Tjena",
                    "Still standing, huh?",
                    "Dreamed the sky fell again",
                    "If it's quiet, it's good",
                    "Your place intact?",
                    "I've seen things",
                    "Weird stuff",
                    "Strange ways",
                    "We live, for now",
                };

                SpeechBubbleManager.Instance.CreateSpeechBubble(
                        npc.transform,
                        texts[Random.Range(0, texts.Length)]);
                Speak = false;
            }

            if (_timestamp + _duration <= Time.time)
            {
                npc.StateMachine.ChangeState<RoamState>();
                return;
            }
        }
    }

    public void Exit(Humon npc)
    {
        Other = null;
        Speak = false;
    }

    public State GetState()
    {
        return State.Socialize;
    }

    private static float _duration = 4f;
    private float _timestamp;
}

} // namespace StateMachine

