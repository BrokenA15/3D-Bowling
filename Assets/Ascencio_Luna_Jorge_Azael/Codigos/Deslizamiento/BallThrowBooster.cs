using UnityEngine;

public class BallThrowBooster : MonoBehaviour
{
    [Header("Potencia extra al soltar")]
    public float extraForce = 5f;
    public float extraUpward = 2f;

    private void OnTriggerExit(Collider other)
    {
        // Detecta si sali� un objeto con tag "Bola"
        if (other.CompareTag("Bola"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Toma la velocidad actual
                Vector3 currentVelocity = rb.linearVelocity;

                // Aumenta fuerza hacia adelante (direcci�n del controlador)
                Vector3 boostedVelocity = currentVelocity * extraForce;

                // A�ade fuerza hacia arriba para evitar que caiga r�pido
                boostedVelocity += Vector3.up * extraUpward;

                // Aplica la nueva velocidad
                rb.linearVelocity = boostedVelocity;

                Debug.Log("Bola impulsada con boost: " + boostedVelocity);
            }
        }
    }
}
