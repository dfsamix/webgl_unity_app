using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputControls inputs;
    public PlayerMovement movement;

    public Vector2 moveInput;
    public Vector2 aimInput;
    public bool isJumpPressed;
    public bool isRunning;

    private void Awake()
    {
        inputs = new InputControls();

        inputs.Character.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Character.Move.canceled += ctx => moveInput = Vector2.zero;

        inputs.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        inputs.Character.Aim.canceled += ctx => aimInput = Vector2.zero;

        ResetJumpAction();
    }

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (inputs.Character.Jump.triggered)
        {
            if (movement.isGrounded)
            {
                isJumpPressed = true;
            }
        }

        if (inputs.Character.Run.triggered)
        {
            isRunning = !isRunning;
        }
    }

    public void ResetJumpAction()
    {
        isJumpPressed = false;
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}