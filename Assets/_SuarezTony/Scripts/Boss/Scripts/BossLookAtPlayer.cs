using UnityEngine;

public class BossLookAtPlayer : MonoBehaviour
{
    [Header("Arms")] 
    public GameObject rightArm;
    public GameObject leftArm;  
    
    [Header("Animations")] 
    public Animator rightArmAnimator;
    public Animator leftArmAnimator;
    public string attackFistDer = "";
    public string attackFistIzq = "";
    public string attackFireball = "";
    public string attackDoubleFist = "";
    public string attackCycloneFistDer = "";
    public string attackCycloneFistIzq = "";

    
    public GameObject fireballPrefab;
    public Transform target;
    [Range(0.1f,1f)]
    public float rotationSpeed = .5f;
   
    public bool isAttacking ;

   private void Start()
   {
       rightArmAnimator.SetBool(attackFistDer,false);
   }
   
    private void Update()
    {
        if (!isAttacking && target != null)
        {
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if(direction.magnitude < 0.001f)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation, 
            rotationSpeed * Time.deltaTime
            );
    }

    public void FistDer()
    {
        isAttacking = true; 
        rightArmAnimator.SetBool(attackFistDer,true);
        
    }

    public void FistIzq()
    {
        isAttacking = true; 
        leftArmAnimator.SetBool(attackFistIzq, true);

    }

    public void Fireball()
    {
        isAttacking = true;
        Instantiate(fireballPrefab);
    }
    
    public void DoubleFireball()
    {
        isAttacking = true; 
    }

    public void DoubleFist()
    {
        isAttacking = true; 
        rightArmAnimator.SetBool(attackFistDer,true);
        leftArmAnimator.SetBool(attackFistIzq, true);
    }

    
    
    public void EndAttack()
    {
        
        isAttacking = false;
    }
}
