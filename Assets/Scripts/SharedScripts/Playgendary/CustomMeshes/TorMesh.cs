using UnityEngine;
using System.Collections.Generic;


public class TorMesh : DrawableMesh 
{
    #region Variables

	public enum GradientDirection
	{
		Radial = 1,
		Circle = 2
	}

    [SerializeField] Mesh mesh;

    [SerializeField] int vertexCount = 20;
    [SerializeField] float segmentAngle = 360;
    [SerializeField] float smallRadius;
    [SerializeField] float bigRadius = 20;

    [SerializeField] Vector2 stretchSize;
    [SerializeField] float startAngle;

    [SerializeField] GradientDirection direction;
    [SerializeField] Gradient gradientColors;

    [SerializeField] int smoothWidth;

    [SerializeField] bool isNeedUpdateEditor;

    List<float> gradientParts = new List<float>();   
      

    public Vector2 StretchSize
    {
        get { return stretchSize; }
        set 
        {
            stretchSize = value;
            UpdateMesh();
        }
    }

    public Vector2 BoundingBox
    {
        get {return new Vector2(2 * (bigRadius + StretchSize.x), 2 * (bigRadius + StretchSize.y)); }
        set 
        {
            Vector2 newSize = new Vector2(Mathf.Max(0, (value.x - 2 * bigRadius) * 0.5f), Mathf.Max(0, (value.y - 2 * bigRadius) * 0.5f));

            if (!newSize.Equals(StretchSize))
                StretchSize = newSize;
        }
    }

    public float BigRadius
    {
        get
        {
            return bigRadius;
        }
        set
        {
            bigRadius = value;
            UpdateMesh();
        }
    }

    public float StartAngle
    {
        get { return startAngle; }
        set { startAngle = value; }
    }

    public float SegmentAngle
    {
        get { return segmentAngle; }
        set { segmentAngle = value; }
    }

    #endregion


    #region Unity Lifecycle
	
    void Awake()
    {
        UpdateMesh();
    }

    #endregion



    #region Public Methods

    public override void UpdateMesh()
	{       
        if (smoothWidth < 0)
        {
            smoothWidth = 0;
        }

        mesh = new Mesh();

        CheckSpriteCollectionData();


        CachedMeshFilter.sharedMesh = mesh;
		mesh.Clear();

		gradientParts.Clear();

        SmoothGradient();

		foreach(GradientColorKey key in gradientColors.colorKeys)
		{
			if(!gradientParts.Contains(key.time))
				gradientParts.Add(key.time);
		}
		foreach(GradientAlphaKey key in gradientColors.alphaKeys)
		{
            if (!gradientParts.Contains(key.time))
            {
                gradientParts.Add(key.time);
            }
		}
		gradientParts.Sort();

        float angleMultiplier = (float)segmentAngle / 360f;

		int colorsCount = gradientParts.Count;
		int parts = colorsCount - 1;

        int vertexesNeeded = (int)(vertexCount * angleMultiplier) + 2;

        int totalVertex = (vertexesNeeded * colorsCount);

		Vector3[] meshVertices = new Vector3[totalVertex];
        int[] meshTriangles = new int[(vertexesNeeded - 1) * parts * 2 * 3];
		Vector2[] meshUV = new Vector2[totalVertex];
		Color[] colors = new Color[totalVertex];
		Vector3[] meshNormals = new Vector3[totalVertex];


		{
            float DeltaRadius = bigRadius + smoothWidth - smallRadius;
            float dAngle = segmentAngle * Mathf.Deg2Rad / (vertexesNeeded - 1);

            for (int i = 0; i < vertexesNeeded; i++) 
			{
				float angle = i * dAngle;
                float x = Mathf.Sin(angle + startAngle * Mathf.Deg2Rad);
                float y = Mathf.Cos(angle + startAngle * Mathf.Deg2Rad);

				for(int c = 0; c < colorsCount; c++)
				{
                    float percentFill = (float)i/vertexesNeeded;
					float radius = (smallRadius + DeltaRadius * gradientParts[c]);

                    meshVertices[i * colorsCount + c] = new Vector3(x, y, 0) * radius * SizeHelper.HeightFactor;

                    StretchVertex(ref meshVertices[i * colorsCount + c]);

                    if (isUVFromSpriteAvailible)
                    {
                        meshUV[i * colorsCount + c] = uvFromSprite;
                    }
                    else
                    {
                        meshUV[i * colorsCount + c] = new Vector2(percentFill, gradientParts[c]);
                    }

                    if(direction == GradientDirection.Radial)
					{
					   colors[i * colorsCount + c] = gradientColors.Evaluate(gradientParts[c]);
					}
					else
					{
						colors[i * colorsCount + c] = gradientColors.Evaluate(percentFill);
					}

					meshNormals[i * colorsCount + c] = Vector3.up;
				}
			}

			int quadIndex = 0;
            for (int i = 0; i < (vertexesNeeded - 1); i++) 
			{
				for(int p = 0; p < parts; p++)
				{
					meshTriangles[quadIndex + 0] = (colorsCount*i + 0 + p) % totalVertex;
					meshTriangles[quadIndex + 1] = (colorsCount*i + 1 + p) % totalVertex;
					meshTriangles[quadIndex + 2] = (colorsCount*(i + 1) + p) % totalVertex;
					meshTriangles[quadIndex + 3] = (colorsCount*(i + 1) + p) % totalVertex;
					meshTriangles[quadIndex + 4] = (colorsCount*i + 1 + p) % totalVertex;
					meshTriangles[quadIndex + 5] = (colorsCount*(i + 1) + 1 + p)% totalVertex;
					quadIndex += 6;
				}
			}
		}

        if (vertAnchor != VerticalAnchor.Center || horizAnchor != HorizontalAnchor.Center)
        {
            RecalculateAnchor(ref meshVertices);
        }

		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.uv = meshUV;
		mesh.colors = colors;
		mesh.normals = meshNormals;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
        #if !UNITY_5_5_OR_NEWER
        mesh.Optimize();
        #endif

        originalMeshColors = colors;
        originalVertices = meshVertices;

        if (Color != Color.white)
            UpdateAnimatedColors();

        if (Scale != Vector2.one)
            UpdateAnimatedScale();

	}   

