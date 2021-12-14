using UnityEngine;

public class MarkCheckpoints : MonoBehaviour {
    public Material checkpointNextMaterial;
    public Material checkpointClearedMaterial;
    public Material checkpointNotMaterial;

    protected GameObject[] checkpointObjects;

	// Use this for initialization
	void Start () {
        checkpointObjects = new GameObject[TagManager.CHECKPOINTS.Length];

        for (int index = 0; index<TagManager.CHECKPOINTS.Length;++index)
        {
            checkpointObjects[index] = GameObject.FindGameObjectWithTag(TagManager.CHECKPOINTS[index]);
            if (index == 0)
            {
                SetMaterialToChildren(checkpointObjects[index], checkpointNextMaterial);
            }
            else
            {
                SetMaterialToChildren(checkpointObjects[index], checkpointNotMaterial);
            }
        }
	}
	
	public void Cleared(int index)
    {
        SetMaterialToChildren(checkpointObjects[index], checkpointClearedMaterial);
        if (index+1<checkpointObjects.Length)
        {
            SetMaterialToChildren(checkpointObjects[index + 1], checkpointNextMaterial);
        }
    }

    protected void SetMaterialToChildren(GameObject root,Material mat)
    {
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = mat;
        }
    }
}
