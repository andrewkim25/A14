using UnityEngine;

public class Boost : MonoBehaviour {
    Drive driveEngine;

    private void Start()
    {
        driveEngine = GameObject.FindObjectOfType<Drive>();
    }

    public void Boosting()
    {
        driveEngine.Boost();
    }

}
