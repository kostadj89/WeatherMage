using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleMesh : MonoBehaviour, ICanTakeDamage
{
    [SerializeField]
    private float explosionForce = 5.0f;     // force applied to mesh pieces when destroyed
    [SerializeField]
    private float explosionRadius = 2.0f;    // radius of the explosion force
    [SerializeField]
    private float upwardsModifier = 0.5f;    // upward force modifier for the explosion
    [SerializeField]
    private float lifeTime = 2f;    // upward force modifier for the explosion
    [SerializeField]
    private int numberOfChunks = 4;

    private Mesh originalMesh;              // original mesh of the game object
    private MeshFilter meshFilter;          // mesh filter component of the game object
    private MeshRenderer meshRenderer;      // mesh renderer component of the game object
    private Vector3 explosionPosition;
    private GridPosition currentGridPosition;

    public static event EventHandler OnDestructibleDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddDestructibleAtGridPosition(currentGridPosition, this);

        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter1))
        {
            this.meshFilter = meshFilter1;
        }

        if (TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer1))
        {
            this.meshRenderer = meshRenderer1;
        }

        if (this.meshFilter!=null)
        {
            originalMesh = this.meshFilter.mesh;
        }

        explosionPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    //DestroyMesh();
        //    DestroyMeshBySplitting();
        //}
    }

    private void DestroyMeshBySplitting()
    {
        OnDestructibleDestroyed?.Invoke(this, EventArgs.Empty);
        LevelGrid.Instance.ClearUnitOrDestructibleAtGridPosition(currentGridPosition, this);

        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());

        SplitMesh();

        // destroy the original game object
        Destroy(gameObject);
    }

    private void SplitMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("No mesh found on the GameObject");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int trianglesPerChunk = triangles.Length / numberOfChunks;

        for (int i = 0; i < numberOfChunks; i++)
        {
            int startTriangleIndex = i * trianglesPerChunk;
            int endTriangleIndex = Mathf.Min(startTriangleIndex + trianglesPerChunk, triangles.Length);

            CreateChunk(vertices, triangles, startTriangleIndex, endTriangleIndex);
        }
    }

    private void CreateChunk(Vector3[] vertices, int[] triangles, int startTriangleIndex, int endTriangleIndex)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        for (int i = startTriangleIndex; i < endTriangleIndex; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            if (IsDegenerate(vertices[v1], vertices[v2], vertices[v3]))
            {
                continue;
            }

            newVertices.Add(vertices[v1]);
            newVertices.Add(vertices[v2]);
            newVertices.Add(vertices[v3]);

            newTriangles.Add(newVertices.Count - 3);
            newTriangles.Add(newVertices.Count - 2);
            newTriangles.Add(newVertices.Count - 1);
        }

        Mesh newMesh = new Mesh
        {
            vertices = newVertices.ToArray(),
            triangles = newTriangles.ToArray()
        };

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        GameObject chunk = new GameObject("Mesh Chunk");
        chunk.transform.position = transform.position;
        chunk.transform.rotation = transform.rotation;
        chunk.transform.localScale = Vector3.one;

        MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
        meshFilter.mesh = newMesh;

        MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = GetComponent<MeshRenderer>().sharedMaterials;

        MeshCollider meshCollider = chunk.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        Rigidbody rb = chunk.AddComponent<Rigidbody>();
        rb.mass = 1f;

        Destroy(chunk, 3f);
    }

    private bool IsDegenerate(Vector3 vector31, Vector3 vector32, Vector3 vector33)
    {
        return Vector3.Distance(vector31, vector32) <Mathf.Epsilon
            || Vector3.Distance(vector32, vector33) < Mathf.Epsilon
            || Vector3.Distance(vector33, vector31) < Mathf.Epsilon;
    }

    public void DestroyMesh()
    {
        // disable the mesh renderer so the original mesh is no longer visible
        meshRenderer.enabled = false;

        //if (!originalMesh.isReadable)
        //{
        //    originalMesh.Optimize();
        //    originalMesh.MarkDynamic();
        //}

        // iterate over all the triangles in the original mesh and create a new game object for each triangle
        for (int i=0;i < originalMesh.triangles.Length;i += 3)
        {
            // get the vertices for this triangle
            Vector3 vertex1 = originalMesh.vertices[originalMesh.triangles[i]];
            Vector3 vertex2 = originalMesh.vertices[originalMesh.triangles[i + 1]];
            Vector3 vertex3 = originalMesh.vertices[originalMesh.triangles[i + 2]];

            // calculate the center of the triangle
            Vector3 center = (vertex1 + vertex2 + vertex3) / 3f;

            // create a new game object for this triangle
            GameObject piece = new GameObject();
            piece.transform.position = transform.TransformPoint(center);
            piece.transform.rotation = transform.rotation;
            piece.transform.localScale = transform.localScale;

            // add a mesh filter component to the new game object and set its mesh to a mesh that only contains this triangle
            MeshFilter pieceMeshFilter = piece.AddComponent<MeshFilter>();
            Mesh pieceMesh = new Mesh();
            pieceMesh.vertices = new Vector3[] { vertex1, vertex2, vertex3 };
            pieceMesh.triangles = new int[] { 0, 1, 2 };
            pieceMeshFilter.mesh = pieceMesh;

            // add a mesh renderer component to the new game object and set its material to the material of the original mesh
            MeshRenderer pieceMeshRenderer = piece.AddComponent<MeshRenderer>();
            pieceMeshRenderer.material = meshRenderer.material;

            // add a rigidbody component to the new game object and apply an explosion force to it
            Rigidbody pieceRigidbody = piece.AddComponent<Rigidbody>();
            pieceRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier);

            // destroy the new game object after a certain amount of time
            Destroy(piece, lifeTime);
        }        
    }

    public GridPosition GetGridPosition()
    {
        return currentGridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public void TakeDamage(int damage)
    {
        numberOfChunks += damage / 10;
        DestroyMeshBySplitting();        
    }

    public bool IsOnSameTeam(bool IsEnemy)
    {
        return false;
    }
}
