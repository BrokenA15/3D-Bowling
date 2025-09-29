using UnityEngine;
using UnityEngine.InputSystem; // <- Nuevo sistema

public class BowlingBallController : MonoBehaviour
{
    public float initialSpeed = 15f;
    public float sideForce = 5f;
    public float spinTorque = 10f;
    public float frictionForce = 2f;
    public float minVelocityThreshold = 0.1f;

    private Rigidbody rb;
    private bool hasBeenLaunched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
    }

    void Update()
    {
        // Nuevo sistema: se consulta el mouse así
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !hasBeenLaunched)
        {
            LaunchBall();
        }
    }

    void LaunchBall()
    {
        rb.AddForce(transform.forward * initialSpeed, ForceMode.Impulse);
        rb.AddForce(transform.right * sideForce, ForceMode.Impulse);
        rb.AddTorque(transform.up * spinTorque, ForceMode.Impulse);
        hasBeenLaunched = true;
    }

    void FixedUpdate()
    {
        if (hasBeenLaunched)
        {
            float currentSpeed = rb.linearVelocity.magnitude;

            if (currentSpeed > minVelocityThreshold)
            {
                Vector3 frictionDirection = -rb.linearVelocity.normalized;
                rb.AddForce(frictionDirection * frictionForce, ForceMode.Acceleration);
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                hasBeenLaunched = false;
            }
        }
    }
}