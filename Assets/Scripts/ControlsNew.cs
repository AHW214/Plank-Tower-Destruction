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

    void ShootProjectile(Vector3 SpawnPos, Vector3 SpawnDirect)
    {
        var shoot = GameObject.Instantiate(prefab);
        var rb = shoot.GetComponent<Rigidbody>();
        shoot.transform.position = SpawnPos;
        rb.velocity = SpawnDirect * speed;

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
        {
            ShootProjectile(controllerModelR.transform.position, Quaternion.AngleAxis(45, controllerModelR.transform.right) * controllerModelR.transform.forward);
        }

        else if (deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            ShootProjectile(controllerModelL.transform.position, Quaternion.AngleAxis(45, controllerModelL.transform.right) * controllerModelL.transform.forward);
        }

        else if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu) || deviceL.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            ReloadScene();
        }

        else if (deviceR.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
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

        else if (deviceR.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            TimeWarp(deviceR.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x);
        }

        else if (deviceR.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            TimeReset();
        }
    }
}