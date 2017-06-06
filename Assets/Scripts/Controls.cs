using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Controls : MonoBehaviour
{
    public SteamVR_TrackedObject controllerR;
    public SteamVR_TrackedObject controllerL;
    public Rigidbody controllerTipR;
    public Rigidbody controllerTipL;

    public GameObject prefab;
    public GameObject stuffToScale;
    public float speed;

    private float separation0;
    private float separation1;
    private float scaleFactor;
    private Vector3 originalScale;
    private float touchpadx;
    

    void Awake()
    {
        originalScale = stuffToScale.transform.localScale;
    }

    void Update()
    {
        var deviceR = SteamVR_Controller.Input((int)controllerR.index);
        var deviceL = SteamVR_Controller.Input((int)controllerL.index);

        if (deviceR.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            var shoot = GameObject.Instantiate(prefab);
            var rb = shoot.GetComponent<Rigidbody>();
            shoot.transform.position = controllerTipR.transform.position;
            rb.velocity = transform.forward * speed;

            GameObject.Destroy(shoot, 2); 
        }

        else if (deviceL.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            var shoot = GameObject.Instantiate(prefab);
            var rb = shoot.GetComponent<Rigidbody>();
            shoot.transform.position = controllerTipL.transform.position;
            rb.velocity = transform.forward * speed;

            GameObject.Destroy(shoot, 2);
        }

        else if(deviceR.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu) || deviceL.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            SceneManager.LoadScene("Plank Tower", LoadSceneMode.Single);
        }

        else if(deviceR.GetTouchDown(SteamVR_Controller.ButtonMask.Grip) || deviceL.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Time.timeScale = 0f;
            separation0 = (controllerR.transform.position - controllerL.transform.position).magnitude;
        }

        else if(deviceR.GetTouch(SteamVR_Controller.ButtonMask.Grip) || deviceL.GetTouch(SteamVR_Controller.ButtonMask.Grip))
        {
            separation1 = (controllerR.transform.position - controllerL.transform.position).magnitude;

            scaleFactor = (separation1 / separation0);

            stuffToScale.transform.localScale = originalScale * scaleFactor;
        }

        else if(deviceR.GetTouchUp(SteamVR_Controller.ButtonMask.Grip) || deviceL.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Time.timeScale = 1.0f;
            originalScale = stuffToScale.transform.localScale;

        }

        else if(deviceR.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchpadx = deviceR.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;

            Time.timeScale = 1.0f * (touchpadx + 1);
            if (touchpadx <= 0)
            {
                Time.fixedDeltaTime = 0.02f * (touchpadx + 1);
            }

            else if (touchpadx > 0)
            {
                Time.fixedDeltaTime = .02f;
            }
        }

        else if(deviceR.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}