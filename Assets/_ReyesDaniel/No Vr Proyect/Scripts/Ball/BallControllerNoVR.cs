using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallControllerNoVR : MonoBehaviour
{
    [Header("Configuración")]
    public Transform holdPoint;
    public float pickUpDistance = 3f;
    public float throwForce = 10f;
    public float maxChargeTime = 2f;
    private int totalPinsScore = 0;  
    private int pinTurnScore = 0;

    private Rigidbody rb;
    private Collider col;
    private bool isHolding = false;
    private float chargeTime = 0f;
    private Camera mainCam;
    private bool isCharging = false;

    // Input System
    private PlayerInput playerInput;
    private PlayerControls inputActions;

    private int originalLayer;
    private int ignorePlayerLayer;

    private BowlingUIManager uiManager;

    void Start()
    {
        uiManager = FindFirstObjectByType<BowlingUIManager>();
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        mainCam = Camera.main;

        originalLayer = gameObject.layer;
        ignorePlayerLayer = LayerMask.NameToLayer("IgnorePlayer");

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        inputActions = new PlayerControls();
    }

    void OnEnable()
    {
        inputActions.Player.Interact.performed += ctx => TryPickUpBall();
        inputActions.Player.Throw.started += ctx => StartCharging();
        inputActions.Player.Throw.canceled += ctx => ReleaseThrow();
        inputActions.Player.Drop.performed += ctx => DropBall();
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Interact.performed -= ctx => TryPickUpBall();
        inputActions.Player.Throw.started -= ctx => StartCharging();
        inputActions.Player.Throw.canceled -= ctx => ReleaseThrow();
        inputActions.Player.Drop.performed -= ctx => DropBall();
        inputActions.Disable();
    }

    void Update()
    {
        if (isHolding)
        {
            rb.MovePosition(holdPoint.position);
            rb.MoveRotation(holdPoint.rotation);

            if (isCharging)
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);

                if (uiManager != null)
                    uiManager.UpdateForceBar(chargeTime, maxChargeTime);
            }
        }
    }


    void TryPickUpBall()
    {
        if (!isHolding && IsPlayerClose())
            PickUpBall();
    }

    void StartCharging()
    {
        if (isHolding)
            isCharging = true;
    }

    void ReleaseThrow()
    {
        if (isHolding && isCharging)
        {
            ThrowBall();
            isCharging = false;
        }
    }

    bool IsPlayerClose()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        return Vector3.Distance(transform.position, player.position) <= pickUpDistance;
    }

    void PickUpBall()
    {
        isHolding = true;
        rb.isKinematic = true;
        col.enabled = false;
        gameObject.layer = ignorePlayerLayer;
        chargeTime = 0f;
    }

    void ThrowBall()
    {
        isHolding = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 throwDirection = mainCam.transform.forward;
        float finalForce = throwForce * (chargeTime / maxChargeTime);
        rb.AddForce(throwDirection * finalForce, ForceMode.VelocityChange);
        if (uiManager != null)
            uiManager.ResetForceBar();


        RestoreCollision();
        chargeTime = 0f;
    }

    void DropBall()
    {
        isHolding = false;
        rb.isKinematic = false;
        RestoreCollision();
    }

    void RestoreCollision()
    {
        col.enabled = true;
        gameObject.layer = originalLayer;
    }

    // 🔑 --- MÉTODOS PÚBLICOS PARA EL GAME MANAGER ---

    public bool IsHolding()
    {
        return isHolding;
    }

    public Vector3 GetVelocity()
    {
        return rb.linearVelocity;
    }

    public void ResetBall(Vector3 startPosition)
    {
        isHolding = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        RestoreCollision();
    }
}
