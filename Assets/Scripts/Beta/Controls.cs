using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{
    public SteamVR_TrackedObject controllerR;
    public SteamVR_TrackedObject controllerL;

    public GameObject block;
    public GameObject projectile;
    public GameObject scaleOrigin;

    public Material solid;

    public float length;
    public float width;
    public float height;
    public float offset;
    public int nfloors;
    public int nsides;

    public float base_speed;
    public float incr_speed;
    public float despawn_time;

    const ulong Menu = SteamVR_Controller.ButtonMask.ApplicationMenu;
    const ulong Touchpad = SteamVR_Controller.ButtonMask.Touchpad;
    const ulong Trigger = SteamVR_Controller.ButtonMask.Trigger;
    const ulong Grip = SteamVR_Controller.ButtonMask.Grip;

    const Valve.VR.EVRButtonId TouchpadID = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    bool BuildMode;
    Tower tower;
    Plane ground;

    SteamVR_Controller.Device deviceR;
    SteamVR_Controller.Device deviceL;
    float speed_R, speed_L;
    float init_sep;
    float init_timeStep;
    Vector3 init_scale;

    double init_touchX, final_touchX;
    double init_touchY, final_touchY;
    int tmp_sides = 0, tmp_floors = 0;


    void Awake ()
    {
        init_timeStep = 1.0F / Screen.currentResolution.refreshRate;
        Time.fixedDeltaTime = init_timeStep;

        BuildMode = true;
        tower = null;
        ground = new Plane(Vector3.up, new Vector3(0, 0, 0));
        init_scale = scaleOrigin.transform.localScale;		
	}
	
	void Update ()
    {
        deviceR = SteamVR_Controller.Input((int)controllerR.index);
        deviceL = SteamVR_Controller.Input((int)controllerL.index);

        if (BuildMode)
            BuildControls();
        else
            ShootControls();
    }

    void ShootControls()
    {
        if (deviceR.GetPressDown(Trigger))
            speed_R = base_speed;
        if (deviceL.GetPressDown(Trigger))
            speed_L = base_speed;

        if (deviceR.GetPress(Trigger))
            speed_R += incr_speed;
        if (deviceL.GetPress(Trigger))
            speed_L += incr_speed;

        if (deviceR.GetPressUp(Trigger))
            Shoot(controllerR.transform, speed_R);
        if (deviceL.GetPressUp(Trigger))
            Shoot(controllerL.transform, speed_L);

        if (deviceR.GetPressDown(Menu) || deviceL.GetPressDown(Menu))
            ReloadScene();

        if (deviceR.GetPressDown(Grip) && (deviceL.GetPressDown(Grip) || deviceL.GetPress(Grip)))
            FreezeTime();
        else if (deviceL.GetPressDown(Grip) && deviceR.GetPress(Grip))
            FreezeTime();
        else if (deviceR.GetPress(Grip) && deviceL.GetPress(Grip))
            ScaleTarget();
        else if (deviceR.GetPressUp(Grip) || deviceL.GetPressUp(Grip))
            ResumeTime();
        else if (deviceL.GetTouch(Touchpad))
            WarpTime(deviceL.GetAxis(TouchpadID).x);
        else if (deviceL.GetTouchUp(Touchpad))
            ResetTime();
    }

    void BuildControls()
    {
        Ray raycast;
        float enter;

        if (tower == null) {
            if (deviceR.GetPressDown(Touchpad)) {
                raycast = new Ray(controllerR.transform.position, controllerR.transform.forward);
                enter = 0.0F;

                if (ground.Raycast(raycast, out enter)) {
                    scaleOrigin.transform.position = raycast.GetPoint(enter);

                    tower = new Tower(block, scaleOrigin, length, width, height, nsides, nfloors, offset);
                }
            }
        }

        else if(!tower.placed) {
            raycast = new Ray(controllerR.transform.position, controllerR.transform.forward);
            enter = 0.0F;

            if (ground.Raycast(raycast, out enter)) {
                if (!tower.Active)
                    tower.Active = true;

                tower.origin.transform.position = raycast.GetPoint(enter);

                if (deviceR.GetPressDown(Trigger)) {
                    tower.SetMaterial(solid);
                    tower.EnablePhysics(true);

                    BuildMode = false;
                }

                if(deviceR.GetTouchDown(Touchpad)) {
                    init_touchX = deviceR.GetAxis(TouchpadID).x;
                    init_touchY = deviceR.GetAxis(TouchpadID).y;
                }

                else if (deviceR.GetTouch(Touchpad)) {
                    final_touchX = deviceR.GetAxis(TouchpadID).x;
                    final_touchY = deviceR.GetAxis(TouchpadID).y;

                    double dispX = final_touchX - init_touchX;
                    double dispY = final_touchY - init_touchY;
                    double absX = Math.Abs(dispX);
                    double absY = Math.Abs(dispY);

                    if (absX >= 0.34F && absY < 0.34F) {
                        if ((tmp_sides = nsides + (int)(3 * dispX)) >= 3) {
                            tower.Despawn();
                            tower = new Tower(block, scaleOrigin, length, width, height, tmp_sides, nfloors, offset);
                        }
                    }

                    else if (absY >= 0.34F && absX < 0.34F) {
                        if ((tmp_floors = nfloors + (int)(3 * dispY)) >= 1) {
                            tower.Despawn();
                            tower = new Tower(block, scaleOrigin, length, width, height, nsides, tmp_floors, offset);
                        }
                    }
                }

                else if (deviceR.GetTouchUp(Touchpad))
                {
                    if(tmp_sides >= 3)
                        nsides = tmp_sides;
                    if(tmp_floors >= 1)
                        nfloors = tmp_floors;
                }
            }
            else
                tower.Active = false;
        }       
    }

    float GetDistance(Transform trans_1, Transform trans_2)
    {
        return (trans_2.position - trans_1.position).magnitude;
    }

    void Shoot(Transform origin, float speed)
    {
        Vector3 direct = Quaternion.AngleAxis(45, origin.right) * origin.forward;
        GameObject shot = GameObject.Instantiate(projectile, origin.position, origin.rotation);

        shot.GetComponent<Rigidbody>().velocity = speed * direct;

        GameObject.Destroy(shot, despawn_time);
    }

    void FreezeTime()
    {
        Time.timeScale = 0.0F;
        init_sep = GetDistance(controllerR.transform, controllerL.transform);
    }

    void ScaleTarget()
    {
        float final_sep = GetDistance(controllerR.transform, controllerL.transform);
        float scale_factor = final_sep / init_sep;

        scaleOrigin.transform.localScale = init_scale * scale_factor;
    }

    void ResumeTime()
    {
        Time.timeScale = 1.0F;
        init_scale = scaleOrigin.transform.localScale;
    }

    void WarpTime(float input)
    {
        Time.timeScale = 1.0F + input;
        if (input <= 0)
            Time.fixedDeltaTime = init_timeStep * Time.timeScale;
        else
            Time.fixedDeltaTime = init_timeStep;
    }

    void ResetTime()
    {
        Time.timeScale = 1.0F;
        Time.fixedDeltaTime = init_timeStep;
    }

    void ReloadScene()
    {
        Scene active_scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active_scene.buildIndex, LoadSceneMode.Single);
    }
}
