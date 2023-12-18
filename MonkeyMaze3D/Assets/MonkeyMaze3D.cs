using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.XR;

public class MonkeyMaze3D : MonoBehaviour
{
    public GameObject main_cam;
    public GameObject start;
    public GameObject end;
    public Maze_data maze_info;
    public GameObject wallPrehab;
    public Texture2D background_texture;
    public Texture2D long_wall_texture;
    public Texture2D medium_wall_texture;
    public Texture2D short_wall_texture;
    GameObject wall;


    // Start is called before the first frame update
    void Start()
    {
        string json_file = "maze_layout.json";
        string json_data = File.ReadAllText(json_file);
        maze_info = JsonUtility.FromJson<Maze_data>(json_data);
        Create_maze();
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    // function for creating the maze based on the input Json file
    void Create_maze()
    {
        // spawn the main camera to starting position
        var start_position = maze_info.start_position;
        main_cam = GameObject.Find("Main Camera");
        main_cam.transform.position = new Vector3(start_position.start_x / 5, 0f, start_position.start_y / 5);
        Debug.Log(maze_info.walls.Length);
        for (int i = 0; i < maze_info.walls.Length; i++)
       {
            var cur_wall = maze_info.walls[i];
            var cur_stimuli = maze_info.stimuli_dir[i].stimuli_path;
            Debug.Log(cur_stimuli);
            var is_horizontal = maze_info.directions[i].is_horizontal;
            Vector3 start_point = new Vector3(cur_wall.start_x / 5, 0f, cur_wall.start_y / 5);
            Vector3 end_point = new Vector3(cur_wall.end_x / 5, 0f, cur_wall.end_y / 5);
            SetStart(start_point);
            SetEnd(end_point);
            Adjust(is_horizontal, cur_stimuli);
        }

    }

    // auxiliary functions for building the walls
    void SetStart(Vector3 x)
    {
        start.transform.position = x;
        wall = (GameObject)Instantiate(wallPrehab, start.transform.position, Quaternion.identity);

    }

    void SetEnd(Vector3 y)
    {
        end.transform.position = y;
    }

    /*
     * invoking the wall building method
     */
    void Adjust(bool is_horizontal, string stimuli_dir)
    {
        AdjustWall(is_horizontal, stimuli_dir);
    }

    /*
     * build the wall in between start point and the end point
     */
    void AdjustWall(bool is_horizontal, string stimuli_dir)
    {
        // create the wall without texture
        start.transform.LookAt(end.transform.position);
        end.transform.LookAt(start.transform.position);
        float distance = Vector3.Distance(start.transform.position, end.transform.position);
        wall.transform.position = start.transform.position + distance / 2 * start.transform.forward;
        wall.transform.rotation = start.transform.rotation;
        wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, distance);

        // Create materials for each surface of the cube
        Material background_Material = new Material(Shader.Find("Standard"));
        background_Material.mainTexture = background_texture;
        Material long_Material = new Material(Shader.Find("Standard"));
        long_Material.mainTexture = long_wall_texture;
        Material medium_Material = new Material(Shader.Find("Standard"));
        medium_Material.mainTexture = medium_wall_texture;
        Material short_Material = new Material(Shader.Find("Standard"));
        short_Material.mainTexture = short_wall_texture;

        //// adjust the texture based on the length of the wall (old method)
        //MeshFilter meshFilter = wall.GetComponent<MeshFilter>();
        ////Create_Submeshes(meshFilter);
        //MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
        //Debug.Log("render: " + meshRenderer.materials.Length);
        //if (meshRenderer == null || meshFilter == null)
        //{
        //    Debug.LogError("MeshRenderer or MeshFilter component not found on the cube GameObject!");
        //    return;
        //}

        //// Get the cube's mesh
        //Mesh mesh = meshFilter.mesh;
        //// Set the materials for each sub-mesh (each face of the cube)
        //Material[] materials = new Material[mesh.subMeshCount];
        //Debug.Log(mesh.subMeshCount);
        //for (int i = 0; i < mesh.subMeshCount; i++)
        //{
        //    if (i == 2) // Index 2 corresponds to the top face of the cube
        //        materials[i] = background_Material;
        //    else // All other faces are the side faces
        //    {
        //        if (distance < 3)
        //        {
        //            materials[i] = background_Material;
        //        }
        //        else if (distance >= 3 && distance < 10)
        //        {
        //            materials[i] = short_Material;
        //        }
        //        else if (distance >= 10 && distance < 30)
        //        {
        //            materials[i] = medium_Material;
        //        }
        //        else
        //        {
        //            materials[i] = long_Material;
        //        }
        //    }
        //}
        //// Assign materials to the cube's mesh
        //meshRenderer.materials = materials;

        // adjust the texture based on the length of the wall (new method)
        MeshRenderer left_render = wall.transform.Find("left").GetComponent<MeshRenderer>();
        Material[] left_material = new Material[1];
        Debug.Log("render: " + left_render.sharedMaterials.Length);

        MeshRenderer right_render = wall.transform.Find("right").GetComponent<MeshRenderer>();
        Material[] right_material = new Material[1];
        //Debug.Log(distance);

        //// rending texture based on the distance
        //if (distance < 5)
        //{
        //    left_material[0] = background_Material;
        //    right_material[0] = background_Material;
        //}
        //else if (distance >= 5 && distance < 10)
        //{
        //    left_material[0] = short_Material;
        //    right_material[0] = short_Material;
        //}
        //else if (distance >= 10 && distance < 30)
        //{
        //    left_material[0] = medium_Material;
        //    right_material[0] = medium_Material;
        //}
        //else
        //{
        //    left_material[0] = long_Material;
        //    right_material[0] = long_Material;
        //}



        // rending texture with unique face combination
        Texture2D cur_texture = Resources.Load<Texture2D>(stimuli_dir);
        Material cur_material = new Material(Shader.Find("Standard")); // You can use a different shader if needed
        cur_material.mainTexture = cur_texture;
        left_material[0] = cur_material;
        right_material[0] = cur_material;

        left_render.materials = left_material;
        right_render.materials = right_material;



        wall.transform.Find("left").transform.localPosition = new Vector3(-0.511f, 0f, 0f);
        wall.transform.Find("right").transform.localPosition = new Vector3(0.511f, 0f, 0f);

    }

