using UnityEngine;

namespace StateMachine
{
    public class InAirState : IState
    {
        private int IsFalling = Animator.StringToHash("falling");
        private bool m_dropInitiated = false;
        public void Enter(Humon npc)
        {
            m_dropInitiated = false;
            if (npc.Navigation.Agent.enabled)
            {
                npc.Navigation.Agent.enabled = false;
            }
            npc.Animator.SetBool(IsFalling, true);
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
            npc.Animator.SetBool(IsFalling, false);
        }
        public State GetState()
        {
            return State.InAir;
        }
    }
}