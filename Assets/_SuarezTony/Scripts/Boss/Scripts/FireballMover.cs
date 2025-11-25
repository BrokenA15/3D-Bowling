using System;
using UnityEngine;

public class FireballMover : MonoBehaviour
{
    private Vector3 targetPoint;
    private float duration = 1.5f;
    private MeshRenderer meshRenderer;
    private float t = 0f;
    private bool moving = false;


    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
      
        
    }

    public void Init(Vector3 point, float moveDuration)
    {
        targetPoint = point;
        duration = moveDuration;
        moving = true;
    }

    void Update()
    {
        if (!moving) return;

        t += Time.deltaTime / duration;
        float easedT = 1f - Mathf.Pow(1f - t, 3); 

        transform.position = Vector3.Lerp(transform.position, targetPoint, easedT);

        if (t >= 1f)
        {
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        
        if (other.CompareTag("Player")  || other.CompareTag("Ground") || other.CompareTag("Fireball") || other.CompareTag("Particle"))
        {
            meshRenderer.enabled = false;
        }
    }
}
