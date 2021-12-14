using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(FindObjectOfType<PauseMenu>().transform.parent.gameObject);
        DontDestroyOnLoad(FindObjectOfType<PupulateLevels>().transform.parent.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
