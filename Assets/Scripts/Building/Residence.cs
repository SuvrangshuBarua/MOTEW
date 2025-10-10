using UnityEngine;
using System.Collections.Generic;

namespace Building
{

public class Residence : BaseBuilding
{
    [SerializeField]
    public uint ResidentCapacity;

    private List<Humon> _residents = new ();

    void Start()
    {
        Construct();
    }

    void FixedUpdate()
    {
        if (!State.IsConstructed)
        {
            return;
        }

        // souls that are no longer with us :d
        _residents.RemoveAll((resident) => resident == null);

        // try spawn
        if (_residents.Count < ResidentCapacity)
        {
            if (Random.Range(0f, 1f) < .01f)
            {
                SpawnHumon();
            }
        }
    }

    void SpawnHumon()
    {
        var pos = new Vector3(-6, 1, 0) + transform.position;
        _residents.Add(GameManager.Instance.SpawnHumon(pos));
    }
}

} // namespace Building

