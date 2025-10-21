using UnityEngine;
using System.Collections;

public class TreeAsset : MonoBehaviour
{
    [SerializeField]
    private GameObject _firePrefab;
    private GameObject _vfx;
    private bool _onFire = false;
    private float _duration = 5f;

    public void SetOnFire(Fire source)
    {
        if (!_onFire)
        {
            _onFire = true;

            _vfx = UnityEngine.Object.Instantiate(_firePrefab, transform);
            var lc = _vfx.transform.localScale;
            _vfx.transform.localScale = new Vector3(lc.x * 1.5f, lc.y * 2.5f, lc.z * 1.5f);
            Destroy(gameObject, _duration);
            StartCoroutine(StopVfx());
        }
    }

    IEnumerator StopVfx()
    {
        yield return new WaitForSeconds(_duration - 0.5f);

        var ps = _vfx.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}

