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
    

    [Header("Behaviours")] 
    public GameObject fireballPrefab;
    public Transform target;
    [Range(0.1f,1f)]
    public float rotationSpeed = .5f;

    public bool canAttack = true;
    public bool isAttacking ;
    [SerializeField]
    private float attackCooldown;
    private float timer;
       
       
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

            // puedes seguir agregando aquí...
        }
    }
    
    
}
