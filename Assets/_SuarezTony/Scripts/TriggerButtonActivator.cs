using UnityEngine;
using UnityEngine.UI; 

public class TriggerButtonActivator : MonoBehaviour
{
    [Header(" Referencia al botón UI")]
    public Button targetButton; 

    [Header(" Tag a detectar")]
    public string tagObjetivo = "Bola"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagObjetivo))
        {
            if (targetButton != null)
            {
                targetButton.onClick.Invoke();

              
            }
        }
    }
}
