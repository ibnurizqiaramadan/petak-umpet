using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    // public

    [SerializeField] Transform playerCamera = null; 
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] float jumpSpeed = 8.0F; // kecepatan loncat
    [SerializeField] AnimationCurve jumpFallOff;
    [SerializeField] [Range(0.0f, 5.0f)] float moveSmoothTime = 0.2f;
    [SerializeField] [Range(0.0f, 5.0f)] float mouseSmoothTime = 0.03f;
    public float walkSpeed = 6.0f;
    float cameraPitch = 0.0f;
    float velocityY = 0.0f;

    public Animator anim; //animasi

    CharacterController controller = null;
    public Vector2 currentDir = Vector2.zero;
    public Vector2 currentDirVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    //ground check
    Transform groundCheck; 
    bool isGrounded; // Pernyataan GroundCheck
    float groundDistance = 0.4f; // Ground Distance
    LayerMask groundMask; // Ground layer Type
    Vector3 pos = Vector3.zero;

    bool isJumping = false;


    //coyote time
    float jumpRememberTime = 0.2f; 
    float jumpRememberTimer; 

    float groundedRememberTime = 0.2f; 
    float groundedRememberTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame          
    void Update()
    {
        PlayerMouseLook();
        PlayerMovement();
        PlayerAnimation();
    }

    //camera
    void PlayerMouseLook() 
    {
        // Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity; 
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    //pergerakan player
    void PlayerMovement()
    {
       
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded) velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime; //gravitasi

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        PlayerJump();
    }
    //player animasi controller 
    void PlayerAnimation()
    {
        anim.SetFloat("float_vertical", Input.GetAxis("Vertical"));

        anim.SetFloat("float_horizontal", Input.GetAxis ("Horizontal"));
        /*
        if (Input.GetKeyDown(KeyCode.LeftControl))
            anim.SetTrigger("trigger_jongkok");

            else if (Input.GetKeyUp(KeyCode.LeftControl))
                    anim.ResetTrigger("trigger_jongkok");
        */
    }

    //player movement jump updater
    void PlayerJump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    //player movement jump event
    private IEnumerator JumpEvent()
    {
        controller.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do {

            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            controller.Move(Vector3.up * jumpForce * jumpSpeed * Time.deltaTime);
            timeInAir += Time.deltaTime;

            yield return null;
        } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);
        controller.slopeLimit = 45.0f;
        isJumping = false;
    }
}
