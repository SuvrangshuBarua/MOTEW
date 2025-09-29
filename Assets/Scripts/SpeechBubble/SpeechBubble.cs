using System;
using System.Collections;
using System.Drawing;
using GrimTools.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

[System.Serializable]
public class SpeechBubbleData
{
    public string text;
    public Sprite emoji;
    public float duration = 3f;
    public bool autoHide = true;
    
}
public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private Image m_emojiImage;
    [SerializeField] private TextMeshProUGUI m_textField;
    [SerializeField] private CanvasGroup m_canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Positioning")]
    [SerializeField] private Vector3 offset = Vector3.up * 2f;
    [SerializeField] private bool followTarget = true;
    [SerializeField] private bool billboardToCamera = true;
    [SerializeField] private float positionSmoothTime = 0.9f;
    
    private RectTransform m_rectTransform;
    private Vector3 m_originalScale;
    private Coroutine m_displayRoutine;
    private Camera m_camera;
    private Transform m_target;
    private Vector3 m_currentVelocity; // For smooth damping
    

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_originalScale = m_rectTransform.localScale;
        
        if(m_canvasGroup== null)
            m_canvasGroup = GetComponent<CanvasGroup>();
        if(m_canvasGroup == null)
            m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        
        
        m_canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        m_camera = Camera.main;
        
    }
    
    private void LateUpdate()
    {
        if (followTarget && m_target != null)
        {
            UpdatePosition();
        }
        if (billboardToCamera && m_camera != null)
        {
            transform.rotation = m_camera.transform.rotation;
        }
    }

    public void ShowSpeechBubble(Transform target, SpeechBubbleData data)
    {
        m_target = target;
        if (m_displayRoutine != null)
            StopCoroutine(m_displayRoutine);
        
        SetContent(data);   
        
        m_displayRoutine = StartCoroutine(DisplayRoutine(data));
    }


    public void ShowSpeechBubble(Transform target, string text, Sprite emoji = null, float duration = 3f, bool autoHide = true)
    {
        var data = new SpeechBubbleData
        {
            text = text,
            emoji = emoji,
            duration = duration,
            autoHide = autoHide
        };
        
        ShowSpeechBubble(target, data);
    }

    public void Hide()
    {
        if (m_displayRoutine != null)
        {
            StopCoroutine(m_displayRoutine);
            m_displayRoutine = null;
        }

        StartCoroutine(HideRoutine());
    }
    private IEnumerator HideRoutine()
    {
        yield return StartCoroutine(FadeOutRoutine());
    }
    
    private void SetContent(SpeechBubbleData data)
    {
        // Set text
        if (m_textField != null)
        {
            m_textField.text = data.text;
        }
        
        // Set emoji
        if (m_emojiImage != null)
        {
            if (data.emoji != null)
            {
                m_emojiImage.sprite = data.emoji;
                m_emojiImage.gameObject.SetActive(true);
            }
            else
            {
                m_emojiImage.gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePosition()
    {
       if(m_target == null) return;
            
       Vector3 targetWorldPos = m_target.position + offset;
        
       if (positionSmoothTime > 0)
       {
           transform.position = Vector3.SmoothDamp(
               transform.position,
               targetWorldPos,
               ref m_currentVelocity,
               positionSmoothTime
           );
       }
       else
       {
           transform.position = targetWorldPos;
       }
    }

    private IEnumerator DisplayRoutine(SpeechBubbleData data)
    {

        if (m_target != null)
        {
            transform.position = m_target.position + offset;
            m_currentVelocity = Vector3.zero;
        }
        
        yield return StartCoroutine(FadeInRoutine(scaleCurve));
        
        if (data.autoHide && data.duration > 0)
        {
            yield return new WaitForSeconds(data.duration);
            yield return StartCoroutine(FadeOutRoutine());
        }

        
        
    }
    
    private IEnumerator FadeInRoutine(AnimationCurve curve)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            float t = elapsedTime / fadeInDuration;
            float curveValue = scaleCurve.Evaluate(t);
            
            m_canvasGroup.alpha = t;
            transform.localScale = m_originalScale * curveValue;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        m_canvasGroup.alpha = 1f;
        transform.localScale = m_originalScale;
    }
    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            float t = 1f - (elapsedTime / fadeOutDuration);
            float curveValue = scaleCurve.Evaluate(t);
            
            m_canvasGroup.alpha = t;
            transform.localScale = m_originalScale * curveValue;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        m_canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;
        SpeechBubbleManager.Instance.HideSpeechBubble(m_target);
        ObjectPool.Instance.ReturnToPool(this.gameObject);
        m_displayRoutine = null;
    }
    
}
