using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PolygonGen : MonoBehaviour
{
    public SteamVR_TrackedObject controllerR;
    public Material material;
    private GameObject polygon;
    private double init, final;
    private int n = 4, n2;

    TowerGen towerGen;
    
    void Start ()
    {
        towerGen = GetComponent<TowerGen>();
        Mesh mesh = PolyGen(n, 0.06);

        polygon = new GameObject("Polygon");
        polygon.transform.position = controllerR.transform.position + 0.06F * controllerR.transform.up;
        polygon.transform.SetParent(controllerR.transform);
        polygon.AddComponent<MeshFilter>();
        polygon.AddComponent<MeshRenderer>();
        polygon.GetComponent<Renderer>().material = material; 
        polygon.GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        var deviceR = SteamVR_Controller.Input((int)controllerR.index);

        if (deviceR.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            init = deviceR.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
        }

        else if (deviceR.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            final = deviceR.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;

            double disp = final - init;
            n2 = n + (int)Math.Round(3 * disp);

            if(n2 >= 3)
                polygon.GetComponent<MeshFilter>().mesh = PolyGen(n2, 0.06);   
        }

        else if (deviceR.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (n2 != n && n2 >= 3)
            {
                n = n2;
                towerGen.BuildTower(n);
            }
            init = 0;
        }
    }

    Mesh PolyGen(int n, double r)
    {
        //double s = r * 2 * Math.Sin(Math.PI / n);
        double angle = Math.PI / 2;
        double angle_sym = (2 * Math.PI) / n;

        Vector3[] verts = new Vector3[n];

        for (int i = 0; i < verts.Length; i++)
        {
            float x = (float)(r * Math.Cos(angle));
            float z = (float)(r * Math.Sin(angle));
            verts[i] = new Vector3(x, 0, z);
            angle += angle_sym;
        }

        int[] tris = new int[3 * (n - 2)];
        int vert = n - 1;

        for (int i = 0; i < tris.Length; i += 3)
        {
            tris[i] = 0;
            tris[i + 1] = vert;
            tris[i + 2] = vert - 1;

            vert--;
        }

        Mesh mesh = new Mesh
        {
            vertices = verts,
            triangles = tris
        };

        return mesh;
    }
}
