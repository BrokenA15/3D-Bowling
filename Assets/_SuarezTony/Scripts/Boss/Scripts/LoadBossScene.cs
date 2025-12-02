using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBossScene : MonoBehaviour
{
    public string nameBossScene;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nameBossScene);
        }
    }
}
