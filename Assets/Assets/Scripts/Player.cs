using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float minX = -5f;
    public float maxX = 5f;
    float speed = 3f;
    bool hitting;
    bool canHitBall = false; 

    public Transform ball;
    public Transform quad;
    Animator animator;

    ShotManager shotManager;
    Shot currentShot;

    [SerializeField] Transform serveRight;
    [SerializeField] Transform serveLeft;

    bool servedRight = true;

    private Vector2 moveInput;
    private PlayerInput playerInput;

    Vector3 initialPos;
    private Rigidbody rb;

    // Reference to StaminaSystem
    private StaminaSystem staminaSystem;

    // Reference to ServeManager
    private ServeManager serveManager;

    // Reference to SoundManager
    private SoundManager soundManager; // Add SoundManager reference

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Find the StaminaSystem component
        staminaSystem = GetComponent<StaminaSystem>();  // Assumes StaminaSystem is attached to the same GameObject
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        currentShot = shotManager.topSpin;

        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();

        // Find ServeManager in the scene or set it in the inspector
        serveManager = FindObjectOfType<ServeManager>();

        // Find SoundManager in the scene
        soundManager = FindObjectOfType<SoundManager>(); // Assumes SoundManager is present in the scene
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Check if the ball is at the bot's serve position
        if (Vector3.Distance(ball.position, serveManager.botServePosition.position) < 0.5f)
        {
            canHitBall = true;  // Allow player to hit the ball when it's at the bot's serve position
        }

        if (canHitBall)
        {
            // Handling shot selection and hitting
            if (Input.GetKeyDown(KeyCode.F))
            {
                hitting = true;
                currentShot = shotManager.topSpin;
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                hitting = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                hitting = true;
                currentShot = shotManager.flat;
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                hitting = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                hitting = true;
                currentShot = shotManager.flatServe;
                GetComponent<BoxCollider>().enabled = false;
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                hitting = false;
                GetComponent<BoxCollider>().enabled = true;
                ball.transform.position = transform.position + new Vector3(0.2f, 1, 0);
                Vector3 targetPosition = PickRandomTargetWithinQuad();
                Vector3 directionToTarget = (targetPosition - ball.position).normalized;
            }
        }

        Vector3 moveDirection = new Vector3(h + moveInput.x, 0, v + moveInput.y);

        // Only move if there is enough stamina and the player is not hitting the ball
        if (moveDirection != Vector3.zero && !hitting && staminaSystem.CanMove())
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);

            // Drain stamina while moving
            staminaSystem.DrainStamina();

            // Clamp the player's position within the movement range
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            transform.position = clampedPosition;
        }

        // Enable hitting when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canHitBall = true;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    Vector3 PickRandomTargetWithinQuad()
    {
        Vector3 quadSize = quad.localScale;
        Vector3 quadPosition = quad.position;

        float quadMinX = quadPosition.x - (quadSize.x / 2);
        float quadMaxX = quadPosition.x + (quadSize.x / 2);
        float quadMinZ = quadPosition.z - (quadSize.z / 2);
        float quadMaxZ = quadPosition.z + (quadSize.z / 2);

        float randomX = Random.Range(quadMinX, quadMaxX);
        float randomZ = Random.Range(quadMinZ, quadMaxZ);

        return new Vector3(randomX, ball.position.y, randomZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && canHitBall)
        {
            Vector3 targetPosition = PickRandomTargetWithinQuad();
            Vector3 directionToTarget = (targetPosition - ball.position).normalized;
            other.GetComponent<Rigidbody>().velocity = directionToTarget * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
            Vector3 ballDir = ball.position - transform.position;

            // Play hit sound when the player hits the ball
            if (soundManager != null)
            {
                soundManager.PlayHitSoundWithDelay(0f); // 1 second delay before playing the hit sound
            }

            if (ballDir.x >= 0)
            {
                animator.Play("backhand");
            }
            else
            {
                animator.Play("forehand");
            }

            ball.GetComponent<Ball>().hitter = "player";
            ball.GetComponent<Ball>().playing = true;
        }
    }

    public void ResetPlayerPosition()
    {
        // Reset player position and disable hitting
        canHitBall = false; 
    }

    public void SetCanHitBall(bool value)
    {
        canHitBall = value;
    }

    public void Reset()
    {
        if (servedRight)
            transform.position = serveLeft.position;
        else
            transform.position = serveRight.position;

        servedRight = !servedRight;         
    }
}
