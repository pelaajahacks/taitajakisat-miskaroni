using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 6f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 200f;
    public Transform cameraHolder;

    [Header("Slide")]
    public float slideSpeed = 14f;
    public float slideTime = 0.6f;
    public float slideHeight = 1f;

    CharacterController controller;

    float yVelocity;
    float xRotation = 0f;

    bool isSliding = false;
    float slideTimer;

    float normalHeight;
    Vector3 normalCenter;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        normalHeight = controller.height;
        normalCenter = controller.center;
    }

    void Update()
    {
        MouseLook();
        Movement();
        HandleSlide();
    }

    // ================= MOUSE LOOK =================

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ================= MOVEMENT =================

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float speed = walkSpeed;

        // Run
        if (Input.GetKey(KeyCode.LeftShift) && !isSliding)
            speed = runSpeed;

        // Jump (including slide cancel)
        if (controller.isGrounded)
        {
            if (yVelocity < 0)
                yVelocity = -2f;

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isSliding)
                    StopSlide(); // Cancel slide
                    TryJump();

                yVelocity = jumpForce;
            }
        }

        // Gravity
        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    // ================= SLIDE =================

    void HandleSlide()
    {
        // Start slide
        if (Input.GetKeyDown(KeyCode.LeftControl) && controller.isGrounded && !isSliding)
        {
            StartSlide();
        }

        // Slide movement
        if (!isSliding)
            return;

        slideTimer -= Time.deltaTime;

        // If cancelled, stop immediately
        if (slideTimer <= 0f)
        {
            StopSlide();
            return;
        }

        Vector3 slideMove = transform.forward * slideSpeed;
        controller.Move(slideMove * Time.deltaTime);
    }



    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideTime;

        // Shrink collider
        controller.height = slideHeight;
        controller.center = new Vector3(0, slideHeight / 2f, 0);
    }

    void StopSlide()
    {
        isSliding = false;

        // Restore collider
        controller.height = normalHeight;
        controller.center = normalCenter;
    }

    void TryJump()
    {
        if (!controller.isGrounded)
            return;

        // HARD cancel slide
        if (isSliding)
        {
            isSliding = true;
            slideTimer = 0f;
            StopSlide();
        }

        if (yVelocity < 0)
            yVelocity = -2f;

        yVelocity = jumpForce;
    }

}