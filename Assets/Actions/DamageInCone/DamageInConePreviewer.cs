using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInConePreviewer : MonoBehaviour
{
    public int Resolution = 30;


    [HideInInspector]
    public IActor actor;
    [HideInInspector]
    public CardSystem.Effects.DamageInCone spawner;

    int numStacks;
    internal int NumStacks
    {
        set
        {
            numStacks = value;

            filter = GetComponent<MeshFilter>();
            filter.mesh = new Mesh();

            Vector3[] vertices = new Vector3[Resolution + 1];
            float arcWidth = Mathf.Clamp(spawner.arcWidth * numStacks * Mathf.Deg2Rad, 0, 2f * Mathf.PI);
            vertices[0] = new Vector3(0, 0, 0);

            for (int i = 0; i < Resolution; i++)
            {
                float angle = i * (arcWidth / (Resolution - 1)) - arcWidth / 2f;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * spawner.range * numStacks;
            }
            filter.mesh.vertices = vertices;


            int[] triangles = new int[3 * (Resolution - 1)];
            for (int i = 0; i < triangles.Length; i += 3)
            {
                triangles[i] = 0;
                triangles[i + 1] = i/3 + 2;
                triangles[i + 2] = i/3 + 1;
            }

            filter.mesh.triangles = triangles;
        }
        get { return numStacks; }
    }

    MeshFilter filter;

    void Start()
    {
        GetComponent<MeshRenderer>().material.color = spawner.previewColor;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = actor.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}
