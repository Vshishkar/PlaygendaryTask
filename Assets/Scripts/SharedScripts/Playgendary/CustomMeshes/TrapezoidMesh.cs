using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TrapezoidMesh : DrawableMesh 
{
    #region Variables

    [SerializeField] float topWidth;
    [SerializeField] float bottomWidth;
    [SerializeField] float height;  
    [SerializeField] float smoothFactorX = 0.01f;
    [SerializeField] float smoothFactorY = 0.01f;


    public float TopWidth
    {
        get
        {
            return topWidth;
        }
        set
        {
            topWidth = value;
            UpdateMesh();
        }
    }

    public float BottomWidth
    {
        get
        {
            return bottomWidth;
        }
        set
        {
            bottomWidth = value;
            UpdateMesh();
        }
    }

    public float Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
            UpdateMesh();
        }
    }

    #endregion


    #region Unity Lifecycle

    void Awake()
    {
        UpdateMesh();
    }

    #endregion


    #region Private


    public override void UpdateMesh()
    {
        Mesh mesh = new Mesh();

        CachedMeshFilter.sharedMesh = mesh;

        CheckSpriteCollectionData();

        Vector3 [] vertices = new Vector3[8];
        Vector2[] uvs = new Vector2[8];

        Color [] colors = new Color[8];

        int[] triangles = new int[10 * 3]
        { 
            0,2,1,
            0,3,2,
            4,3,0,
            4,7,3,
            7,2,3,
            7,6,2,
            6,1,2,
            6,5,1,
            5,0,1,
            5,4,0        
        };

        vertices[0] = new Vector3(-bottomWidth * 0.5f, -height * 0.5f, 0);
        vertices[1] = new Vector3(-topWidth * 0.5f, height * 0.5f, 0);
        vertices[2] = new Vector3(topWidth * 0.5f, height * 0.5f, 0); 
        vertices[3] = new Vector3(bottomWidth * 0.5f, -height * 0.5f, 0);                 
 
        for (int i = 4; i < 8; i++)
        {
            vertices[i] = new Vector3(vertices[i - 4].x * (1 + smoothFactorX), vertices[i - 4].y * (1 + smoothFactorY), 1);
        }

        for (int i = 0; i < 8; i++)
        {
            vertices[i] *= SizeHelper.HeightFactor;

            colors[i] = Color.white;

            if (i > 3)
            {
                colors[i].a = 0;
            }

            if (isUVFromSpriteAvailible)
            {
                uvs[i] = uvFromSprite;
            }
            else
            {
                uvs[i] = Vector2.zero;
            }
        }

        RecalculateAnchor(ref vertices);

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        originalMeshColors = colors;
        originalVertices = vertices;

        if (Color != Color.white)
            UpdateAnimatedColors();

        if (Scale != Vector2.one)
            UpdateAnimatedScale();

    }

    public override void SetBaseMeshColor(Color c)
    {
        this.Color = c;

        UpdateAnimatedColors();
    }

    #if UNITY_EDITOR
    void OnValidate()
    {       
        UpdateMesh();
    }
    #endif

    #endregion
}