using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {
    protected LevelProgressManager.SnapShot[] points;
    protected int currentMaxIndex;
    protected float startTime;

    public float BestTime
    {
        get { return points[points.Length - 1].time; }
    }

    // Use this for initialization
    void Start () {
        points = GhostSerializer.DeSerializeObject(Application.persistentDataPath + "/Level" + (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1)+".xml");
        currentMaxIndex = 0;
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (CurrentTime > points[currentMaxIndex].time)
        {
            for (int index= currentMaxIndex+1; index<points.Length;++index)
            {
                if (CurrentTime <= points[index].time)
                {
                    currentMaxIndex = index;
                    break;
                }
            }
        }

        if ((CurrentTime == points[currentMaxIndex].time)||(currentMaxIndex==0))
        {
            SetValues(points[currentMaxIndex]);
        }else 
        {
            transform.position = points[currentMaxIndex - 1].position + (points[currentMaxIndex].position - points[currentMaxIndex - 1].position) * ((CurrentTime - points[currentMaxIndex - 1].time) /(points[currentMaxIndex].time - points[currentMaxIndex - 1].time));
            transform.eulerAngles = points[currentMaxIndex - 1].euler;
        }

	}
    protected float CurrentTime
    {
            get { return Time.time - startTime; }
    }
    protected void SetValues(LevelProgressManager.SnapShot snapshot)
    {
        transform.position = snapshot.position;
        transform.eulerAngles = snapshot.euler;
    }
}
