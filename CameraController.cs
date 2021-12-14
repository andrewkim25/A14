using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public GameObject[] mustHaveInView;
    public float zoomSpeed;
    public float minCameraDistance;
    public float maxCameraDistance;

    protected GameObject car;

    protected Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        car = GameObject.FindGameObjectWithTag(TagManager.PLAYER_VEHICLE);
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePosition();
	}

    private void UpdatePosition()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = car.transform.position.x;
        newPosition.z = car.transform.position.z;


        Rect mustSeeArea;

        float minX, minZ;
        float maxX, maxZ;

        minX = float.MaxValue;
        minZ = float.MaxValue;
        maxX = float.MinValue;
        maxZ = float.MinValue;

        foreach (GameObject mustSee in mustHaveInView)
        {
            minX = Mathf.Min(mustSee.transform.position.x, minX);
            minZ = Mathf.Min(mustSee.transform.position.z, minZ);

            maxX = Mathf.Max(mustSee.transform.position.x, maxX);
            maxZ = Mathf.Max(mustSee.transform.position.z, maxZ);
        }

        minX = Mathf.Min(car.transform.position.x, minX);
        minZ = Mathf.Min(car.transform.position.z, minZ);

        maxX = Mathf.Max(car.transform.position.x, maxX);
        maxZ = Mathf.Max(car.transform.position.z, maxZ);
        mustSeeArea = new Rect(minX,minZ,maxX-minX,maxZ-minZ);

        float aspectRatio = 1f * Screen.width / Screen.height;
        float mustSeeRatio = mustSeeArea.width / mustSeeArea.height;
        float fovX, fovZ;

        fovZ = cam.fieldOfView;
        fovX = Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2) * cam.aspect);

        float fitXHeight = FitX(mustSeeArea,fovX);
        float fitZHeight = FitZ(mustSeeArea,fovZ);
        float height = Mathf.Clamp(Mathf.Max(fitXHeight, fitZHeight), minCameraDistance, maxCameraDistance);

        newPosition.y = Mathf.Lerp(transform.position.y, height, zoomSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    private float FitX(Rect mustSeeArea,float fovX)
    {
        float left = transform.position.x - mustSeeArea.xMin;
        float right = mustSeeArea.xMax - transform.position.x;
        return Mathf.Max(left, right) / Mathf.Tan(fovX / 2 / 180f * Mathf.PI);
    }
    private float FitZ(Rect mustSeeArea, float fovZ)
    {
        float up = mustSeeArea.yMax - transform.position.z;
        float down = transform.position.z - mustSeeArea.yMin;
        return Mathf.Max(up, down) / Mathf.Tan(fovZ / 2 / 180f * Mathf.PI);
    }
}
