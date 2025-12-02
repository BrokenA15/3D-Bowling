using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Damage Settings")]
    public float damageCooldown = 0.3f; 
    private bool canTakeDamage = true;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage) return;

        currentHealth -= amount;
        Debug.Log("Jugador recibió daño. Vida actual: " + currentHealth);

        StartCoroutine(DamageCooldown());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Aquí puedes poner animación, respawn, game over, etc.
    }
}
