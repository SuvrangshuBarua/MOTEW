using UnityEngine;
using Unity.AI.Navigation;

public class Humon : MonoBehaviour
{
    private Perception _perception;
    private God.Draggable _draggable;

    private Nav.Patrol _patrol = null;

    void Start()
    {
        _perception = gameObject.AddComponent<Perception>();
        _perception.Subscribe(5, 1, Perception.Type.Single, LayerMask.GetMask("Building"), InteractWithBuilding);

        _draggable = gameObject.AddComponent<God.Draggable>();
        _draggable.OnStartDrag += Die;

        // FIXME:
        GetComponent<Navigation>().Surface = GameObject.Find("floor").GetComponent<NavMeshSurface>();
    }

    void FixedUpdate()
    {
        if (_patrol != null)
        {
            _patrol.Update();
        }
        else
        {
            var nav = GetComponent<Navigation>();
            if (nav.ReachedDestination)
            {
                nav.SetDestination(nav.GetRandomDestination());
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void InteractWithBuilding(Collider collider)
    {
        var building = collider.GetComponentInParent<Building.Residence>();

        if (building.State.IsInConstruction)
        {
            if (_patrol != null)
            {
                // TODO: hack, fix if patrol is used for more than building construction
                return;
            }

            System.Func<Vector3> pos = () =>
            {
                while (true)
                {
                    // Get a point thats within the construction radius, but not inside
                    // the building collider, to avoid getting stuck.
                    // Im sure there is a smarter way

                    Vector2 off = Random.insideUnitCircle * building.ConstructionSiteRadius;
                    var pos = new Vector3(
                            building.transform.position.x + off.x,
                            transform.position.y,
                            building.transform.position.z + off.y);

                    var box = building.Collider;
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

            _patrol = new Nav.Patrol(GetComponent<Navigation>(), pos(), pos());
            building.AddConstructionWorker(gameObject);
        }

        if (building.State.IsConstructed)
        {
            if (_patrol != null)
            {
                // TODO: hack, fix if patrol is used for more than building construction
                _patrol = null;
            }

            building.Visit(gameObject, 2);
            return;
        }
    }

    void Patrol()
    {
    }
}


