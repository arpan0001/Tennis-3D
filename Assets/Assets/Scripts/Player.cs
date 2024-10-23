using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;  // Needed for Coroutines

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
    private StaminaSystem staminaSystem;
    private ServeManager serveManager;
    private SoundManager soundManager;

    private int shotCounter = 0;

    // References to the GameObjects
    [SerializeField] private GameObject objectToDisable;
    [SerializeField] private GameObject object2;

    // Delay in seconds before enabling object2
    [SerializeField] private float enableDelay = 1f;

    // Reference to the CameraShake script
    private CameraShake cameraShake;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        staminaSystem = GetComponent<StaminaSystem>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        currentShot = shotManager.topSpin;

        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();

        serveManager = FindObjectOfType<ServeManager>();
        soundManager = FindObjectOfType<SoundManager>();

        // Get reference to CameraShake
        cameraShake = FindObjectOfType<CameraShake>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Vector3.Distance(ball.position, serveManager.botServePosition.position) < 0.5f)
        {
            canHitBall = true;
        }

        Vector3 moveDirection = new Vector3(h + moveInput.x, 0, v + moveInput.y);

        if (moveDirection != Vector3.zero && !hitting && staminaSystem.CanMove())
        {
            // Disable the object when the player moves
            if (objectToDisable != null && objectToDisable.activeSelf)
            {
                objectToDisable.SetActive(false);
                StartCoroutine(EnableObject2AfterDelay()); // Start coroutine to enable object2 after a delay
            }

            transform.Translate(moveDirection * speed * Time.deltaTime);

            if (Time.frameCount % 5 == 0)  // Drain stamina every 5 frames instead of every frame
            {
                staminaSystem.DrainStamina(0.2f);  // Reduced drain rate to 0.2f
            }

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            transform.position = clampedPosition;
        }

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
            shotCounter++;

            if (shotCounter % 4 == 0)
            {
                currentShot = shotManager.flat;
            }
            else
            {
                currentShot = shotManager.topSpin;
            }

            Vector3 targetPosition = PickRandomTargetWithinQuad();
            Vector3 directionToTarget = (targetPosition - ball.position).normalized;
            other.GetComponent<Rigidbody>().velocity = directionToTarget * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
            Vector3 ballDir = ball.position - transform.position;

            if (soundManager != null)
            {
                soundManager.PlayHitSoundWithDelay(0f);
            }

            // Shake the camera when hitting the ball
            if (cameraShake != null)
            {
                cameraShake.ShakeCamera();
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

    // Coroutine to enable object2 after a delay
    IEnumerator EnableObject2AfterDelay()
    {
        yield return new WaitForSeconds(enableDelay);  // Wait for the specified delay
        if (object2 != null)
        {
            object2.SetActive(true);  // Enable object2
        }
    }
}
