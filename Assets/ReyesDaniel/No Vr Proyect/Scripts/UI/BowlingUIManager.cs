using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BowlingUIManager : MonoBehaviour
{
    [Header("Texto de estado")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI pinText;
    public TextMeshProUGUI stateText;

    [Header("Barra de fuerza")]
    public Image forceBarFill;

    [Header("Configuraci�n")]
    public Color lowForceColor = Color.green;
    public Color highForceColor = Color.red;

    private void Start()
    {
        ResetForceBar();
        UpdateTurn(1);
        UpdateRound(1);
        UpdateState("Grab the ball to start");
    }

    public void UpdateForceBar(float charge, float maxCharge)
    {
        if (forceBarFill == null) return;

        float t = Mathf.Clamp01(charge / maxCharge);
        forceBarFill.fillAmount = t;
        forceBarFill.color = Color.Lerp(lowForceColor, highForceColor, t);
    }

    public void ResetForceBar()
    {
        if (forceBarFill != null)
        {
            forceBarFill.fillAmount = 0f;
            forceBarFill.color = lowForceColor;
        }
    }

    public void UpdatePins(int pin)
    {
        pinText.text = $"Total Pin score: {pin} ";
    }

    public void UpdateTurn(int turn)
    {
        if (turnText != null)
            turnText.text = $"Turn: {turn}";
    }

    public void UpdateRound(int round)
    {
        if (roundText != null)
            roundText.text = $"Round: {round} / 5";
    }

    public void UpdateState(string msg)
    {
        if (stateText != null)
            stateText.text = msg;
    }
}
