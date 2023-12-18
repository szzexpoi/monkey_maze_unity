using UnityEngine;

public class CubeWithSubmeshes : MonoBehaviour
{
    public Material[] faceMaterials; // Array of materials for each face of the cube

    void Start()
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
        cubeMesh.RecalculateNormals();
        // Assign the materials to the MeshRenderer component
        Renderer renderer = GetComponent<Renderer>();
        Debug.Log(renderer.materials.Length);
        if (renderer != null)
        {
            renderer.materials = faceMaterials;
        }

        // Assign the modified mesh to the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = cubeMesh;
        }
    }
}
