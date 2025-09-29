using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = false;

    void Start()
    {
        wristUI.SetActive(false); // Al inicio desactivado
    }

    public void PauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayWristUI();
        }
    }

    public void DisplayWristUI()
    {
        activeWristUI = !activeWristUI; // invertir el estado

        if (activeWristUI)
        {
            wristUI.SetActive(true);
            Time.timeScale = 0; // pausar
        }
        else
        {
            wristUI.SetActive(false);
            Time.timeScale = 1; // reanudar
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // importante, reanudar antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
