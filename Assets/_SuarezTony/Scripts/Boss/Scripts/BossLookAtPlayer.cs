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
    public Animator rightArmAnimator;
    public Animator leftArmAnimator;
    public string attackFistDer = "";
    public string attackFistIzq = "";
    public string attackFireball = "";
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
   
   [SerializeField]
   private float attackCooldown;
   private float timer;

   void Update()
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

       if (target != null)
           RotateTowardsTarget();
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
        Instantiate(fireballPrefab);
    }
    
    public void DoubleFireball()
    {
        isAttacking = true; 
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
    
    public void ChooseAttack()
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

            // puedes seguir agregando aquí...
        }
    }
    
    
}