    public override void SetBaseMeshColor(Color c)
    {
        GradientColorKey[] newColorKeys = new GradientColorKey[gradientColors.colorKeys.Length];

        for (int i = 0; i < gradientColors.colorKeys.Length; i++)
        {
            GradientColorKey key = gradientColors.colorKeys[i];

            newColorKeys[i] = new GradientColorKey(c, key.time);

        }

        gradientColors.SetKeys(newColorKeys, gradientColors.alphaKeys);

        UpdateMesh();
    }

    #endregion

    #region Private

    void SmoothGradient()
    {
        if (smoothWidth > 0)
        {
            if (gradientColors.colorKeys.Length < 2 || gradientColors.alphaKeys.Length < 2)
            {
                return;
            }

            GradientAlphaKey[] newAlphaKeys;

            if (smallRadius < 0.1f)
            {
                newAlphaKeys = new GradientAlphaKey[3];

                newAlphaKeys[0] = new GradientAlphaKey(255, 0);
                newAlphaKeys[1] = new GradientAlphaKey(255, 1 - smoothWidth / (bigRadius + smoothWidth));
                newAlphaKeys[2] = new GradientAlphaKey(0, 1);
            }
            else
            {
                newAlphaKeys = new GradientAlphaKey[4];

                newAlphaKeys[0] = new GradientAlphaKey(0, 0);

                newAlphaKeys[0] = new GradientAlphaKey(255, smoothWidth / (bigRadius + smoothWidth));
                newAlphaKeys[1] = new GradientAlphaKey(255, 1 - smoothWidth / (bigRadius + smoothWidth));
                newAlphaKeys[2] = new GradientAlphaKey(0, 1);
            }

            gradientColors.SetKeys(gradientColors.colorKeys, newAlphaKeys);
        }
    }

    void StretchVertex(ref Vector3 v)
    {
        if (v.x > 0)
        {
            v.x += StretchSize.x * SizeHelper.HeightFactor;
        }
        else if (v.x < 0)
        {
            v.x -= StretchSize.x * SizeHelper.HeightFactor;
        }

        if (v.y > 0)
        {
            v.y += StretchSize.y * SizeHelper.HeightFactor;
        }
        else if (v.y < 0)
        {
            v.y -= StretchSize.y * SizeHelper.HeightFactor;
        }
    }

    #endregion



#if UNITY_EDITOR
	void OnValidate()
	{
        if(isNeedUpdateEditor)
			UpdateMesh();
	}
#endif
}
