using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [Header(" Sonido a reproducir")]
    public AudioClip sonido;        

    [Header(" Configuración de audio")]
    public AudioSource audioSource;  

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bola"))
        {
            if (sonido != null)
            {
                audioSource.PlayOneShot(sonido);
            }
            else
            {
                Debug.LogWarning("No se asignó ningún sonido en el inspector.");
            }
        }
    }
}
