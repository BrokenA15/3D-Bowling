using UnityEngine;

public class pinosDetectar : MonoBehaviour
{
    private bool isCounted = false;
    private Rigidbody rb;

    [Header("Detecci�n")]
    [SerializeField] private float fallenThreshold = 20f;   // �ngulo para considerar inclinado
    [SerializeField] private float stillnessThreshold = 0.1f; // velocidad m�nima
    [SerializeField] private float raycastDistance = 0.2f;   // distancia para tocar el suelo
    [SerializeField] private LayerMask floorMask;            // capa del suelo

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Comprueba si el pino est� ca�do (inclinaci�n + tocando el piso con raycast).
    /// </summary>
    public bool IsFallen()
    {
        // 1. Calcular inclinaci�n
        float tiltX = Mathf.Abs(transform.eulerAngles.x);
        float tiltZ = Mathf.Abs(transform.eulerAngles.z);

        if (tiltX > 180) tiltX = 360 - tiltX;
        if (tiltZ > 180) tiltZ = 360 - tiltZ;

        bool tilted = (tiltX > fallenThreshold || tiltZ > fallenThreshold);

        // 2. Comprobar si est� tocando el suelo
        bool touchingFloor = Physics.Raycast(transform.position, Vector3.down, raycastDistance, floorMask);

        // 3. Comprobar que ya no se est� moviendo mucho
        bool almostStill = rb.linearVelocity.magnitude < stillnessThreshold;

        return tilted && touchingFloor && almostStill;
    }

    /// <summary>
    /// Intenta contar este pino (s�lo la primera vez que cae).
    /// </summary>
    public bool TryCount()
    {
        if (!isCounted && IsFallen())
        {
            isCounted = true;
            return true;
        }
        return false;
    }

    public void ResetPin()
    {
        isCounted = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity; // vuelve recto
    }
}
