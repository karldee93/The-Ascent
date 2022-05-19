using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorLogic : MonoBehaviour
{
    Rigidbody rb;
    GameObject spawner;
    GameObject player;
    public GameObject smoke, smokeSpawn, explosion;
    public float downwardForce = 1f;
    float offsetX;
    float offsetZ;
    Vector3 distToPlayer;
    float explosionForce = 1500f, radius = 13f, height = 4f;
    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        player = GameObject.FindGameObjectWithTag("Player");
        spawner.GetComponent<Meteor>().spawn = true;
        rb = GetComponent<Rigidbody>();
        offsetX = Random.Range(0, 1);
        offsetZ = Random.Range(0, 1);
        Vector3 position = gameObject.transform.position;
        position = new Vector3(gameObject.transform.position.x + offsetX, gameObject.transform.position.y, gameObject.transform.position.z + offsetZ);
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = transform.position - player.transform.position;
        rb.AddForce(Vector3.down / 2, ForceMode.Force);
        if (gameObject.transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WindArea")
        {
            // do nothing
        }
        else if (other.gameObject.tag == "WallRun")
        {
            // do nothing
        }
        else if (other.gameObject.tag == "WindArea2")
        {
            // do nothing
        }
        else
        {
            GameObject tempFlash;
            tempFlash = Instantiate(smoke, smokeSpawn.transform.position, smoke.transform.rotation);
            GameObject tempFlash2;
            tempFlash2 = Instantiate(explosion, smokeSpawn.transform.position, explosion.transform.rotation);
            //gameObject.GetComponent<SphereCollider>().isTrigger = true;
            if (distToPlayer.magnitude <= radius)
            {
                if (distToPlayer.magnitude < 4f)
                {
                    explosionForce = 100f;
                    height = 0.5f;
                }
                else
                {
                    explosionForce = 1500f;
                    height = 4f;
                }
                player.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, radius, height, ForceMode.Force);
                //player.GetComponent<Rigidbody>().AddForce(transform.up * 1000);
            }
            Destroy(tempFlash2, 1f);
            Destroy(tempFlash, 1f);
            Destroy(gameObject, 1f);
        }

    }
}
