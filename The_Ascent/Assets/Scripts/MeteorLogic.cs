using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorLogic : MonoBehaviour
{
    Rigidbody rb;
    GameObject spawner;
    GameObject player;
    public float downwardForce = 1f;
    float offsetX;
    float offsetZ;
    Vector3 distToPlayer;
    float explosionForce = 4000f, radius = 9f, height = 4f;
    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        player = GameObject.FindGameObjectWithTag("Player");
        spawner.GetComponent<Meteor>().spawn = true;
        rb = GetComponent<Rigidbody>();
        offsetX = Random.Range(0, 26);
        offsetZ = Random.Range(0, 26);
        Vector3 position = gameObject.transform.position;
        position = new Vector3(gameObject.transform.position.x + offsetX, gameObject.transform.position.y, gameObject.transform.position.z + offsetZ);
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = transform.position - player.transform.position;
        rb.AddForce(Vector3.down, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<SphereCollider>().isTrigger = true;
        if(distToPlayer.magnitude <= radius)
        {
            Debug.Log(distToPlayer.magnitude);
            if(distToPlayer.magnitude < 4f)
            {
                explosionForce = 3000f;
                height = 2.5f;
            }
            else
            {
                explosionForce = 4000f;
                height = 4f;
            }
            player.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, radius, height, ForceMode.Force);
            player.GetComponent<Movement>().meteorHasHit = true;
            //player.GetComponent<Rigidbody>().AddForce(transform.up * 1000);
        }
        Destroy(gameObject, 1f);
    }
}
