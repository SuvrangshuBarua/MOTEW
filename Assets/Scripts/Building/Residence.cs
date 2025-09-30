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
        OnConstruction += Rebake;
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
            if (Random.Range(0f, 1f) < .01f)
            {
                SpawnHumon();
            }
        }
    }

    void Rebake()
    {
        // collider don't get considered for nav mesh generation,
        // create a temporary mesh for baking

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);

        box.transform.position = Collider.transform.position;
        box.transform.rotation = Collider.transform.rotation;
        box.transform.localScale = Collider.size;
        box.isStatic = true;

        FindFirstObjectByType<Unity.AI.Navigation.NavMeshSurface>().BuildNavMesh();

        Destroy(box);
    }

    void SpawnHumon()
    {
        var spawnPos = new Vector3(-6, 1, 0) + transform.position;
        var humon = Instantiate(_humon, spawnPos, Quaternion.identity);
        _residents.Add(humon);
    }
}

} // namespace Building

