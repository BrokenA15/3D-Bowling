using System;
using UnityEngine;

public class DisableWall : MonoBehaviour
{
    public GameObject Wall1;
    public GameObject Wall2;

    private void Start()
    {
        Wall1.SetActive(true);
        Wall2.SetActive(true);
    }

    public void WallDisable()
    {
        Wall1.SetActive(false);
        Wall2.SetActive(false);
    } 
    
    
    
}
