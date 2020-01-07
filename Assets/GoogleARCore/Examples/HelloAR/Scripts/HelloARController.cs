//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;    
    using UnityEngine.SceneManagement;
    using PublicDefine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject AndyPointPrefab;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        //==========================================================================================================//
        //==================================================== 추가됨 =================================================//
        //==========================================================================================================//
        //public PlaneDiscoveryGuide TestSc;
        //public GameObject testObj;
        //public UIManager uiManager;
        public GameObject CloneParent;
        public GameObject foodParent;

        public Canvas TableSettingUI;
        //public GameObject SelectUI;

        public GameObject tablePrefabs;
        public GameObject curryPrefabs;
        public GameObject riceNoodlePrefabs;
        public GameObject chinaNoodlePrefabs;
        public GameObject misoSoupPrefabs;

        // test.. 정리해야 함....
        public Text selectPartText;        
        //public GameObject curryEatingObject;
        //public GameObject riceNoodleEatingObject;
        //public GameObject chinaNoodleEatingObject;
        //public GameObject misoSoupEatingObject;
        //public GameObject tableObject;
        //public GameObject chairObject;
        public StagePlay m_StagePlay;

        //public GameObject selectObjectParent;
        private GameObject holdingTarget = null;
        private bool bHolding = false;
        //private bool bCheckClick = false;

        // 테이블과 기타 등등...
        private TABLE_SET eTableSet = TABLE_SET.NONE;
        private bool bTable = false;
        private bool bCurry = false;
        private bool bRiceNoodle = false;
        private bool bChinaNoodle = false;
        private bool bMisoSoup = false;
        private ROUTE eRoute = ROUTE.TABLE_SETTING;

        private GameObject[] selectObjects = new GameObject[4];
        private GameObject[] aSelectObjects = new GameObject[4];
        private bool bReset = false;

        private TableNavi tableNaviSc;
                
        // 흠... 아무래도.. 나누어야 할 듯....
        [HideInInspector] public bool bTouchEnd = false;
        [HideInInspector] public bool bTouchCurry = false;
        [HideInInspector] public bool bTouchRiceNoodle = false;
        [HideInInspector] public bool bTouchChinaNoodle = false;
        [HideInInspector] public bool bTouchMisoSoup = false;

        void Start()
        {
            bReset = false;
            m_StagePlay = FindObjectOfType<StagePlay>();
            for(int i = 0; i < aSelectObjects.Length; ++i)
            {
                aSelectObjects[i] = foodParent.transform.GetChild(i).gameObject;                    
            }

            //for(int k=0;k<m_StagePlay.sceneLoader.object3DDict.Count;k++)
            //{
            //    switch(m_StagePlay.sceneLoader.object3DDict[k].name)
            //    {
            //        case "Viet_food_01":
            //            curryEatingObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //        case "Viet_nodle_01":
            //            riceNoodleEatingObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //        case "Viet_nodle_02":
            //            chinaNoodleEatingObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //        case "Viet_food_02":
            //            misoSoupEatingObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //        case "table":
            //            tableObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //        case "chair_1":
            //            chairObject = m_StagePlay.sceneLoader.object3DDict[k];
            //            break;
            //    }
            //}
            TableSettingUI.gameObject.SetActive(true);
        }
        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();
            // If the player has not touched the screen, we are done with this update.            
            //Touch touch;
            /*
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }  
            */
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Should not handle input if the player is pointing on UI.
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }
            }
            
            if (ROUTE.TABLE_SETTING == eRoute)
            {
                if (TABLE_SET.TABLE == eTableSet)
                {
                    if (false == bTable)
                        MakeObject();
                    else if (true == bTable)
                        DragObject();
                }
                else if (TABLE_SET.CURRY == eTableSet)
                {
                    if (false == bCurry)
                        MakeObject();
                    else if (true == bCurry)
                        DragObject();
                }
                else if (TABLE_SET.RICE_NOODLE == eTableSet)
                {
                    if (false == bRiceNoodle)
                        MakeObject();
                    else if (true == bRiceNoodle)
                        DragObject();
                }
                else if (TABLE_SET.CHINA_NOODLE == eTableSet)
                {
                    if (false == bChinaNoodle)
                        MakeObject();
                    else if (true == bChinaNoodle)
                        DragObject();
                }
                else if (TABLE_SET.MISO_SOUP == eTableSet)
                {
                    if (false == bMisoSoup)
                        MakeObject();
                    else if (true == bMisoSoup)
                        DragObject();
                }
            }
            else if (ROUTE.SELECT_FOOD == eRoute)
            {
                ObjectSelect();                                                     // 테이블 차리기 끝.. Object Select....                               
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }
        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }

        //=======================================================================================================//
        //================================================ 추가된 것 ================================================//
        //=======================================================================================================//
        // Object 생성..
        void MakeObject()
        {
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                return;

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Choose the Andy model for the Trackable that got hit.                   
                    GameObject andyObject = MakeTableObject(eTableSet, hit.Pose.position, hit.Pose.rotation);
                    if (null == andyObject)
                        return;

                    // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                    andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    //var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    // Make Andy model a child of the anchor.
                    //andyObject.transform.parent = anchor.transform;
                    andyObject.transform.parent = CloneParent.transform;                    
                }
            }
        }
        // Object 생성 후 터치 선택한 이후,, 드래그해서 Object 이동....
        void DragObject()
        {
            if (Input.touchCount == 1)
            {
                if (true == bHolding)
                {
                    Touch touch = Input.GetTouch(0);
                    TrackableHit hit;
                    TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

                    if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                    {
                        if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                        {
                            Debug.Log("Hit at back of the current DetectedPlane");
                        }
                        else
                        {                            
                            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)                            
                            {                                
                                holdingTarget.transform.localPosition = new Vector3(hit.Pose.position.x, hit.Pose.position.y, hit.Pose.position.z);

                                if (TABLE_SET.CURRY == eTableSet)
                                    bTouchCurry = false;
                                else if (TABLE_SET.RICE_NOODLE == eTableSet)
                                    bTouchRiceNoodle = false;
                                else if (TABLE_SET.CHINA_NOODLE == eTableSet)
                                    bTouchChinaNoodle = false;
                                else if (TABLE_SET.MISO_SOUP == eTableSet)
                                    bTouchMisoSoup = false;

                                bTouchEnd = false;
                            }
                            else if(touch.phase == TouchPhase.Ended)
                            {                                
                                bTouchEnd = true;

                                if (TABLE_SET.CURRY == eTableSet)
                                    bTouchCurry = true;
                                else if (TABLE_SET.RICE_NOODLE == eTableSet)
                                    bTouchRiceNoodle = true;
                                else if (TABLE_SET.CHINA_NOODLE == eTableSet)
                                    bTouchChinaNoodle = true;
                                else if (TABLE_SET.MISO_SOUP == eTableSet)
                                    bTouchMisoSoup = true;
                            }                            
                        }
                    }
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = FirstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position);

                    int layer_mask = -1;
                    if (TABLE_SET.TABLE == eTableSet)
                    {
                        layer_mask = LayerMask.GetMask("Table");
                    }
                    else if (TABLE_SET.CURRY == eTableSet)
                    {
                        layer_mask = LayerMask.GetMask("Curry");
                    }
                    else if (TABLE_SET.RICE_NOODLE == eTableSet)
                    {
                        layer_mask = LayerMask.GetMask("Rice_Noodle");
                    }
                    else if (TABLE_SET.CHINA_NOODLE == eTableSet)
                    {
                        layer_mask = LayerMask.GetMask("China_Noodle");
                    }
                    else if (TABLE_SET.MISO_SOUP == eTableSet)
                    {
                        layer_mask = LayerMask.GetMask("Miso_Soup");
                    }
                    
                    bool bCheck = Physics.Raycast(ray, out hit, 30f, layer_mask);
                    if (true == bCheck)
                    {
                        holdingTarget = hit.collider.gameObject;
                        bHolding = true;
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    bHolding = false;
                    //holdingTarget = null;
                }
            }
            // 일단 투 터치는 막아놓는다.... 스케일 관련해서는 일단 다 막는다.
            /*
            else if (Input.touchCount == 2)                          // 투 터치 일 경우...
            {
                if (null != holdingTarget)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchZero.position - touchZero.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // 위에서 길이가 나옴....                                            
                    deltaMagnitudeDiff = Mathf.Abs(deltaMagnitudeDiff);
                    //float fOriginScaleX = holdingTarget.transform.localScale.x;
                    //float fOriginScaleY = holdingTarget.transform.localScale.y;
                    //float fOriginScaleZ = holdingTarget.transform.localScale.z;
                    // 0.2 ~ 0.6 까지 제한...                    
                    //float fChangeScale = Mathf.Clamp((deltaMagnitudeDiff / 2000), 0.05f, 0.25f);
                    float fChangeScale = Mathf.Clamp((deltaMagnitudeDiff / 1500), 0.2f, 0.6f);                    

                    //float fValue = fDiffMag - deltaMagnitudeDiff;

                    // 아래는 수정 예정... 현재 크기에서 더해지거나 마이너스 되는 방식....
                    holdingTarget.transform.localScale = new Vector3(fChangeScale, fChangeScale, fChangeScale);                                    
                    // test....
                    //holdingTarget.transform.localScale = new Vector3(fOriginScaleX + fDiffMag, fOriginScaleY + fDiffMag, fOriginScaleZ + fDiffMag);
                    //holdingTarget.transform.localScale = new Vector3(fOriginScaleX+ fChangeScale, fChangeScale+ fChangeScale, fChangeScale+fChangeScale);                    
                }
            }            
            */
        }

        // table 셋팅이 끝난 후에 클릭..
        void ObjectSelect()
        {
            if (Input.touchCount == 1)
            {
                RaycastHit hit;
                Ray ray = FirstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position);
                //Ray ray = FirstPersonCamera.ScreenPointToRay(Input.mousePosition);
                bool bCheck = Physics.Raycast(ray, out hit, 30f);
                if (true == bCheck)
                {
                    int iLayer = hit.collider.gameObject.layer;                    
                    if (11 == iLayer)                                                                                               // curry 선택시에
                    {                        
                        //uiManager.SetTestText("Curry Select");
                        CurrySelect();
                    }
                    else if (12 == iLayer)                                                                                    // Rice Noodle 선택시...
                    {
                        //uiManager.SetTestText("Rice Noodle Select");
                        RiceNoodleSelect();
                    }
                    else if (13 == iLayer)                                                                                      // China Noodle 선택시..
                    {                        
                        //uiManager.SetTestText("China Noodle Select");
                        ChinaNoodleSelect();
                    }
                    else if (14 == iLayer)                                                                                         // Miso Soup 선택시...
                    {                        
                        //uiManager.SetTestText("Miso Soup Select");
                        MisoSoupSelect();
                    }
                }
            }
        }

        
        
        public void ResetTableSetting()
        {
            // 테이블 부터 전부 삭제하고,, 초기화한다.            
            List<int> index = new List<int>();
            for(int k=0;k<CloneParent.transform.childCount;k++)
            {
                for(int j=0;j<m_StagePlay.sceneLoader.object3DDict.Count;j++)
                {
                    if(CloneParent.transform.GetChild(k).name==m_StagePlay.sceneLoader.object3DDict[j].name)
                    {
                        index.Add(j);
                    }
                }
            }
            for(int k=0;k<index.Count;k++)
            {
                m_StagePlay.sceneLoader.object3DDict.Remove(index[k]);
            }
            for (int i = 0; i < CloneParent.transform.childCount; ++i)
            {
                Destroy(CloneParent.transform.GetChild(i).gameObject);
            }
            for (int k=0;k<m_StagePlay.sceneLoader.object3DDict.Count;k++)
            {
                Debug.Log(k);
                Debug.Log(m_StagePlay.sceneLoader.object3DDict[k].name);
            }
            Debug.Log(eTableSet);
            bTable = false;
            bCurry = false;
            bRiceNoodle = false;
            bChinaNoodle = false;
            bMisoSoup = false;
            eTableSet=TABLE_SET.NONE;
            index.Clear();
            Debug.Log(eTableSet);
        }

        //=======================================================================================================//
        //============================================== OBJECT SELECT ==============================================//
        //=======================================================================================================//
        void CurrySelect()
        {
            // 카레를 선택했을 때,, 대사 등의 event 처리 해야함...
            //CloneParent.SetActive(false);            
            for (int i = 0; i < aSelectObjects.Length; ++i)           
                aSelectObjects[i].SetActive(false);
            
            //aSelectObjects[0].SetActive(true);
            // 그리고 버튼.... 등장해야 함....            
            //TableSettingUI.SetActive(false);
            //SelectUI.SetActive(true);
            //eRoute = ROUTE.SELECT_FOOD;
            //eTableSet = TABLE_SET.CURRY;            
            m_StagePlay.Index = 8;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            m_StagePlay.StageSet();
        }
        void RiceNoodleSelect()
        {
            // 카레를 선택했을 때,, 대사 등의 event 처리 해야함...
            //CloneParent.SetActive(false);
            for (int i = 0; i < aSelectObjects.Length; ++i)
                aSelectObjects[i].SetActive(false);

            //aSelectObjects[1].SetActive(true);
            // 그리고 버튼.... 등장해야 함....            
            //TableSettingUI.SetActive(false);
            //SelectUI.SetActive(true);
            //eTableSet = TABLE_SET.RICE_NOODLE;
            Debug.Log("Rice");
            m_StagePlay.Index = 20;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            m_StagePlay.StageSet();
        }
        void ChinaNoodleSelect()
        {
            // 카레를 선택했을 때,, 대사 등의 event 처리 해야함...
            //CloneParent.SetActive(false);
            for (int i = 0; i < aSelectObjects.Length; ++i)
                aSelectObjects[i].SetActive(false);

            //aSelectObjects[2].SetActive(true);
            // 그리고 버튼.... 등장해야 함....            
            //TableSettingUI.SetActive(false);
            //SelectUI.SetActive(true);
            //eTableSet = TABLE_SET.CHINA_NOODLE;
            m_StagePlay.Index = 35;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            m_StagePlay.StageSet();
        }
        void MisoSoupSelect()
        {
            // 카레를 선택했을 때,, 대사 등의 event 처리 해야함...
            //CloneParent.SetActive(false);
            for (int i = 0; i < aSelectObjects.Length; ++i)
                aSelectObjects[i].SetActive(false);

            //aSelectObjects[3].SetActive(true);
            // 그리고 버튼.... 등장해야 함....            
            //TableSettingUI.SetActive(false);
            //SelectUI.SetActive(true);
            //eTableSet = TABLE_SET.MISO_SOUP;
            m_StagePlay.Index = 44;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            m_StagePlay.StageSet();
        }

        //=======================================================================================================//
        //============================================ TABLE SETTING BUTTON ==========================================//
        //=======================================================================================================//

        public void TableButtonEvent()
        {
            if (ROUTE.TABLE_SETTING != eRoute)
                return;
            
            eTableSet = TABLE_SET.TABLE;
            //uiManager.SetTextNotice(eTableSet);
        }
        public void Food1ButtonEvent()
        {
            if (ROUTE.TABLE_SETTING != eRoute)
                return;
            if (false == bTable)
                return;

            eTableSet = TABLE_SET.CURRY;
            //uiManager.SetTextNotice(eTableSet);
            tableNaviSc.SetTableObject(TABLE_SET.CURRY);            
        }
        public void Food2ButtonEvent()
        {
            if (ROUTE.TABLE_SETTING != eRoute)
                return;
            if (false == bTable)
                return;

            eTableSet = TABLE_SET.RICE_NOODLE;
            //uiManager.SetTextNotice(eTableSet);
            tableNaviSc.SetTableObject(TABLE_SET.RICE_NOODLE);            
        }
        public void Food3ButtonEvent()
        {
            if (ROUTE.TABLE_SETTING != eRoute)
                return;
            if (false == bTable)
                return;

            eTableSet = TABLE_SET.CHINA_NOODLE;
            //uiManager.SetTextNotice(eTableSet);
            tableNaviSc.SetTableObject(TABLE_SET.CHINA_NOODLE);            
        }
        public void Food4ButtonEvent()
        {
            if (ROUTE.TABLE_SETTING != eRoute)
                return;
            if (false == bTable)
                return;

            eTableSet = TABLE_SET.MISO_SOUP;
            //uiManager.SetTextNotice(eTableSet);
            tableNaviSc.SetTableObject(TABLE_SET.MISO_SOUP);            
        }
        public void ResetButtonEvent()
        {
            // 그냥 Scene을 다시 로드한다. 이것이 좀 더 편리한 방법일 듯...
            if (true == bReset)
                return;

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            m_StagePlay.StageSet();
            eTableSet = TABLE_SET.NONE;
            bReset = true;
        }
        public void TableSettingComplete()
        {            
            if(true == bTable && true == bCurry && true == bRiceNoodle && true == bChinaNoodle && true == bMisoSoup)
            {
                bool bCheckCurry = false;
                bool bCheckRiceNoodle = false;
                bool bCheckChinaNoodle = false;
                bool bCheckMisoSoup = false;

                bCheckCurry = tableNaviSc.GetSettle(TABLE_SET.CURRY);
                bCheckRiceNoodle = tableNaviSc.GetSettle(TABLE_SET.RICE_NOODLE);
                bCheckChinaNoodle = tableNaviSc.GetSettle(TABLE_SET.CHINA_NOODLE);
                bCheckMisoSoup = tableNaviSc.GetSettle(TABLE_SET.MISO_SOUP);

                //uiManager.SetTestText("rice noodle: " + bCheckRiceNoodle.ToString());
                
                if (true == bCheckCurry && true == bCheckRiceNoodle && true == bCheckChinaNoodle && true == bCheckMisoSoup)
                {
                    // 이러면 OK...
                    eRoute = ROUTE.SELECT_FOOD;
                    PlayerInfo.Instance.isComplite = true;
                    Debug.Log("player:"+PlayerInfo.Instance.isComplite);
                    m_StagePlay.forwardDown();
                    //uiManager.SetTestText("테이블 차리기 끝");
                }
                else
                {
                    Debug.Log("Failed");
                }
            }
            else
            {
                //uiManager.SetTestText("테이블을 다 차려야 합니다.");
            }
        }

        //=======================================================================================================//
        //============================================ TABLE SELECT BUTTON ==========================================//
        //=======================================================================================================//        
        public void BackButtonEvent()
        {                        
            CloneParent.SetActive(true);
            for (int i = 0; i < aSelectObjects.Length; ++i)
                aSelectObjects[i].SetActive(false);

            //TableSettingUI.SetActive(true);
            //SelectUI.SetActive(false);

            //rightHand.SetActive(false);

            //curryEatingObject.SetActive(false);
            //riceNoodleEatingObject.SetActive(false);
            //chinaNoodleEatingObject.SetActive(false);
            //misoSoupEatingObject.SetActive(false);
            //tableObject.SetActive(false);
            //chairObject.SetActive(false);
        }
        public void LeftHandButtonEvent()
        {
            
        }
        public void RightHandButtonEvent()
        {            
            if(TABLE_SET.CURRY ==  eTableSet)
            {
                aSelectObjects[0].SetActive(false);
                //curryEatingObject.SetActive(true);
                //tableObject.SetActive(true);
                //chairObject.SetActive(true);
            }
        }
        public void ForkButtonEvent()
        {
            selectPartText.text = "fork";
        }
        public void ChopstickButtonEvent()
        {            
            if (TABLE_SET.RICE_NOODLE == eTableSet)
            {
                // 젓가락...
                aSelectObjects[1].SetActive(false);
                //riceNoodleEatingObject.SetActive(true);
                //tableObject.SetActive(true);
                //chairObject.SetActive(true);
            }

            if (TABLE_SET.CHINA_NOODLE == eTableSet)
            {
                // 젓가락...
                aSelectObjects[2].SetActive(false);
                //chinaNoodleEatingObject.SetActive(true);
                //tableObject.SetActive(true);
                //chairObject.SetActive(true);
            }
        }
        public void SpoonButtonEvent()
        {
            if (TABLE_SET.MISO_SOUP == eTableSet)
            {
                // 숟가락....
                aSelectObjects[3].SetActive(false);
                //misoSoupEatingObject.SetActive(true);
                //tableObject.SetActive(true);
                //chairObject.SetActive(true);
            }
        }

        //=======================================================================================================//
        //================================================= GETTER ================================================//
        //=======================================================================================================//

        GameObject MakeTableObject(TABLE_SET _eTmpTableSet, Vector3 _pos, Quaternion _rot)
        {
            TABLE_SET eTmpTableSet = _eTmpTableSet;
            GameObject tableObject = null;

            if (TABLE_SET.TABLE == eTmpTableSet && false == bTable)
            {
                tableObject = Instantiate(tablePrefabs, _pos, _rot);
                tableObject.name = tablePrefabs.name;
                tableNaviSc = tableObject.transform.GetComponent<TableNavi>();                  // script 연결....
                bTable = true;
                if(!m_StagePlay.sceneLoader.object3DDict.ContainsValue(tableObject))
                {
                    Debug.Log("success");
                    m_StagePlay.sceneLoader.object3DDict.Add(m_StagePlay.sceneLoader.object3DDict.Count, tableObject);
                }
            }
            else if (TABLE_SET.CURRY == eTmpTableSet && false == bCurry)
            {
                tableObject = Instantiate(curryPrefabs, _pos, _rot);
                tableObject.name = curryPrefabs.name;
                bCurry = true;
                if (!m_StagePlay.sceneLoader.object3DDict.ContainsValue(tableObject))
                {
                    Debug.Log("success2");
                    m_StagePlay.sceneLoader.object3DDict.Add(m_StagePlay.sceneLoader.object3DDict.Count, tableObject);
                }
            }
            else if (TABLE_SET.RICE_NOODLE == eTmpTableSet && false == bRiceNoodle)
            {
                tableObject = Instantiate(riceNoodlePrefabs, _pos, _rot);
                tableObject.name = riceNoodlePrefabs.name;
                bRiceNoodle = true;
                if (!m_StagePlay.sceneLoader.object3DDict.ContainsValue(tableObject))
                {
                    Debug.Log("success3");
                    m_StagePlay.sceneLoader.object3DDict.Add(m_StagePlay.sceneLoader.object3DDict.Count, tableObject);
                }
            }
            else if (TABLE_SET.CHINA_NOODLE == eTmpTableSet && false == bChinaNoodle)
            {
                tableObject = Instantiate(chinaNoodlePrefabs, _pos, _rot);
                tableObject.name = chinaNoodlePrefabs.name;
                bChinaNoodle = true;
                if (!m_StagePlay.sceneLoader.object3DDict.ContainsValue(tableObject))
                {
                    Debug.Log("success4");
                    m_StagePlay.sceneLoader.object3DDict.Add(m_StagePlay.sceneLoader.object3DDict.Count, tableObject);
                }
            }
            else if (TABLE_SET.MISO_SOUP == eTmpTableSet && false == bMisoSoup)
            {
                tableObject = Instantiate(misoSoupPrefabs, _pos, _rot);
                tableObject.name = misoSoupPrefabs.name;
                bMisoSoup = true;
                if (!m_StagePlay.sceneLoader.object3DDict.ContainsValue(tableObject))
                {
                    Debug.Log("success5");
                    m_StagePlay.sceneLoader.object3DDict.Add(m_StagePlay.sceneLoader.object3DDict.Count, tableObject);
                }
            }
            return tableObject;            
        }

    }    
}
