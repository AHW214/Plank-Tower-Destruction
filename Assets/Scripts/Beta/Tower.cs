using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tower : MonoBehaviour
{
    public GameObject block;
    public GameObject origin;

    public GameObject MakeTower(Vector3 pos, float l, float w, float h, int s, int f, float off)
    {
        GameObject floor;
        Quaternion rot;
        float ext_angle;

        origin.transform.position = pos;
        pos += new Vector3(0, h / 2, 0);
        rot = block.transform.rotation;
        ext_angle = To_Deg(Math.PI / sides);
        block.transform.localscale = new Vector3(w, h, l);

        MakeBase();

        GameObject new_floor;
        for(int i = 1; i < f; i++) {
            pos += new Vector3(0, h, 0);
            rot *= Quaternion.Euler(0, ext_angle, 0);

            new_floor = GameObject.Instantiate(floor, pos, rot, origin.transform);
            new_floor.name = "Floor " + (i + 1);
        }

        void MakeBase()
        {
            GameObject new_block;
            Vector3 new_pos;
            Quaternion new_rot;
            float new_angle;
            float apothem;

            floor = new GameObject("Floor 1");
            floor.transform.SetParent(origin.transform);
            apothem = Calc_Apoth(l, s);

            for (int i = 0; i < s; i++) {
                new_angle = 2 * i * ext_angle;
                new_rot = rot * Quaternion.Euler(0, new_angle, 0);
                new_pos = pos + new_rot * new Vector3(off * apothem, 0, 0);

                new_block = GameObject.Instantiate(block, new_pos, new_rot, floor.transform);
                block.name = "Block " + (i + 1);
            }
        }
    }

    float To_Deg(double rad)
    {
        return (float)(rad * (180 / Math.PI));
    }

    float Calc_Apoth(int length, int nsides)
    {
        return (float)(length / (2 * Math.Tan(Math.PI / nsides)));
    }

    
}


