using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControlsNew : MonoBehaviour
{
    public SteamVR_TrackedObject controllerR;
    public SteamVR_TrackedObject controllerL;
    public Rigidbody controllerModelR;
    public Rigidbody controllerModelL;

    public GameObject prefab;
    public GameObject stuffToScale;
    public float base_speed; 
    
    private float speed_R, speed_L;
    private float separation0;
    private float separation1;
    private float scaleFactor;
    private Vector3 originalScale;
    private float touchpadx;
    

    void Awake()
    {
        originalScale = stuffToScale.transform.localScale;
    }

    void ShootProjectile(Transform controller, float speed)
    {
        var direct = Quaternion.AngleAxis(45, controller.right) * controller.forward;
        var shoot = GameObject.Instantiate(prefab, controller.position, controller.rotation);
        shoot.GetComponent<Rigidbody>().velocity = speed * direct;

        GameObject.Destroy(shoot, 2);
    }

    void FreezeTime(Vector3 Pos1, Vector3 Pos2)
    {
        Time.timeScale = 0f;
        separation0 = (Pos1 - Pos2).magnitude;
    }

    void ScaleStuff(Vector3 Pos1, Vector3 Pos2)
    {
        separation1 = (Pos1 - Pos2).magnitude;
        scaleFactor = (separation1 / separation0);
        stuffToScale.transform.localScale = originalScale * scaleFactor;
    }

    void UnfreezeTime()
    {
        Time.timeScale = 1.0f;
        originalScale = stuffToScale.transform.localScale;
    }

    void TimeWarp(float touchX)
    {
        Time.timeScale = 1.0f * (touchX + 1);
        if (touchX <= 0)
        {
            Time.fixedDeltaTime = 0.02f * (touchX + 1);
        }

        else if (touchX > 0)
        {
            Time.fixedDeltaTime = 0.02f;
        }
    }

    void TimeReset()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene("Plank Tower", LoadSceneMode.Single);
    }

    void Update()
    {
        var deviceR = SteamVR_Controller.Input((int)controllerR.index);
        var deviceL = SteamVR_Controller.Input((int)controllerL.index);

        if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            speed_R = base_speed;

        if (deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            speed_L = base_speed;

        if (deviceR.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            speed_R += 0.25F;

        if (deviceL.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            speed_L += 0.25F;

        if (deviceR.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            ShootProjectile(controllerModelR.transform, speed_R);
        
        if (deviceL.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            ShootProjectile(controllerModelL.transform, speed_L);


        if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu) || deviceL.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            ReloadScene();
        }

        if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            FreezeTime(controllerR.transform.position, controllerL.transform.position);
        }

        else if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && deviceL.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            FreezeTime(controllerR.transform.position, controllerL.transform.position);     
        }

        else if (deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && deviceR.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            FreezeTime(controllerR.transform.position, controllerL.transform.position);
        }

        else if (deviceR.GetPress(SteamVR_Controller.ButtonMask.Grip) && deviceL.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            ScaleStuff(controllerR.transform.position, controllerL.transform.position);
        }

        else if (deviceR.GetPressUp(SteamVR_Controller.ButtonMask.Grip) || deviceL.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            UnfreezeTime();
        }

        else if (deviceL.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            TimeWarp(deviceL.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x);
        }

        else if (deviceL.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            TimeReset();
        }
    }
}