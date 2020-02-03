using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ArModeButtonEvent()
    {
        Debug.Log("Ar Mode");
        SceneManager.LoadScene("AR_Mode");
    }
    public void GPSModeButtonEvent()
    {
        Debug.Log("GPS Mode");
        SceneManager.LoadScene("GPS_Scene");
    }
    public void ArgumentImageButtonEvent()
    {
        Debug.Log("Argument Mode");
        SceneManager.LoadScene("AugmentedImage_Mode");
    }

}
