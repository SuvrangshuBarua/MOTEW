// stolen from the world

using UnityEngine;
using UnityEngine.EventSystems;

public class Expand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float moveDistance = 500f;
    public float moveSpeed = 5f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private bool isHovered = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        targetPosition = originalPosition;
    }

    private void Update()
    {
        // Smoothly move toward target position
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetPosition = originalPosition + Vector2.up * moveDistance;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetPosition = originalPosition;
    }
}

