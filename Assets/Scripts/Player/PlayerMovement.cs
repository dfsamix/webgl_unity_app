using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs inputs;

    public LayerMask floorDetectors;
    private Vector3 detectBox = new Vector3(.2f, .1f, .2f);
    private float shiftY = .2f;
    private float maxDistance = .1f;

    private Vector2 move;
    private Vector2 aim;
    private bool isJumpPresed;
    public bool isGrounded;

    private float accelerate = .88f;
    private float moveSpeed;
    private float walkSpeed = 2.0f;
    private float runSpeed = 4.0f;
    private float forceMultiplier = 1000.0f;
    private float jumpForce;
    private float jumpUpMultiplier;
    private float jumpDownMultiplier;

    public bool isWalking;
    public bool isRunning;

    private Vector3 lookDirection;
    private float rotAlongGlobalY;
    private float lastRotation;
    public bool isStopAiming;
    public Vector3 aimPoint;
    private float lerpSpeed = 6.66f;

    //debug only
    [SerializeField] private TextMeshProUGUI Text_JumpPressed;
    [SerializeField] private TextMeshProUGUI Text_isGroundedPlayer;
    [SerializeField] private TextMeshProUGUI Text_isGroundedAnimator;
    [SerializeField] private TextMeshProUGUI Text_yVelocity;
    [SerializeField] private TextMeshProUGUI Text_look;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        inputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        move = inputs.moveInput;
        aim = inputs.aimInput;

        isRunning = inputs.isRunning;

        //debug
        ChangeColor(isJumpPresed, Text_JumpPressed);
        ChangeColor(true, Text_yVelocity);
        Text_yVelocity.text = "velocity  " + rb.linearVelocity.ToString();
        Text_look.text = "look  " + lastRotation;
    }

    private void FixedUpdate()
    {
        Move(move);
        Aim();

        //debug
        ChangeColor(IsGrounded(), Text_isGroundedPlayer);
    }

    private void Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(aim);
        if (Physics.Raycast(ray, out var hitPoint, Mathf.Infinity, floorDetectors))
        {
            lookDirection = hitPoint.point - transform.position;
            lookDirection.y = 0f;
            lookDirection = lookDirection.normalized;

            rb.transform.forward = Vector3.Slerp(rb.transform.forward, lookDirection, Time.fixedDeltaTime * lerpSpeed);

            if (hitPoint.collider.name != "NearCollider")
            {
                isStopAiming = true;
            }
            else
            {
                isStopAiming = false;
            }
        }

        //-1 right 1 left turnings
        lastRotation = rotAlongGlobalY - transform.eulerAngles.y;
        // hit

        Vector3 pointPos = transform.position;
        pointPos.y += 1.7f;
        Debug.DrawLine(pointPos, hitPoint.point, Color.red);

        rotAlongGlobalY = transform.eulerAngles.y;
        aimPoint = hitPoint.point;
    }

    private void Move(Vector2 move)
    {
        isJumpPresed = inputs.isJumpPressed;
        isGrounded = IsGrounded();

        if (move != Vector2.zero)
        {
            isWalking = true;
        }

        jumpForce = 2.0f;
        jumpUpMultiplier = 75.0f;
        jumpDownMultiplier = 120.0f;

        if (isWalking)
        {
            moveSpeed = walkSpeed;
        }
        if (isRunning)
        {
            moveSpeed = runSpeed;
        }

        Vector3 move3 = new Vector3(move.x, 0, move.y);

        if (isGrounded)
        {
            rb.maxLinearVelocity = moveSpeed;
            if (move3.magnitude > 0)
            {
                rb.AddForce(move3 * accelerate * forceMultiplier, ForceMode.Force);
            }
            else
            {
                rb.AddForce(rb.linearVelocity * -accelerate * forceMultiplier, ForceMode.Force);
            }
        }

        if (isJumpPresed)
        {
            if (isGrounded)
            {
                Vector3 moveJump = new Vector3(move.x * .4f, jumpForce, move.y * .4f);
                if (move.magnitude == 0) { moveJump.y *= 1.15f; jumpUpMultiplier = 78f; }

                rb.maxLinearVelocity = jumpForce * jumpUpMultiplier;
                rb.AddForce(moveJump * jumpForce * jumpUpMultiplier, ForceMode.Impulse);
                inputs.ResetJumpAction();
            }
        }

        if (!isGrounded)
        {
            Vector3 moveJump = new Vector3(move.x, -1.0f, move.y);
            if (move.magnitude == 0) { jumpDownMultiplier = 180f; }
            rb.maxLinearVelocity = jumpForce * jumpDownMultiplier;
            rb.AddForce(moveJump * jumpForce * jumpDownMultiplier, ForceMode.Force);
        }

        if (move == Vector2.zero)
        {
            isWalking = false;
        }
    }

    private bool IsGrounded()
    {
        Vector3 pos = transform.position;
        pos.y += shiftY;

        if (Physics.BoxCast(pos, detectBox, -transform.up, transform.rotation, maxDistance, floorDetectors))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //debug
    private void ChangeColor(bool col, TextMeshProUGUI text)
    {
        if (col)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.red;
        }
    }
}