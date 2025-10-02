using UnityEngine;
using UnityEngine.Assertions;

namespace StateMachine
{

public class ConstructionState : IState
{
    public Building.BaseBuilding Building = null;

    private Nav.Patrol _patrol;

    public void Enter(Humon npc)
    {
        Assert.IsNotNull(Building, "Building must be assigned before changing states");

        Building.AddConstructionWorker(npc.gameObject);

        System.Func<Vector3> pos = () =>
        {
            while (true)
            {
                // Get a point thats within the construction radius, but not inside
                // the building collider, to avoid getting stuck.
                // Im sure there is a smarter way

                Vector2 off = Random.insideUnitCircle * Building.ConstructionSiteRadius;
                var pos = new Vector3(
                        Building.transform.position.x + off.x,
                        npc.transform.position.y,
                        Building.transform.position.z + off.y);

                var box = Building.Collider;
                var local = box.transform.InverseTransformPoint(pos);
                var half = box.size * 0.5f;

                if ((Mathf.Abs(local.x - box.center.x) <= half.x &&
                    Mathf.Abs(local.y - box.center.y) <= half.y &&
                    Mathf.Abs(local.z - box.center.z) <= half.z) == false)
                {
                    return pos;
                }
            }
        };

        _patrol = new Nav.Patrol(npc.Navigation, pos(), pos());
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
        Building = null;
        _patrol = null;
    }

    public State GetState()
    {
        return State.Construction;
    }
}

} // namespace StateMachine

