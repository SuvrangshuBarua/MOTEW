using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace Building {

public enum State
{
    InConstruction,
    Constructed,
    InDestruction,
    Destructed,
}

public class BaseBuilding : MonoBehaviour
{
    [SerializeField]
    protected uint _visitorCapacity;

    protected List<GameObject> _visitors = new ();

    [SerializeField]
    protected Building.State _state = Building.State.Destructed;


    Coroutine _task = null;

    [SerializeField]
    public GameObject BuildingPrefab = null;
    private GameObject _building = null;
    private List<GameObject> _parts = new ();

    [SerializeField]
    private GameObject _particle; 

    [SerializeField]
    private uint _constructionRate;


    public List<GameObject> Visitors()
    {
        return _visitors;
    }

    public bool Visit(GameObject visitor, float duration)
    {
        if (_visitors.Count == _visitorCapacity)
            return false;

        visitor.SetActive(false);
        _visitors.Add(visitor);
        StartCoroutine(Leave(visitor, duration));

        return true;
    }

    private IEnumerator Leave(GameObject visitor, float duration)
    {
        yield return new WaitForSeconds(duration);

        visitor.SetActive(true);
        _visitors.Remove(visitor);
    }

    public void Construct()
    {
        CancelTask();
        _task = StartCoroutine(DoConstruct());
    }

    public void Destruct()
    {
        CancelTask();
        _task = StartCoroutine(DoDestruct());
    }

    private IEnumerator DoConstruct()
    {
        Assert.AreNotEqual(_state, Building.State.InConstruction);

        _state = Building.State.InConstruction;

        _building = Instantiate(BuildingPrefab, transform);
        foreach (Transform part in _building.transform)
        {
            part.gameObject.SetActive(false);
            _parts.Add(part.gameObject);
        }

        foreach (var part in _parts)
        {
            part.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }

        _state = Building.State.Constructed;
    }

    private IEnumerator DoDestruct()
    {
        Assert.AreNotEqual(_state, Building.State.InDestruction);
        _state = Building.State.InDestruction;

        // EXPLODE.

        foreach (var part in _parts)
        {
            if (part.TryGetComponent<Rigidbody>(out var body))
            {
                body.AddExplosionForce(5000, transform.position, 10);
                body.useGravity = true;
                Instantiate(_particle, part.transform.position, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(1);

        foreach (var part in _parts)
        {
            Instantiate(_particle, part.transform.position, Quaternion.identity);
            part.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        _parts.Clear();
        Destroy(_building);

        _state = Building.State.Destructed;
    }

    private void CancelTask()
    {
        if (_task != null)
        {
            StopCoroutine(_task);
        }
    }
}

} // namespace Building

