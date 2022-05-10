using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;
    private Vector3 velocity, hookShotPosition, characterVelMomentum;
    private float hookshotSize;
    public GameObject playerObj;
    Vector3 moveDirection;

    public LineRenderer lr;
    private Vector3 grapplePoint; // point to grapple to
    public LayerMask whatIsGrappleable;
    public Transform gunBarrel, cam, player;
    private float maxDistance = 30f;
    private SpringJoint joint;
    public State state; // hold current state
    public enum State
    {
        Normal,
        // when the player is being pulled
        HookshotPullPlayer,
        HookshotRope // visualise the hookshot
    }
    private void Awake()
    {
        lr = GetComponent<LineRenderer>(); // get line renderer component
    }
    // Start is called before the first frame update
    void Start()
    {
        state = State.Normal; // start in normal state
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            // if the state is normal run
            case State.Normal:
                HandleHookshotStart();
                break;
            case State.HookshotRope:
                HandleHookshotRope();
                break;
            case State.HookshotPullPlayer:
                playerObj.GetComponent<Movement>().HandleHookshotMovement(hookshotTransform, hookShotPosition);
                break;

        }
        //HandleHookshotStart();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            StopGrapple();
        }
    }
    private void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            playerObj.GetComponent<Movement>().isSwinging = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>(); // add a spring joint component to the player
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float disFromPoint = Vector3.Distance(player.position, grapplePoint);
            
            joint.maxDistance = disFromPoint;
            joint.minDistance = disFromPoint;

            joint.spring = 100f; // handles pull and push
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }
    private void HandleHookshotStart()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerObj.GetComponent<Movement>().pullIn = true;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit, 100, whatIsGrappleable))
            {
                debugHitPointTransform.position = raycastHit.point;
                hookShotPosition = raycastHit.point; // get position to hook
                hookshotSize = 0f;
                hookshotTransform.gameObject.SetActive(true);
                hookshotTransform.localScale = Vector3.zero;
                state = State.HookshotRope; // send player toward position
                //playerObj.GetComponent<Movement>().HandleHookshotMovement(hookshotTransform, hookShotPosition);
            }

        }
      
        //playerObj.GetComponent<Movement>().HandleHookshotMovement(hookshotTransform, hookShotPosition);
    }
    private void HandleHookshotRope()
    {
        lr.positionCount = 2;
        //hookshotTransform.LookAt(hookShotPosition); // look at the position of the hookshot
        //// scale the rope size and define speed for rope to travel at
        //float hookshotRopeSpeed = 200f;
        //hookshotSize += hookshotRopeSpeed * Time.deltaTime;
        //hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);
        state = State.HookshotPullPlayer; // send player toward position
        // if hookshotsize is larger than the distence toward the hookshot position then rope has reached the hookshot position
        //if (hookshotSize >= (Vector3.Distance(hookshotTransform.position, hookShotPosition)))
        //{
            
        //}
    }
    void DrawRope()
    {
        if (!joint)
        {
            return;
        }
        lr.SetPosition(0, gunBarrel.position);
        lr.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        playerObj.GetComponent<Movement>().isSwinging = false;
        Debug.Log("gone");
        lr.positionCount = 0;
        Destroy(joint);
    }
}
