using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class Fire : MonoBehaviour
{
    public int Damage;
    public float Radius;
    public float Duration;

    void Start()
    {
        // check static objects
        Collider[] hits = Physics.OverlapSphere(
                transform.position, Radius,
                LayerMask.GetMask("Building"));

        foreach (var hit in hits)
        {
            hit.GetComponentInParent<Building.BaseBuilding>()
                    .SetOnFire(this);
        }

        // fade out fire
        StartCoroutine(StopVfx());
    }

    void FixedUpdate()
    {
        // check dynamic objects
        Collider[] hits = Physics.OverlapSphere(
                transform.position, Radius,
                LayerMask.GetMask("NPC"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Humon>(out var humon))
            {
                humon.SetOnFire(this);
            }
        }
    }

    IEnumerator StopVfx()
    {
        Assert.IsTrue(Duration > 2f);

        yield return new WaitForSeconds(Duration - 2f);
        var ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}

