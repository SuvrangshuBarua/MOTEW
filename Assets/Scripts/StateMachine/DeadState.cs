using UnityEngine;

namespace StateMachine
{

public class DeadState : IState
{
    public void Enter(Humon npc)
    {
    }

    public void Update(Humon npc)
    {
    }

    public void Exit(Humon npc)
    {
    }

    public State GetState()
    {
        return State.Dead;
    }
}

} // StateMachine

