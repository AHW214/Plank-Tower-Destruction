using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerGen : MonoBehaviour {

    public GameObject prefab;

    public float block_length;
    public float block_width;
    public float block_height;

    public int tower_sides;
    public int tower_height;

    public float block_offset;

	void Start ()
    {
        BuildTower(tower_sides, tower_height, block_length, block_width, block_height, block_offset);
	}

    public void BuildTower(int sides, int t_height = 15, float length = 7, float width = 0.5F, float b_height = 1, float offset = 1.1F)
    {
        GameObject tower = GameObject.Find("Tower");
        if (tower)
            Destroy(tower);

        tower = new GameObject("Tower");
        tower.transform.SetParent(GameObject.Find("Tower Scale Origin").transform);

        GameObject floor = new GameObject("Floor 1");
        floor.transform.SetParent(tower.transform);

        prefab.transform.localScale = new Vector3(width, b_height, length);

        Vector3 init_pos = new Vector3(0, b_height / 2, 0);
        Quaternion init_rot = prefab.transform.rotation;

        float apothem = (float)(length / (2 * Math.Tan(Math.PI / sides)));
        float exterior_angle = To_deg(Math.PI / sides);

        float new_angle;
        Vector3 new_pos;
        Quaternion new_rot;

        for (int s = 0; s < sides; s++)
        {
            new_angle = 2 * s * exterior_angle;
            new_rot = init_rot * Quaternion.Euler(0, new_angle, 0);
            new_pos = init_pos + new_rot * new Vector3(offset * apothem, 0, 0);

            GameObject block = GameObject.Instantiate(prefab, new_pos, new_rot, floor.transform);
            block.name = "Block " + (s + 1);
        }

        init_pos += new Vector3(0, b_height / 2, 0);

        for (int h = 1; h < t_height; h++)
        {
            init_rot *= Quaternion.Euler(0, exterior_angle, 0);
            GameObject floor_dupe = GameObject.Instantiate(floor, init_pos, init_rot, tower.transform);
            floor_dupe.name = "Floor " + (h + 1);

            init_pos += new Vector3(0, b_height, 0);
        }
    }

    float To_deg(double rad)
    {
        return (float)(rad * (180 / Math.PI));
    }
}


