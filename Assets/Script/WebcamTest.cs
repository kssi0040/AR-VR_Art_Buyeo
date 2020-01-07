using UnityEngine;
using System.Collections;

public class WebcamTest : MonoBehaviour {
	public string deviceName;
	WebCamTexture wct;
	
	
	// Use this for initialization
	void Start () {
		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		wct = new WebCamTexture(deviceName, Screen.width, Screen.height, 30);
        //renderer.material.mainTexture = wct;
        GetComponent<Renderer>().material.mainTexture = wct;
		wct.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
