using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowlingGameManager : MonoBehaviour
{
    public enum TurnState
    {
        WaitingThrow,   // Esperando a que el jugador lance la bola
        BallThrown,     // La bola ya fue lanzada y está en movimiento
        ProcessingPins  // La bola se detuvo, procesando pinos
    }

    [Header("Pinos")]
    public GameObject pinPrefab;
    public Transform[] pinSpawnPoints;
    public List<GameObject> currentPins = new List<GameObject>();

    [Header("Bola")]
    public Rigidbody bowlingBallRB;
    public float stopThreshold = 0.1f;
    public float stopTimeRequired = 2f;

    [Header("Pinos caídos")]
    public float fallenPinDestroyDelay = 0.3f;
    public float pinCheckDelay = 0.5f;

    private int turn = 1;
    private float stillTimer = 0f;
    private TurnState currentState = TurnState.WaitingThrow;

    void Start()
    {
        if (bowlingBallRB == null)
            bowlingBallRB = Object.FindFirstObjectByType<Rigidbody>();

        SetupPins();
        ResetBallPosition();
        currentState = TurnState.WaitingThrow;
    }

    void Update()
    {
        DetectBallThrow();
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
        // Limpiar pinos existentes
        foreach (GameObject pin in currentPins)
            if (pin != null)
                Destroy(pin);

        currentPins.Clear();

        foreach (Transform spawn in pinSpawnPoints)
        {
            GameObject pin = Instantiate(pinPrefab, spawn.position, spawn.rotation);
            currentPins.Add(pin);
        }
    }

    IEnumerator ProcessPins()
    {
        yield return new WaitForSeconds(pinCheckDelay);

        List<GameObject> standingPins = new List<GameObject>();

        foreach (GameObject pin in currentPins)
        {
            if (pin == null) continue;

            float angle = Vector3.Angle(pin.transform.up, Vector3.up);

            if (angle < 60f) // de pie
            {
                standingPins.Add(pin);
            }
            else
            {
                Destroy(pin, fallenPinDestroyDelay);
            }
        }

        currentPins = standingPins;

        // Lógica de turnos
        if (currentPins.Count == 0)
        {
            turn = 1;
            SetupPins();
            ResetBallPosition();
        }
        else if (turn == 1)
        {
            turn = 2;
            ResetBallPosition();
        }
        else
        {
            turn = 1;
            SetupPins();
            ResetBallPosition();
        }

        // Volvemos a esperar un lanzamiento
        currentState = TurnState.WaitingThrow;
    }

    void ResetBallPosition()
    {
        if (bowlingBallRB == null) return;

        bowlingBallRB.linearVelocity = Vector3.zero;
        bowlingBallRB.angularVelocity = Vector3.zero;
        bowlingBallRB.transform.position = new Vector3(2.30749917f, 0.451000005f, 1.50999999f);
        bowlingBallRB.transform.rotation = Quaternion.identity;
    }
}
