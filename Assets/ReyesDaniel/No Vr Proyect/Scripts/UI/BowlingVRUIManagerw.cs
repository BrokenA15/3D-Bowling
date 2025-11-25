using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BowlingVRUIManagerw : MonoBehaviour
{
    public Text pinText;



    public void UpdatePins(int pin)
    {
        pinText.text = $"Total Pin score: {pin} ";
    }
}
