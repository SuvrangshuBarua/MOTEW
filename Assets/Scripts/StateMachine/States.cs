using UnityEngine;

namespace StateMachine
{
    public class RoamState : IState
    {
        private Vector3 m_targetPosition;
        private float m_navigationWaitTime;
        private bool m_isWaiting; 
        public void Enter(Humon npc)
        {
            if (npc.Navigation.Agent != null)
            {
                npc.Navigation.Agent.ResetPath();
            }
            ResetState(npc);
        }

        private void ResetState(Humon npc)
        {
            m_isWaiting = false;
            m_navigationWaitTime = 0.0f;
            m_targetPosition = npc.Navigation.GetRandomDestination();
            npc.Navigation.SetDestination(m_targetPosition);
        }

        public void Update(Humon npc)
        {
            if (m_isWaiting)
            {
                if (m_navigationWaitTime > 0)
                {
                    m_navigationWaitTime -= Time.deltaTime;
                }
                else
                {
                    ResetState(npc);
                }
            }
            else
            {
                
                if (npc.Navigation.ReachedDestination)
                {
                    m_isWaiting = true;
                    m_navigationWaitTime = 1f;
                    
                }
            }
        }

        public void Exit(Humon npc)
        {
            npc.Navigation.StopMoving();
        }

       
    }
    
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
    }
}
