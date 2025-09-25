using UnityEngine;

public class Humon : MonoBehaviour
{
    private Perception _perception;
    private God.Draggable _draggable;

    void Start()
    {
        _perception = gameObject.AddComponent<Perception>();
        _draggable = gameObject.AddComponent<God.Draggable>();

        _draggable.OnStartDrag += Die;

        _perception.Subscribe(20, 5, Perception.Type.Single, LayerMask.GetMask("Building"), VisitBuilding);
    }

    void Update()
    {
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void VisitBuilding(Collider collider)
    {
        var building = collider.GetComponentInParent<Building.Residence>();
        building.Visit(gameObject, 2);
    }
}


