using UnityEngine;

public enum RoundFloatEnum 
{

	DontRoundFloat = -1,
	RoundToInteger = 0,
	RoundTo1DiginAfterPoint = 1,
	RoundTo2DiginAfterPoint = 2,
	RoundTo3DiginAfterPoint = 3,
	RoundTo4DiginAfterPoint = 4,
}

public enum SizeFactor 
{
	One,
	MaxFactor,
	MinFactor,
	TwoFactor,
	HeightFactor,
	WidthFactor,
	MirrorTwoFactor,
    OnlyWidhtFactor,
    OnlyHeightFactor
}

public enum Fit 
{
	DontFit,
	Height,
	Width,
	Full
}

public enum AnchorType
{
    Top,
    Bottom,
    Left,
    Right,
    Center
}

static public class SizeHelper 
{		
	static float factor = -1f;
	static public float Factor 
    {
		get 
        {
			if (factor < 0f)
            {
				factor = HeightFactor / WidthFactor;
			}
			return factor;
		}
	}

	static public Vector2 Factors 
    {
		get { return new Vector2(WidthFactor, HeightFactor); }
	}
	
	static float heightFactor = -1f;
	static public float HeightFactor 
    {
		get
        {
			if (heightFactor < 0f) 
            {
                heightFactor = ScreenDimentions.Height / (float) SizeHelperSettings.Instance.baseHeight;
			}
			return heightFactor;
		}
	}
	
	static float widthFactor = -1f;
	static public float WidthFactor 
    {
		get 
        {
			if (widthFactor < 0f) 
            {
                widthFactor = ScreenDimentions.Width / (float) SizeHelperSettings.Instance.baseWidth;
			}
			return widthFactor;
		}
	}
	
	static float maxFactor = -1f;
	static public float MaxFactor
    {
		get 
        {
			if (maxFactor < 0f) 
            {
				maxFactor = Mathf.Max(HeightFactor, WidthFactor);
			}
			return maxFactor;
		}
	}
	
	static float minFactor = -1f;
	static public float MinFactor
    {
		get 
        {
			if (minFactor < 0f) 
            {
				minFactor = Mathf.Min(HeightFactor, WidthFactor);
			}
			return minFactor;
		}
	}

	static float RoundFloat(float value, RoundFloatEnum roundPreference) 
    {
		switch (roundPreference) 
        {
            case RoundFloatEnum.RoundToInteger:
            case RoundFloatEnum.RoundTo1DiginAfterPoint:
            case RoundFloatEnum.RoundTo2DiginAfterPoint:
            case RoundFloatEnum.RoundTo3DiginAfterPoint:
            case RoundFloatEnum.RoundTo4DiginAfterPoint:
                value *= Mathf.Pow(10f, ((int) roundPreference));
                value = (int) ((Mathf.Abs(value) + 0.5f) * Mathf.Sign(value));
                value /= Mathf.Pow(10f, ((int) roundPreference));
                break;
		}
		return value;
	}

	static Vector2 RoundVector2(Vector2 vector, RoundFloatEnum roundPreference) 
    {
        switch (roundPreference) 
        {
            case RoundFloatEnum.RoundToInteger:
            case RoundFloatEnum.RoundTo1DiginAfterPoint:
            case RoundFloatEnum.RoundTo2DiginAfterPoint:
            case RoundFloatEnum.RoundTo3DiginAfterPoint:
            case RoundFloatEnum.RoundTo4DiginAfterPoint:
                vector.Scale(Vector2.one * Mathf.Pow(10f, ((int) roundPreference)));
                for (int i = 0; i < 2; i++) 
                {
                    vector[i] = (int) ((Mathf.Abs(vector[i]) + 0.5f) * Mathf.Sign(vector[i]));
                }
                vector.Scale(Vector2.one / Mathf.Pow(10f, ((int) roundPreference)));
                break;
		}
		return vector;
	}
	
