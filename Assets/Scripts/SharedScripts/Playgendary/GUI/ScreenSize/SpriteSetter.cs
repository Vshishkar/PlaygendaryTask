using UnityEngine;

[AddComponentMenu("Inventain/Size/SpriteSetter")]
public class SpriteSetter : SizeSetter {
	
	public SizeFactor spriteFactorType = SizeFactor.MinFactor;
	public SizeFactor positionFactorType = SizeFactor.MinFactor;
	
	protected override void UpdateSize() {
		SizeHelper.RecalculateSizeSprite(GetComponent<tk2dBaseSprite>(), spriteFactorType, Fit.DontFit, roundFloatPreference);
		SizeHelper.RecalculatePosition(transform, positionFactorType, roundFloatPreference);
	}
}
