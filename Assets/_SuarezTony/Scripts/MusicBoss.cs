using System.Collections;
using UnityEngine;

public class MusicBoss : MonoBehaviour
{
    public GameObject musicaBoss;
    public GameObject musicaVictory;
    public GameObject gritoBoss;
    
    public BossManager manager;
    void Start()
    {
        musicaBoss.SetActive(true);
        musicaVictory.SetActive(false);
        gritoBoss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.currentHealth <= 0)
        {

            StartCoroutine(PlayVictory());
        }
       
    }

  
    
    private IEnumerator PlayVictory()
    {
        gritoBoss.SetActive(true);
        yield return new WaitForSeconds(8);
        musicaBoss.SetActive(false);
        musicaVictory.SetActive(true);
    }
}
