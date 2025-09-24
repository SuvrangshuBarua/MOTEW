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

    protected List<Humon> _visitors = new ();

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

    public List<Humon> Visitors()
    {
        return _visitors;
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

        _building = Instantiate(BuildingPrefab);
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
        yield return null;
    }

    private IEnumerator DoDestruct()
    {
        Assert.AreNotEqual(_state, Building.State.InDestruction);
        _state = Building.State.InDestruction;

        // EXPLODE.

        foreach (var part in _parts)
        {
            Debug.Log("X");
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

        yield return null;
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

