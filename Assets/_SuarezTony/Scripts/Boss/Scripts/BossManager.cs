using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
   
    [System.Serializable]
    public class BossAttack
    {
        public string name;     
        public float weight;    
    }

    public string menuPrincipal;
    public GameObject winCanvas;
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
    public string attackFire = "";

    [Header("Behaviours")] 
    
    public bool canAttack = true;
    public bool isAttacking ;
    [SerializeField]
    private float attackCooldown;
    private float timer;
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;

    public ParticleSystem fire;
    public SphereCollider fireCollider;
    public Transform launchPoint;
    public float fireballDuration = 2f;
    public float fireballDistanceMax = 50f;
    public float raycastMultiplier = 1.5f;  
    [Range(5f,20f)]
    public float gizmoSphereRadius = 0.3f;
    
    [Header("Health")] 
    public int maxHealth = 150;

    public int currentHealth;

    [Header("Material")] 
    public MeshRenderer bossMeshRender;
    public Material armsRenderer;
    public Material bossRenderer;
    public Material bossSpit;
    public Material bossAngry;
    
    
    
    public Color maxHealthColor = new Color(254,255,200);
    public Color lowHealthColor = Color.red;
    
    private bool isFlashing = false;
    private Color currentLerpedColor;
    
    [Header("SFX")] 
    public GameObject sfxGrito1;
    public GameObject sfxGrito2;
    public GameObject sfxGrito3;
    public GameObject sfxBolaFuego;
    public GameObject sfxFuego;
    public GameObject sfxGolpe;


    private void Start()
    {
        sfxGolpe.SetActive(false);
        sfxGrito1.SetActive(false);
        sfxGrito2.SetActive(false);
        sfxGrito3.SetActive(false);
        sfxFuego.SetActive(false); 
        sfxBolaFuego.SetActive(false);
       bossAnimator.SetBool(bossDead, false);
       fireCollider.enabled = false;
       canAttack = true;
       currentHealth = maxHealth;
       bossRenderer.color = maxHealthColor;
       armsRenderer.color = maxHealthColor;
       bossSpit.color = maxHealthColor;
       bossAngry.color = maxHealthColor;
       fire.Stop();
       winCanvas.SetActive(false);

   }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss recibió daño. Vida restante: " + currentHealth);
        StartCoroutine(GritoGolpe());

        UpdateDamageColor();

        BlackFlash();
        
        if (currentHealth <= 100)
        {
            SecondPhase();
            sfxGrito2.SetActive(true);
        }
        if (currentHealth <= 50)
        {
            ThirdPhase();
            sfxGrito3.SetActive(true);
        }
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
   }

   private void UpdateDamageColor()
   {
       float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);

       currentLerpedColor = Color.Lerp(lowHealthColor, maxHealthColor, healthPercent);

       if (!isFlashing)
       {
            bossRenderer.color = currentLerpedColor;
            armsRenderer.color = currentLerpedColor;
            bossAngry.color = currentLerpedColor;
            bossSpit.color = currentLerpedColor;
       }
          
   }
   
   private void BlackFlash()
   {
       if (!isFlashing)
           StartCoroutine(FlashBlackRoutine());
   }

   private IEnumerator FlashBlackRoutine()
   {
       isFlashing = true;
       
       bossRenderer.color = Color.white;
       armsRenderer.color = Color.white;
       bossSpit.color = Color.white;
       bossAngry.color = Color.white;
       yield return new WaitForSeconds(0.05f);
       bossRenderer.color = currentLerpedColor;
       armsRenderer.color = currentLerpedColor;
       bossSpit.color = currentLerpedColor;
       bossAngry.color = currentLerpedColor;

       isFlashing = false;
   }

   public void Golpe()
   {
       StartCoroutine(ActivateGolpe());

   }
   
   public void ActivateFireball()
   {
       StartCoroutine(BolaFuego());
   }

   public void ActivateFire()
   {
       StartCoroutine(Fuego());
   }

   private IEnumerator GritoGolpe()
   {
       sfxGrito1.SetActive(true);
       yield return new WaitForSeconds(3f);
       sfxGrito1.SetActive(false);
   }
   
    private IEnumerator ActivateGolpe()
   {
       sfxGolpe.SetActive(true);
       yield return new WaitForSeconds(3f);
       sfxGolpe.SetActive(false);
   }

   private IEnumerator BolaFuego()
   {
       sfxBolaFuego.SetActive(true);
       yield return new WaitForSeconds(0.65f);
       sfxBolaFuego.SetActive(false);
   }
   
   private IEnumerator Fuego()
   {
       sfxFuego.SetActive(true);
       yield return new WaitForSeconds(7.2f);
       sfxFuego.SetActive(false);
   }

   
   private void Die()
   {
       
       canAttack = false;
       bossAnimator.SetBool(bossDead, true);
       StartCoroutine(EsperarCanvas());
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
        bossMeshRender.material = bossSpit;
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

    public void Fire()
    {
        isAttacking = true;
        bossAnimator.SetTrigger(attackFire);
    }
    public void LaunchFire()
    {
        bossMeshRender.material = bossSpit;
        fire.Play();
        fireCollider.enabled = true;

    }

    public void StopFire()
    {
        fire.Stop();
        fireCollider.enabled = false;

    }
    
    
    public void EndAttack()
    {
        
        isAttacking = false;
    }

    public void MainMaterial()
    {
        bossMeshRender.material = bossRenderer;
    }

    public void AngryMaterial()
    {
        bossMeshRender.material = bossAngry;
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
            
            case "Fire":
                Fire();
                break;

          
        }
    }
    
    private void SecondPhase()
    {
        Debug.Log("Boss en fase 2: aumentando probabilidades!");

        foreach (var atk in attacks)
        {
            if (atk.name == "Fireball")
                atk.weight = 40f;

            if (atk.name == "DoubleFireball")
                atk.weight = 30f;

            if (atk.name == "FistDer" || atk.name == "FistIzq")
                atk.weight = 0f;

            if (atk.name == "DoubleFist")
                atk.weight = 30f;
            
            if(atk.name == "Fire")
                atk.weight = 0f;
        }
    }
    
    private void ThirdPhase()
    {
        Debug.Log("Boss en fase 3: aumentando probabilidades!");

        foreach (var atk in attacks)
        {
            if (atk.name == "Fireball")
                atk.weight = 10f;

            if (atk.name == "DoubleFireball")
                atk.weight = 30f;
            
            if (atk.name == "DoubleFist")
                atk.weight = 0f;

            if (atk.name == "FistDer" || atk.name == "FistIzq")
                atk.weight = 0f;

            if (atk.name == "DoubleFist")
                atk.weight = 0f;
            
            if(atk.name == "Fire")
                atk.weight = 50f;

        }
    }

    private IEnumerator EsperarCanvas()
    {
        yield return new WaitForSeconds(10f);
        winCanvas.SetActive(true);
        yield return new WaitForSeconds(30f);
        SceneManager.LoadScene(menuPrincipal);
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