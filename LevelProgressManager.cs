using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MarkCheckpoints))]
public class LevelProgressManager : MonoBehaviour {
    public int checkPoints = 3;
    public int mustHaveInViewCheckpoints = 2;
    public float[] times;

    public GameObject[] stars;
    protected MarkCheckpoints checkpointMarker;
    protected bool[] checkpointsCrossed;
    protected GameObject[] checkpointObjects;
    protected CameraController cameraControl;
    protected Timer timer;
    protected Drive player;
    protected List<SnapShot> snaps;
    protected float startTime;
    protected CongratsMenu congrats;

    [System.Serializable]
    public class SnapShot
    {
        public SnapShot()
        {
            time = 0;
            position = Vector3.zero;
            euler = Vector3.zero;
        }
        public SnapShot(float time,Vector3 position,Vector3 euler)
        {
            this.time = time;
            this.position = position;
            this.euler = euler;
        }
        [SerializeField] public float time;
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 euler;
    }

    private void Awake()
    {
        congrats = FindObjectOfType<CongratsMenu>();
        congrats.gameObject.SetActive(false);
        cameraControl = FindObjectOfType<CameraController>();
        timer = FindObjectOfType<Timer>();
        checkpointMarker = GetComponent<MarkCheckpoints>();
    }

    // Use this for initialization
    void Start () {
        startTime = Time.time;
        PlayerPrefs.SetString(PlayerPrefKeys.LAST_LEVEL, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        player = FindObjectOfType<Drive>();
        snaps = new List<SnapShot>();
        FadeToBlack ftb = FindObjectOfType<FadeToBlack>();
        if (ftb)
        {
            ftb.UnFade();
        } 

        if (checkPoints > TagManager.CHECKPOINTS.Length)
        {
            Debug.LogError("You are trying to use more checkpoints than created, please create more checkpoint objects and tags");
        }

        checkpointsCrossed = new bool[checkPoints];
        checkpointObjects = new GameObject[checkPoints];
        for (int index = 0; index < checkPoints;++index)
        {
            checkpointsCrossed[index] = false;
            checkpointObjects[index] = GameObject.FindGameObjectWithTag(TagManager.CHECKPOINTS[index]);
        }

        stars = new GameObject[TagManager.STARS.Length];
        for (int index = 0; index < stars.Length; ++index)
        {
            stars[index] = GameObject.FindGameObjectWithTag(TagManager.STARS[index]);
        }

        GameObject[] mushHaveInViewObjects = new GameObject[mustHaveInViewCheckpoints];
        for (int index = 0; index< mustHaveInViewCheckpoints; ++index)
        {
            mushHaveInViewObjects[index] = checkpointObjects[index];
        }

        cameraControl.mustHaveInView = mushHaveInViewObjects;
    }
	
    public void CheckpointCrossed(int checkpointIndex)
    {
        Debug.Log("Crossed "+checkpointIndex);

        bool allPreviousPassed = true;
        for (int index = 0; index<checkpointIndex;++index)
        {
            if (!checkpointsCrossed[index])
            {
                allPreviousPassed = false;
                break;
            }
        }

        if ((allPreviousPassed)&&(!checkpointsCrossed[checkpointIndex]))
        {
            checkpointsCrossed[checkpointIndex] = true;
            if (checkpointIndex == checkpointsCrossed.Length - 1)
            {
                Win(timer.GetCurrent());
            }
            checkpointMarker.Cleared(checkpointIndex);
        }
        else
        {
            return;
        }

        GameObject[] newList = null;
        if (checkpointIndex + mustHaveInViewCheckpoints < checkpointObjects.Length)
        {
            newList = new GameObject[mustHaveInViewCheckpoints];
            for (int index = 1; index < mustHaveInViewCheckpoints;++index)
            {
                newList[index - 1] = checkpointObjects[checkpointIndex + index];
            }
            newList[checkpointIndex + mustHaveInViewCheckpoints - 1] = checkpointObjects[checkpointIndex + mustHaveInViewCheckpoints];
        }
        else
        {
            newList = new GameObject[cameraControl.mustHaveInView.Length - 1];
            for (int index = 0; index<newList.Length;++index)
            {
                newList[index] = cameraControl.mustHaveInView[index + 1];
            }
        }
        cameraControl.mustHaveInView = newList;
    }
    public void Paused(bool newState)
    {
        if (newState)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    protected void Win(float winTime)
    {
        timer.Stopped = true;
        int starCount = stars.Length;
        for (int index = 0; index < stars.Length - 1; ++index)
        {
            if (winTime < times[index])
            {
                break;
            }
            else
            {
                --starCount;
            }
        }
        int currentLevel = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);

        if (starCount> int.Parse(PlayerPrefs.GetString(PlayerPrefKeys.STAR_RATING + currentLevel)))
        {
            PlayerPrefs.SetString(PlayerPrefKeys.STAR_RATING + currentLevel, starCount.ToString()); ///Not the best storage but it will have to do for today
        }
        if (!PlayerPrefs.HasKey(PlayerPrefKeys.STAR_RATING + (currentLevel+1)))
        {
            PlayerPrefs.SetString(PlayerPrefKeys.STAR_RATING + (currentLevel+1), "0"); //Unlock next
        }
        FindObjectOfType<PupulateLevels>().WonLevel(currentLevel, starCount);

        Ghost ghost = FindObjectOfType<Ghost>();
        if (ghost != null) 
        {
            if (ghost.BestTime > Time.time - startTime)
            {
                GhostSerializer.SerializeObject(snaps.ToArray(), Application.persistentDataPath + "/Level" + currentLevel + ".xml");
            }
        }
        else
        {
            GhostSerializer.SerializeObject(snaps.ToArray(), Application.persistentDataPath + "/Level" + currentLevel + ".xml");
        }
        congrats.gameObject.SetActive(true);
    }

	void Update () {
        if (!timer.Stopped)
        {
            float currentTime = timer.GetCurrent();
            for (int index = 0; index < stars.Length - 1; ++index)
            {
                if (currentTime > times[index])
                {
                    stars[index].SetActive(false);
                }
                else
                {
                    break;
                }
            }
        }
	}

    void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            snaps.Add(new SnapShot(Time.time - startTime, player.transform.position, player.transform.eulerAngles));
        }
    }
}
