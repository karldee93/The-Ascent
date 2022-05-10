using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public LineRenderer lr;
    private Vector3 grapplePoint; // point to grapple to
    public LayerMask whatIsGrappleable;
    public Transform gunBarrel, cam, player;
    private float maxDistance = 30f;
    private SpringJoint joint;
    private void Awake()
    {
        lr = GetComponent<LineRenderer>(); // get line renderer component
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>(); // add a spring joint component to the player
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float disFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = disFromPoint;
            joint.minDistance = disFromPoint;

            joint.spring = 1f; // handles pull and push
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
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
        lr.positionCount = 0;
        Destroy(joint);
    }
}
