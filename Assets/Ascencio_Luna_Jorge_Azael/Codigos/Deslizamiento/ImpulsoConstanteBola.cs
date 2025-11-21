using UnityEngine;

public class ImpulsoConstanteBola : MonoBehaviour
{
    [Tooltip("Tag del objeto que se impulsar�")]
    public string targetTag = "Bola";

    [Tooltip("Multiplicador de impulso proporcional a la velocidad actual")]
    public float multiplicadorImpulso = 1.2f;

    [Tooltip("Impulso mínimo si la bola casi no se mueve")]
    public float impulsoMinimo = 2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float velocidadActual = rb.linearVelocity.magnitude;

                if (velocidadActual > 0.1f)
                {
                    
                    rb.linearVelocity *= multiplicadorImpulso;
                }
                else
                {
                    
                    Vector3 direccion = transform.forward;
                    direccion.y = 0f;
                    direccion.Normalize();

                    rb.AddForce(direccion * impulsoMinimo, ForceMode.VelocityChange);
                }
            }
        }
    }
}
