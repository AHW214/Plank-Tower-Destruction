using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tower
{
    public bool placed;
    public GameObject origin;
    GameObject[] floors;
    
    public Tower(GameObject block, GameObject org, float l, float w, float h, int s, int f, float off)
    {
        Vector3 pos;
        Quaternion rot;
        float ext_angle;

        placed = false;
        origin = org;
        floors = new GameObject[f];
        
        pos = origin.transform.position + new Vector3(0, h / 2, 0);
        rot = origin.transform.rotation;
        ext_angle = To_Deg(Math.PI / s);
        block.transform.localScale = new Vector3(l, h, w);

        float new_angle;
        Vector3 new_pos;
        Quaternion new_rot;
        float apothem;

        floors[0] = new GameObject("Floor 1");
        floors[0].transform.SetParent(origin.transform);
        apothem = Calc_Apoth(block.GetComponent<Renderer>().bounds.size.x, s);
        Debug.Log(apothem);

        for (int i = 0; i < s; i++) {
            new_angle = 2 * i * ext_angle;
            new_rot = rot * Quaternion.Euler(0, new_angle, 0);
            new_pos = pos + new_rot * new Vector3(0, 0, off * apothem);

            GameObject.Instantiate(block, new_pos, new_rot, floors[0].transform);
        }

        pos += new Vector3(0, h / 2, 0);

        for (int i = 1; i < f; i++) {
            rot *= Quaternion.Euler(0, ext_angle, 0);

            floors[i] = GameObject.Instantiate(floors[0], pos, rot, origin.transform);
            floors[i].name = "Floor " + (i + 1);

            pos += new Vector3(0, h, 0);
        }
    }

    public void EnablePhysics(bool b)
    {
        if (b)
            placed = true;

        Rigidbody[] blocks;
        for(int i = 0; i < floors.Length; i++) {
            blocks = floors[i].GetComponentsInChildren<Rigidbody>();

            for(int j = 0; j < blocks.Length; j++)
                blocks[j].isKinematic = !b;
        }
    }

    public void SetMaterial(Material m)
    {
        Renderer[] blocks;
        for(int i = 0; i < floors.Length; i++) {
            blocks = floors[i].GetComponentsInChildren<Renderer>();

            for(int j = 0; j < blocks.Length; j++)
                blocks[j].material = m;
        }
    }

    float To_Deg(double rad)
    {
        return (float)(rad * (180 / Math.PI));
    }

    float Calc_Apoth(float length, int nsides)
    {
        return (float)(length / (2 * Math.Tan(Math.PI / nsides)));
    }

    
}