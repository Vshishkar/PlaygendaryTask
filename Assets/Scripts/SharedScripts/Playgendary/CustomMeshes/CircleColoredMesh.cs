using UnityEngine;
 
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CircleColoredMesh : tk2dBaseSprite 
{
	#region Variables
    [SerializeField] int vertexCount = 10;


    public int VertexCount
    {
        get { return vertexCount; }
        set { vertexCount = value; }
    }

	#endregion
 

    #region Unity Lifecycle
    new void Awake()
    {
        base.Awake();

        Build();
        UpdateMaterial();
    }

    #endregion


	#region Interfaces
	#endregion
 
 
	#region Public
    public override void Build()
    {
		MeshClear();

		var sprite = CurrentSprite;

        Vector3[] vertices = new Vector3[vertexCount * 2 - 1];
        Vector3[] normals = new Vector3[vertexCount * 2 - 1];
        Color[] colors = new Color[vertexCount * 2 - 1];
        Vector2[] uvs = new Vector2[vertexCount * 2 - 1];

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = ((float)i - 1f) / ((float)vertexCount - 1f) * Mathf.PI * 2;

            vertices[i] = new Vector3(scale.x * Mathf.Sin(angle), scale.y * Mathf.Cos(angle), 0);
            colors[i] = color;

            normals[i] = Vector3.one;
            uvs[i] = sprite.uvs[0];

            if (i > 0)
            {
                vertices[vertexCount - 1 + i] = new Vector3(1.03f * scale.x * Mathf.Sin(angle), 1.03f * scale.y * Mathf.Cos(angle), 0);
                colors[vertexCount - 1 + i] = color;
                colors[vertexCount - 1 + i].a = 0;

                normals[vertexCount - 1 + i] = Vector3.one;
                uvs[vertexCount - 1 + i] = sprite.uvs[0];
            }
        }

        vertices[0] = Vector3.zero;

        int [] triangles = new int[(vertexCount - 1) * 9];

        for (int i = 1; i < vertexCount - 1; i++)
        {
            triangles[(i - 1) * 9] = 0;
            triangles[(i - 1) * 9 + 1] = i + 1;
            triangles[(i - 1) * 9 + 2] = i;

            triangles[(i - 1) * 9 + 3] = i;
            triangles[(i - 1) * 9 + 4] = vertexCount + i;
            triangles[(i - 1) * 9 + 5] = vertexCount - 1 + i;

            triangles[(i - 1) * 9 + 6] = i;
            triangles[(i - 1) * 9 + 7] = i + 1;
            triangles[(i - 1) * 9 + 8] = vertexCount + i;
        }

        triangles[(vertexCount - 2) * 9] = 0;
        triangles[(vertexCount - 2) * 9 + 1] = 1;
        triangles[(vertexCount - 2) * 9 + 2] = vertexCount - 1;

        triangles[(vertexCount - 2) * 9 + 3] = vertexCount - 1;
        triangles[(vertexCount - 2) * 9 + 4] = vertexCount - 1 + 1;
        triangles[(vertexCount - 2) * 9 + 5] = (vertexCount - 1) * 2;

        triangles[(vertexCount - 2) * 9 + 6] = vertexCount - 1;
        triangles[(vertexCount - 2) * 9 + 7] = 1;
        triangles[(vertexCount - 2) * 9 + 8] = vertexCount;


		MeshVertices = vertices;
		MeshColors = colors;
		MeshUV = uvs;
		MeshTriangles = triangles;
    }
     
	#endregion
 
 
	#region Private
    protected override void UpdateMaterial()
    {
		CurrentMaterial = CurrentSprite.materialInst;
    }

    protected override void UpdateColors(){}// reupload color data only
    protected override void UpdateVertices(){} // reupload vertex data only
    protected override void UpdateGeometry(){} // update full geometry (including indices)
    protected override int  GetCurrentVertexCount(){ return vertexCount; } // return current vertex count

	#endregion
}