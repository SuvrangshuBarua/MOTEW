using UnityEngine;
using Unity.AI.Navigation;
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

    // hack
    [SerializeField]
    private NavMeshSurface _navSurface;

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
        var spawnPos = new Vector3(-4, 1, 8);
        var humon = Instantiate(_humon, spawnPos, Quaternion.identity);
        humon.GetComponent<Navigation>().Surface = _navSurface;
        _residents.Add(humon);
    }
}

} // namespace Building

