using UnityEngine;
using System.Collections;


public class BolitaTakeDamage : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    public bool isOnCooldown = false;
    public float respawnCooldown = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PinoBoss"))
        {
           
            BossLookAtPlayer boss = collision.collider.GetComponentInParent<BossLookAtPlayer>();

            if (boss != null)
            {
                boss.TakeDamage(10);  
            }
            StartCoroutine(RespawnRoutine());
        }
    }
    
    private IEnumerator RespawnRoutine()
    {
        isOnCooldown = true;

       
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        
        yield return new WaitForSeconds(respawnCooldown);

        transform.position = startPosition;
        transform.rotation = startRotation;

        
        rb.isKinematic = false;

        isOnCooldown = false;
    }
}
