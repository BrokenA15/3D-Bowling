using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowlingGameManagerNoVR : MonoBehaviour
{
    public enum TurnState
    {
        TurnPreparation,
        WaitingThrow,
        BallThrown,
        ProcessingPins
    }
    
    public DisableWall disableWall;
    public bool endgame;

    [Header("Pinos")]
    public GameObject pinPrefab;
    public Transform[] pinSpawnPoints;
    public List<GameObject> currentPins = new List<GameObject>();
    private int totalPinsScore = 0;
    private int pinTurnScore = 0;
    private int pinosEnPieAnterior = 10;


    private List<Vector3> initialPinPositions = new List<Vector3>();
    private List<Quaternion> initialPinRotations = new List<Quaternion>();

    [Header("Bola")]
    public BallControllerNoVR ballController;
    public float stopThreshold = 0.5f;
    public float stopTimeRequired = 5f;

    [Header("Pinos caídos")]
    public float fallenPinDestroyDelay = 0.3f;
    public float pinCheckDelay = 0.5f;
    public float snapDuration = 0.3f;

    [Header("UI")]
    public BowlingUIManager uiManager;

    private int round = 1;
    [SerializeField]
    private const int maxRounds = 2;
    private int turn = 1;
    private float stillTimer = 0f;
    private TurnState currentState = TurnState.TurnPreparation;

    void Start()
    {

        if (ballController == null)
            ballController = FindFirstObjectByType<BallControllerNoVR>();
        if (uiManager == null)
            uiManager = FindFirstObjectByType<BowlingUIManager>();

        SetupPins();
        pinosEnPieAnterior = 10;

        ResetBallPosition();
        EnterTurnPreparation();
    }

    void Update()
    {
        if (currentState == TurnState.TurnPreparation)
        {
            if (ballController.IsHolding())
            {
                currentState = TurnState.WaitingThrow;
                uiManager.UpdateState("Throw the ball");
            }
        }
        else if (currentState == TurnState.WaitingThrow)
        {
            DetectBallThrow();
        }
    }

    void FixedUpdate()
    {
        if (currentState == TurnState.BallThrown)
            DetectBallStop();
    }

    void DetectBallThrow()
    {
        if (currentState == TurnState.WaitingThrow && ballController != null)
        {
            if (!ballController.IsHolding() && ballController.GetVelocity().magnitude > stopThreshold)
            {
                currentState = TurnState.BallThrown;
                stillTimer = 0f;
                uiManager.UpdateState("Waiting for the ball to come back");
            }
        }
    }

    void DetectBallStop()
    {
        if (ballController.GetVelocity().magnitude < stopThreshold)
        {
            stillTimer += Time.fixedDeltaTime;

            if (stillTimer >= stopTimeRequired)
            {
                currentState = TurnState.ProcessingPins;
                StartCoroutine(ProcessPins());
            }
        }
        else
        {
            stillTimer = 0f;
        }
    }

    public void SetupPins()
    {
        foreach (GameObject pin in currentPins)
            if (pin != null)
                Destroy(pin);

        currentPins.Clear();
        initialPinPositions.Clear();
        initialPinRotations.Clear();

        foreach (Transform spawn in pinSpawnPoints)
        {
            GameObject pin = Instantiate(pinPrefab, spawn.position, spawn.rotation);
            currentPins.Add(pin);
            initialPinPositions.Add(spawn.position);
            initialPinRotations.Add(spawn.rotation);
        }
    }

    IEnumerator ProcessPins()
    {
        uiManager.UpdateState("Checking pins");
        yield return new WaitForSeconds(pinCheckDelay);

        List<GameObject> standingPins = new List<GameObject>();

        for (int i = 0; i < currentPins.Count; i++)
        {
            
            GameObject pin = currentPins[i];
            if (pin == null) continue;

            Quaternion currentRot = pin.transform.rotation;
            Quaternion initialRot = initialPinRotations[i];
            float angle = Quaternion.Angle(currentRot, initialRot);

            if (angle < 30f)
            {
                standingPins.Add(pin);
                StartCoroutine(SnapPinToPosition(pin, initialPinPositions[i], initialPinRotations[i]));
            }
            else
            {
                Destroy(pin, fallenPinDestroyDelay);
            }
        }

        currentPins = standingPins;
        int pinosCaidosEsteTurno = pinosEnPieAnterior - currentPins.Count;
        if (pinosCaidosEsteTurno < 0) pinosCaidosEsteTurno = 0; // Seguridad

        totalPinsScore += pinosCaidosEsteTurno;
        pinosEnPieAnterior = currentPins.Count;
        uiManager.UpdatePins( totalPinsScore);

        //  --- LÓGICA DE CHUZA ---
        bool isStrike = (currentPins.Count == 0 && turn == 1);
        if (isStrike)
        {
            uiManager.UpdateState("¡Strike!");
            round++;
            turn = 1;

            SetupPins();
            pinosEnPieAnterior = 10;


            if (round > maxRounds)
            {
                uiManager.UpdateState("🎉 Game ending");
                endgame = true;
                if (endgame == true)
                {
                     disableWall.WallDisable();
                }
               


            }

            uiManager.UpdateTurn(turn);
            uiManager.UpdateRound(round);

            yield return new WaitForSeconds(2f);
            ResetBallPosition();
            EnterTurnPreparation();
            yield break; // ← terminamos aquí para no continuar
        }
        // 🟢 ------------------------

        // Si NO hubo chuza:
        if (currentPins.Count == 0)
        {
            turn = 1;
            round++;
            SetupPins();
            pinosEnPieAnterior = 10;

        }
        else if (turn == 1)
        {
            turn = 2;
        }
        else
        {
            turn = 1;
            round++;
            SetupPins();
            pinosEnPieAnterior = 10;

        }

        if (round > maxRounds)
        {
            uiManager.UpdateState("🎉 Game ending");
            endgame = true;
            if (endgame == true)
            {
                disableWall.WallDisable();
            }


        }

        uiManager.UpdateTurn(turn);
        uiManager.UpdateRound(round);

        yield return new WaitForSeconds(2f);
        ResetBallPosition();
        EnterTurnPreparation();
    }


    IEnumerator SnapPinToPosition(GameObject pin, Vector3 targetPos, Quaternion targetRot)
    {
        Rigidbody rb = pin.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        Vector3 startPos = pin.transform.position;
        Quaternion startRot = pin.transform.rotation;
        float elapsed = 0f;

        while (elapsed < snapDuration)
        {
            float t = elapsed / snapDuration;
            pin.transform.position = Vector3.Lerp(startPos, targetPos, t);
            pin.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pin.transform.position = targetPos;
        pin.transform.rotation = targetRot;
        rb.isKinematic = false;
    }

    void ResetBallPosition()
    {
        if (ballController == null) return;
        ballController.ResetBall(new Vector3(608.169983f,701.289978f,555.900024f));
    }

    void EnterTurnPreparation()
    {
        currentState = TurnState.TurnPreparation;
        uiManager.UpdateState("Grab the ball to start your turn");
    }
}

