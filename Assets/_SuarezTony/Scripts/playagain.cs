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
}
