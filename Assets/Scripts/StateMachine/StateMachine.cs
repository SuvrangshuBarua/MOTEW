using UnityEngine;
using System;
using System.Collections.Generic;

namespace StateMachine
{
    public enum State
    {
        Roam = 0,
        InAir,
        Construction,
        Panic,
        Fire,
        Dead
    }

    public interface IState
    {
        void Enter(Humon npc);
        void Update(Humon npc);
        void Exit(Humon npc);
        State GetState();
    }

    public class StateMachine
    {
        private IState m_currentState;
        private Dictionary<Type, IState> m_states = new();
        private Humon m_owner;
        
        public StateMachine(Humon owner)
        {
            m_owner = owner;
        }
        
        public void AddState<T>(T state) where T : IState
        {
            m_states[typeof(T)] = state;
        }
        public void ChangeState<T>() where T : IState
        {
            if(m_states.ContainsKey(typeof(T))) ChangeState(m_states[typeof(T)]);
            else Debug.LogError($"State {typeof(T).Name} not found");
        }

        private void ChangeState(IState newState)
        {
            m_currentState?.Exit(m_owner);
            m_currentState = newState;
            m_currentState?.Enter(m_owner);
        }
        
        public void Update()
        {
            m_currentState?.Update(m_owner);
        }
        
        public IState CurrentState => m_currentState;

        public T GetState<T>() where T : IState
        {
            if (m_states.ContainsKey(typeof(T))) return (T)m_states[typeof(T)];
            return default(T);       
        }
    }
}
