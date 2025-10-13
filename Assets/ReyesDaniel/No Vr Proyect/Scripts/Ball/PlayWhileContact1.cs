using UnityEngine;

public class PlayWhileContact1 : MonoBehaviour
{
    [Tooltip("Tag a detectar (por defecto: Bola)")]
    public string targetTag = "Bola";

    [Tooltip("Clip que se reproducirá")]
    public AudioClip clip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true; // importante para que suene mientras esté en contacto
    }

    // ------- Trigger 3D -------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            audioSource.Stop();
        }
    }

    // ------- Trigger 2D -------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            audioSource.Stop();
        }
    }
}