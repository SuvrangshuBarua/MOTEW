using UnityEngine;
using UnityEngine.Assertions;

namespace StateMachine
{

    public class ConstructionState : IState
    {
        public Building.BaseBuilding Building = null;

        private Nav.Patrol _patrol;
        private float _currentAngle = 0f;


        public void Enter(Humon npc)
        {
            float AngleStep = 30f; // Degrees between each patrol point
            Assert.IsNotNull(Building, "Building must be assigned before changing states");

            Building.AddConstructionWorker(npc.gameObject);

            // Start at a random angle to distribute workers around the circle
            _currentAngle = Random.Range(0f, 360f);

            // Randomly change the direction of patrol
            if (Random.value > 0.5f)
            {
                AngleStep *= -1f;
            }
          
            System.Func<Vector3> pos = () =>
            {
                _currentAngle += AngleStep;
                if (_currentAngle >= 360f)
                    _currentAngle -= 360f;

                float radians = _currentAngle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(
                Mathf.Cos(radians) * Building.ConstructionSiteRadius,
                Mathf.Sin(radians) * Building.ConstructionSiteRadius);

                return new Vector3(
                Building.transform.position.x + offset.x * 0.8f,
                npc.transform.position.y,
                Building.transform.position.z + offset.y * 0.8f);
            };
            npc.Navigation.TogglePanicSpeed();
            // Run 12 patrol points around the building
            _patrol = new Nav.Patrol(npc.Navigation, pos(), pos(), pos(), pos(), pos(), pos(), pos(), pos(), pos(), pos(), pos(), pos());
        }

        public void Update(Humon npc)
        {
            if (Building.State.IsConstructed)
            {
                npc.StateMachine.ChangeState<RoamState>();
                return;
            }

            _patrol.Update();
        }

        public void Exit(Humon npc)
        {
            Building.RemoveConstructionWorker(npc.gameObject);
            npc.Navigation.TogglePanicSpeed();
            Building = null;
            _patrol = null;
        }

        public State GetState()
        {
            return State.Construction;
        }
    }

} // namespace StateMachine

