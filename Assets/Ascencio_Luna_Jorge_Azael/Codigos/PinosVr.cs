using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PinosVr : MonoBehaviour
{
    public enum TurnState
    {
        TurnPreparation,
        ProcessingPins
    }

    [Header("Pinos")]
    public GameObject pinPrefab;
    public Transform[] pinSpawnPoints;
    public List<GameObject> currentPins = new List<GameObject>();

    private List<Vector3> initialPinPositions = new List<Vector3>();
    private List<Quaternion> initialPinRotations = new List<Quaternion>();

    [Header("Pinos caídos")]
    public float fallenPinDestroyDelay = 0.3f;
    public float pinCheckDelay = 0.5f;
    public float snapDuration = 0.3f;

    private int turn = 1;
    private TurnState currentState = TurnState.TurnPreparation;

    void Start()
    {
        SetupPins();
        EnterTurnPreparation();
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

    public void StartPinProcessing()
    {
        if (currentState != TurnState.ProcessingPins)
        {
            currentState = TurnState.ProcessingPins;
            StartCoroutine(ProcessPins());
        }
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
                Destroy(pin, fallenPinDestroyDelay);
            }
        }

        currentPins = standingPins;

        if (currentPins.Count == 0)
        {
            turn = 1;
            SetupPins();
        }
        else if (turn == 1)
        {
            turn = 2;
        }
        else
        {
            turn = 1;
            SetupPins();
        }

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

    void EnterTurnPreparation()
    {
        currentState = TurnState.TurnPreparation;
        Debug.Log("🟢 Turno listo, puedes lanzar cuando quieras.");
    }
}
