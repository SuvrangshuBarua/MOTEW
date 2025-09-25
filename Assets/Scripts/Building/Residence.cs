using UnityEngine;
using System.Collections.Generic;

namespace Building
{

public class Residence : BaseBuilding
{
    [SerializeField]
    private uint _residentCapacity;

    [SerializeField]
    private GameObject _humon;

    private List<GameObject> _residents = new ();

    void Start()
    {
        Construct();
    }

    void FixedUpdate()
    {
        if (_state != Building.State.Constructed)
        {
            return;
        }

        // souls that are no longer with us :d
        _residents.RemoveAll((resident) => resident == null);

        // try spawn
        if (_residents.Count < _residentCapacity)
        {
            if (Random.Range(0f, 1f) < .1f)
            {
                SpawnHumon();
            }
        }
    }

    void SpawnHumon()
    {
        var spawnPos = new Vector3(0, 20, 0) + transform.position;
        var humon = Instantiate(_humon, spawnPos, Quaternion.identity);
        _residents.Add(humon);
    }
}

} // namespace Building

