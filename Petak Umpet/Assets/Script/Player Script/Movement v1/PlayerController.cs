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
    [SerializeField]  float speed = 6.0F; // kecepatan
    [SerializeField]  float jumpSpeed = 8.0F; // kecepatan loncat
    [SerializeField] [Range(0.0f, 5.0f)] float moveSmoothTime = 0.2f;
    [SerializeField] [Range(0.0f, 5.0f)] float mouseSmoothTime = 0.03f;
    public float walkSpeed = 6.0f;
    float cameraPitch = 0.0f;
    float velocityY = 0.0f;

    
    //ground check
    public Transform groundCheck; 
    bool isGrounded; // Pernyataan GroundCheck
    public float groundDistance = 0.4f; // Ground Distance
    public LayerMask groundMask; // Ground layer Type
    private Vector3 pos = Vector3.zero;
    



    CharacterController controller = null;
    public Vector2 currentDir = Vector2.zero;
    public Vector2 currentDirVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    //coyote time
    public float jumpRememberTime = 0.2f; 
    private float jumpRememberTimer; 

    public float groundedRememberTime = 0.2f; 
    private float groundedRememberTimer = 0f;

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
        // Loncat
        jumpRememberTimer -= Time.deltaTime; 

        if (Input.GetButtonDown ("Jump"))   
        jumpRememberTimer = jumpRememberTime; 

        if (isGrounded)
        groundedRememberTimer = groundedRememberTime; 
        else
        groundedRememberTimer -= Time.deltaTime; 

        if (jumpRememberTimer > 0f && groundedRememberTimer > 0f)
        {
        pos.y = jumpSpeed;
        }


        // groundcheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
         if(isGrounded && velocityY < 0)
         {
             pos.y = -2f;
         }

        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook() //camera
    {
        // Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity; 
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement() //pergerakan player
    {
       
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded) velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime; //gravitasi

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);


    }
}
