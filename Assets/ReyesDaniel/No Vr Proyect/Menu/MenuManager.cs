using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panel Faders")]
    public PanelFader MainMenu;
    public PanelFader Opciones;
    public PanelFader Creditos;

    void Start()
    {
        ShowMainMenu();
    }

    // Mostrar menú principal con fade
    public void ShowMainMenu()
    {
        MainMenu.Show();
        Opciones.Hide();
        Creditos.Hide();
    }

    // Iniciar juego
    public void StartGame()
    {
        SceneManager.LoadScene("NoVRDaniel");
    }

    // Mostrar opciones con fade
    public void ShowOptions()
    {
        MainMenu.Hide();
        Opciones.Show();
        Creditos.Hide();
    }

    // Mostrar créditos con fade
    public void ShowCredits()
    {
        MainMenu.Hide();
        Opciones.Hide();
        Creditos.Show();
    }

    public void GoBack()
    {
        Debug.Log("GoBack() called on MenuManager: ");
        ShowMainMenu();
    }


    // Salir del juego
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
