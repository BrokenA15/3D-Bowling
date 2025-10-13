using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<Vector3> initialPinPositions = new List<Vector3>();
    private List<Quaternion> initialPinRotations = new List<Quaternion>();

    [Header("Bola (VR)")]
    public Rigidbody bowlingBallRB;
    public float stopThreshold = 0.5f;
    public float stopTimeRequired = 5f;

    [Header("Pinos caídos")]
    public float fallenPinDisableDelay = 0.3f;
    public float pinCheckDelay = 0.5f;
    public float snapDuration = 0.3f;

    [Header("Retraso de regeneración")]
    public float pinSetupDelay = 2f; // ⏱ tiempo antes de que se generen los nuevos pinos

    private int turn = 1;
    private float stillTimer = 0f;
    private TurnState currentState = TurnState.TurnPreparation;

    void Start()
    {
        if (bowlingBallRB == null)
            bowlingBallRB = FindFirstObjectByType<Rigidbody>();

        SetupPins();
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
        if (currentState == TurnState.WaitingThrow && bowlingBallRB != null)
        {
            if (bowlingBallRB.linearVelocity.magnitude > stopThreshold)
            {
                currentState = TurnState.BallThrown;
                stillTimer = 0f;
                Debug.Log("🎳 Bola lanzada");
            }
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

        Debug.Log("🆕 Pinos listos");
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

            if (angle < 30f) // sigue de pie
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

        // 🟡 --- Lógica de turnos y chuza ---
        if (turn == 1)
        {
            if (currentPins.Count == 0)
            {
                Debug.Log("💥 CHUZA en el primer tiro! Reinicio tras breve pausa...");
                turn = 1;
                StartCoroutine(ResetTurnAfterDelay()); // 👈 usa intervalo antes de regenerar
            }
            else
            {
                turn = 2;
                Debug.Log("➡️ Segundo tiro, quedan " + currentPins.Count + " pinos.");
                ResetBallPosition();
                EnterTurnPreparation();
            }
        }
        else
        {
            Debug.Log("🔁 Fin del turno, reiniciando todos los pinos tras breve pausa...");
            turn = 1;
            StartCoroutine(ResetTurnAfterDelay()); // 👈 usa intervalo antes de regenerar
        }
    }

    IEnumerator DisablePin(GameObject pin, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (pin != null)
        {
            pin.SetActive(false);
            Debug.Log("❌ Pino desactivado");
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
            if (pin == null || !pin.activeInHierarchy)
                yield break;

            float t = elapsed / snapDuration;
            pin.transform.position = Vector3.Lerp(startPos, targetPos, t);
            pin.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pin != null && pin.activeInHierarchy)
        {
            pin.transform.position = targetPos;
            pin.transform.rotation = targetRot;
            rb.isKinematic = false;
        }
    }

    IEnumerator ResetTurnAfterDelay()
    {
        // ⏳ Espera para evitar choques visuales o físicos con los pinos previos
        yield return new WaitForSeconds(pinSetupDelay);

        SetupPins();
        ResetBallPosition();
        EnterTurnPreparation();
    }

    void ResetBallPosition()
    {
        if (bowlingBallRB == null) return;

        bowlingBallRB.linearVelocity = Vector3.zero;
        bowlingBallRB.angularVelocity = Vector3.zero;
        bowlingBallRB.transform.position = new Vector3(11.07f, 1.222f, -19.437f);
        bowlingBallRB.transform.rotation = Quaternion.identity;
    }

    void EnterTurnPreparation()
    {
        currentState = TurnState.TurnPreparation;
        Debug.Log("🟢 Turno listo, espera a que el jugador tome la bola (VR).");
    }
}
