using UnityEngine;
using System.Collections;


public class BolitaTakeDamage : MonoBehaviour
{
    
    [Header("Respawn Points")]
    public Transform[] respawnPoints;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    public int damage = 10;
    public bool isOnCooldown = false;
    public float respawnCooldown = 3f;
    private MeshRenderer meshRenderer;
    public ParticleSystem collisionParticles;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        collisionParticles.Stop();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PinoBoss"))
        {
           
            BossManager boss = collision.collider.GetComponentInParent<BossManager>();

            if (boss != null)
            {
                boss.TakeDamage(damage);  
            }
            StartCoroutine(RespawnRoutine());
        }
        else if (collision.collider.CompareTag("RespawnBall"))
        {
            StartCoroutine(RespawnRoutine());

        }
    }
    
    private IEnumerator RespawnRoutine()
    {
       collisionParticles.Play();
       yield return new WaitForSeconds(0.5f);
       collisionParticles.Stop();

        isOnCooldown = true;
        meshRenderer.enabled = false;

       
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        
        yield return new WaitForSeconds(respawnCooldown);

        if (respawnPoints.Length > 0)
        {
            int index = Random.Range(0, respawnPoints.Length);
            transform.position = respawnPoints[index].position;
            transform.rotation = respawnPoints[index].rotation;
        }
        else
        {
            // fallback si no hay puntos asignados
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        rb.isKinematic = false;
        meshRenderer.enabled = true;

        isOnCooldown = false;
    }
}
