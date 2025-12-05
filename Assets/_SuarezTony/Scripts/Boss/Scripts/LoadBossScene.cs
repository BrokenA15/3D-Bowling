using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBossScene : MonoBehaviour
{
    public string nameBossScene;
    public GameObject musicaGame;

    void Start()
    {
        musicaGame.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            musicaGame.SetActive(false);

            SceneManager.LoadScene(nameBossScene);
        }
    }
}
