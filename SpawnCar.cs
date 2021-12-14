using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCar : MonoBehaviour {
    public float spawnOffset;
    private const string carPath = @"LevelAssets/Car";

	// Use this for initialization
	void Awake ()
    {
        GameObject newVehicleAsset = Resources.Load(carPath) as GameObject;
        GameObject newVehicle = Instantiate(newVehicleAsset) as GameObject;

        newVehicleAsset.transform.position = transform.position + transform.forward * spawnOffset + transform.up;
	}
	
}
