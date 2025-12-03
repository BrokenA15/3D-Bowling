using UnityEngine;
using UnityEngine.SceneManagement;

public class playagain : MonoBehaviour
{
  public string nombreescena;

  public void ChangeScene()
  {
    Debug.Log("lasfjsgk");
    SceneManager.LoadScene(nombreescena);
  }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
