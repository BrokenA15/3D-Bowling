using UnityEngine;

public class BallGlideVR : MonoBehaviour
{
    [Header("Deslizamiento")]
    public float airDrag = 0.02f;          // poca resistencia en el aire
    public float groundDrag = 0.1f;        // poco frenado en el suelo
    public float glideBoost = 1.2f;        // ligero impulso para mantener velocidad
    public float maxSpeed = 12f;           // velocidad m�xima permitida

    private Rigidbody rb;
    private bool wasReleased = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Llama este evento desde "On Release"
    public void OnBallReleased()
    {
        wasReleased = true;

        // Reducir drag para que no se frene en seco
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
    }

    void FixedUpdate()
    {
        if (!wasReleased) return;

        // Detectar si est� en el suelo
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        if (isGrounded)
        {
            rb.linearDamping = groundDrag;       // poca fricci�n en suelo -> desliza
        }
        else
        {
            rb.linearDamping = airDrag;          // casi nada de drag -> sigue volando
        }

        // Mantener una sensaci�n de deslizamiento
        Vector3 boosted = rb.linearVelocity * glideBoost;

        if (boosted.magnitude < maxSpeed)
        {
            rb.linearVelocity = boosted;
        }
    }
}
