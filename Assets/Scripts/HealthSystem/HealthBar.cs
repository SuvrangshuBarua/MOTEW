using System;
using System.Collections;
using Microlight.MicroBar;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] MicroBar healthBar;
    private Coroutine fadeCoroutine; // Store reference to the coroutine

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
            healthBar.gameObject.SetActive(false); // Hide initially
        }
    }

    public void UpdateCurrentHealth(int currentHealth)
    {
        // Update the health bar's fill amount based on the current health
        if (healthBar != null)
        {
            // Stop the existing coroutine if it's running
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            healthBar.UpdateBar(currentHealth);
            fadeCoroutine = StartCoroutine(FadeInOut(5f)); // Show for 5 seconds
        }
    }

    private IEnumerator FadeInOut(float duration)
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true); // Show health bar
            yield return new WaitForSeconds(duration);
            healthBar.gameObject.SetActive(false); // Hide health bar
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
