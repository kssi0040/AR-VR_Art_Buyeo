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

namespace GoogleARCore.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Rendering;
    using PublicDefine;

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
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject TrackedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyAndroidPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;


        //==========================================================================================================//
        //==================================================== 추가됨 =================================================//
        //==========================================================================================================//
        public GameObject CloneParent;
        public UIManager uiManager;

        public GameObject tablePrefabs;
        public GameObject curryPrefabs;        
        public GameObject riceNoodlePrefabs;
        public GameObject chinaNoodlePrefabs;
        public GameObject misoSoupPrefabs;
        public GameObject selectObjectParent;

        private GameObject holdingTarget = null;        
        private bool bHolding = false;
        private bool bCheckClick = false;

        // 테이블과 기타 등등...
        private TABLE_SET eTableSet = TABLE_SET.NONE;
        private bool bTable = false;
        private bool bCurry = false;
        private bool bRiceNoodle = false;
        private bool bChinaNoodle = false;
        private bool bMisoSoup = false;

        private ROUTE eRoute = ROUTE.NONE;

        private GameObject[] selectObjects = new GameObject[4];
        private float fDiffMag = 0.2f;



        void Start()
        {            
            //Debug.Log("start");
            for(int i = 0; i < selectObjects.Length; ++i)
            {
                selectObjects[i] = selectObjectParent.transform.GetChild(i).gameObject;
            }
            eRoute = ROUTE.TABLE_SETTING;
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        /// 
        /*
        public void Update()
        {

            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            _QuitOnConnectionErrors();
            
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                if (!m_IsQuitting && Session.Status.IsValid())
                {
                    SearchingForPlaneUI.SetActive(true);
                }

                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            Session.GetTrackables<TrackedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            }

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<TrackedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);

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
            else if(ROUTE.SELECT_FOOD == eRoute)
            {
                ObjectSelect();
            }
           
            // 오브젝트를 생성 및 이동에 대한 컨트롤....            
            

            // object 생성 및 이동이 완료되었을 때....
        }


    */
        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
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
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }

        //=======================================================================================================//
        //================================================ 추가된 것 ================================================//
        //=======================================================================================================//
        void MakeObject()
        {           
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                return;
            
            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;
            
            // 테이블부터,,, curry, rice noodle, china noodle, miso soup            
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // 매개변수에서 받아와서 만든다...
                GameObject andyObject = MakeTableObject(eTableSet, hit.Pose.position, hit.Pose.rotation);                                                
                if (null == andyObject)
                    return;

                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                // Andy should look at the camera but still be flush with the plane.
                if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                {
                    // Get the camera position and match the y-component with the hit position.
                    Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                    cameraPositionSameY.y = hit.Pose.position.y;

                    // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                    andyObject.transform.LookAt(cameraPositionSameY, andyObject.transform.up);
                }
                // Make Andy model a child of the anchor.                                
                //var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                //andyObject.transform.parent = anchor.transform;                                                                
                andyObject.transform.parent = CloneParent.transform;
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
                    TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                        TrackableHitFlags.FeaturePointWithSurfaceNormal;

                    if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                    {                       
                        if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                        {
                            // Get the camera position and match the y-component with the hit position.
                            Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                            cameraPositionSameY.y = hit.Pose.position.y;

                            //holdingTarget = Instantiate(AndyAndroidPrefab, hit.Pose.position, hit.Pose.rotation);
                            holdingTarget.transform.localPosition = new Vector3(hit.Pose.position.x, hit.Pose.position.y, hit.Pose.position.z);
                        }
                    }
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {                    
                    RaycastHit hit;
                    Ray ray = FirstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position);

                    int layer_mask = LayerMask.GetMask("");
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

                    //bool bCheck = Physics.Raycast(ray, out hit, 30f);
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
                    float fOriginScaleX = holdingTarget.transform.localScale.x;
                    float fOriginScaleY = holdingTarget.transform.localScale.y;
                    float fOriginScaleZ = holdingTarget.transform.localScale.z;                    
                    // 0.2 ~ 0.6 까지 제한...                    
                    //float fChangeScale = Mathf.Clamp((deltaMagnitudeDiff / 2000), 0.05f, 0.25f);
                    //float fChangeScale = Mathf.Clamp((deltaMagnitudeDiff / 1500), 0.2f, 0.6f);                    

                    float fValue = fDiffMag - deltaMagnitudeDiff;

                    // 아래는 수정 예정... 현재 크기에서 더해지거나 마이너스 되는 방식....
                    //holdingTarget.transform.localScale = new Vector3(fChangeScale, fChangeScale, fChangeScale);                                    
                    // test....
                    holdingTarget.transform.localScale = new Vector3(fOriginScaleX + fDiffMag, fOriginScaleY + fDiffMag, fOriginScaleZ + fDiffMag);
                    //holdingTarget.transform.localScale = new Vector3(fOriginScaleX+ fChangeScale, fChangeScale+ fChangeScale, fChangeScale+fChangeScale);                    
                }
            }
        }               
        GameObject MakeTableObject(TABLE_SET _eTmpTableSet, Vector3 _pos, Quaternion _rot)
        {
            TABLE_SET eTmpTableSet = _eTmpTableSet;
            GameObject tableObject = null;
            
            if (TABLE_SET.TABLE == eTmpTableSet && false == bTable)
            {
                tableObject = Instantiate(tablePrefabs, _pos, _rot);
                bTable = true;
            }
            else if (TABLE_SET.CURRY == eTmpTableSet && false == bCurry)
            {
                tableObject = Instantiate(curryPrefabs, _pos, _rot);
                bCurry = true;
            }
            else if (TABLE_SET.RICE_NOODLE == eTmpTableSet  && false == bRiceNoodle)
            {
                tableObject = Instantiate(riceNoodlePrefabs, _pos, _rot);
                bRiceNoodle = true;
            }
            else if (TABLE_SET.CHINA_NOODLE == eTmpTableSet && false == bChinaNoodle)
            {
                tableObject = Instantiate(chinaNoodlePrefabs, _pos, _rot);
                bChinaNoodle = true;
            }
            else if (TABLE_SET.MISO_SOUP == eTmpTableSet && false == bMisoSoup)
            {
                tableObject = Instantiate(misoSoupPrefabs, _pos, _rot);
                bMisoSoup = true;
            }
            //tableObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            return tableObject;
        }
        //  생성 후에,, // 피킹한다.
        void ObjectSelect()
        {
            if (Input.touchCount == 1)
            //if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = FirstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position);
                //Ray ray = FirstPersonCamera.ScreenPointToRay(Input.mousePosition);

                bool bCheck = Physics.Raycast(ray, out hit, 30f);
                if (true == bCheck)
                {                                        
                    int iLayer = hit.collider.gameObject.layer;
                    //Debug.Log("layer: " + iLayer);
                    if (11 == iLayer)           // curry 선택시에
                    {
                        // event 발생...
                        EventObjectSelect(TABLE_SET.CURRY);                        
                    }
                    else if (12 == iLayer)      // Rice Noodle
                    {
                        // event 발생...
                        EventObjectSelect(TABLE_SET.RICE_NOODLE);
                    }
                    else if (13 == iLayer)      // China Noodle
                    {
                        // event 발생...
                        EventObjectSelect(TABLE_SET.CHINA_NOODLE);
                    }
                    else if (14 == iLayer)      // Miso Soup
                    {
                        // event 발생...
                        EventObjectSelect(TABLE_SET.MISO_SOUP);
                    }
                }
            }                        
        }
        // 오브젝트 셀렉트 했을 때 발생하는 Event...
        void EventObjectSelect(TABLE_SET _eTmpTableSet)
        {
            // 일단 배경 지우고,,,  or 배경 그대로 두고,, 음식을 확대시켜서 고정한다.            
            // 고정되면 조금 어색하다... 이것은 수정될 수 있음..
            // 고정되지 않은,, 카메라 앞에 위치만 시킨다. << 이런 방향으로...

            TABLE_SET eTmpTableSet = _eTmpTableSet;
            for (int i = 0; i < CloneParent.transform.childCount; ++i)
            {
                CloneParent.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (TABLE_SET.CURRY == _eTmpTableSet)
            {
                selectObjects[0].SetActive(true);
            }
            else if (TABLE_SET.RICE_NOODLE == _eTmpTableSet)
            {
                selectObjects[1].SetActive(true);
            }
            else if (TABLE_SET.CHINA_NOODLE == _eTmpTableSet)
            {
                selectObjects[2].SetActive(true);
            }
            else if (TABLE_SET.MISO_SOUP == _eTmpTableSet)
            {
                selectObjects[3].SetActive(true);
            }
            eRoute = ROUTE.PICKED_FOOD;
        }

        public void ResetTableSetting()
        {
            // 테이블 부터 전부 삭제하고,, 초기화한다.
            for(int i = 0; i < CloneParent.transform.childCount; ++i)
            {
                Destroy(CloneParent.transform.GetChild(i).gameObject);
            }

            bTable = false;
            bCurry = false;
            bRiceNoodle = false;
            bChinaNoodle = false;
            bMisoSoup = false;
        }

        //=======================================================================================================//
        //============================================ RIGHT BUTTON EVNET ===========================================//
        //=======================================================================================================//
        
        // RIGHT SIDE...
        public void TableButtonEvent()
        {            
            eTableSet = TABLE_SET.TABLE;
            uiManager.SetTextNotice(eTableSet);
        }
        public void Food1ButtonEvent()
        {            
            eTableSet = TABLE_SET.CURRY;
            uiManager.SetTextNotice(eTableSet);
        }
        public void Food2ButtonEvent()
        {            
            eTableSet = TABLE_SET.RICE_NOODLE;
            uiManager.SetTextNotice(eTableSet);
        }
        public void Food3ButtonEvent()
        {            
            eTableSet = TABLE_SET.CHINA_NOODLE;
            uiManager.SetTextNotice(eTableSet);
        }
        public void Food4ButtonEvent()
        {            
            eTableSet = TABLE_SET.MISO_SOUP;
            uiManager.SetTextNotice(eTableSet);
        }
        public void ResetButtonEvent()
        {
            eTableSet = TABLE_SET.NONE;            
            uiManager.SetTextNotice(eTableSet);
            ResetTableSetting();

            for (int i = 0; i < selectObjects.Length; ++i)
            {
                selectObjects[i].SetActive(false);
            }
            eRoute = ROUTE.TABLE_SETTING;
        }

        // LEFT SIDE
        public void TableSettingComplete()
        {
            // 바로 셀렉트 로 넘어간다....
            Debug.Log("Table Setting Complete");
            eRoute = ROUTE.SELECT_FOOD;
        }
        public void BackToTable()
        {
            Debug.Log("Back To Table");
            eRoute = ROUTE.SELECT_FOOD;
            // 여기서 다시 테이블로...
            for (int i = 0; i < selectObjects.Length; ++i)
            {
                selectObjects[i].SetActive(false);
            }

            for (int i = 0; i < CloneParent.transform.childCount; ++i)
            {
                CloneParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

    }
}
