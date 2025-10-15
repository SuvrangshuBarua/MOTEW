using Microlight.MicroBar;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] MicroBar healthBar;

    private void LateUpdate()
    {
        // Make health bar Y and Z axis 0 so that it always faces front
        if (healthBar != null)
        {
            // Keep only X rotation, reset Y and Z to 0 in world space
            healthBar.transform.rotation = Quaternion.Euler(healthBar.transform.eulerAngles.x, 0, 0);
        }
    }

    public void Initialize(int maxHealth)
    {
        // Initialize the health bar with the maximum health
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }
    }

    public void UpdateCurrentHealth(int currentHealth)
    {
        // Update the health bar's fill amount based on the current health
        if (healthBar != null)
        {
            healthBar.UpdateBar(currentHealth);
        }
    }

    public void Destroy()
    {
        // Destroy the health bar game object
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
            healthBar = null;
        }
    }
}
