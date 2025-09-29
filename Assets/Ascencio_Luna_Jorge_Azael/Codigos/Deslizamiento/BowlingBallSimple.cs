using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BowlingBallSimple : MonoBehaviour
{
    public float radius = 0.1085f; // metros
    public float mass = 7.0f;      // kg (ajustable)
    public float muK = 0.05f;      // fricci�n cin�tica (ajusta)
    public float rollingResistance = 0.02f; // torque de rodadura (ajusta)
    public float slipThreshold = 0.02f; // cuando considerar rodamiento (m/s)

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.inertiaTensor = Vector3.one * (2f / 5f) * mass * radius * radius;
        rb.angularDamping = 0f; // controlamos nosotros
    }

    void FixedUpdate()
    {
        // Suponemos superficie horizontal, normal = +Y
        Vector3 normal = Vector3.up;
        Vector3 contactPoint = transform.position - normal * radius;
        // velocidad del punto de contacto debida a traslaci�n:
        Vector3 v_cm = rb.linearVelocity;
        // velocidad tangencial del punto por rotaci�n:
        Vector3 v_t = Vector3.Cross(rb.angularVelocity, -normal * radius);
        // velocidad de deslizamiento:
        Vector3 v_slip = v_cm + v_t;

        // componente tangencial (proyectar sobre plano):
        Vector3 v_slip_tangent = v_slip - Vector3.Dot(v_slip, normal) * normal;
        float slipSpeed = v_slip_tangent.magnitude;

        float N = mass * Physics.gravity.magnitude; // aproximaci�n

        if (slipSpeed > 0.0001f)
        {
            // Fuerza de fricci�n cin�tica:
            Vector3 f_friction = -v_slip_tangent.normalized * (muK * N);
            // Aplicar en el centro (se puede aplicar en contacto para torque):
            rb.AddForce(f_friction * Time.fixedDeltaTime, ForceMode.Acceleration); // usa Acceleration para independencia de masa

            // Torque por fricci�n (por simplificar asumimos r perpendicular):
            Vector3 torque = Vector3.Cross(-normal * radius, f_friction);
            rb.AddTorque(torque * Time.fixedDeltaTime / (1.0f), ForceMode.Acceleration);
        }

        // Rolling resistance: peque�o torque en sentido opuesto a omega
        if (rb.angularVelocity.sqrMagnitude > 0.000001f)
        {
            Vector3 rollTorque = -rb.angularVelocity.normalized * rollingResistance * rb.angularVelocity.magnitude;
            rb.AddTorque(rollTorque * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Forzar transici�n a rodamiento si el deslizamiento es muy peque�o
        if (slipSpeed < slipThreshold)
        {
            // fijar angular velocity para cumplir v = w x r (sobre el plano XZ)
            Vector3 vHoriz = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            // w para rodar sin deslizar: |w| = |v| / R, direcci�n: eje perpendicular
            if (vHoriz.magnitude > 0.0001f)
            {
                Vector3 wDir = Vector3.Cross(Vector3.up, vHoriz.normalized).normalized;
                Vector3 targetW = wDir * (vHoriz.magnitude / radius);
                // Suaviza la asignaci�n para evitar saltos
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, targetW, 0.2f);
                // Elimina componente vertical peque�a
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0f, rb.angularVelocity.z);
            }
            else
            {
                // detener rotaci�n horizontal si la bola casi no se mueve
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, 0.2f);
            }
        }
    }
}