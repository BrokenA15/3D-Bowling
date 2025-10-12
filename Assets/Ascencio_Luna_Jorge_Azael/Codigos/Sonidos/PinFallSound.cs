using UnityEngine;

public class PinFallSound : MonoBehaviour
{
    [Tooltip("Altura mínima o umbral para considerar que el pino está caído")]
    public float fallYThreshold = 0.1f;

    [Tooltip("Ángulo máximo de inclinación para considerar que sigue de pie")]
    public float uprightAngleThreshold = 15f;

    [Tooltip("Clip de sonido al caer")]
    public AudioClip fallSound;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Verificar si el pino está caído
        bool isFallen = false;

        // Opción 1: Detectar por altura (posición Y)
        if (transform.position.y <= fallYThreshold)
        {
            isFallen = true;
        }

        // Opción 2: Detectar por inclinación
        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);
        if (tiltAngle > uprightAngleThreshold)
        {
            isFallen = true;
        }

        // Si está caído y aún no ha sonado, reproducir sonido
        if (isFallen && !hasPlayed && fallSound != null)
        {
            audioSource.PlayOneShot(fallSound);
            hasPlayed = true;
        }

        // Si se levanta de nuevo (opcional: reiniciar el estado)
        if (!isFallen && hasPlayed)
        {
            hasPlayed = false;
        }
    }
}
