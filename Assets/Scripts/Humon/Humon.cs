using UnityEngine;

public class Humon : MonoBehaviour
{
    private Perception _perception;

    void Start()
    {
        _perception = gameObject.AddComponent<Perception>();
    }

    void Update()
    {
        
    }
}

