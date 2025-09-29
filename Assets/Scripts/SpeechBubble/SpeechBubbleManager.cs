using System;
using System.Collections.Generic;
using GrimTools.Runtime;
using GrimTools.Runtime.Core;
using UnityEngine;

public class SpeechBubbleManager : PersistantMonoSingleton<SpeechBubbleManager>
{
    [SerializeField] private GameObject m_speechBubblePrefab;
    [SerializeField] private Canvas m_canvas;
    private Dictionary<Transform, SpeechBubble> m_activeSpeechBubbles = new ();

    protected override void Awake()
    {
        base.Awake();
        
        if(m_canvas == null)
            m_canvas = FindFirstObjectByType<Canvas>();
    }

    public SpeechBubble CreateSpeechBubble(Transform target, string text, Sprite emoji = null, float duration = 3f)
    {
        if(m_activeSpeechBubbles.ContainsKey(target))
            return m_activeSpeechBubbles[target];
        
        var speechBubble = ObjectPool.Instance.GetObject(m_speechBubblePrefab);
        speechBubble.transform.SetParent(m_canvas.transform, false);
        var rectTransform = speechBubble.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        
        var bubble = speechBubble.GetComponent<SpeechBubble>();
        if (bubble != null)
        {
            bubble.ShowSpeechBubble(target, text, emoji, duration);
        }
        m_activeSpeechBubbles.Add(target, bubble);
        return bubble;
    }
    public SpeechBubble CreateSpeechBubble(Transform target, SpeechBubbleData data)
    {
        if(m_activeSpeechBubbles.ContainsKey(target))
            return m_activeSpeechBubbles[target];
        
        var speechBubble = ObjectPool.Instance.GetObject(m_speechBubblePrefab);
        speechBubble.transform.SetParent(m_canvas.transform, false);
        var rectTransform = speechBubble.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        var bubble = speechBubble.GetComponent<SpeechBubble>();
        if (bubble != null)
        {
            bubble.ShowSpeechBubble(target, data);
        }
        m_activeSpeechBubbles.Add(target, bubble);
        return bubble;
    }
    public void HideSpeechBubble(Transform target)
    {
        if (m_activeSpeechBubbles.TryGetValue(target, out var bubble))
        {
            bubble.Hide();
            m_activeSpeechBubbles.Remove(target);
        }
    }
    public void HideAllSpeechBubbles()
    {
        foreach (var bubble in m_activeSpeechBubbles.Values)
        {
            bubble.Hide();
        }
        m_activeSpeechBubbles.Clear();
    }

    public bool RemoveItselfAsActive(Transform target)
    {
        return m_activeSpeechBubbles.Remove(target);
    }
    
}