    void Create_Submeshes(MeshFilter meshFilter)
    {
        // Create a new empty mesh for the cube
        Mesh cubeMesh = new Mesh();
        cubeMesh.name = "CubeWithSubmeshes";

        // Define vertices for each face of the cube
        Vector3[] frontFaceVertices = { new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f) };
        Vector3[] backFaceVertices = { new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f) };
        // Define vertices for the other faces similarly

        // Define triangles for each face
        int[] frontFaceTriangles = { 0, 1, 2, 0, 2, 3 };
        int[] backFaceTriangles = { 0, 1, 2, 0, 2, 3 };
        // Define triangles for the other faces similarly

        // Combine the vertices and triangles for each face into a single array
        Vector3[] allVertices = new Vector3[frontFaceVertices.Length + backFaceVertices.Length /* + ... */];
        frontFaceVertices.CopyTo(allVertices, 0);
        backFaceVertices.CopyTo(allVertices, frontFaceVertices.Length /* + ... */);

        int[] allTriangles = new int[frontFaceTriangles.Length + backFaceTriangles.Length /* + ... */];
        frontFaceTriangles.CopyTo(allTriangles, 0);
        backFaceTriangles.CopyTo(allTriangles, frontFaceTriangles.Length /* + ... */);

        // Assign the combined vertices and triangles to the mesh
        cubeMesh.vertices = allVertices;
        cubeMesh.triangles = allTriangles;

        // Create a separate sub-mesh for each face
        int numFaces = 6; // A cube has 6 faces
        int[] subMeshTriangles = new int[numFaces * frontFaceTriangles.Length];

        // Assign triangles for each sub-mesh (one sub-mesh per face)
        for (int faceIndex = 0; faceIndex < numFaces; faceIndex++)
        {
            int triangleStartIndex = faceIndex * frontFaceTriangles.Length;
            frontFaceTriangles.CopyTo(subMeshTriangles, triangleStartIndex);
            // Do the same for other faces' triangles
        }

        // Assign the triangles for each sub-mesh separately
        cubeMesh.subMeshCount = numFaces;
        for (int subMeshIndex = 0; subMeshIndex < numFaces; subMeshIndex++)
        {
            cubeMesh.SetTriangles(subMeshTriangles, subMeshIndex);
        }

        meshFilter.mesh = cubeMesh;
    }
}
