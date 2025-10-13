using UnityEngine;

public class PinFallSound : MonoBehaviour
{
<<<<<<< Updated upstream
    [Tooltip("Altura mínima o umbral para considerar que el pino está caído")]
    public float fallYThreshold = 0f;
=======
    [Tooltip("Rotación inicial del prefab (usualmente -90 en X)")]
    public Vector3 initialRotation = new Vector3(-90f, 0f, 0f);
>>>>>>> Stashed changes

    [Tooltip("Diferencia de ángulo para considerar que cayó (grados)")]
    public float rotationChangeThreshold = 30f;

    [Tooltip("Distancia mínima de movimiento en Y o Z para considerar que cayó")]
    public float movementThreshold = 0.1f;

    [Tooltip("Clip de sonido al caer")]
    public AudioClip fallSound;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private Vector3 initialPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        // Guardamos la rotación y posición iniciales
        initialRotation = transform.eulerAngles;
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calcular cambio de rotación respecto a la inicial
        float rotationDifference = Quaternion.Angle(Quaternion.Euler(initialRotation), transform.rotation);

        // Calcular movimiento en Y o Z
        float movement = Mathf.Abs(transform.position.y - initialPosition.y) +
                         Mathf.Abs(transform.position.z - initialPosition.z);

        bool hasFallen = (rotationDifference > rotationChangeThreshold) || (movement > movementThreshold);

        // Si cayó y aún no ha sonado
        if (hasFallen && !hasPlayed && fallSound != null)
        {
            audioSource.PlayOneShot(fallSound);
            hasPlayed = true;
        }

        // Si vuelve a su posición original (opcional, reiniciar)
        if (!hasFallen && hasPlayed)
        {
            hasPlayed = false;
        }
    }
}