	static Vector3 RoundVector3(Vector3 vector, RoundFloatEnum roundPreference)
    {
		switch (roundPreference) 
        {
            case RoundFloatEnum.RoundToInteger:
            case RoundFloatEnum.RoundTo1DiginAfterPoint:
            case RoundFloatEnum.RoundTo2DiginAfterPoint:
            case RoundFloatEnum.RoundTo3DiginAfterPoint:
            case RoundFloatEnum.RoundTo4DiginAfterPoint:
                vector.Scale(Vector3.one * Mathf.Pow(10f, ((int) roundPreference)));
                for (int i = 0; i < 3; i++)
                {
                    vector[i] = (int) ((Mathf.Abs(vector[i]) + 0.5f) * Mathf.Sign(vector[i]));
                }
                vector.Scale(Vector3.one / Mathf.Pow(10f, ((int) roundPreference)));
                break;
		}
		return vector;
	}

	static Vector3 RecalculateByFactor(Vector3 baseValue, SizeFactor type) 
    {
        if (type == SizeFactor.MaxFactor)
        {
            baseValue.x *= MaxFactor;
            baseValue.y *= MaxFactor;
        }
        else if (type == SizeFactor.MinFactor)
        {
            baseValue.x *= MinFactor;
            baseValue.y *= MinFactor;
        }
        else if (type == SizeFactor.TwoFactor)
        {
            baseValue.x *= WidthFactor;
            baseValue.y *= HeightFactor;
        }
        else if (type == SizeFactor.HeightFactor)
        {
            baseValue.x *= HeightFactor;
            baseValue.y *= HeightFactor;
        }
        else if (type == SizeFactor.WidthFactor)
        {
            baseValue.x *= WidthFactor;
            baseValue.y *= WidthFactor;
        }
        else if (type == SizeFactor.MirrorTwoFactor)
        {
            baseValue.x *= HeightFactor;
            baseValue.y *= WidthFactor;
        }
        else if (type == SizeFactor.OnlyHeightFactor)
        {
            baseValue.y *= HeightFactor;
        }
        else if (type == SizeFactor.OnlyWidhtFactor)
        {
            baseValue.x *= WidthFactor;
        }

		return baseValue;
	}
    	
	static Vector3 RecalculateByFit(Vector3 baseValue, Fit type, bool half) 
    {
		if (type == Fit.Height) 
        {
            baseValue.y = half ? (ScreenDimentions.Height / 2f) : ScreenDimentions.Height;
		} 
        else if (type == Fit.Width) 
        {
            baseValue.x = half ? (ScreenDimentions.Width / 2f) : ScreenDimentions.Width;
		} 
        else if (type == Fit.Full) 
        {
            baseValue.x = half ? (ScreenDimentions.Width / 2f) : ScreenDimentions.Width;
            baseValue.y = half ? (ScreenDimentions.Height / 2f) : ScreenDimentions.Height;
		}

		return baseValue;
	}
	
	static public Vector3 RecalculatePosition(Vector3 position, SizeFactor factorType, RoundFloatEnum roundPreference = RoundFloatEnum.DontRoundFloat) 
    {
		return RoundVector3(RecalculateByFactor(position, factorType), roundPreference);
	}

	static float RecalculateValue(float value, SizeFactor factorType) 
    {
		if (factorType == SizeFactor.MaxFactor) 
        {
			value *= MaxFactor;
		} 
        else if (factorType == SizeFactor.MinFactor) 
        {
			value *= MinFactor;
		} 
        else if (factorType == SizeFactor.HeightFactor) 
        {
			value *= HeightFactor;
		} 
        else if (factorType == SizeFactor.WidthFactor)
        {
			value *= WidthFactor;
		}

		return value;
	}

	static public void RecalculatePosition(Transform transform, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
        if (transform == null) 
            return;

		transform.localPosition = RecalculateByFactor(transform.localPosition, factorType);
		transform.localPosition = RoundVector3(transform.localPosition, roundPreference);
	}

	static public void RecalculateScale(Transform transform, SizeFactor factorType, RoundFloatEnum roundPreference) {
		if (transform == null) 
            return;

		transform.localScale = RecalculateByFactor(transform.localScale, factorType);
		transform.localScale = RoundVector3(transform.localScale, roundPreference);
	}

