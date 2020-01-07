using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableNavi : MonoBehaviour
{
    // 총 4개....
    // 1.. 커리
    // 2..  쌀국수
    // 3..장수면
    // 4.. 청국장..
    private TABLE_SET eTableSet = TABLE_SET.NONE;
    private GameObject[] aTableObjects = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < aTableObjects.Length; ++i)
        {
            aTableObjects[i] = this.transform.GetChild(i+1).gameObject;
            aTableObjects[i].SetActive(false);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetTableObject(TABLE_SET _eTableSet)
    {
        for (int i = 1; i < aTableObjects.Length; ++i)
            aTableObjects[i].SetActive(false);
        
        if(_eTableSet == TABLE_SET.CURRY)
            aTableObjects[0].SetActive(true);
        else if(_eTableSet == TABLE_SET.RICE_NOODLE)
            aTableObjects[1].SetActive(true);
        else if (_eTableSet == TABLE_SET.CHINA_NOODLE)
            aTableObjects[2].SetActive(true);
        else if (_eTableSet == TABLE_SET.MISO_SOUP)
            aTableObjects[3].SetActive(true);
    }    

    public bool GetSettle(TABLE_SET _eTableSet)
    {
        bool bCheck = false;
        if (_eTableSet == TABLE_SET.CURRY)
            bCheck = aTableObjects[0].transform.GetComponent<TableSetting>().GetSettle(_eTableSet);
        else if (_eTableSet == TABLE_SET.RICE_NOODLE)
            bCheck = aTableObjects[1].transform.GetComponent<TableSetting>().GetSettle(_eTableSet);
        else if (_eTableSet == TABLE_SET.CHINA_NOODLE)
            bCheck = aTableObjects[2].transform.GetComponent<TableSetting>().GetSettle(_eTableSet);
        else if (_eTableSet == TABLE_SET.MISO_SOUP)
            bCheck = aTableObjects[3].transform.GetComponent<TableSetting>().GetSettle(_eTableSet);

        return bCheck;
    }
}
