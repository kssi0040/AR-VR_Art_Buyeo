
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
     
// 비디오 캡처 및 저장하기...
public class Test1 : MonoBehaviour {

    static readonly float MaxRecordingTime = 5f;

    VideoCapture m_videoCapture = null;
    float m_stopRecordingTimer = float.MaxValue;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if (m_videoCapture == null || !m_videoCapture.IsRecording)
        {
            return;
        }

        if (Time.time > m_stopRecordingTimer)
        {
            m_videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
        }
    }
    void StartVideoCapterTest()
    {
        Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width + res.height).First();
        Debug.Log(cameraResolution);
        float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
        Debug.Log(cameraFramerate);


        VideoCapture.CreateAsync(false, delegate (VideoCapture videoCapture)
        {
            if (videoCapture != null)
            {
                m_videoCapture = videoCapture;
                Debug.Log("Created VideoCapture Instance!");

                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                m_videoCapture.StartVideoModeAsync(cameraParameters, VideoCapture.AudioState.ApplicationAndMicAudio, OnStartVideoCaptureMode);
            }
            else
            {
                Debug.LogError("Failed to create VideoCapture Instacel!");
            }
        });
    }

    void OnStartVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Video Capture Mode!");
        string timeStamp = Time.time.ToString().Replace(".", "").Replace(" : ", "");
        string fileName = string.Format("TestVideo_{0}.mp4", timeStamp);
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        filePath = filePath.Replace("/", @"\");
        m_videoCapture.StartRecordingAsync(filePath, OnStartedRecordingVideo);
    }
    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Video Capture Mode!");
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
        m_stopRecordingTimer = Time.time + MaxRecordingTime;
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        m_videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }

}
*/