using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;   

public class StatusBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float hideDelay = 2f;
    [SerializeField] private float valueLerpDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.2f;
    
    [Header("Positioning")]
    [SerializeField] private Vector3 offset = Vector3.up * 2f;
    [SerializeField] private bool followTarget = true;
    [SerializeField] private bool billboardToCamera = true;
    [SerializeField] private float positionSmoothTime = 0.9f;

    private float m_currentValue;
    private float m_maxValue = 100f;
    private float m_targetValue;
    private float m_hideTimer;
    private float m_valueChangeStartTime;
    private CanvasGroup m_canvasGroup;
    private bool m_isTransitioning;

    private Camera m_camera;
    private Transform m_target;
    private Vector3 m_currentVelocity;
    private Coroutine m_displayRoutine;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_canvasGroup.alpha = 0f;
        
        m_camera = Camera.main;
    }

    public void SetMaxValue(float maxValue)
    {
        m_maxValue = maxValue;
        slider.maxValue = m_maxValue;
    }

    public void UpdateValue(float value)
    {
        m_targetValue = value;
        m_valueChangeStartTime = Time.time;
        m_hideTimer = hideDelay;
        m_isTransitioning = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void ShowStatusBar(Transform target, float value)
    {
        m_target = target;
        m_targetValue = value;
        m_valueChangeStartTime = Time.time;
        
        if(m_displayRoutine != null)
            StopCoroutine(m_displayRoutine);
        m_displayRoutine = StartCoroutine(DisplayStatusBar(fadeDuration));
    }

    private IEnumerator DisplayStatusBar(float fadeDuration)
    {
        if (m_target != null)
        {
            transform.position = m_target.position + offset;
            m_currentVelocity = Vector3.zero;
        }
        yield return StartCoroutine(FadeIn());
        
        if (m_target != null)
        {
            UpdatePosition();
        }
    }

    private void Update()
    {
        if (m_isTransitioning)
        {
            float timeSinceStart = Time.time - m_valueChangeStartTime;
            float progress = timeSinceStart / valueLerpDuration;

            if (progress < 1f)
            {
                m_currentValue = Mathf.Lerp(slider.value, m_targetValue, progress);
                slider.value = m_currentValue;
            }
            else
            {
                m_currentValue = m_targetValue;
                slider.value = m_currentValue;
                m_isTransitioning = false;
            }
        }

       
    }

    private void LateUpdate()
    {
        if(followTarget && m_target != null)
        {
            UpdatePosition();
        }
        if(billboardToCamera && m_camera != null)
        {
            transform.LookAt(m_camera.transform);
        }
        // transform.LookAt( m_target.position)
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

    private IEnumerator FadeIn()
    {
        float startTime = Time.time;
        float startAlpha = m_canvasGroup.alpha;

        while (Time.time - startTime < fadeDuration)
        {
            float progress = (Time.time - startTime) / fadeDuration;
            m_canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
            yield return null;
        }

        m_canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float startTime = Time.time;
        float startAlpha = m_canvasGroup.alpha;

        while (Time.time - startTime < fadeDuration)
        {
            float progress = (Time.time - startTime) / fadeDuration;
            m_canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            yield return null;
        }

        m_canvasGroup.alpha = 0f;
    }
}
