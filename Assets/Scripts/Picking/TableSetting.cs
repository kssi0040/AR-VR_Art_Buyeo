using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.HelloAR;

public class TableSetting : MonoBehaviour
{
    public TABLE_SET eTableSelect;

    private MeshRenderer mesnRender;
    private HelloARController helloAr = null;

    private bool bCurry = false;
    private bool bRiceNoodle = false;
    private bool bChinaNoodle = false;
    private bool bMisoSoup = false;
    private bool bSetting = false;
    private bool bSettle = false;


    // Start is called before the first frame update
    void Start()
    {
        bSetting = true;
        mesnRender = this.transform.GetComponent<MeshRenderer>();
        helloAr =  FindObjectOfType(typeof(HelloARController)) as HelloARController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingPosition()
    {
        bSetting = true;
    }

    //============================================== COLLIDER  ==================================================//
    private void OnTriggerEnter(Collider other)
    {        
        if (TABLE_SET.CURRY == eTableSelect)
        {
            if (other.tag == "Curry")                            
                bCurry = true;            
        }
        else if(TABLE_SET.RICE_NOODLE == eTableSelect)
        {
            if (other.tag == "RiceNoodle")                            
                bRiceNoodle = true;            
        }
        else if (TABLE_SET.CHINA_NOODLE == eTableSelect)
        {
            if (other.tag == "ChinaNoodle")                            
                bChinaNoodle = true;            
        }
        else if (TABLE_SET.MISO_SOUP == eTableSelect)
        {
            if (other.tag == "MisoSoup")                            
                bMisoSoup = true;            
        }
    }
    private void OnTriggerStay(Collider other)
    {        
        if (TABLE_SET.CURRY == eTableSelect)
        {
            if (other.tag == "Curry")
            {                
                bCurry = true;

                bSetting = helloAr.bTouchCurry;
                if (true == bSetting)
                {
                    float posY = other.gameObject.transform.localPosition.y;
                    other.gameObject.transform.localPosition = new Vector3(this.transform.position.x, posY, this.transform.position.z);
                    mesnRender.enabled = false;                    
                }
                else if (false == bSetting)
                {
                    mesnRender.enabled = true;                    
                }
            }
        }
        else if (TABLE_SET.RICE_NOODLE == eTableSelect)
        {
            if (other.tag == "RiceNoodle")
            {
                //Debug.Log("rice in...");
                bRiceNoodle = true;

                //bSetting = helloAr.bTouchEnd;
                bSetting = helloAr.bTouchRiceNoodle;
                if (true == bSetting)
                {
                    float posY = other.gameObject.transform.localPosition.y;
                    other.gameObject.transform.localPosition = new Vector3(this.transform.position.x, posY, this.transform.position.z);
                    mesnRender.enabled = false;
                }
                else if (false == bSetting)
                {
                    mesnRender.enabled = true;
                }
            }
        }
        else if (TABLE_SET.CHINA_NOODLE == eTableSelect)
        {
            if (other.tag == "ChinaNoodle")
            {
                //Debug.Log("rice in...");
                bChinaNoodle = true;

                //bSetting = helloAr.bTouchEnd;
                bSetting = helloAr.bTouchChinaNoodle;
                if (true == bSetting)
                {
                    float posY = other.gameObject.transform.localPosition.y;
                    other.gameObject.transform.localPosition = new Vector3(this.transform.position.x, posY, this.transform.position.z);
                    mesnRender.enabled = false;
                }
                else if (false == bSetting)
                {
                    mesnRender.enabled = true;
                }
            }
        }
        else if (TABLE_SET.MISO_SOUP == eTableSelect)
        {
            if (other.tag == "MisoSoup")
            {
                //Debug.Log("rice in...");
                bMisoSoup = true;

                //bSetting = helloAr.bTouchEnd;
                bSetting = helloAr.bTouchMisoSoup;
                if (true == bSetting)
                {
                    float posY = other.gameObject.transform.localPosition.y;
                    other.gameObject.transform.localPosition = new Vector3(this.transform.position.x, posY, this.transform.position.z);
                    mesnRender.enabled = false;
                }
                else if (false == bSetting)
                {
                    mesnRender.enabled = true;
                }
            }
        }




    }
    private void OnTriggerExit(Collider other)
    {        
        if (TABLE_SET.CURRY == eTableSelect)
        {
            if (other.tag == "Curry")
            {                
                bCurry = false;
                mesnRender.enabled = true;
            }
        }
        else if (TABLE_SET.RICE_NOODLE == eTableSelect)
        {
            if (other.tag == "RiceNoodle")
            {                
                bRiceNoodle = false;
                mesnRender.enabled = true;
            }
        }
        else if (TABLE_SET.CHINA_NOODLE == eTableSelect)
        {
            if (other.tag == "ChinaNoodle")
            {                
                bChinaNoodle = false;
                mesnRender.enabled = true;
            }
        }
        else if (TABLE_SET.MISO_SOUP == eTableSelect)
        {
            if (other.tag == "MisoSoup")
            {                
                bMisoSoup = false;
                mesnRender.enabled = true;
            }
        }
    }

    //============================================== GETTER ==================================================//
    public bool GetSettle(TABLE_SET _eTableSet)
    {
        bool bCheck = false;
        if (TABLE_SET.CURRY == _eTableSet)
            bCheck = bCurry;
        else if (TABLE_SET.RICE_NOODLE == _eTableSet)
            bCheck = bRiceNoodle;
        else if (TABLE_SET.CHINA_NOODLE == _eTableSet)
            bCheck = bChinaNoodle;
        else if (TABLE_SET.MISO_SOUP == _eTableSet)
            bCheck = bMisoSoup;

        return bCheck;
    }

}
