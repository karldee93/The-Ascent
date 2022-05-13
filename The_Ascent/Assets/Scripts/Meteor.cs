using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public Transform[] spawnLocation;
    public GameObject meteororite;
    int spawnLocationNum;
    float timer = 1f;
    public bool spawn = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            timer -= 1 * Time.deltaTime;
            if (timer <= 0)
            {
                spawnLocationNum = Random.Range(0, spawnLocation.Length);
                SpawnMeteor();
            }
        }
    }

    void SpawnMeteor()
    { 
        spawn = false;
        spawnLocationNum = Random.Range(0, spawnLocation.Length);
        Instantiate(meteororite, spawnLocation[spawnLocationNum].transform.position, spawnLocation[spawnLocationNum].transform.rotation);
        
        timer = 1f;
    }
}
