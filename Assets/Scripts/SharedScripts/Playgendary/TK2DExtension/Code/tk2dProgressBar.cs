using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class tk2dProgressBar : tk2dSprite 
{
	#region Constants

	static float[] constRadial360Matrix = new float[] {
		// x0 y0  x1   y1
		0.5f, 1f, 0f, 0.5f, // quadrant 0
		0.5f, 1f, 0.5f, 1f, // quadrant 1
		0f, 0.5f, 0.5f, 1f, // quadrant 2
		0f, 0.5f, 0f, 0.5f, // quadrant 3
	};

	#endregion


	#region Temp Variables

	Vector3[] tempVertices;
	Vector2[] tempUvs;
	Color[] tempColors;
	int[] tempIndexes;

	List<Vector3> tempVertexList = new List<Vector3>();
	List<Vector2> tempUVList = new List<Vector2>();

	// temp arrays for Radial types
	Vector2[] tempOXY;
	Vector2[] tempOUV;

	#endregion


	/// <summary>
	/// from superclass
	/// </summary>
	public override void Build() {
		BuildMesh();

		UpdateMaterial();
		CreateCollider();
	}

	protected override void UpdateGeometry() {
		BuildMesh();
	}

	protected override void UpdateVertices() {
		BuildMesh();
	}

	/// end


	public enum FillDirection{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360,
	}

	[SerializeField] FillDirection mFillDirection = FillDirection.Radial360;
	[SerializeField] float mFillAmount = 1.0f;
	[SerializeField] bool mInvert = false;
	[SerializeField] bool mRotated = false;

	bool changed = false;

	protected override void Update() 
	{
		base.Update();

		if (changed) 
		{
			BuildMesh();
		}
	}

	void BuildMesh()
	{
		MeshClear();

		if (meshColliderMesh != null)
		{
			meshColliderMesh.Clear();
		}


		tk2dSpriteDefinition sprite = CurrentSprite;
		Bounds bounds = sprite.GetBounds();
		Vector3 actualSpriteSize = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);

		// round fill amount - so vertices and uv become rounded
		float curFillAmount = mFillAmount;
		bool fillWasRounded = false;

		if ( !isRoundShiftDisabled && (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical) )
		{
			float curSize = (mFillDirection == FillDirection.Horizontal) ? actualSpriteSize.x : actualSpriteSize.y;

			curFillAmount *= curSize;
			curFillAmount = (mInvert) ? Mathf.Ceil(curFillAmount) : Mathf.Floor(curFillAmount);
			curFillAmount /= curSize;

			fillWasRounded = true;
		}

		OnFill(ref tempVertices, ref tempUvs, ref tempColors, curFillAmount);

        Vector3 centerCorrection = new Vector3(-bounds.size.x * 0.5f * scale.x, bounds.size.y * 0.5f * scale.y) + bounds.center;
		Vector3 totalScale = actualSpriteSize;
		totalScale.Scale(scale);

		for (int i = 0; i < tempVertices.Length; i++) 
		{
			tempVertices[i] = Vector3.Scale(tempVertices[i], totalScale) + centerCorrection;

			if (!isRoundShiftDisabled)
			{
				if (fillWasRounded)
				{
					tempVertices[i] = RoundVertex(tempVertices[i]);
				}
				else
				{
					tempVertices[i] = FloorVertex(tempVertices[i]);
				}
			}
		}


        if (rotated ^ (sprite.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)) 
		{
			float u0 = 1;
			float v0 = 1;
			float u1 = 0;
			float v1 = 0;

			foreach (Vector2 v in sprite.uvs) {
				u0 = u0 > v.x ? v.x : u0;
				v0 = v0 > v.y ? v.y : v0;

				u1 = u1 < v.x ? v.x : u1;
				v1 = v1 < v.y ? v.y : v1;
			}

			float du = (u1 - u0);
			float dv = (v1 - v0);
			Vector2 zero = new Vector2(u0, v0);
			Vector2 zero1 = new Vector2(u1, v0);

			for (int i = 0; i < tempUvs.Length; i++) 
			{
				Vector2 uvpos = (tempUvs[i] - zero);
				tempUvs[i] = zero1 + new Vector2(-du * uvpos.y / dv, dv * uvpos.x / du);
			}
		}
        else
        if (rotated ^ (sprite.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)) 
        {
            float u0 = 1;
            float v0 = 1;
            float u1 = 0;
            float v1 = 0;

            foreach (Vector2 v in sprite.uvs) {
                u0 = u0 > v.x ? v.x : u0;
                v0 = v0 > v.y ? v.y : v0;

                u1 = u1 < v.x ? v.x : u1;
                v1 = v1 < v.y ? v.y : v1;
            }

            float du = (u1 - u0);
            float dv = (v1 - v0);
            Vector2 zero = new Vector2(u0, v0);
            Vector2 zero1 = new Vector2(u0, v0);

            for (int i = 0; i < tempUvs.Length; i++) 
            {
                Vector2 uvpos = (tempUvs[i] - zero);
                tempUvs[i] = zero1 + new Vector2(du * uvpos.y / dv, dv * uvpos.x / du);
            }
        }

		int quads = tempVertices.Length / 4;
		tempIndexes = ArrayExtention.EnsureLength(tempIndexes, quads * 6);

		for (int i = 0; i < quads; i++) 
		{
			tempIndexes[6 * i + 0] = 4 * i + 0;
			tempIndexes[6 * i + 1] = 4 * i + 1;
			tempIndexes[6 * i + 2] = 4 * i + 2;
			tempIndexes[6 * i + 3] = 4 * i + 2;
			tempIndexes[6 * i + 4] = 4 * i + 3;
			tempIndexes[6 * i + 5] = 4 * i + 0;
		}


		MeshVertices = tempVertices;
		MeshUV = tempUvs;
		MeshColors = tempColors;
		MeshTriangles = tempIndexes;

		if (meshColliderMesh != null)
		{
			meshColliderMesh.vertices = tempVertices;
			meshColliderMesh.uv = tempUvs;
			meshColliderMesh.colors = tempColors;
			meshColliderMesh.triangles = tempIndexes;
		}


		changed = false;
	}

	/// <summary>
	/// ////
	/// </summary>
	/// <value>The fill direction.</value>
	/// 
	public FillDirection fillDirection {
		get {
			return mFillDirection;
		}
		set {
			if (mFillDirection != value) {
				mFillDirection = value;
				changed = true;
			}
		}
	}

	public float fillAmount {
		get {
			return mFillAmount;
		}
		set {
			float val = Mathf.Clamp01(value);

			if (mFillAmount != val) {
				mFillAmount = val;
				changed = true;
			}
		}
	}

	public bool invert {
		get {
			return mInvert;
		}
		set {
			if (mInvert != value) {
				mInvert = value;
				changed = true;
			}
		}
	}

	public bool rotated {
		get {
			return mRotated;
		}
		set {
			if (mRotated != value) {
				mRotated = value;
				changed = true;
			}
		}
	}


	bool AdjustRadial(Vector3[] xy, Vector2[] uv, float fill, bool invert)
	{
		// TODO: do not allocate if AdjustRadial(Vector2[] ...) returns immidiately

		Vector2[] oxy = new Vector2[xy.Length];
		for (int i = 0; i < oxy.Length; i++)
		{
			oxy[i] = xy[i];
		}
		
		// Adjust the quad radially, and if 'false' is returned (it's not visible), just exit
		var result = AdjustRadial(oxy, uv, fill, invert);
		
		for (int i = 0; i < xy.Length; i++)
		{
			if (result)
			{
				xy[i] = oxy[i];
			}
			else
			{
				xy[i] = Vector2.zero;
			}
		}

		return result;
	}


	//??????///
	bool AdjustRadial(Vector2[] xy, Vector2[] uv, float fill, bool invert)
	{
		// Nothing to fill
		if (fill < 0.001f)
			return false;

		// Nothing to adjust
		if (!invert && fill > 0.999f)
			return true;

		// Convert 0-1 value into 0 to 90 degrees angle in radians  
		float angle = Mathf.Clamp01(fill);
		if (!invert)
			angle = 1f - angle;
		angle *= 90f * Mathf.Deg2Rad;

		// Calculate the effective X and Y factors
		float fx = Mathf.Sin(angle);
		float fy = Mathf.Cos(angle);

		// Normalize the result, so it's projected onto the side of the rectangle
		if (fx > fy) {
			fy *= 1f / fx;
			fx = 1f;

			if (!invert) {
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, fy);
				xy[3].y = xy[0].y;

				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, fy);
				uv[3].y = uv[0].y;
			}
		} else
		if (fy > fx) {
			fx *= 1f / fy;
			fy = 1f;

			if (invert) {
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, fx);
				xy[1].x = xy[0].x;

				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, fx);
				uv[1].x = uv[0].x;
			}
		} else {
			fx = 1f;
			fy = 1f;
		}

		if (invert) {
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, fy);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, fy);
		} else {
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, fx);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, fx);
		}
		return true;
	}

	/// <summary>
	/// Helper function that copies the contents of the array, rotated by the specified offset.
	/// </summary>

	void Rotate(Vector2[] v, int offset)
	{
		for (int i = 0; i < offset; ++i) 
		{
			Vector2 v0 = new Vector2(v[3].x, v[3].y);

			v[3].x = v[2].y;
			v[3].y = v[2].x;

			v[2].x = v[1].y;
			v[2].y = v[1].x;

			v[1].x = v[0].y;
			v[1].y = v[0].x;

			v[0].x = v0.y;
			v[0].y = v0.x;
		}
	}

	/// <summary>
	/// Virtual function called by the UIScreen that fills the buffers.
	/// </summary>

	public void OnFill(ref Vector3[] verts_array, ref Vector2[] uvs_array, ref Color[] cols_array, float curFillAmount) 
	{
		tk2dSpriteDefinition sprite = CurrentSprite;


		float x0 = 0f;
		float y0 = 0f;
		float x1 = 1;
		float y1 = -1;


		float u0 = 1;
		float v0 = 1;
		float u1 = 0;
		float v1 = 0;

		foreach (Vector2 v in sprite.uvs) 
		{
			u0 = u0 > v.x ? v.x : u0;
			v0 = v0 > v.y ? v.y : v0;

			u1 = u1 < v.x ? v.x : u1;
			v1 = v1 < v.y ? v.y : v1;
		}


		// Horizontal and vertical filled sprites are simple -- just end the sprite prematurely
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical) 
		{
			float du = (u1 - u0) * curFillAmount;
			float dv = (v1 - v0) * curFillAmount;

			if (fillDirection == FillDirection.Horizontal) 
			{
				if (mInvert) 
				{
					x0 = (1f - curFillAmount);
					u0 = u1 - du;
				}
				else 
				{
					x1 *= curFillAmount;
					u1 = u0 + du;
				}
			} 
			else if (fillDirection == FillDirection.Vertical) 
			{
				if (mInvert) 
				{
					y1 *= curFillAmount;
					v0 = v1 - dv;
				}
				else 
				{
					y0 = -(1f - curFillAmount);
					v1 = v0 + dv;
				}
			}
		}



		bool is4VertexType = (fillDirection == FillDirection.Radial90 || fillDirection == FillDirection.Horizontal || fillDirection == FillDirection.Vertical);

		// Starting quad for the sprite
		verts_array = ArrayExtention.EnsureLength(verts_array, 4, !is4VertexType);
		uvs_array = ArrayExtention.EnsureLength(uvs_array, 4, !is4VertexType);

		verts_array[0] = new Vector2(x1, y0);
		verts_array[1] = new Vector2(x1, y1);
		verts_array[2] = new Vector2(x0, y1);
		verts_array[3] = new Vector2(x0, y0);

		uvs_array[0] = new Vector2(u1, v1);
		uvs_array[1] = new Vector2(u1, v0);
		uvs_array[2] = new Vector2(u0, v0);
		uvs_array[3] = new Vector2(u0, v1);


		if (is4VertexType)
		{
			if (fillDirection == FillDirection.Radial90) 
			{
				// Adjust the quad radially, and if 'false' is returned (it's not visible), just exit
				AdjustRadial(verts_array, uvs_array, curFillAmount, mInvert);
			}
		}
		else
		{
			tempVertexList.Clear();
			tempUVList.Clear();

			tempOXY = ArrayExtention.EnsureLength(tempOXY, 4);
			tempOUV = ArrayExtention.EnsureLength(tempOUV, 4);

			if (fillDirection == FillDirection.Radial180) 
			{
				for (int i = 0; i < 2; ++i) 
				{
					tempOXY[0] = new Vector2(0f, 0f);
					tempOXY[1] = new Vector2(0f, 1f);
					tempOXY[2] = new Vector2(1f, 1f);
					tempOXY[3] = new Vector2(1f, 0f);

					tempOUV[0] = new Vector2(0f, 0f);
					tempOUV[1] = new Vector2(0f, 1f);
					tempOUV[2] = new Vector2(1f, 1f);
					tempOUV[3] = new Vector2(1f, 0f);

					// Each half must be rotated 90 degrees clockwise in order for it to fill properly
					if (mInvert)
					{
						if (i > 0) 
						{
							Rotate(tempOXY, i);
							Rotate(tempOUV, i);
						}
					} 
					else if (i < 1) 
					{
						Rotate(tempOXY, 1 - i);
						Rotate(tempOUV, 1 - i);
					}

					// Each half must fill in only a part of the space
					float x, y;

					if (i == 1) 
					{
						x = mInvert ? 0.5f : 1f;
						y = mInvert ? 1f : 0.5f;
					}
					else 
					{
						x = mInvert ? 1f : 0.5f;
						y = mInvert ? 0.5f : 1f;
					}

					tempOXY[1].y = Mathf.Lerp(x, y, tempOXY[1].y);
					tempOXY[2].y = Mathf.Lerp(x, y, tempOXY[2].y);
					tempOUV[1].y = Mathf.Lerp(x, y, tempOUV[1].y);
					tempOUV[2].y = Mathf.Lerp(x, y, tempOUV[2].y);

					float amount = (curFillAmount) * 2 - i;
					bool odd = (i % 2) == 1;

					if (AdjustRadial(tempOXY, tempOUV, amount, !odd)) 
					{
						if (mInvert)
							odd = !odd;

						// Add every other side in reverse order so they don't come out backface-culled due to rotation
						if (odd) 
						{
							for (int b = 0; b < 4; ++b) 
							{
								x = Mathf.Lerp(verts_array[0].x, verts_array[2].x, tempOXY[b].x);
								y = Mathf.Lerp(verts_array[0].y, verts_array[2].y, tempOXY[b].y);

								float u = Mathf.Lerp(uvs_array[0].x, uvs_array[2].x, tempOUV[b].x);
								float v = Mathf.Lerp(uvs_array[0].y, uvs_array[2].y, tempOUV[b].y);

								tempVertexList.Add(new Vector3(x, y, 0f));
								tempUVList.Add(new Vector2(u, v));
							}
						} 
						else 
						{
							for (int b = 3; b > -1; --b) 
							{
								x = Mathf.Lerp(verts_array[0].x, verts_array[2].x, tempOXY[b].x);
								y = Mathf.Lerp(verts_array[0].y, verts_array[2].y, tempOXY[b].y);

								float u = Mathf.Lerp(uvs_array[0].x, uvs_array[2].x, tempOUV[b].x);
								float v = Mathf.Lerp(uvs_array[0].y, uvs_array[2].y, tempOUV[b].y);

								tempVertexList.Add(new Vector3(x, y, 0f));
								tempUVList.Add(new Vector2(u, v));
							}
						}
					}
				}
			} 
			else if (fillDirection == FillDirection.Radial360) 
			{
				for (int i = 0; i < 4; ++i) 
				{
					tempOXY[0] = new Vector2(0f, 0f);
					tempOXY[1] = new Vector2(0f, 1f);
					tempOXY[2] = new Vector2(1f, 1f);
					tempOXY[3] = new Vector2(1f, 0f);

					tempOUV[0] = new Vector2(0f, 0f);
					tempOUV[1] = new Vector2(0f, 1f);
					tempOUV[2] = new Vector2(1f, 1f);
					tempOUV[3] = new Vector2(1f, 0f);

					// Each quadrant must be rotated 90 degrees clockwise in order for it to fill properly
					if (mInvert) 
					{
						if (i > 0) 
						{
							Rotate(tempOXY, i);
							Rotate(tempOUV, i);
						}
					} 
					else if (i < 3) 
					{
						Rotate(tempOXY, 3 - i);
						Rotate(tempOUV, 3 - i);
					}

					// Each quadrant must fill in only a quarter of the space
					for (int b = 0; b < 4; ++b) 
					{
						int index = (mInvert) ? (3 - i) * 4 : i * 4;

						float fx0 = constRadial360Matrix[index];
						float fy0 = constRadial360Matrix[index + 1];
						float fx1 = constRadial360Matrix[index + 2];
						float fy1 = constRadial360Matrix[index + 3];

						tempOXY[b].x = Mathf.Lerp(fx0, fy0, tempOXY[b].x);
						tempOXY[b].y = Mathf.Lerp(fx1, fy1, tempOXY[b].y);
						tempOUV[b].x = Mathf.Lerp(fx0, fy0, tempOUV[b].x);
						tempOUV[b].y = Mathf.Lerp(fx1, fy1, tempOUV[b].y);
					}

					float amount = (curFillAmount) * 4 - i;
					bool odd = (i % 2) == 1;

					if (AdjustRadial(tempOXY, tempOUV, amount, !odd)) 
					{
						if (mInvert)
							odd = !odd;

						// Add every other side in reverse order so they don't come out backface-culled due to rotation
						if (odd) 
						{
							for (int b = 0; b < 4; ++b) 
							{
								float x = Mathf.Lerp(verts_array[0].x, verts_array[2].x, tempOXY[b].x);
								float y = Mathf.Lerp(verts_array[0].y, verts_array[2].y, tempOXY[b].y);
								float u = Mathf.Lerp(uvs_array[0].x, uvs_array[2].x, tempOUV[b].x);
								float v = Mathf.Lerp(uvs_array[0].y, uvs_array[2].y, tempOUV[b].y);

								tempVertexList.Add(new Vector3(x, y, 0f));
								tempUVList.Add(new Vector2(u, v));
							}
						} 
						else 
						{
							for (int b = 3; b > -1; --b) 
							{
								float x = Mathf.Lerp(verts_array[0].x, verts_array[2].x, tempOXY[b].x);
								float y = Mathf.Lerp(verts_array[0].y, verts_array[2].y, tempOXY[b].y);
								float u = Mathf.Lerp(uvs_array[0].x, uvs_array[2].x, tempOUV[b].x);
								float v = Mathf.Lerp(uvs_array[0].y, uvs_array[2].y, tempOUV[b].y);

								tempVertexList.Add(new Vector3(x, y, 0f));
								tempUVList.Add(new Vector2(u, v));
							}
						}
					}
				}
			}

			verts_array = ArrayExtention.EnsureLength(verts_array, tempVertexList.Count);
			uvs_array = ArrayExtention.EnsureLength(uvs_array, tempUVList.Count);

			for (int i = 0; i < tempVertexList.Count; i++)
			{
                verts_array[i] = tempVertexList[i];
			}

			for (int i = 0; i < tempUVList.Count; i++)
			{
				uvs_array[i] = tempUVList[i];
			}
		}

		// Fill the buffer with the quad for the sprite
		cols_array = ArrayExtention.EnsureLength(cols_array, verts_array.Length);
		for (int i = 0; i < cols_array.Length; ++i) 
		{
			cols_array[i] = color;
		}
	}
}
