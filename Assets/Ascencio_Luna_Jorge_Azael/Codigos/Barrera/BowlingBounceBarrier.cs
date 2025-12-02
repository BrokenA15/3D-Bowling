using UnityEngine;

public class BowlingBounceBarrier : MonoBehaviour
{
    [Header("Rebote de la bola")]
    public float bounceForce = 1.5f;      // fuerza del rebote
    public float upwardBoost = 0.1f;      // peque�o impulso hacia arriba para naturalidad

    private void OnCollisionEnter(Collision collision)
    {
        // Solo rebotar si es la bola
        if (collision.collider.CompareTag("Bola"))
        {
            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
            if (rb == null) return;

            // Direcci�n contraria a la normal del choque
            Vector3 normal = collision.contacts[0].normal;

            // Rebote simulando boliche real
            Vector3 bounceDirection = Vector3.Reflect(rb.linearVelocity, normal);

            // Aplicar fuerza adicional
            bounceDirection *= bounceForce;

            // Peque�o impulso vertical
            bounceDirection += Vector3.up * upwardBoost;

            // Aplicar nueva velocidad
            rb.linearVelocity = bounceDirection;
        }
    }
}
