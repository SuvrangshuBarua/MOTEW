using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] public float Duration;

void Start()
    {
        Destroy(gameObject, Duration);
    }

}

