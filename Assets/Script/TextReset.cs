using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {        
        foreach (Transform child in this.transform)
        {
            if(child.GetComponent<InputField>()!=null)
            {
                child.GetComponent<InputField>().text = string.Empty;
            }            
        }
    }   
}
