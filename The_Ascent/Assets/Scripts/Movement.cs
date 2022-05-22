using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Movement : MonoBehaviour
{
    [SerializeField] float wallDistance = 0.5f;
    [SerializeField] float minimumJumpHeight = 1.5f;
    [SerializeField] float wallRunGravity;
    [SerializeField] float wallJumpForce;
    public CameraMovement camMove;
    private FOV camFov;
    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;
    public GameObject uiManager;
    public Transform orientation;
    public float moveSpeed;
    public float groundDrag;
    public LayerMask whatIsGround, whatIsWall;
    public float playerHeight;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump = true;
    public bool pullIn, isSwinging;
    public bool isGrounded, shakeCamera = true, canMove, gameWon;
    float horizontalInput;
    float verticalInput;
    public Vector3 characterVelMomentum;
    Vector3 moveDirection;
    public GameObject grapplingGun, windArea, playerObj;
    private GameObject door, environment, meteorSpawner;
    public Rigidbody rb;
    public bool inWindArea, applyWind, windType1, windType2, wallLeft, wallRight, wallLeftOrientation, wallRightOrientation;
    float windTimer = 3, applyWindTimer = 3, platformFallTimer = 1f, timeTaken;
    public int minCounter;
    RaycastHit leftWallHit, rightWallHit, checkPlatform;
    public ParticleSystem speedLines;
    public AudioSource jump, backgroundMusic, pickUp;
    // Start is called before the first frame update
    void Start()
    {
        door = GameObject.FindGameObjectWithTag("EndPoint");
        environment = GameObject.FindGameObjectWithTag("Environment");
        meteorSpawner = GameObject.FindGameObjectWithTag("Spawner");
        camFov = camMove.GetComponent<FOV>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameWon)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            //canMove = Physics.Raycast(transform.position, Vector3.down, 5f, whatIsGround);
            PlayerInput();
            SpeedLimit();
            CheckWall();
            CheckPlatforms();
            WindControl();
            TrackTime();
            // apply drag to player
            if (isGrounded)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0;
            }
            if (!isSwinging && canMove)
            {
                MovePlayer();
            }
            if (isSwinging && Input.GetKey(KeyCode.Space))
            {
                Jump();
                grapplingGun.GetComponent<GrapplingGun>().StopGrapple();
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
            if (gameObject.transform.position.y < -5)
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    void TrackTime()
    {
        uiManager.GetComponent<UIManager>().timer = timeTaken;
        timeTaken += 1f * Time.deltaTime;
        if (timeTaken >= 60f)
        {
            minCounter += 1;
            uiManager.GetComponent<UIManager>().minCount = minCounter;
            timeTaken = 0f;
        }
    }

    void WindControl()
    {
        if (inWindArea)
        {
            windTimer -= 1f * Time.deltaTime;
            if (windTimer <= 0)
            {
                applyWind = true;
                windTimer = Random.Range(0, 7);
            }
            if (applyWind)
            {
                applyWindTimer -= 1f * Time.deltaTime;
                if (windType1)
                {
                    if (applyWindTimer <= 0)
                    {
                        int windStrength = Random.Range(0, 16);
                        float randDirection = Random.Range(0, 2);
                        windArea.GetComponent<WindArea>().strength = windStrength;
                        if (randDirection == 0)
                        {
                            windArea.GetComponent<WindArea>().direction = new Vector3(5f, 0f, 0f);
                        }
                        else if (randDirection == 1)
                        {
                            windArea.GetComponent<WindArea>().direction = new Vector3(0f, 0f, 5f);
                        }
                        applyWind = false;
                        applyWindTimer = 3f;
                    }
                }
                else if (windType2)
                {
                    if (applyWindTimer <= 0)
                    {
                        int windStrength = 60;
                        float randDirection = Random.Range(0, 2);
                        windArea.GetComponent<WindArea>().strength = windStrength;
                        if (randDirection == 0)
                        {
                            windArea.GetComponent<WindArea>().direction = new Vector3(5f, 0f, 0f);
                        }
                        else if (randDirection == 1)
                        {
                            windArea.GetComponent<WindArea>().direction = new Vector3(0f, 0f, 5f);
                        }
                        applyWind = false;
                        applyWindTimer = 3f;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 wind = windArea.GetComponent<WindArea>().direction * windArea.GetComponent<WindArea>().strength * Time.deltaTime;
        if (inWindArea && applyWind)
        {
            rb.AddForce(wind);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WindArea")
        {
            windArea = other.gameObject;
            inWindArea = true;
            windType1 = true;
        }
        else if (other.gameObject.tag == "WindArea2")
        {
            windArea = other.gameObject;
            inWindArea = true;
            windType2 = true;
        }
        else if (other.gameObject.tag == "WallRun")
        {
            canMove = false;
        }
        else if (other.gameObject.tag == "EndPoint")
        {
            backgroundMusic.Stop();
            door.SetActive(false);
            meteorSpawner.SetActive(false);
            environment.SetActive(false);
            gameWon = true;
            Time.timeScale = 0f;
            uiManager.GetComponent<PauseMenu>().winScreen.SetActive(true);
            Debug.Log("Compelted");
        }
        else if (other.gameObject.tag == "HookAmmo")
        {
            pickUp.Play();
            grapplingGun.GetComponent<GrapplingGun>().hookShotAmmo += 1;
            uiManager.GetComponent<UIManager>().ammo = grapplingGun.GetComponent<GrapplingGun>().hookShotAmmo;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "WindArea")
        {
            //Debug.Log("send");
            //Vector3 wind = windArea.GetComponent<WindArea>().direction * windArea.GetComponent<WindArea>().strength;
            //float momentumAddtionalSpeed = 1f;
            //// keep the momentum of the currect hook shot and add some extra to accoutn for the jump momentum
            //characterVelMomentum = wind * momentumAddtionalSpeed;
            //rb.AddForce(characterVelMomentum / 1, ForceMode.Impulse);
            inWindArea = false;
        }
        else if (other.gameObject.tag == "WallRun")
        {
            canMove = true;
        }
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
    public void HandleHookshotMovement(Transform hookshotTransform, Vector3 hookShotPosition, RaycastHit raycastHit)
    {
        camFov.SetCameraFov(HOOKSHOT_FOV);
        speedLines.Play();
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

        if (raycastHit.collider.tag == "FakePlatform")
        {
            platformFallTimer -= 1 * Time.deltaTime;
            if (platformFallTimer <= 0)
            {
                float momentumAddtionalSpeed = 2.5f;
                // keep the momentum of the currect hook shot and add some extra to accoutn for the jump momentum
                characterVelMomentum = hookshotDir * hookshotSpeed * momentumAddtionalSpeed;
                rb.AddForce(characterVelMomentum / 5, ForceMode.Impulse);
                StopHookshot();
            }
        }
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
    public void StopHookshot()
    {
        camFov.SetCameraFov(NORMAL_FOV);
        platformFallTimer = 1f;
        grapplingGun.GetComponent<GrapplingGun>().lr.positionCount = 0;
        grapplingGun.GetComponent<GrapplingGun>().state = GrapplingGun.State.Normal;
        rb.drag = 0;
        pullIn = false;
        speedLines.Stop();
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
        jump.Play();
        if (isSwinging)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // ensure that velocity is 0 so height is always the same

            rb.AddForce(transform.up * 9.5f, ForceMode.Impulse);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // ensure that velocity is 0 so height is always the same

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ResetJump()
    {
        canJump = true;
    }

    void CheckPlatforms()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out checkPlatform, minimumJumpHeight))
        {
            if (checkPlatform.collider.tag == "FakePlatform")
            {
                if (shakeCamera)
                {
                    //StartCoroutine(camMove.Shake(1.5f, .03f));
                    //shakeCamera = false;
                }
                StartCoroutine(grapplingGun.GetComponent<GrapplingGun>().FakePlatformFall(checkPlatform));
                //checkPlatform.collider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.down * 50, ForceMode.Force);
                //checkPlatform.collider.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //Destroy(checkPlatform.collider.gameObject, 3f);
            }
        }
        else
        {
            shakeCamera = true;
        }
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -playerObj.transform.right, out leftWallHit, wallDistance, whatIsWall);
        wallRight = Physics.Raycast(transform.position, playerObj.transform.right, out rightWallHit, wallDistance, whatIsWall);
        wallLeftOrientation = Physics.Raycast(transform.position, -orientation.right, wallDistance);
        wallRightOrientation = Physics.Raycast(transform.position, orientation.right, wallDistance);


        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
                Debug.Log("runnnn");
            }
            else if (wallRight)
            {
                StartWallRun();
                Debug.Log("runnnn right");
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        if (wallRightOrientation || wallLeftOrientation)
        {
            MovePlayer();
        }

        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (wallLeft)
            {
                jump.Play();
                Vector3 wallJumpDir = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                jump.Play();
                Vector3 wallJumpDir = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            }
        }
    }
    void StopWallRun()
    {
        rb.useGravity = true;
    }
}