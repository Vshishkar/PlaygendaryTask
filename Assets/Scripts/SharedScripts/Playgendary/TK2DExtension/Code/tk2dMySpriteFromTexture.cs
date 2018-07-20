using UnityEngine;
using System.Collections;

public class tk2dMySpriteFromTexture : MonoBehaviour {

	public tk2dSpriteCollectionSize spriteCollectionSize = new tk2dSpriteCollectionSize();
	public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;
	public bool rebuild = true;

	[SerializeField]
	tk2dSpriteCollectionData spriteCollection;
	[SerializeField]
	Texture texture;
	tk2dBaseSprite sprite;

	void Awake() {
		CheckSprite();
	}

	void Build() {
		if (texture != null) {
			DestroyInternal();
			GameObject go = new GameObject("tk2dSpriteFromTexture - " + texture.name);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			Vector2 anchorPos = tk2dSpriteGeomGen.GetAnchorOffset(anchor, texture.width, texture.height);
			spriteCollection = tk2dRuntime.SpriteCollectionGenerator.CreateFromTexture(
				go, 
				texture, 
				spriteCollectionSize,
				new Vector2(texture.width, texture.height),
				new string[] { "unnamed" } ,
				new Rect[] { new Rect(0f, 0f, texture.width, texture.height) },
				null,
				new Vector2[] { anchorPos },
				new bool[] { false }
			);
			string objName = "SpriteFromTexture " + texture.name;
			spriteCollection.spriteCollectionName = objName;
			spriteCollection.spriteDefinitions[0].material.name = objName;
			spriteCollection.spriteDefinitions[0].material.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
			Sprite.SetSprite(spriteCollection, 0);
		}
	}

	void DestroyInternal() {
		if (IsSpriteCollectionExist) {
			if (GetComponent<Renderer>() != null) GetComponent<Renderer>().material = null;
			DestroyImmediate(spriteCollection.spriteDefinitions[0].material);
			DestroyImmediate(spriteCollection.gameObject);
			spriteCollection = null;
		}
	}

	void CheckSprite() {
		if (!IsSpriteCollectionExist) Build();
		if (Sprite.Collection == null) Sprite.SetSprite(spriteCollection, 0);
	}

	public Material CollectionMaterial {
		get {
			if (spriteCollection != null) return spriteCollection.spriteDefinitions[0].material;
			return null;
		}
	}

	public Texture Texture {
		get { return texture; }
		set {
			if (texture != value) {
				texture = value;
				Build();
			}
		}
	}

	public tk2dBaseSprite Sprite {
		get {
			if (sprite == null) sprite = GetComponent<tk2dBaseSprite>();
			if (sprite == null) {
				CustomDebug.Log("tk2dSpriteFromTexture - Missing sprite object. Creating.");
				sprite = gameObject.AddComponent<tk2dSprite>();
			}
			return sprite;
		}
	}

	public bool IsSpriteCollectionExist {
		get { return spriteCollection != null; }
	}

	public tk2dSpriteCollectionData SpriteCollection {
		get { return spriteCollection; }
	}

	public void ForceBuild() {
		Build();
	}

	public void Clear() {
		DestroyInternal();
	}
}
