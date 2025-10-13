using UnityEngine;
using UnityEngine.SceneManagement;


public class ManagerBotones : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string vrScene = "VR";
    [SerializeField] private string noVrSceneName = "NoVRDaniel";

    public void vrReiniciar()
    {
        SceneManager.LoadScene(vrScene);
    }

    public void NoVRReiniciar()
    {
        SceneManager.LoadScene(noVrSceneName);
    }
    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliste");
    }

}
