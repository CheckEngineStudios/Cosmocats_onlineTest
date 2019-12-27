using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public List<Transform> spawnPoints;
    void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>())
        {
            if(trans!=this.transform)
                spawnPoints.Add(trans);
        }
    }

}
