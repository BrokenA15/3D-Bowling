using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRSceneLoader : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string vrSceneName = "VR";
    [SerializeField] private string noVrSceneName = "NoVRDaniel";

    private void Start()
    {
            
        if(XRSettings.isDeviceActive)
        {
            SceneManager.LoadScene(vrSceneName);
        }
        else
        {
            SceneManager.LoadScene(noVrSceneName);
        }


    }
}
