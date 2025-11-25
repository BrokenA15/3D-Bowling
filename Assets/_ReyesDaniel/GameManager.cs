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
    private int totalPinosCaidos = 0;       // Total acumulado entre rondas
    private int pinosEnPieAnterior = 10;    // Cuántos pinos había antes del tiro
    private int pinosCaidosEsteTurno = 0;   // Pinos derribados en este tiro

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
    public float pinRespawnDelay = 2f; // 🕒 Tiempo antes de volver a generar los pinos

    private int turn = 1;
    private float stillTimer = 0f;
    private TurnState currentState = TurnState.TurnPreparation;

    void Start()
    {
        if (bowlingBallRB == null)
            bowlingBallRB = FindFirstObjectByType<Rigidbody>();

        StartCoroutine(SetupPinsWithDelay(0f)); // genera al inicio sin delay
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
        // 🔄 Limpia pinos anteriores
        foreach (GameObject pin in currentPins)
        {
            if (pin != null)
                Destroy(pin);
        }

        currentPins.Clear();
        initialPinPositions.Clear();
        initialPinRotations.Clear();

        // 🔹 Instancia nuevos pinos
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
        pinosCaidosEsteTurno = pinosEnPieAnterior - currentPins.Count;
        if (pinosCaidosEsteTurno < 0) pinosCaidosEsteTurno = 0;

        // 🟢 Sumar al total general
        totalPinosCaidos += pinosCaidosEsteTurno;


        // 📊 Mostrar información en consola
        Debug.Log($"🎳 Turno {turn}: Caídos = {pinosCaidosEsteTurno}, Total = {totalPinosCaidos}");
        ActPin.UpdatePins(totalPinosCaidos);

        // 🟡 --- Lógica de turnos y chuza ---
        if (turn == 1)
        {
            if (currentPins.Count == 0)
            {
                // 🎯 CHUZA
                Debug.Log("💥 CHUZA en el primer tiro! Reiniciando pinos.");
                turn = 1;
                pinosEnPieAnterior = 10; // reinicia los pinos para la nueva ronda
                StartCoroutine(SetupPinsWithDelay(pinRespawnDelay));
            }
            else
            {
                // Pasar al segundo turno
                turn = 2;
                pinosEnPieAnterior = currentPins.Count; // guarda cuántos quedaron de pie
                Debug.Log($"➡️ Segundo tiro, quedan {pinosEnPieAnterior} pinos en pie.");
            }
        }
        else
        {
            // Fin de la ronda
            Debug.Log("🔁 Fin de la ronda, reiniciando pinos.");
            turn = 1;
            pinosEnPieAnterior = 10; // reinicia para la siguiente ronda
            StartCoroutine(SetupPinsWithDelay(pinRespawnDelay));
        }

        // Reinicia la bola y el estado del turno
        ResetBallPosition();
        EnterTurnPreparation();
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

    void ResetBallPosition()
    {
        if (bowlingBallRB == null) return;

        bowlingBallRB.linearVelocity = Vector3.zero;
        bowlingBallRB.angularVelocity = Vector3.zero;
        bowlingBallRB.transform.position = new Vector3(11.7180004f, 0.768000007f, -19.4759998f);
        bowlingBallRB.transform.rotation = Quaternion.identity;
    }

    void EnterTurnPreparation()
    {
        currentState = TurnState.TurnPreparation;
        Debug.Log("🟢 Turno listo, espera a que el jugador tome la bola (VR).");
    }
}
