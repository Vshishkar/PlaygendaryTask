using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class tmSkinnedBatchedInstance : tmBatchedInstance 
{
	#region Temp Variables

	Transform[] tempBones;
	Matrix4x4[] tempBindPoses;
	BoneWeight[] tempBoneWeights;

    Dictionary<int, int> bonesPerMesh = new Dictionary<int, int>();
	#endregion


	protected override void Awake()
	{
		gameObject.AddComponent<SkinnedMeshRenderer>();
	}


	void Update()
	{
		Bounds newBounds = new Bounds();

		if (BatchedParts.Count > 0)
		{
			for (int index = 0, partsCount = BatchedParts.Count; index < partsCount; index++)
			{
				tmBatchObject part = BatchedParts[index];

				if(!part.IsNull())
				{
					var curBounds = part.Mesh.bounds;
					curBounds.center += part.CachedTransform.position;
					newBounds.Encapsulate(curBounds);
				}
			}

			newBounds.center -= CachedTransform.position;
		}
		
		CachedSkinnedRender.localBounds = newBounds;
	}


	protected override void PostRecombine(CombineInstance[] combos)
	{
		int bones = 0;
		for (int i = 0, combosLength = combos.Length; i < combosLength; i++) 
		{
			var mci = combos[i];
            int meshHash = mci.mesh.GetInstanceID();
            int meshBones;
            if (!bonesPerMesh.TryGetValue(meshHash, out meshBones))
            {
                var bindposes = mci.mesh.bindposes;// here leak ~1kb per frame
                bool bonesExist = (bindposes != null) && bindposes.Length > 0;
                meshBones += bonesExist ? bindposes.Length : 1;
                bonesPerMesh.Add(meshHash, meshBones);
            }
            bones += meshBones;
		}

		tempBones = ArrayExtention.EnsureLength(tempBones, bones);
		tempBindPoses = ArrayExtention.EnsureLength(tempBindPoses, bones);
		tempBoneWeights = ArrayExtention.EnsureLength(tempBoneWeights, Mesh.vertexCount);

		int boneIndex = 0;
		int vertexIndexBase = 0;
		for (int partIndex = 0, partsCount = BatchedParts.Count; partIndex < partsCount; partIndex++)
		{
			tmBatchObject part = BatchedParts[partIndex];
			SkinnedMeshRenderer partSMR = part.SkinnedMeshRender;

			if(partSMR)
			{
				Transform[] partBones = partSMR.bones;
				BoneWeight[] partWeights = part.Mesh.boneWeights;
				Matrix4x4[] partBindPoses = part.Mesh.bindposes;
				Matrix4x4 partToBatchMatrix = part.CachedTransform.worldToLocalMatrix * CachedTransform.localToWorldMatrix;

				int meshBonesCount = partBones.Length;
				for (int i = 0; i < meshBonesCount; i++)
				{
					tempBones[boneIndex + i] = partBones[i];
					tempBindPoses[boneIndex + i] = partBindPoses[i] * partToBatchMatrix;
				}

				int partWeightsLength = partWeights.Length;
				for (int i = 0; i < partWeightsLength; i++) 
				{
					BoneWeight bw = partWeights[i];
					bw.boneIndex0 += boneIndex;
					bw.boneIndex1 += boneIndex;
					bw.boneIndex2 += boneIndex;
					bw.boneIndex3 += boneIndex;
					tempBoneWeights[vertexIndexBase] = bw;
					vertexIndexBase++;
				}

				boneIndex += meshBonesCount;
			}
			else
			{
				Transform bone = part.CachedTransform;
				tempBones[boneIndex] = bone;
				tempBindPoses[boneIndex] = bone.worldToLocalMatrix * CachedTransform.localToWorldMatrix;

				int curVertCount = part.Mesh.vertexCount;
				for (int vertexIndex = 0; vertexIndex < curVertCount; ++vertexIndex)
				{
					tempBoneWeights[vertexIndexBase].boneIndex0 = boneIndex;
					tempBoneWeights[vertexIndexBase].weight0 = 1;
					vertexIndexBase++;
				}

				boneIndex++;
			}
		}

		Mesh.boneWeights = tempBoneWeights;
		Mesh.bindposes = tempBindPoses;

		CachedSkinnedRender.quality = SkinQuality.Auto;
		CachedSkinnedRender.bones = tempBones;
		CachedSkinnedRender.sharedMesh = Mesh;
		CachedSkinnedRender.localBounds = Mesh.bounds;
		CachedSkinnedRender.enabled = (Mesh.vertexCount > 0);
	}


	protected override void ClearMesh()
	{
		Mesh.boneWeights = null;
		Mesh.bindposes = null;

		CachedSkinnedRender.bones = null;
		CachedSkinnedRender.sharedMesh = null;
		CachedSkinnedRender.enabled = false;
	}
}
