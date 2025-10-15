using UnityEngine;

public interface IHealthBar
{
    void InitializeHealthBar(int maxHealth);
    void UpdateHealthBar(int currentHealth);
    void DestroyHealthBar();
}
