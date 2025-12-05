using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   // ← IMPORTANTE para el nuevo Input System

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Canvas o panel raíz del menú de pausa")]
    public GameObject pauseCanvas;

    [Header("Escenas")]
    [Tooltip("Nombre de la escena de menú principal")]
    public string menuSceneName = "VR o NoVR";

    [Header("UINormal")]
    public GameObject gameplayUI;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }

    void Update()
    {
        // Nuevo Input System
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);


        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        isPaused = false;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(menuSceneName);
    }
}
