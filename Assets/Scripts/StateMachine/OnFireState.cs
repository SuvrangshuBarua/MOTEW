using UnityEngine;

namespace StateMachine
{

public class OnFireState : IState
{
    public Fire Source;
    public SpeechBubble SpeechBubble;

    private GameObject _firePrefab;
    private GameObject _vfx;

    public OnFireState(GameObject firePrefab)
    {
        _firePrefab = firePrefab;
    }

    public void Enter(Humon npc)
    {
        var fire = npc.gameObject.AddComponent<FireDamage>();
        fire.Damage = Source.Damage;

        // TODO: make this constexpr
        string[] texts = {
            "AAAAAAHH",
            "I'M ON FIRE",
            "NOT LIKE THIS",
            "WHO SET ME ON FIRE?!",
            "HOLD THE LINE",
            "EVERY MAN FOR HIMSELF",
            "CALL 112!",
        };

        SpeechBubble = SpeechBubbleManager.Instance
                .CreateSpeechBubble(
                    npc.transform,
                    texts[Random.Range(0, texts.Length)]);

        _vfx = UnityEngine.Object.Instantiate(
                _firePrefab, npc.transform);
        _vfx.transform.localScale *= 0.5f;

        npc.Navigation.TogglePanicSpeed();
    }

    public void Update(Humon npc)
    {
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

        UnityEngine.Object.Destroy(
                npc.GetComponent<FireDamage>());
        UnityEngine.Object.Destroy(_vfx);

        SpeechBubble = null;
    }

    public State GetState()
    {
        return State.OnFire;
    }
}

} // namespace StateMachine

