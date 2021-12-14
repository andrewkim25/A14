using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGhost : MonoBehaviour
{
    private const string ghostPath = @"LevelAssets/Ghost";
    // Use this for initialization
    void Awake()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/Level"+ (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1)+".xml"))
        {
            GameObject newVehicleAsset = Resources.Load(ghostPath) as GameObject;
            GameObject newVehicle = Instantiate(newVehicleAsset) as GameObject;
            newVehicle.transform.position = transform.position + transform.forward * FindObjectOfType<SpawnCar>().spawnOffset + transform.up;
        }
    }
}
