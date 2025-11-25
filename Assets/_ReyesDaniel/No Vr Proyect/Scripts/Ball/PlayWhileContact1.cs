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


 
}