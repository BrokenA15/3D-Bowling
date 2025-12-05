using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panel Faders")]
    public PanelFader MainMenu;
    public PanelFader Opciones;
    public PanelFader Creditos;
    public GameObject musicaMenu;

    void Start()
    {
        Cursor.visible = true;
        musicaMenu.SetActive(true);
        ShowMainMenu();
    }

    // Mostrar men� principal con fade
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
        musicaMenu.SetActive(false);
    }

    // Mostrar opciones con fade
    public void ShowOptions()
    {
        MainMenu.Hide();
        Opciones.Show();
        Creditos.Hide();
    }

    // Mostrar cr�ditos con fade
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
