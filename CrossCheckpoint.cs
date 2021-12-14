using UnityEngine;

public class CrossCheckpoint : MonoBehaviour {
    LevelProgressManager lpm;
    private void Start()
    {
        lpm = FindObjectOfType<LevelProgressManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string[] tagParts = null;
        try
        {
            tagParts = tag.Split(' ');
        }
        catch
        {
            Debug.LogError("You might have forgotten to tag the element : "+ gameObject.name);
        }
        lpm.CheckpointCrossed(int.Parse(tagParts[1]) - 1);
    }
}
