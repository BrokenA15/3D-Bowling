using UnityEngine;

public class ImpulsoConstanteBola : MonoBehaviour
{
    [Tooltip("Tag del objeto que se impulsar�")]
    public string targetTag = "Bola";

    [Tooltip("Fuerza de impulso constante (intensidad)")]
    public float fuerzaImpulso = 10f;

    private void OnTriggerStay(Collider other)
    {
        // Detecta si el objeto dentro del trigger tiene el tag asignado
        if (other.CompareTag(targetTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Solo aplicar impulso si la bola tiene algo de movimiento
                if (rb.linearVelocity.magnitude > 0.1f)
                {
                    // Tomar la direcci�n actual del movimiento (normalizada)
                    Vector3 direccionMovimiento = rb.linearVelocity.normalized;

                    // Aplicar fuerza en la misma direcci�n de su movimiento actual
                    rb.AddForce(direccionMovimiento * fuerzaImpulso * Time.deltaTime, ForceMode.VelocityChange);
                }
                else
                {
                    // Si la bola est� casi detenida, usar la direcci�n del trigger como referencia
                    Vector3 direccion = transform.forward; // o transform.right, seg�n tu escena
                    direccion.y = 0f;
                    direccion.Normalize();

                    rb.AddForce(direccion * fuerzaImpulso * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }
    }
}