	static public void RecalculateFOV(Camera camera, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (camera == null) 
            return;

		camera.fieldOfView = RecalculateValue(camera.fieldOfView, factorType);
		camera.fieldOfView = RoundFloat(camera.fieldOfView, roundPreference);
	}

	static public void RecalculateMask(tk2dUIMask mask, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (mask == null) 
            return;

		mask.size = RecalculateByFactor(mask.size, factorType);
		mask.size = RoundVector2(mask.size, roundPreference);
		mask.Build();
	}
	
	static public void RecalculateGrid(Grid grid, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (grid == null) 
            return;

		Vector2 distance = new Vector2(grid.cellWidth, grid.cellHeight);
		distance = RecalculateByFactor(distance, factorType);
		distance = RoundVector2(distance, roundPreference);
		grid.cellWidth = distance.x;
		grid.cellHeight = distance.y;
	}
	
	static public void RecalculateCollider(BoxCollider collider, SizeFactor factorType, Fit fitType, RoundFloatEnum roundPreference)
    {
		if (collider == null) 
            return;

		collider.size = RecalculateByFactor(collider.size, factorType);
		collider.size = RecalculateByFit(collider.size, fitType, false);
		collider.size = RoundVector3(collider.size, roundPreference);
		collider.center = RecalculateByFactor(collider.center, factorType);
		collider.center = RoundVector3(collider.center, roundPreference);
	}

	static public void RecalculateSizeParticleSystem(ParticleSystem particleSystem, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (particleSystem == null) 
            return;

		RecalculateScale(particleSystem.transform, factorType, roundPreference);

        #if UNITY_5_5_OR_NEWER
        var main = particleSystem.main;
        main.startSizeMultiplier = RecalculateValue(main.startSizeMultiplier, factorType);
        main.startSizeMultiplier = RoundFloat(main.startSizeMultiplier, roundPreference);
        #else
        particleSystem.startSize = RecalculateValue(particleSystem.startSize, factorType);
        particleSystem.startSize = RoundFloat(particleSystem.startSize, roundPreference);
        #endif
	}

	static public void RecalculateSizeColorQuad(ColorQuad quad, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (quad == null) 
            return;

		quad.Size = RecalculateByFactor(quad.Size, factorType);
		quad.Size = RoundVector2(quad.Size, roundPreference);
	}

	static public void RecalculateSizeGradientQuad(GradientQuad quad, SizeFactor factorType, RoundFloatEnum roundPreference) 
    {
		if (quad == null) 
            return;

		quad.Size = RecalculateByFactor(quad.Size, factorType);
		quad.Size = RoundVector2(quad.Size, roundPreference);
	}

	static public void RecalculateSizeSprite(tk2dBaseSprite sprite, SizeFactor factorType, Fit fitType, RoundFloatEnum roundPreference) 
    {
		if (sprite == null) 
            return;

		sprite.scale = RecalculateByFactor(sprite.scale, factorType);
		sprite.scale = RecalculateByFit(sprite.scale, fitType, true);
		sprite.scale = RoundVector3(sprite.scale, roundPreference);
	}
	
	static public void RecalculateSizeSlicedSprite(tk2dSlicedSprite sprite, SizeFactor factorType, RoundFloatEnum roundPreference)
    {
		if (sprite == null) 
            return;

		sprite.dimensions = RecalculateByFactor(sprite.dimensions, factorType);
		sprite.dimensions = RoundVector2(sprite.dimensions, roundPreference);
	}

	static public void RecalculateSizeSpriteFromTexture(tk2dSpriteFromTexture spriteFromTexture, SizeFactor factorType, Fit fitType, RoundFloatEnum roundPreference) 
    {
		if (spriteFromTexture == null)
            return;

		spriteFromTexture.ForceBuild();
		RecalculateSizeSprite(spriteFromTexture.GetComponent<tk2dBaseSprite>(), factorType, fitType, roundPreference);
	}
	
	static public void RecalculateSizeText(tk2dTextMesh label, SizeFactor type, RoundFloatEnum roundPreference) 
    {
		if (label == null) 
            return;

		label.scale = RecalculateByFactor(label.scale, type);
		label.scale = RoundVector3(label.scale, roundPreference);
	}
}
