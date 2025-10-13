using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayWhileContact : MonoBehaviour
{
    [Tooltip("Tag a detectar (por defecto: Bola)")]
    public string targetTag = "Bola";

    [Tooltip("Clip que se reproducirá")]
    public AudioClip clip;

    [Tooltip("Velocidad de extinción del sonido (volumen por segundo)")]
    public float fadeSpeed = 0.5f;

    private AudioSource audioSource;
    private bool isInContact = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 1f;
    }

    void Update()
    {
        // Si está en contacto, bajar gradualmente el volumen
        if (isInContact && audioSource.isPlaying)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);

            // Si el volumen llega a 0, detener el audio
            if (audioSource.volume <= 0.01f)
                audioSource.Stop();
        }
    }

    // ------- Trigger 3D -------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = 1f;
            audioSource.Play();
            isInContact = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isInContact = false;
            audioSource.Stop();
            audioSource.volume = 1f;
        }
    }

    // ------- Trigger 2D -------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = 1f;
            audioSource.Play();
            isInContact = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isInContact = false;
            audioSource.Stop();
            audioSource.volume = 1f;
        }
    }
}
