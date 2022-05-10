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
    bool isGrounded;
    float horizontalInput;
    float verticalInput;
    public Vector3 characterVelMomentum;
    Vector3 moveDirection;
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