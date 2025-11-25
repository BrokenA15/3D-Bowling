using UnityEngine;
using System.Collections.Generic;

public class BossLookAtPlayer : MonoBehaviour
{
    
    public Transform target;
    [Range(0.1f,1f)]
    public float rotationSpeed = .5f;
    
    private void LateUpdate()
    {
        RotateTowardsTarget();
    }
    
    private void RotateTowardsTarget()
    {
        var direction = target.position - transform.position;
        direction.y = 0;
        if(direction.magnitude < 0.001f)
            return;
        
        var targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation, 
            rotationSpeed * Time.deltaTime
            );
    }
}
