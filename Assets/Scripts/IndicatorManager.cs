using UnityEngine;
using GrimTools.Runtime.Core;

// Really this is just a factory, but IndicatorFactory is used
// by something else in global namespace lol
public class IndicatorManager : PersistantMonoSingleton<IndicatorManager>
{
    [SerializeField] private GameObject _prefab;

    protected override void Awake()
    {
        base.Awake();
    }

    public void New(Transform at, string what)
    {
        GameObject indicator = Instantiate(_prefab, at.position, Quaternion.Euler(transform.eulerAngles.x, 0, 0));

        var text = indicator.GetComponent<TextMesh>();
        text.text = what;
        text.fontSize = 20;
    }

    public void NewOffset(Transform at, string what, Vector3 off)
    {
        GameObject indicator = Instantiate(_prefab, at.position, Quaternion.Euler(transform.eulerAngles.x, 0, 0));

        indicator.transform.localPosition = off;

        var text = indicator.GetComponent<TextMesh>();
        text.text = what;
        text.fontSize = 20;
    }
}

