using UnityEngine;
using System;
using System.Collections.Generic;


public static class tk2dTextGeomGen
{
	// Channel select color constants
	static readonly Color32[] channelSelectColors = new Color32[] { new Color32(0,0,255,0), new Color(0,255,0,0), new Color(255,0,0,0), new Color(0,0,0,255) };


	public class GeomData
	{
		tk2dTextMeshData textMeshData = null;
		int currentAllocatedCharacters = 0;

		// Use this to get a correctly set up textgeomdata object
		// This uses a static global tmpData object and is not thread safe
		// Fortunately for us, neither is the rest of Unity.
		public GeomData(tk2dTextMeshData _textMeshData, int _currentAllocatedCharacters)
		{
			textMeshData = _textMeshData;
			currentAllocatedCharacters = _currentAllocatedCharacters;
		}


		public int RequiredAllocatedCharacters
		{
			get
			{
				return Math.Max(currentAllocatedCharacters, textMeshData.RequiredCharactersCount);
			}
		}


		public bool ReallocRequired
		{
			get
			{
				return RequiredAllocatedCharacters != currentAllocatedCharacters;
			}
		}


		public tk2dTextMeshData TextMeshData
		{
			get
			{
				return textMeshData;
			}
		}


		public int CurrentAllocatedCharacters
		{
			get
			{
				return currentAllocatedCharacters;
			}
		}
	}


	public class InlineStyler
	{
		// Inline styling
		public Color32 meshTopColor = new Color32(255, 255, 255, 255);
		public Color32 meshBottomColor = new Color32(255, 255, 255, 255);
		public float meshGradientTexU = 0.0f;
		public int curGradientCount = 1;
	}


	/// <summary>
	/// Calculates the mesh dimensions for the given string
	/// and returns a width and height.
	/// </summary>
	public static Vector2 GetMeshDimensionsForString(this tk2dTextMeshData geomData, tk2dColoredText inputText, int startIdx, int count)
	{
		tk2dTextMeshData data = geomData;
		tk2dFontData _fontInst = data.FontInst;

		float maxWidth = 0.0f;
		
		float cursorX = 0.0f;
		float cursorY = 0.0f;

		for (int i = startIdx; (i < inputText.Length) && (i < (startIdx + count)); ++i)
		{
			int idx = inputText[i];
			if (idx == '\n')
			{
				maxWidth = Mathf.Max(cursorX, maxWidth);
				cursorX = 0.0f;
				cursorY -= data.ActualLineSpaceHeight;
			}
			else
			{
				// Get the character from dictionary / array
				tk2dFontChar chr = _fontInst.GetCharForIndex(idx, 0);
				
				cursorX += (chr.advance + data.Spacing) * data.TotalScale.x;
				if (data.kerning && i < inputText.Length - 1)
				{
					foreach (var k in _fontInst.kerning)
					{
						if (k.c0 == inputText[i] && k.c1 == inputText[i+1])
						{
							cursorX += k.amount * data.TotalScale.x;
							break;
						}
					}
				}
			}
		}
		
		maxWidth = Mathf.Max(cursorX, maxWidth);
		cursorY -= data.ActualLineHeight;
		
		return new Vector2(maxWidth, cursorY);
	}


	public static Vector2 GetMeshDimensionsForString(this tk2dTextMeshData geomData, tk2dColoredText inputText)
	{
		return GetMeshDimensionsForString(geomData, inputText, 0, inputText.Length);
	}


	public static Vector2 GetMeshDimensionsForString(this tk2dTextMeshData geomData, string inputString)
	{
		return GetMeshDimensionsForString(geomData, new tk2dColoredText(inputString, Color.white));
	}


	public static float GetYAnchorForHeight(this tk2dTextMeshData geomData, float textHeight)
	{
		tk2dTextMeshData data = geomData;
		tk2dFontData _fontInst = data.FontInst;

		float lineHeight = data.ActualLineHeight;
		switch (data.anchor)
		{
		case TextAnchor.UpperCenter:
		case TextAnchor.UpperLeft:
		case TextAnchor.UpperRight:
			return -lineHeight;

		case TextAnchor.MiddleCenter:
		case TextAnchor.MiddleLeft:
		case TextAnchor.MiddleRight:
		{
			float y = - textHeight / 2.0f - lineHeight;
			if (_fontInst.version >= 2) 
			{
				float ty = _fontInst.texelSize.y * data.TotalScale.y;
				return Mathf.Floor(y / ty) * ty;
			}
			else
			{
				return y;
			}
		}

		case TextAnchor.LowerCenter:
		case TextAnchor.LowerLeft:
		case TextAnchor.LowerRight:
			return - textHeight - lineHeight;
		}

		return -lineHeight;
	}


