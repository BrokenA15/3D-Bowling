using UnityEngine;

public class RegresoBola : MonoBehaviour
{
    [Tooltip("Tag del objeto que se impulsará")]
    public string targetTag = "Bola";

    [Tooltip("Fuerza de impulso constante hacia adelante (eje Z del trigger)")]
    public float fuerzaImpulso = 10f;

    private void OnTriggerStay(Collider other)
    {
        // Detecta si el objeto dentro del trigger tiene el tag asignado
        if (other.CompareTag(targetTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Dirección hacia adelante del trigger, sin componente vertical
                Vector3 direccion = transform.right;
                direccion.y = 0f; // evitar empuje hacia arriba o abajo
                direccion.Normalize();

                // Aplicar fuerza continua mientras la bola esté en contacto
                rb.AddForce(direccion * fuerzaImpulso * Time.deltaTime, ForceMode.VelocityChange);
            }
        }
    }
}
