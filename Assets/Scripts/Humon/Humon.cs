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
    }

    void Update()
    {
    }

    void Die()
    {
        Destroy(gameObject);
    }
}

