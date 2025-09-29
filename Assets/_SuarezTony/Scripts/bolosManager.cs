using UnityEngine;
using System.Collections;

public class bolosManager : MonoBehaviour
{
    [Header("Referencias")]
    public pinosDetectar[] pins;

    private int totalScore = 0;
    private int scoreFirstThrow = 0;
    private int throwNumber = 1;
    private bool canCheckPins = true;

    public void CheckPins()
    {
        if (!canCheckPins) return;

        int fallenThisThrow = 0;

        foreach (pinosDetectar pin in pins)
        {
            if (pin.TryCount())
            {
                fallenThisThrow++;
            }
        }

        if (throwNumber == 1)
        {
            scoreFirstThrow = fallenThisThrow;
            totalScore = scoreFirstThrow;
            Debug.Log($"🎯 Primer tiro: {scoreFirstThrow} pinos caídos. Total: {totalScore}");

            StartCoroutine(NextThrowDelay(5f));
        }
        else if (throwNumber == 2)
        {
            totalScore += fallenThisThrow;
            Debug.Log($"🎯 Segundo tiro: {fallenThisThrow} pinos más. Total acumulado: {totalScore}");

            // Reinicio de ronda
            throwNumber = 1;
            scoreFirstThrow = 0;
            Debug.Log("🔄 Ronda terminada, listo para iniciar otra.");
        }
    }

    private IEnumerator NextThrowDelay(float seconds)
    {
        canCheckPins = false;
        Debug.Log($"⏳ Esperando {seconds}s para segundo tiro...");
        yield return new WaitForSeconds(seconds);

        throwNumber = 2;
        canCheckPins = true;
        Debug.Log("🎳 Segundo tiro disponible.");
    }
}
