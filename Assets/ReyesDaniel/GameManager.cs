using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BowlingGameManager : MonoBehaviour
{
    public enum TurnState
    {
        TurnPreparation,
        WaitingThrow,
        BallThrown,
        ProcessingPins
    }

    [Header("Pinos")]
    public GameObject pinPrefab;
    public Transform[] pinSpawnPoints;
    public List<GameObject> currentPins = new List<GameObject>();
    private int totalPinosCaidos = 0;
    private int pinosEnPieAnterior = 10;
    private int pinosCaidosEsteTurno = 0;

    private List<Vector3> initialPinPositions = new List<Vector3>();
    private List<Quaternion> initialPinRotations = new List<Quaternion>();

    [Header("Bola (VR)")]
    public Rigidbody bowlingBallRB;
    public float stopThreshold = 0.5f;
    public float stopTimeRequired = 5f;
    public BowlingVRUIManagerw ActPin;

    [Header("Pinos caídos")]
    public float fallenPinDisableDelay = 0.3f;
    public float pinCheckDelay = 0.5f;
    public float snapDuration = 0.3f;

    [Header("Tiempos")]
    public float pinRespawnDelay = 2f;

    [Header("Juego")]
    public int maxRondas = 10;
    private int rondaActual = 1;

    [Header("Escena Final")] 
    public DisableWall wallDisable;
    public string nextSceneName = "BossVR";

    private int turn = 1;
    private float stillTimer = 0f;
    private TurnState currentState = TurnState.TurnPreparation;

    void Start()
    {
        if (bowlingBallRB == null)
            bowlingBallRB = FindFirstObjectByType<Rigidbody>();

        StartCoroutine(SetupPinsWithDelay(0f));
        ResetBallPosition();
        EnterTurnPreparation();
    }

    void Update()
    {
        if (currentState == TurnState.TurnPreparation)
        {
            if (bowlingBallRB.linearVelocity.magnitude > 0.05f)
                currentState = TurnState.WaitingThrow;
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
        if (currentState == TurnState.WaitingThrow && bowlingBallRB.linearVelocity.magnitude > stopThreshold)
        {
            currentState = TurnState.BallThrown;
            stillTimer = 0f;
        }
    }

    void DetectBallStop()
    {
        if (bowlingBallRB.linearVelocity.magnitude < stopThreshold)
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
        {
            if (pin != null)
                Destroy(pin);
        }

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

        Debug.Log("🆕 Pinos listos");
    }

    IEnumerator SetupPinsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetupPins();
    }

    IEnumerator ProcessPins()
    {
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
                StartCoroutine(DisablePin(pin, fallenPinDisableDelay));
            }
        }

        currentPins = standingPins;
        pinosCaidosEsteTurno = pinosEnPieAnterior - currentPins.Count;
        if (pinosCaidosEsteTurno < 0) pinosCaidosEsteTurno = 0;

        totalPinosCaidos += pinosCaidosEsteTurno;

        ActPin.UpdatePins(totalPinosCaidos);

        Debug.Log($"🎳 Turno {turn} Ronda {rondaActual}: Caídos = {pinosCaidosEsteTurno}, Total = {totalPinosCaidos}");

        // ----------- LÓGICA PRINCIPAL --------------

        if (turn == 1)
        {
            // CHUZA → pasar a siguiente ronda
            if (currentPins.Count == 0)
            {
                //Debug.Log("💥 CHUZA! Avanza de ronda.");

                rondaActual++;
                turn = 1;
                pinosEnPieAnterior = 10;

                if (rondaActual > maxRondas)
                {
                    wallDisable.WallDisable();
                   
                    yield break;
                }

                StartCoroutine(SetupPinsWithDelay(pinRespawnDelay));
            }
            else
            {
                // Pasar al segundo tiro
                turn = 2;
                pinosEnPieAnterior = currentPins.Count;
                //Debug.Log("➡️ Pasa al segundo tiro.");
            }
        }
        else  // turn == 2
        {
            //Debug.Log("🔁 Fin de la ronda normal.");

            rondaActual++;
            turn = 1;
            pinosEnPieAnterior = 10;

            if (rondaActual > maxRondas)
            {
                //Debug.Log("🏁 Se completaron las rondas. Cargando escena final...");
                wallDisable.WallDisable();

                yield break;
            }

            StartCoroutine(SetupPinsWithDelay(pinRespawnDelay));
        }

        ResetBallPosition();
        EnterTurnPreparation();
    }

    IEnumerator DisablePin(GameObject pin, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (pin != null)
        {
            pin.SetActive(false);
        }
    }

    IEnumerator SnapPinToPosition(GameObject pin, Vector3 targetPos, Quaternion targetRot)
    {
        if (pin == null || !pin.activeInHierarchy)
            yield break;

        Rigidbody rb = pin.GetComponent<Rigidbody>();
        if (rb == null)
            yield break;

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
        if (bowlingBallRB == null) return;

        bowlingBallRB.linearVelocity = Vector3.zero;
        bowlingBallRB.angularVelocity = Vector3.zero;
        bowlingBallRB.transform.position = new Vector3(11.4320002f, 0.520427763f, -18.9416275f);
        bowlingBallRB.transform.rotation = Quaternion.identity;
    }

    void EnterTurnPreparation()
    {
        currentState = TurnState.TurnPreparation;
        Debug.Log("🟢 Turno preparado");
    }
}
