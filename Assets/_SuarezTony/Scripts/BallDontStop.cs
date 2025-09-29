using UnityEngine;

public class BallDontStop : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bola"))
        {
            Debug.Log("bola entro");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Debug.Log("toco la bola");
            if (rb != null)
            {
                rb.AddForce(Vector3.forward * 2f, ForceMode.VelocityChange);
                Debug.Log("aumentar velocidad");
            }
        }    
    }
}
