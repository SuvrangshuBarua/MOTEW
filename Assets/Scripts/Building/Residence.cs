using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace Building
{

public class Residence : BaseBuilding
{
    [SerializeField]
    public uint ResidentCapacity;

    [SerializeField]
    private Vector3 _spawnOffset;

    private List<Humon> _residents = new ();

    void Start()
    {
        Assert.IsFalse(_spawnOffset == Vector3.zero);
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
        var pos = _spawnOffset + transform.position;
        _residents.Add(GameManager.Instance.SpawnHumon(pos));
    }
}

} // namespace Building

