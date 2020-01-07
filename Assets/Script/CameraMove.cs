using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    bool Up=false;
    bool Down = false;
    bool Left = false;
    bool Right = false;
    public float speed;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Up == true)
        {
            Debug.Log("UP transform.eulerAngles.x: " + transform.eulerAngles.x);

            if (transform.eulerAngles.x >= 300f || transform.eulerAngles.x == 0 || transform.eulerAngles.x <= 60)
            {
                this.transform.Rotate((-90.0f * Time.deltaTime) / speed, 0.0f, 0.0f);
            }
            else if (transform.eulerAngles.x < 300 && transform.eulerAngles.x > 200)
            {
                transform.eulerAngles = new Vector3(300, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else
            {
                this.transform.Rotate((-90.0f * Time.deltaTime) / speed, 0.0f, 0.0f);
            }
            
        }

        if (Down == true)
        {
            if (transform.eulerAngles.x <= 60 || transform.eulerAngles.x > 300)
            {
                this.transform.Rotate((90.0f * Time.deltaTime) / speed, 0.0f, 0.0f);
            }
            else if (transform.eulerAngles.x > 60 && transform.eulerAngles.x < 70)
            {
                transform.eulerAngles = new Vector3(60, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            else
            {
                this.transform.Rotate((90.0f * Time.deltaTime) / speed, 0.0f, 0.0f);
            }

        }

        if (Left == true)
        {
            this.transform.Rotate(0.0f, (-90.0f * Time.deltaTime) / speed, 0.0f);
        }

        if (Right == true)
        {
            this.transform.Rotate(0.0f, (90.0f * Time.deltaTime) / speed, 0.0f);
        }
	}

    private void FixedUpdate()
    {
        this.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }

    public void UpButtonDown()
    {
        Up = true;
    }

    public void UpButtonUp()
    {
        Up = false;
    }

    public void DownButtonDown()
    {
        Down = true;
    }

    public void DownButtonUp()
    {
        Down = false;
    }

    public void LeftButtonDown()
    {
        Left = true;
    }

    public void LeftButtonUp()
    {
        Left = false;
    }

    public void RightButtonDown()
    {
        Right = true;
    }

    public void RightButtonUp()
    {
        Right = false;
    }
}
