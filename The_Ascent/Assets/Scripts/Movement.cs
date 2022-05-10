using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform orientation;
    public float moveSpeed;
    public float groundDrag;
    public LayerMask whatIsGround;
    public float playerHeight;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump = true;
    public bool pullIn, isSwinging;
    bool isGrounded;
    float horizontalInput;
    float verticalInput;
    public Vector3 characterVelMomentum;
    Vector3 moveDirection;
    public GameObject grapplingGun;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SpeedLimit();
        // apply drag to player
        if (isGrounded)
        {
            rb.drag = groundDrag;
            MovePlayer();
        }
        else
        {
            rb.drag = 0;
        }
        rb.velocity += characterVelMomentum; // increase velocity by the momentum
        // provide a constant reduction to momentum
        if (characterVelMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f; // define drag value to slow momentum down
            rb.velocity -= characterVelMomentum;
            characterVelMomentum -= characterVelMomentum * momentumDrag * Time.deltaTime; // reduce momentum by the drag 
            if (characterVelMomentum.magnitude < .0f)
            {
                characterVelMomentum = Vector3.zero; // once the momentum is small enough set to 0
            }
        }
    }

    private void FixedUpdate()
    {

    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space) && canJump && isGrounded)
        {
            canJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        // move in direction faced
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    public void HandleHookshotMovement(Transform hookshotTransform, Vector3 hookShotPosition)
    {
        grapplingGun.GetComponent<GrapplingGun>().lr.SetPosition(0, grapplingGun.GetComponent<GrapplingGun>().gunBarrel.position);
        grapplingGun.GetComponent<GrapplingGun>().lr.SetPosition(1, hookShotPosition);
        hookshotTransform.LookAt(hookShotPosition);
        Vector3 hookshotDir = (hookShotPosition - rb.position).normalized; // get direction of hookshot
        float minSpeed = 10f;
        float maxSpeed = 40f;
        // speed is the distence between the player position and hookshot position, will begin fast and slow down as it gets closer
        // also clamp speed between the min and max speed to ensure a realistic speed
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookShotPosition), minSpeed, maxSpeed);
        float hookshotSpeedMultiplier = 2f; // this is used incase the player is too close to the hookposition resulting in a slow speed
        if (pullIn)
        {
            rb.drag = groundDrag;
            moveDirection = hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime;
            rb.MovePosition(transform.position + moveDirection.normalized / 3.5f); // move toward that position

        }
        // allow for distence check to see if player has reached the position
        float reachedPosition = 1f;
        // if player has reached the position change state back to normal
        if (Vector3.Distance(transform.position, hookShotPosition) < reachedPosition)
        {
            grapplingGun.GetComponent<GrapplingGun>().lr.positionCount = 0;
            rb.drag = 0;
            pullIn = false;
            StopHookshot();
        }
        // if hookshot is attemped while moving cancel current hookshot
        if (CancelHookshot())
        {
            float momentumAddtionalSpeed = 2.5f;
            // keep the momentum of the currect hook shot and add some extra to accoutn for the jump momentum
            characterVelMomentum = hookshotDir * hookshotSpeed * momentumAddtionalSpeed;
            rb.AddForce(characterVelMomentum / 5, ForceMode.Impulse);
            StopHookshot();
        }
        if (TestInputJump() && pullIn)
        {
            float momentumAddtionalSpeed = 2.5f;
            // keep the momentum of the currect hook shot and add some extra to accoutn for the jump momentum
            characterVelMomentum = hookshotDir * hookshotSpeed * momentumAddtionalSpeed;
            rb.AddForce(characterVelMomentum / 5, ForceMode.Impulse);
            rb.AddForce(transform.up * 6, ForceMode.Impulse);
            StopHookshot();
        }
        if (SwitchToSwing())
        {
            float momentumAddtionalSpeed = 2.5f;
            characterVelMomentum = hookshotDir * hookshotSpeed * momentumAddtionalSpeed;
            rb.AddForce(characterVelMomentum / 5, ForceMode.Impulse);
            grapplingGun.GetComponent<GrapplingGun>().lr.positionCount = 0;
            grapplingGun.GetComponent<GrapplingGun>().state = GrapplingGun.State.Normal;
            rb.drag = 0;
            pullIn = false;
        }
    }
    private bool CancelHookshot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
    private void StopHookshot()
    {
        grapplingGun.GetComponent<GrapplingGun>().lr.positionCount = 0;
        grapplingGun.GetComponent<GrapplingGun>().state = GrapplingGun.State.Normal;
        rb.drag = 0;
        pullIn = false;
    }

    public bool SwitchToSwing()
    {
        return (Input.GetKey(KeyCode.Q));
    }

    private bool TestInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }
    void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // if flatVel is greater than movespeed limit
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed; // calc what max vel should be
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // apply this as vel
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // ensure that velocity is 0 so height is always the same

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        canJump = true;
    }
}
