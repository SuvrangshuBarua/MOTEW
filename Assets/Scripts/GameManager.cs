using GrimTools.Runtime.Core;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class GameManager : PersistantMonoSingleton<GameManager>
{
    public bool canAfford;
    public event Action<int> OnCashChanged;
    public uint Population => (uint) _humons.Count;

    private int humonDeathCount = 0;
    public int HumonDeathCount => humonDeathCount;
    
    public event Action<int> OnHumonDeathCountChanged;

    public uint PopulationCapacity()
    {
        uint capacity = 0;

        foreach (var residence in _residences)
        {
            capacity += residence.ResidentCapacity;
        }

        return capacity;
    }

    private int _cash;
    private Bounds _bounds;

    [SerializeField] private Humon _humonPrefab;
    [SerializeField] private Building.Residence _residencePrefab;

    private List<Humon> _humons = new ();
    private List<Building.Residence> _residences = new ();

    private void FixedUpdate()
    {
        _humons.RemoveAll((humon) => humon == null);

        if (Population >= PopulationCapacity())
        {
            Debug.Log($"{Population}/{PopulationCapacity()}");
            SpawnResidence();
        }
    }

    private void Awake()
    {
        var nav = FindFirstObjectByType<
                Unity.AI.Navigation.NavMeshSurface>();
        var bounds = nav.navMeshData.sourceBounds;

        _bounds = new Bounds(bounds.center, bounds.size);
    }

    public Humon SpawnHumon(Vector3 pos)
    {
        var humon = Instantiate(_humonPrefab, pos,
                Quaternion.identity);
        _humons.Add(humon);
        return humon;
    }

    public void IncreaseDeathCount()
    {
        humonDeathCount++;
        OnHumonDeathCountChanged?.Invoke(humonDeathCount);
    }
    public void DestroyHumon(Humon humon)
    {
        _humons.Remove(humon);
        Destroy(humon);
    }

    public void AddCash(int amount)
    {
        _cash += amount;
        OnCashChanged?.Invoke(_cash);
    }
    public int GetCash()
    {
        return _cash;
    }
    public void SetCash(int amount)
    {
        _cash = amount;
        OnCashChanged?.Invoke(_cash);
    }
    public bool CanDeductCash(int amount)
    {
        if (_cash >= amount)
            return true;
        return false;
    }

    private void SpawnResidence()
    {
        var residence = Instantiate(_residencePrefab,
                new Vector3(0, 10000, 0),
                new Quaternion(0, Random.value, 0, 1));
        residence.Instantiate();

        var pos = TryPlaceObject(residence.Collider);

        if (pos == null)
        {
            Debug.LogWarning("Found no place to spawn residence");
            Destroy(residence.gameObject);
            return;
        }

        residence.gameObject.transform.position = pos.Value;
        _residences.Add(residence);
    }

    private Vector3? TryPlaceObject(Collider col)
    {
        var box = col as BoxCollider;
        Assert.IsNotNull(box);
        var half = Vector3.Scale(
                box.size * 0.5f, box.transform.lossyScale);

        // make sure the object is not placed too close
        var buffer = 2f;
        half.x += buffer;
        half.z += buffer;

        Assert.IsTrue(box.size.x == box.size.z);
        var min = new Vector2(
                _bounds.min.x + MathF.Sqrt(2) * half.x,
                _bounds.min.z + MathF.Sqrt(2) * half.z);
        var max = new Vector2(
                _bounds.max.x - MathF.Sqrt(2) * half.x,
                _bounds.max.z - MathF.Sqrt(2) * half.z);

        for (var tries = 0; tries < 100; ++tries)
        {
            var pos = new Vector3(
                    Random.Range(min.x, max.x),
                    0,
                    Random.Range(min.y, max.y));

            var cols = Physics.OverlapBox(pos, half,
                    col.transform.rotation,
                    LayerMask.GetMask("Building", "NPC"));

            if (cols.Length == 0)
            {
                return pos;
            }
        }

        return null;
    }
} 
