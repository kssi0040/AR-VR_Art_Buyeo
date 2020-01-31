using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance = null;
    private static readonly object padlock = new object();
    public int currentStage;

    public Dictionary<int, GameObject>[] character2DDict;
    public Dictionary<int, GameObject>[] object2DDict;
    public Dictionary<int, GameObject>[] Medal2DDict;
    public Dictionary<int, GameObject> character3DDict = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> object3DDict = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Medal3DDict = new Dictionary<int, GameObject>();
    
    public List<Canvas> CanvasList = new List<Canvas>();

    public AssetBundle myLoadedAssetBundle;

    private SceneLoader()
    {

    }
    
    public static SceneLoader Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance==null)
                {
                    instance = new SceneLoader();
                }
                return instance;
            }
        }
    }

    private void Awake()
    {
        for (int k = 0; k < GameObject.FindObjectsOfType<Canvas>().Length; k++)
        {
            CanvasList.Add(GameObject.FindWithTag(k.ToString()).GetComponent<Canvas>());
        }

        //for(int j=0;j<CanvasList.Count;j++)
        //{
        //    Debug.Log(CanvasList[j]);
        //}

        //string assetBundleDirectory = "Assets/AssetBundles";
        //myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundleDirectory + "/", "에셋번들 이름"));
        //if(myLoadedAssetBundle==null)
        //{
        //    Debug.Log("Fail Load");
        //}
        //else
        //{
        //    GoToScenario();
        //}
        GoToScenario();     
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToScenario()
    {
        LoadCharacter2D();
        LoadObject2D();
        //LoadMedal2D();
        LoadCharacter3D();
        LoadObject3D();
        //LoadMedal3D();
        //LoadBranch();
        for(int k=0;k<object3DDict.Count;k++)
        {
            Debug.Log(object3DDict[k].name);
        }
    }

    void LoadCharacter2D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[currentStage].Count!=0)
        {
            character2DDict = new Dictionary<int, GameObject>[XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount];
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                character2DDict[k] = new Dictionary<int, GameObject>();
                character2DDict[k].Clear();
            }

            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                if(XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[currentStage].Count>k)
                {
                    if(XML_Reader.Instance.scenarioToDict.CreateChar2DDictionary[currentStage,k]!=null)
                    {
                        foreach(int key in XML_Reader.Instance.scenarioToDict.CreateChar2DDictionary[currentStage,k].Keys)
                        {
                            //GameObject assetChar2D = myLoadedAssetBundle.LoadAsset(XML_Reader.Instance.scenarioToDict.CreateChar2DDictionary[currentStage, k][key].Name, typeof(GameObject)) as GameObject;

                            GameObject assetChar2D = Resources.Load(XML_Reader.Instance.scenarioToDict.CreateChar2DDictionary[currentStage, k][key].Name) as GameObject;

                            GameObject prefab = null;

                            prefab = assetChar2D;

                            if(prefab!=null)
                            {
                                GameObject goModel = Instantiate(prefab) as GameObject;
                                goModel.transform.SetParent(CanvasList[XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[currentStage][k].ID].transform.GetChild(0));
                                goModel.name = XML_Reader.Instance.scenarioToDict.CreateChar2DDictionary[currentStage, k][key].Name;
                                character2DDict[k].Add(key, goModel);
                            }
                        }
                    }
                }
            }
        }
    }
    void LoadCharacter3D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[currentStage]!=null)
        {
            if(character3DDict.Count!=0)
            {
                character3DDict.Clear();
            }

            foreach(int key in XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[currentStage].Keys)
            {
                //GameObject assetChar3D = myLoadedAssetBundle.LoadAsset(XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[currentStage][key].Name, typeof(GameObject)) as GameObject;
                GameObject assetChar3D = Resources.Load(XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[currentStage][key].Name) as GameObject;
                GameObject prefab = null;
                prefab = assetChar3D;

                if(prefab!=null)
                {
                    GameObject goModel = Instantiate(prefab) as GameObject;

                    goModel.name = XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[currentStage][key].Name;
                    character3DDict.Add(key, goModel);
                }
            }
        }
    }
    void LoadObject2D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[currentStage].Count!=0)
        {
            object2DDict = new Dictionary<int, GameObject>[XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount];
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                object2DDict[k] = new Dictionary<int, GameObject>();
                object2DDict[k].Clear();
            }
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                if(XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[currentStage].Count>k)
                {
                    if(XML_Reader.Instance.scenarioToDict.CreateObj2DDictionary[currentStage,k]!=null)
                    {
                        foreach(int key in XML_Reader.Instance.scenarioToDict.CreateObj2DDictionary[currentStage,k].Keys)
                        {
                            //GameObject assetObj2D = myLoadedAssetBundle.LoadAsset<GameObject>(XML_Reader.Instance.scenarioToDict.CreateObj2DDictionary[currentStage, k][key].Name);
                            GameObject assetObj2D = Resources.Load(XML_Reader.Instance.scenarioToDict.CreateObj2DDictionary[currentStage, k][key].Name) as GameObject;

                            GameObject prefab = null;
                            prefab = assetObj2D;

                            if(prefab!=null)
                            {
                                GameObject obj = Instantiate(prefab) as GameObject;

                                obj.transform.SetParent(CanvasList[XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[currentStage][k].ID].transform.GetChild(0));
                                obj.name = XML_Reader.Instance.scenarioToDict.CreateObj2DDictionary[currentStage, k][key].Name;
                                object2DDict[k].Add(key, obj);
                            }
                        }
                    }
                }
            }
        }
    }
    void LoadObject3D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[currentStage]!=null)
        {
            if(object3DDict.Count!=0)
            {
                object3DDict.Clear();
            }

            foreach(int key in XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[currentStage].Keys)
            {
                //GameObject assetObj3D = myLoadedAssetBundle.LoadAsset(XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[currentStage][key].Name, typeof(GameObject)) as GameObject;
                GameObject assetObj3D = Resources.Load(XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[currentStage][key].Name) as GameObject;
                GameObject prefab = null;

                prefab = assetObj3D;

                if(prefab!=null)
                {
                    GameObject obj = Instantiate(prefab) as GameObject;

                    obj.name = XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[currentStage][key].Name;
                    object3DDict.Add(key, obj);
                }
            }
        }
    }
    void LoadMedal2D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateMedalType2DDictionary.Count!=0)
        {
            Medal2DDict = new Dictionary<int, GameObject>[XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount];
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                Medal2DDict[k] = new Dictionary<int, GameObject>();
                Medal2DDict[k].Clear();
            }
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[currentStage].CanvasCount;k++)
            {
                if(XML_Reader.Instance.scenarioToDict.CreateMedalType2DDictionary.Count>k)
                {
                    if(XML_Reader.Instance.scenarioToDict.CreateMedal2DDictionary[k]!=null)
                    {
                        foreach(int key in XML_Reader.Instance.scenarioToDict.CreateMedal2DDictionary[k].Keys)
                        {
                            GameObject assetMedal2D = myLoadedAssetBundle.LoadAsset<GameObject>(XML_Reader.Instance.scenarioToDict.CreateMedal2DDictionary[k][key].Name);

                            GameObject prefab = null;
                            prefab = assetMedal2D;

                            if(prefab!=null)
                            {
                                GameObject Medal = Instantiate(prefab) as GameObject;

                                Medal.transform.SetParent(CanvasList[XML_Reader.Instance.scenarioToDict.CreateMedalType2DDictionary[k].ID].transform.GetChild(0));
                                Medal.name = XML_Reader.Instance.scenarioToDict.CreateMedal2DDictionary[k][key].Name;
                                Medal2DDict[k].Add(key, Medal);
                            }
                        }
                    }
                }
            }
        }
    }
    void LoadMedal3D()
    {
        if(XML_Reader.Instance.scenarioToDict.CreateMedal3DDictionary!=null)
        {
            if(Medal3DDict.Count!=0)
            {
                Medal3DDict.Clear();
            }

            foreach(int key in XML_Reader.Instance.scenarioToDict.CreateMedal3DDictionary.Keys)
            {
                GameObject assetMedal3D = myLoadedAssetBundle.LoadAsset(XML_Reader.Instance.scenarioToDict.CreateMedal3DDictionary[key].Name, typeof(GameObject)) as GameObject;

                GameObject prefab = null;
                prefab = assetMedal3D;

                if(prefab!=null)
                {
                    GameObject Medal = Instantiate(prefab) as GameObject;

                    Medal.name = XML_Reader.Instance.scenarioToDict.CreateMedal3DDictionary[key].Name;
                    Medal3DDict.Add(key, Medal);
                }
            }
        }
    }
    //void LoadBranch()
    //{        
    //    PlayerInfo.Instance.BranchDictionary = new Dictionary<int, bool>[XML_Reader.Instance.scenarioToDict.StageSetDictionary[currentStage].branchLists.Length];
    //    for(int k=0;k< XML_Reader.Instance.scenarioToDict.StageSetDictionary[currentStage].branchLists.Length; k++)
    //    {
    //        PlayerInfo.Instance.BranchDictionary[k] = new Dictionary<int, bool>();
    //        PlayerInfo.Instance.BranchDictionary[k].Clear();
    //    }

    //    for(int k=0;k< XML_Reader.Instance.scenarioToDict.StageSetDictionary[currentStage].branchLists.Length; k++)
    //    {
    //        if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[k].branchLists==null)
    //        {

    //        }
    //        for(int j=0;j<PlayerInfo.Instance.BranchDictionary[k].Count;j++)
    //        {
    //            for(int q=0;q<XML_Reader.Instance.scenarioToDict.StageSetDictionary[currentStage].branchLists[j].Count;q++)
    //            {
    //                int key = j;
    //                bool branchTrueFalse = false;
    //                if(!PlayerInfo.Instance.BranchDictionary[k].ContainsKey(key))
    //                {
    //                    PlayerInfo.Instance.BranchDictionary[k].Add(key, branchTrueFalse);
    //                }
    //            }
    //        }
    //    }
    //}    
}
