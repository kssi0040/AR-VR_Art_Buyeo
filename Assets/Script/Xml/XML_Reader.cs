using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class XML_Reader : MonoBehaviour
{
    private static XML_Reader instance = null;
    private static readonly object padlock = new object();
    [HideInInspector]
    public bool readCompleted;

    public ScenarioToDict scenarioToDict = new ScenarioToDict();
    private XML_Reader()
    {

    }

    public static XML_Reader Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance==null)
                {
                    instance = new XML_Reader();
                }
                return instance;
            }
        }
    }

    public string fileName = string.Empty;
    public string filePath = string.Empty;

    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)        
        filePath += (Application.streamingAssetsPath + "/" + fileName);
#elif UNITY_ANDROID
        filePath += Application.persistentDataPath + "/" + fileName;
        
#endif      
        Debug.Log(filePath);
        readCompleted = false;
        //scenarioToDict.Load(filePath);
        //readCompleted = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Process());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Process()
    {
        if (!File.Exists(filePath))
        {
            yield return new WaitUntil(() => FirebaseManager.Instance.GetInit());
            // 파일이 존재하지 않는다면???
            FirebaseManager.Instance.XmlFileDownload(fileName);
            yield return new WaitUntil(() => FirebaseManager.Instance.GetXmlFile());
        }

        WWW www = new WWW(filePath);
        yield return www;
        if(www.isDone)
        {
            scenarioToDict.Load(filePath);
            readCompleted = true;
        }
    }
}
