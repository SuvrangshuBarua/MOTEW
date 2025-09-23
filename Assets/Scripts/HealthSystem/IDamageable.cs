using UnityEngine;

public class ClickToDamage : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private int damagePerTap = 10;
    [SerializeField] private LayerMask damageableMask = ~0;

    private void Reset()
    {
        if (!cam) cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DoDamageAt(Input.mousePosition, "mouse");
            Debug.Log("DoDamageAt called"); // idk
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DoDamageAt(Input.GetTouch(0).position, "touch");
            Debug.Log("DoDamageAt called");
        }
    }

    private void DoDamageAt(Vector2 screenPos, object source)
    {
        if (!cam) return;
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, damageableMask))
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damagePerTap, source);
            }
        }
    }
}
