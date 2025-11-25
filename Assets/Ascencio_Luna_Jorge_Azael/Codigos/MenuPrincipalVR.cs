using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalVR : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject menuPrincipal;
    public GameObject menuOpciones;

    public void PlayGame()
    {
        // Carga la siguiente escena del Build
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }

    public void OpenOptions()
    {
        menuPrincipal.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void BackToMenu()
    {
        menuOpciones.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void ExitGame()
    {
        // Solo funciona en build
        Application.Quit();
    }
}
