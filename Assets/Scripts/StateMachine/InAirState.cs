using UnityEngine;

namespace StateMachine
{
    public class InAirState : IState
    {
        private bool m_dropInitiated = false;
        public void Enter(Humon npc)
        {
            m_dropInitiated = false;
            if (npc.Navigation.Agent.enabled)
            {
                npc.Navigation.Agent.enabled = false;
            }
        }

        public void Update(Humon npc)
        {
            if (!npc.IsBeingDragged && !m_dropInitiated)
            {
                m_dropInitiated = true;
                npc.DropToSurface();
            }
        }

        public void Exit(Humon npc)
        {
            m_dropInitiated = false;
        }
        public State GetState()
        {
            return State.InAir;
        }
    }
}