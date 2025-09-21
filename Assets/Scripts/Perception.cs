using UnityEngine;
using System;
using System.Collections.Generic;

public class Perception : MonoBehaviour
{
    private GameObject _triggerObject;
    private SphereCollider _trigger;
    private List<Perceiver> _perceivers = new ();
    private float _disabledUntil = 0f;

    public enum Type
    {
        // Can only perceive one object at a time
        Single,
        // Can perceive multiple objects at a time
        Multiple,
    }

    //
    // Returns the perceiver used to unsubscribe the callback.
    //
    // Example 1.
    // Perception.Subscribe(10, 5, Perception.Type.Multiple, LayerMask.GetMask("NPC"), OnPerceive);
    //
    // Every 5 seconds, call OnPerceive for all objects of type "NPC" within 10 range.
    //
    //
    // Example 2.
    // Perception.Subscribe(5, 1, Perception.Type.Single, LayerMask.GetMask("NPC"), OnPerceive);
    //
    // Every second, call OnPerceive for at most 1 object of type "NPC" within 5 range.
    //
    public Perceiver Subscribe(float radius, float timeoutSeconds, Perception.Type type,
        LayerMask layers, Action<Collider> onPerceive)
    {
        var perceiver = new Perceiver(radius, timeoutSeconds, type, layers, onPerceive);
        _perceivers.Add(perceiver);

        if (_trigger == null)
        {
            // Make sure the collider is not directly attached to the
            // gameObject, otherwise it will get put in the same layer
            // and interfere with raycasting etc.
            _triggerObject = new GameObject(gameObject.name + "Perception");
            _triggerObject.layer = LayerMask.NameToLayer("Perception");
            _triggerObject.transform.parent = gameObject.transform;
            _triggerObject.transform.localPosition = Vector3.zero;

            _trigger = _triggerObject.AddComponent<SphereCollider>();
            _trigger.isTrigger = true;
        }

        _trigger.radius = Math.Max(_trigger.radius, radius);
        _trigger.enabled = true;

        return perceiver;
    }

    public void Unsubscribe(Perceiver perceiver)
    {
        _perceivers.Remove(perceiver);
    }

    void FixedUpdate()
    {
        if (Time.time >= _disabledUntil)
        {
            // at least one perceiver is ready
            _trigger.enabled = true;
        }

        foreach (var perceiver in _perceivers)
        {
            perceiver.OnFixedUpdate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        float dist = Vector3.Distance(_trigger.transform.position,
                other.ClosestPoint(_trigger.transform.position));

        foreach (var perceiver in _perceivers)
        {
            perceiver.TryPerceive(other, dist);
        }
    }

    void OnTriggerStay(Collider other)
    {
        float minTimeout = 0f;
        float dist = Vector3.Distance(_trigger.transform.position,
                other.ClosestPoint(_trigger.transform.position));

        foreach (var perceiver in _perceivers)
        {
            float remaining = perceiver.RemainingTimeout();
            if (remaining > 0f)
            {
                minTimeout = Math.Min(minTimeout, remaining);
                continue;
            }

            perceiver.TryPerceive(other, dist);

            minTimeout = Math.Min(minTimeout, perceiver.RemainingTimeout());
        }

        if (minTimeout > 0f)
        {
            // all perceivers are timed out, we can suspend
            _trigger.enabled = false;
            _disabledUntil = Time.time + minTimeout;
        }
    }    

    public class Perceiver
    {
        private float _radius;
        private float _timeout;
        private float _deferredTimeout;
        private float _timestamp;
        private Perception.Type _type;
        private LayerMask _layers;
        private System.Action<Collider> _onPerceive;

        internal Perceiver(float radius, float timeout, Perception.Type type,
            LayerMask layers, Action<Collider> onPerceive)
        {
            _radius = radius;
            _timeout = timeout;
            _type = type;
            _onPerceive += onPerceive;
            _timestamp = 0;
            _layers = layers;
        }

        internal void OnFixedUpdate()
        {
            if (_type == Perception.Type.Multiple)
            {
                _timestamp = _deferredTimeout;
            }
        }

        internal float RemainingTimeout()
        {
            float remaining = _timeout - (Time.time - _timestamp);
            return Math.Max(0f, remaining);
        }

        internal void TryPerceive(Collider other, float dist)
        {
            if (dist > _radius)
            {
                return;
            }

            if ((_layers & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }
            
            _onPerceive.Invoke(other);

            if (_type == Perception.Type.Single)
            {
                _timestamp = Time.time;
            }
            else
            {
                _deferredTimeout = Time.time;
            }
        }
    }
}

