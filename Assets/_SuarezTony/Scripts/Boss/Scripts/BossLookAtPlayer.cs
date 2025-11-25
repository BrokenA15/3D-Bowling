using UnityEngine;
using System.Collections.Generic;

public class BossLookAtPlayer : MonoBehaviour
{
    
    [System.Serializable]
    public class BossAttack
    {
        public string name;     
        public float weight;    
    }
    public List<BossAttack> attacks = new List<BossAttack>();

    [Header("Arms")] 
    public GameObject rightArm;
    public GameObject leftArm;

    [Header("Animations")] 
    public Animator bossAnimator;
    public Animator rightArmAnimator;
    public Animator leftArmAnimator;
    public string bossDead = "";
    public string attackFistDer = "";
    public string attackFistIzq = "";
    public string attackFireball = "";
    public string attackDoubleFireball = "";

    [Header("Behaviours")] 
   
    public Transform target;
    [Range(0.1f,1f)]
    public float rotationSpeed = .5f;
    public bool canAttack = true;
    public bool isAttacking ;
    [SerializeField]
    private float attackCooldown;
    private float timer;
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public Transform launchPoint;
    public float fireballDuration = 2f;
    public float fireballDistanceMax = 50f;
    public float raycastMultiplier = 1.5f;  
    [Range(5f,20f)]
    public float gizmoSphereRadius = 0.3f;
    
       
       
    [Header("Health")] 
    public int maxHealth = 100;
    [SerializeField]
    private int currentHealth;
    


  
   
   
    private void Start()
   {
       bossAnimator.SetBool(bossDead, false);
       canAttack = true;
       currentHealth = maxHealth;
   }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss recibió daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
   void Update()
   {
       if (canAttack)
       {
           if (!isAttacking)
           {
               timer += Time.deltaTime;
               if (timer >= attackCooldown)
               {
                   ChooseAttack(); 
                   timer = 0;
               }
           }
       }
       

       if (target != null)
           RotateTowardsTarget();
   }

   private void Die()
   {
       Debug.Log("Boss derrotado");
       canAttack = false;
       bossAnimator.SetBool(bossDead, true);
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
        rightArmAnimator.SetTrigger(attackFistDer);
        
    }

    public void FistIzq()
    {
        isAttacking = true; 
        leftArmAnimator.SetTrigger(attackFistIzq);

    }

    public void Fireball()
    {
        isAttacking = true;
        bossAnimator.SetTrigger(attackFireball);
    }

    public void LaunchFireball()
    {
        int ignoreLayer = LayerMask.NameToLayer("LimiteInter");
        int ignoreBoss = LayerMask.NameToLayer("Boss");
        int mask = ~((1 << ignoreLayer) | (1 << ignoreBoss));
        
        Vector3 origin = launchPoint.position;
        Vector3 direction = launchPoint.forward;


        RaycastHit hit;
        Vector3 targetPoint;
        
        float rayLength = fireballDistanceMax * raycastMultiplier;

        if (Physics.Raycast(origin, direction, out hit, rayLength, mask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = origin + direction * fireballDistanceMax;
        }

        GameObject fb = Instantiate(fireballPrefab, origin, Quaternion.identity);

        FireballMover mover = fb.GetComponent<FireballMover>();

        if (mover != null)
        {
            mover.Init(targetPoint, fireballDuration);
        }
        isAttacking = false;
    }
    
    public void DoubleFireball()
    {
        isAttacking = true; 
        bossAnimator.SetTrigger(attackDoubleFireball);
    }

    public void DoubleFist()
    {
        isAttacking = true; 
        rightArmAnimator.SetTrigger(attackFistDer);
        leftArmAnimator.SetTrigger(attackFistIzq);
    }

    
    
    public void EndAttack()
    {
        
        isAttacking = false;
    }
    
    
    private string GetRandomAttack()
    {
        float totalWeight = 0f;
        foreach (var atk in attacks)
            totalWeight += atk.weight;

        float randomValue = Random.Range(0, totalWeight);

        foreach (var atk in attacks)
        {
            if (randomValue < atk.weight)
                return atk.name;

            randomValue -= atk.weight;
        }

        return attacks[0].name; 
    }

    private void ChooseAttack()
    {
        if (isAttacking)
            return;

        string atk = GetRandomAttack();

        switch (atk)
        {
            case "FistDer":
                FistDer();
                break;

            case "FistIzq":
                FistIzq();
                break;

            case "Fireball":
                Fireball();
                break;

            case "DoubleFist":
                DoubleFist();
                break;
            
            case "DoubleFireball":
                DoubleFireball();
                break;

          
        }
    }
    
    private void OnDrawGizmos()
    {
        if (launchPoint == null)
            return;

        Vector3 origin = launchPoint.position;
        Vector3 direction = launchPoint.forward;

        float rayLength = fireballDistanceMax * raycastMultiplier;

        int ignoreLayer = LayerMask.NameToLayer("LimiteInter");
        int ignoreBoss = LayerMask.NameToLayer("Boss");

        int mask = ~( (1 << ignoreLayer) | (1 << ignoreBoss) );
      
        
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(origin, direction, out hit, rayLength, mask);

        if (hitSomething)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawWireSphere(hit.point, gizmoSphereRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 endPoint = origin + direction * rayLength;
            Gizmos.DrawLine(origin, endPoint);
            Gizmos.DrawWireSphere(endPoint, gizmoSphereRadius);
        }
    }
    
    
}
