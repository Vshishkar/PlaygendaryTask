using UnityEngine;
 

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DrawableMesh : MonoBehaviour 
{
    public enum VerticalAnchor
    {
        Top,
        Center,
        Bottom
    }
    public enum HorizontalAnchor
    {
        Left,
        Center,
        Right
    }

	#region Variables

    [SerializeField] tk2dSpriteCollectionData data;
    [SerializeField] string spriteAtlasName;

    tk2dSpriteCollectionData dataInstance;
    protected Vector2 uvFromSprite;
    protected bool isUVFromSpriteAvailible;   

    [SerializeField] public VerticalAnchor vertAnchor;
    [SerializeField] public HorizontalAnchor horizAnchor;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    [SerializeField] Color color = Color.white;
    protected Color[] originalMeshColors;
    protected Vector3[] originalVertices;

    [SerializeField] Vector2 scale = Vector3.one;

    protected MeshRenderer CachedMeshRenderer
    {
        get
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            return meshRenderer;
        }
    }

    protected MeshFilter CachedMeshFilter
    {
        get
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            return meshFilter;
        }
    }

    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            UpdateAnimatedColors();
        }
    }


    public Vector2 Scale
    {
        get { return scale; }

        set 
        {
            scale = value;
            UpdateAnimatedScale();
        }
    }
	#endregion
 
 
	#region Interfaces
	#endregion
 
 
	#region Public
    public virtual void UpdateMesh(){}

    public virtual void SetBaseMeshColor(Color c){}

	#endregion
 
 
	#region Private

    protected virtual void UpdateAnimatedColors()
    {
        if (originalMeshColors == null)
            return;

        Mesh mesh = CachedMeshFilter.sharedMesh;
        Color[] newColors = (Color[])originalMeshColors.Clone();       

        if (newColors != null)
        {
            for (int i = 0; i < newColors.Length; i++)
            {
                Color c = newColors[i];
                newColors[i] = new Color(c.r * color.r, c.g * color.g, c.b * color.b, c.a * color.a);
            }

            mesh.colors = newColors;
        }
    }

    protected virtual void UpdateAnimatedScale()
    {
        if (originalVertices == null)
            return;

        Mesh mesh = CachedMeshFilter.sharedMesh;

        Vector3[] newVertices = (Vector3[])originalVertices.Clone();

        if (originalVertices != null)
        {
            for (int i = 0; i < newVertices.Length; i++)
            {
                Vector3 v = newVertices[i];
                newVertices[i] = new Vector3(v.x * scale.x, v.y * scale.y, 0);
            }

            mesh.vertices = newVertices;
        }
    }

    protected void CheckSpriteCollectionData()
    {
        if (data != null)
        {
            dataInstance = data.inst;

            if(dataInstance != null)
            {
                foreach (tk2dSpriteDefinition def in dataInstance.spriteDefinitions)
                {
                    if (def.name.Equals(spriteAtlasName))
                    {
                        isUVFromSpriteAvailible = true;
                        uvFromSprite = def.uvs[0];

                        CachedMeshRenderer.sharedMaterial = def.materialInst;

                        break;
                    }
                }
            }
        }
    }

    protected void RecalculateAnchor(ref Vector3[] vertices)
    {
        float maxX = 0;
        float maxY = 0;
        float minX = 0;
        float minY = 0;

        foreach (Vector3 vert in vertices)
        {
            maxX = Mathf.Max(maxX, vert.x);
            minX = Mathf.Min(minX, vert.x);
            maxY = Mathf.Max(maxY, vert.y);
            minY = Mathf.Min(minY, vert.y);
        }

        Vector3 anchorOffset = Vector3.zero;

        if (vertAnchor == VerticalAnchor.Bottom)
        {
            anchorOffset.y = minY;
        }
        else if (vertAnchor == VerticalAnchor.Top)
        {
            anchorOffset.y = maxY;
        }

        if (horizAnchor == HorizontalAnchor.Left)
        {
            anchorOffset.x = minX;
        }
        else if (horizAnchor == HorizontalAnchor.Right)
        {           
            anchorOffset.x = maxX;
        }

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= anchorOffset;
        }
    }   

	#endregion
}