	public static float GetXAnchorForWidth(this tk2dTextMeshData geomData, float lineWidth)
	{
		tk2dTextMeshData data = geomData;
		tk2dFontData _fontInst = data.FontInst;

		int widthAnchor = (int)data.anchor % 3;
		switch (widthAnchor)
		{
		case 0: return 0.0f; // left
		case 1: // center
		{
			float x = -lineWidth / 2.0f;
			if (_fontInst.version >= 2) 
			{
				float tx = _fontInst.texelSize.x * data.TotalScale.x;
				return Mathf.Floor(x / tx) * tx;
			}
			return x;
		}
		case 2: return -lineWidth; // right
		}
		return 0.0f;
	}


	static void PostAlignTextData(Vector3[] pos, int offset, int targetStart, int targetEnd, float offsetX)
	{
		for (int i = targetStart * 4; i < targetEnd * 4; ++i)
		{
			Vector3 v = pos[offset + i];
			v.x += offsetX;
			pos[offset + i] = v;
		}
	}


	public static void GetTextMeshGeomDesc(this GeomData geomData, out int numVertices, out int numIndices)
	{
		numVertices = geomData.RequiredAllocatedCharacters * 4;
		numIndices = geomData.RequiredAllocatedCharacters * 6;
	}


	public static void SetTextMeshGeom(this GeomData geomData, Vector3[] pos, Vector2[] uv, Vector2[] uv2, Color32[] color, int offset)
	{
		tk2dTextMeshData data = geomData.TextMeshData;
		tk2dFontData fontInst = data.FontInst;
		tk2dColoredText formattedText = data.FormattedText;

		InlineStyler curStyler = new InlineStyler();
		curStyler.meshTopColor = new Color32(255, 255, 255, 255);
		curStyler.meshBottomColor = new Color32(255, 255, 255, 255);
		curStyler.meshGradientTexU = (float)data.textureGradient / (float)((fontInst.gradientCount > 0) ? fontInst.gradientCount : 1);
		curStyler.curGradientCount = fontInst.gradientCount;

		Vector2 dims = data.GetMeshDimensionsForString(formattedText);
		float offsetY = data.GetYAnchorForHeight(dims.y);

		float cursorX = 0.0f;
		float cursorY = 0.0f;

		// target is required due to invisible '\n' character
		int target = 0;
		int alignStartTarget = 0;
		for (int i = 0; i < formattedText.Length && target < geomData.CurrentAllocatedCharacters; ++i)
		{
			formattedText.ApplyColorCommand(curStyler, i);

			int idx = formattedText[i];
			tk2dFontChar chr = fontInst.GetCharForIndex(idx, 0);
			
			if (idx == '\n')
			{
				float lineWidth = cursorX;
				int alignEndTarget = target; // this is one after the last filled character
				if (alignStartTarget != target)
				{
					float xOffset = data.GetXAnchorForWidth(lineWidth);
					PostAlignTextData(pos, offset, alignStartTarget, alignEndTarget, xOffset);
				}
				
				
				alignStartTarget = target;
				cursorX = 0.0f;
				cursorY -= data.ActualLineSpaceHeight;
			}
			else
			{
				pos[offset + target * 4 + 0] = new Vector3(cursorX + chr.p0.x * data.TotalScale.x, offsetY + cursorY + chr.p0.y * data.TotalScale.y, 0);
				pos[offset + target * 4 + 1] = new Vector3(cursorX + chr.p1.x * data.TotalScale.x, offsetY + cursorY + chr.p0.y * data.TotalScale.y, 0);
				pos[offset + target * 4 + 2] = new Vector3(cursorX + chr.p0.x * data.TotalScale.x, offsetY + cursorY + chr.p1.y * data.TotalScale.y, 0);
				pos[offset + target * 4 + 3] = new Vector3(cursorX + chr.p1.x * data.TotalScale.x, offsetY + cursorY + chr.p1.y * data.TotalScale.y, 0);

				if (chr.flipped)
				{
					uv[offset + target * 4 + 0] = new Vector2(chr.uv1.x, chr.uv1.y);
					uv[offset + target * 4 + 1] = new Vector2(chr.uv1.x, chr.uv0.y);
					uv[offset + target * 4 + 2] = new Vector2(chr.uv0.x, chr.uv1.y);
					uv[offset + target * 4 + 3] = new Vector2(chr.uv0.x, chr.uv0.y);
				}
				else			
				{
					uv[offset + target * 4 + 0] = new Vector2(chr.uv0.x, chr.uv0.y);
					uv[offset + target * 4 + 1] = new Vector2(chr.uv1.x, chr.uv0.y);
					uv[offset + target * 4 + 2] = new Vector2(chr.uv0.x, chr.uv1.y);
					uv[offset + target * 4 + 3] = new Vector2(chr.uv1.x, chr.uv1.y);
				}
				
				if (fontInst.textureGradients)
				{
					uv2[offset + target * 4 + 0] = chr.gradientUv[0] + new Vector2(curStyler.meshGradientTexU, 0);
					uv2[offset + target * 4 + 1] = chr.gradientUv[1] + new Vector2(curStyler.meshGradientTexU, 0);
					uv2[offset + target * 4 + 2] = chr.gradientUv[2] + new Vector2(curStyler.meshGradientTexU, 0);
					uv2[offset + target * 4 + 3] = chr.gradientUv[3] + new Vector2(curStyler.meshGradientTexU, 0);
				}
				
				if (fontInst.isPacked)
				{
					Color32 c = channelSelectColors[chr.channel];
					color[offset + target * 4 + 0] = c;
					color[offset + target * 4 + 1] = c;
					color[offset + target * 4 + 2] = c;
					color[offset + target * 4 + 3] = c;
				}
				else
				{
					color[offset + target * 4 + 0] = curStyler.meshTopColor;
					color[offset + target * 4 + 1] = curStyler.meshTopColor;
					color[offset + target * 4 + 2] = curStyler.meshBottomColor;
					color[offset + target * 4 + 3] = curStyler.meshBottomColor;
				}
				
				cursorX += (chr.advance + data.Spacing) * data.TotalScale.x;
				
				if (data.kerning && i < formattedText.Length - 1)
				{
					foreach (var k in fontInst.kerning)
					{
						if (k.c0 == formattedText[i] && k.c1 == formattedText[i+1])
						{
							cursorX += k.amount * data.TotalScale.x;
							break;
						}
					}
				}			

				target ++;
			}
		}
		
		if (alignStartTarget != target)
		{
			float lineWidth = cursorX;
			int alignEndTarget = target;
			float xOffset = data.GetXAnchorForWidth(lineWidth);
			PostAlignTextData(pos, offset, alignStartTarget, alignEndTarget, xOffset);
		}
		
		for (int i = target; i < geomData.CurrentAllocatedCharacters; ++i)
		{
			pos[offset + i * 4 + 0] = pos[offset + i * 4 + 1] = pos[offset + i * 4 + 2] = pos[offset + i * 4 + 3] = Vector3.zero;
			uv[offset + i * 4 + 0] = uv[offset + i * 4 + 1] = uv[offset + i * 4 + 2] = uv[offset + i * 4 + 3] = Vector2.zero;

			if (fontInst.textureGradients) 
			{
				uv2[offset + i * 4 + 0] = uv2[offset + i * 4 + 1] = uv2[offset + i * 4 + 2] = uv2[offset + i * 4 + 3] = Vector2.zero;
			}				

			color[offset + i * 4 + 0] = color[offset + i * 4 + 1] = color[offset + i * 4 + 2] = color[offset + i * 4 + 3] = Color.clear;
		}
	}


	public static void SetTextMeshIndices(this GeomData geomData, int[] indices, int offset, int vStart)
	{
		for (int i = 0; i < geomData.CurrentAllocatedCharacters; ++i)
		{
			indices[offset + i * 6 + 0] = vStart + i * 4 + 0;
			indices[offset + i * 6 + 1] = vStart + i * 4 + 1;
			indices[offset + i * 6 + 2] = vStart + i * 4 + 3;
			indices[offset + i * 6 + 3] = vStart + i * 4 + 2;
			indices[offset + i * 6 + 4] = vStart + i * 4 + 0;
			indices[offset + i * 6 + 5] = vStart + i * 4 + 3;
		}
	}
}