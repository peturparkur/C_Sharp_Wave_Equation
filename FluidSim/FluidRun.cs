//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System;

public class FluidRun : MonoBehaviour
{
    FluidSimFlat fluid;
    Matrix<float> matrix;
    public float c = 1;
    public float damp = 1f;
    public float dx = 0.2f;
    public float dy = 0.2f;
    public float dt = 0.01f;

    public int sizeX = 5;
    public int sizeY = 5;

    //public UnityEngine.Object prefab; //the thing that we will visualise stuff with
    //Transform[] vis;
    //public Mesh mesh;
    public float visScale = 5f;
    public float heightScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        matrix = new Matrix<float>(sizeX, sizeY);
        matrix[(int)Math.Ceiling((double)(sizeX/2)), (int)Math.Ceiling((double)(sizeX / 2))] = 5;
        fluid = new FluidSimFlat(matrix, matrix, dx,dy);
        //Debug.Log(matrix.ToString());

        //visualise
        /*
        vis = new Transform[sizeX * sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int t = i * sizeY + j;
                GameObject go = (GameObject)Instantiate(prefab, new Vector3(), Quaternion.identity, transform);
                vis[t] = go.transform;
                vis[t].position = new Vector3(i * dx * visScale, matrix[i, j], j * dy * visScale);
            }
        }*/
        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[sizeX * sizeY];
        int[] tris = new int[(sizeX - 1) * (sizeY - 1) * 2 * 3]; //amount of tris
        //Debug.Log("Max Tris = " + tris.Length/3);
        int n = 0;
        for (int i=0; i<sizeX; i++)
        {
            for(int j=0; j<sizeY; j++)
            {
                int t = i * sizeY + j;
                verts[t] = new Vector3(i * dx*visScale, matrix[i, j]*heightScale, j * dy*visScale);
                //Debug.Log("T = " + t);
                //Debug.Log("N = " + n);

                if (i < sizeX-1)
                {
                    if(j<sizeY-1)
                    {

                        //this was created anticlockwise
                        /*
                        Debug.Log("tris numb = " + (n * 2));
                        tris[n*2 * 3] = t;
                        tris[n*2 * 3 + 1] = t + (1 * sizeY);
                        tris[n*2 * 3 + 2] = t + (1 * sizeY) + 1;
                        //Debug.Log("v1 = " + t);
                        //Debug.Log("v2 = " + (t + (i * sizeY)));
                        //Debug.Log("v3 = " + (t + (i * sizeY) + j));

                        tris[(n*2 + 1) * 3] = t;
                        tris[(n*2 + 1) * 3 + 1] = t + (1 * sizeY) + 1;
                        tris[(n*2 + 1) * 3 + 2] = t + 1;
                        //Debug.Log("u1 = " + t);
                        //Debug.Log("u2 = " + (t + (i * sizeY) + j));
                        //Debug.Log("u3 = " + (t + j));*/

                        tris[n * 2 * 3] = t;
                        tris[n * 2 * 3 + 1] = t + (1 * sizeY) + 1;
                        tris[n * 2 * 3 + 2] = t + (1 * sizeY);

                        tris[(n * 2 + 1) * 3] = t;
                        tris[(n * 2 + 1) * 3 + 1] = t + 1;
                        tris[(n * 2 + 1) * 3 + 2] = t + (1 * sizeY) + 1;

                        n++;
                        //Debug.Log("tris created = " + (n * 2));
                    }
                }
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        MeshRenderer r = this.gameObject.AddComponent<MeshRenderer>();
        r.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;


        
        //fluid.Initialise(values, values, c);
    }

    // Update is called once per frame
    void Update()
    {
        fluid.Step(dt, c,damp);
        matrix = fluid.val;
        for(int i=(int)(sizeX*0.75); i<sizeX; i++)
        {
            for(int j=40; j<80; j++)
            {
                fluid.val[i, j] = 0f;
                //matrix[i, j] = 2f;               
            }
            for(int j=120; j<160; j++)
            {
                fluid.val[i, j] = 0f;
                //matrix[i, j] = 2f;               
            }
        }
        //Debug.Log(matrix.ToString());

        /*for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int t = i * sizeY + j;
                //GameObject go = (GameObject)Instantiate(prefab, new Vector3(), Quaternion.identity, transform);
                //vis[t] = go.transform;
                vis[t].position = new Vector3(i * dx * visScale, matrix[i, j], j * dy * visScale);
            }
        }*/
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        Vector3[] verts = mesh.vertices;

        for (int i=0; i<sizeX; i++)
        {
            for(int j=0; j<sizeY; j++)
            {
                //Debug.DrawRay(new Vector3(i * dx * visScale, 0f, j * dy * visScale), Vector3.up * (matrix[i,j] + 1f), Color.blue);
                verts[i*sizeY+j] = new Vector3(i * dx * visScale, matrix[i, j] * heightScale, j * dy * visScale);
            }
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }
}
