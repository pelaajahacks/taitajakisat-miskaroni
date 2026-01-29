using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Movement Instance;

    [Header("Objects")]

    public Transform cam;
    private Transform look;
    [HideInInspector]
    public Transform head;
    private Transform groundCheck;
    private Rigidbody rb;

    [Header("Running")]
    public float walkSpeed = 55f;
    public float runSpeed = 80f;
    private bool isRunning;

    [Header("Movement")]

    public float speed = 55f;
    public float mouseSensitivity = 3.5f;

    [Space]

    [Tooltip("How much Movement Control in the Air: 0 = No Air Movement | 1 = Same as Ground")]
    [Range(0.0f, 1.0f)]
    public float airMovement = 0.6f;

    [Space]

    [Tooltip("Player Drag when grounded")]
    [Range(0.0f, 10.0f)]
    public float groundDrag = 4f;
    [Tooltip("Player Drag when not grounded")]
    [Range(0.0f, 10.0f)]
    public float airDrag = 3f;

    [Header("Jumping")]

    public float jumpForce = 1300f;

    private bool readyToJump;

    [Header("Wallrunning")]

    public bool useWallrun = true;

    [Space]

    public LayerMask wallrunlayer;
    public float wallRunCheckRange = 1f;
    private Vector3 wallNormal;

    [Space]

    [Tooltip("Minimum Y Velocity to start a wallrun (preventing player to stop falling by starting a wallrun")]
    public float maxYVel = -10;

    [Tooltip("The upwards force applied to the player while wallrunning")]
    public float wallRunUp = 12;
    [Tooltip("The jump force of the wallrun in the normal direction of the wall")]
    public long wallRunJump = 300;

    [Space]

    [Range(0.0f, 1.5f)]
    [Tooltip("The Jump multiplier relative to jumpforce")]
    public float wallRunJumpUpMulti = 0.6f;
    [Range(0.0f, 1.5f)]
    [Tooltip("The Movemnt speed multiplier relative to jumpforce")]
    public float wallRunMovementMulti = 0.7f;

    [Space]

    public bool blockDoubleWallrun = true;
    private GameObject lastWallRunObject;

    [Header("Sliding")]
    public float slideSpeed = 100f;
    public float slideDecayRate = 1f; // fraction per second
    public float slideHeight = 1f;
    private bool isSliding = false;
    private float slideHoldTime = 0f;

    [Header("Camera")]
    public float slideCameraDuration = 0.1f;

    private float currentSlideSpeed;

    [Header("GroundCheck")]

    [Tooltip("Ground Detection Type: Spherecast is more accurate but uses more performance, Raycast uses less performance but is less accurate")]
    public GroundCheckType checkType;
    public enum GroundCheckType
    {
        Spherecast, Raycast
    }

    public bool grounded;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private Vector3 groundNormal;
    private RaycastHit[] groundHits;

    //States
    private bool isWallrunning;

    //Inputs
    private float vertical;
    private float horizontal;
    private bool jump;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();

        look = transform.GetChild(0);
        head = transform.GetChild(1);
        groundCheck = transform.GetChild(2);

        groundNormal = Vector3.zero;
        lastWallRunObject = gameObject;
        wallNormal = Vector3.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        readyToJump = true;

        groundHits = new RaycastHit[10];

        isWallrunning = false;

        Physics.gravity = new Vector3(0, -75f, 0);
    }

    private void Start()
    {
        groundCheck.transform.localPosition = new Vector3(0f, -0.95f, 0f);
    }

    private void Update()
    {
        Look();

        //Input
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        jump = Input.GetKey(KeyCode.Space);

        GroundCheck();

        isRunning = Input.GetKey(KeyCode.LeftShift);
        speed = isRunning ? runSpeed : walkSpeed;

        HandleSlide();
    }

    private void FixedUpdate()
    {
        //Physics
        rb.linearDamping = grounded ? groundDrag : airDrag;

        if (readyToJump && jump && (grounded || isWallrunning))
            Jump();

        if (useWallrun)
            CheckWallRun();

        if (isWallrunning && vertical == 1)
        {
            rb.AddForce(look.up * (wallRunUp * Time.fixedDeltaTime), ForceMode.Impulse);
        }

        if (vertical == 0 && horizontal == 0)
            return;

        float multi = 1f;

        if (!grounded)
            multi = airMovement;

        if (isWallrunning)
            multi = wallRunMovementMulti;

        if (groundNormal != Vector3.zero)
        {
            rb.AddForce(Vector3.Cross(look.right, groundNormal) * (vertical * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
            rb.AddForce(Vector3.Cross(look.forward, groundNormal) * (-horizontal * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(look.forward * (vertical * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
            rb.AddForce(look.right * (horizontal * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
        }
    }

    //Camera Look
    private float xRotation = 0f;
    private float desiredX;
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        desiredX = cam.localRotation.eulerAngles.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.localRotation = Quaternion.Euler(xRotation, desiredX, cam.localRotation.eulerAngles.z);
        look.localRotation = Quaternion.Euler(0, desiredX, 0f);
    }

    void HandleSlide()
    {
        // Start slide while holding Ctrl
        if (Input.GetKey(KeyCode.LeftControl) && grounded && !isSliding)
            StartSlide();

        // Stop slide if released or jump pressed
        if (isSliding && (!Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Space)))
            StopSlide();

        // Update speed while sliding
        if (isSliding)
        {
            slideHoldTime += Time.deltaTime;
            currentSlideSpeed = slideSpeed * Mathf.Pow(slideDecayRate, slideHoldTime);

            // Move player in full input direction
            Vector3 moveDir = (look.forward * vertical + look.right * horizontal).normalized;

            if (moveDir != Vector3.zero)
            {
                Vector3 slideMove = moveDir * currentSlideSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + slideMove);
            }
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideHoldTime = 0f;
        currentSlideSpeed = slideSpeed;

        // Shrink collider
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        cap.height = slideHeight;
        cap.center = new Vector3(0, slideHeight / 2f, 0);

        // Lower camera smoothly
        if (cam != null)
            StartCoroutine(MoveCameraHeight(cam.localPosition.y, slideHeight / 2f, slideCameraDuration));
    }

    void StopSlide()
    {
        isSliding = false;

        // Restore collider
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        cap.height = 2f; // normal height
        cap.center = new Vector3(0, 1f, 0);

        // Raise camera smoothly
        if (cam != null)
            StartCoroutine(MoveCameraHeight(cam.localPosition.y, 1f, slideCameraDuration));
    }

    IEnumerator MoveCameraHeight(float from, float to, float duration)
    {
        float elapsed = 0f;
        Vector3 camPos = cam.localPosition;

        while (elapsed < duration)
        {
            float newY = Mathf.Lerp(from, to, elapsed / duration);
            cam.localPosition = new Vector3(camPos.x, newY, camPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = new Vector3(camPos.x, to, camPos.z);
    }


    private void GroundCheck()
    {
        int c = 0;

        if (checkType == GroundCheckType.Spherecast)
        {
            c = Physics.SphereCastNonAlloc(groundCheck.position, groundCheckRadius, -transform.up, groundHits,
                groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }
        else if (checkType == GroundCheckType.Raycast)
        {
            c = Physics.RaycastNonAlloc(groundCheck.position, -transform.up, groundHits, groundCheckRadius,
                groundLayer, QueryTriggerInteraction.Ignore);
        }

        if (c > 0 && readyToJump)
        {
            grounded = true;
            lastWallRunObject = gameObject;
            groundNormal = groundHits[0].normal;
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.zero;
        }
    }

    private void Jump()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (isWallrunning)
        {
            CameraController.Instance.StopWallrun();
            isWallrunning = false;

            rb.AddForce(wallNormal * (wallRunJump * Time.fixedDeltaTime), ForceMode.Impulse);

            rb.AddForce(transform.up * (jumpForce * wallRunJumpUpMulti * Time.fixedDeltaTime), ForceMode.Impulse);
        }
        else
        {
            if (groundNormal != Vector3.zero)
            {
                rb.AddForce(transform.up * jumpForce / 2 * Time.fixedDeltaTime, ForceMode.Impulse);
                rb.AddForce(groundNormal * jumpForce / 2 * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            else
                rb.AddForce(transform.up * (jumpForce * Time.fixedDeltaTime), ForceMode.Impulse);
        }

        readyToJump = false;
        grounded = false;
        groundNormal = Vector3.zero;
        Invoke(nameof(ResetJump), 0.15f);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void CheckWallRun()
    {
        if (grounded)
        {
            if (isWallrunning)
            {
                CameraController.Instance.StopWallrun();
                isWallrunning = false;
            }

            return;
        }

        if (Physics.Raycast(transform.position, look.right, out RaycastHit righthit, wallRunCheckRange, wallrunlayer))
        {
            if (!isWallrunning && rb.linearVelocity.y < maxYVel)
                return;

            if (!isWallrunning && blockDoubleWallrun && righthit.transform.gameObject == lastWallRunObject)
                return;

            if (!isWallrunning)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallRunJumpUpMulti, rb.linearVelocity.z);

            lastWallRunObject = righthit.transform.gameObject;
            wallNormal = righthit.normal;
            CameraController.Instance.StartWallrun(true);
            isWallrunning = true;
        }
        else if (Physics.Raycast(transform.position, -look.right, out RaycastHit lefthit, wallRunCheckRange, wallrunlayer))
        {
            if (!isWallrunning && rb.linearVelocity.y < maxYVel)
                return;

            if (!isWallrunning && blockDoubleWallrun && lefthit.transform.gameObject == lastWallRunObject)
                return;

            if (!isWallrunning)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallRunJumpUpMulti, rb.linearVelocity.z);

            lastWallRunObject = lefthit.transform.gameObject;
            wallNormal = lefthit.normal;
            CameraController.Instance.StartWallrun(false);
            isWallrunning = true;
        }
        else if (isWallrunning)
        {
            //Stop Wallrunning
            CameraController.Instance.StopWallrun();
            isWallrunning = false;
        }
    }
}