using System;
using UnityEngine;

public class ActivateExplosion : MonoBehaviour
{
    private ParticleSystem explosion;
    private float fixedLocalY;
  

    private void Start()
    {
        explosion =  GetComponent<ParticleSystem>();
      
        fixedLocalY = transform.localPosition.y;
      
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.y = fixedLocalY;
        transform.localPosition = pos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fireball"))
        {
            explosion.Stop();
            explosion.Play();
           
        }
    }
    
 
}
