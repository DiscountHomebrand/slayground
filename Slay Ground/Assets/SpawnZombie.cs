using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombie : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject[] spawnLocation;
    public float spawnInterval;
    public float spawnReduced;  // amount spawn time gets reduced every spawn
    public float spawnTimer;
    public float minspawn;

   
    void Start()
    {
        foreach(GameObject spawn in spawnLocation){
            Instantiate(zombiePrefab, spawn.transform.position, Quaternion.identity);
        }

        spawnTimer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {

        if (spawnTimer > 0.0f) {
				spawnTimer -= Time.deltaTime;
		}else{
            foreach(GameObject spawn in spawnLocation){
            Instantiate(zombiePrefab, spawn.transform.position, Quaternion.identity);
            }
            spawnInterval -= spawnReduced;
            spawnTimer = spawnInterval;
            if (spawnTimer < minspawn){
                spawnTimer = minspawn;
            }
        }
       
    }
}
