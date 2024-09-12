using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float minX = -5f;  // Minimum X range
    public float maxX = 5f;   // Maximum X range
    float speed = 3f;
    bool hitting;

    public Transform ball;
    public Transform quad;  // Reference to the Quad in the scene
    Animator animator;

    ShotManager shotManager;
    Shot currentShot;

    private Vector2 moveInput; 
    private PlayerInput playerInput; 

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); 
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        currentShot = shotManager.topSpin;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");

        // Handle hitting logic
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

        // Move player based on keyboard input and joystick
        Vector3 moveDirection = new Vector3(h + moveInput.x, 0, v + moveInput.y);
        if (moveDirection != Vector3.zero && !hitting)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>(); 
    }

    // Randomly pick a target position within the bounds of the quad
    Vector3 PickRandomTargetWithinQuad()
    {
        // Get the quad's position and size
        Vector3 quadSize = quad.localScale;  // Scale of the quad (local scale represents width and height in x and z)
        Vector3 quadPosition = quad.position;  // Position of the quad in the world

        // Calculate the boundaries of the quad in world space
        float quadMinX = quadPosition.x - (quadSize.x / 2);  // Left edge of the quad
        float quadMaxX = quadPosition.x + (quadSize.x / 2);  // Right edge of the quad
        float quadMinZ = quadPosition.z - (quadSize.z / 2);  // Bottom edge of the quad
        float quadMaxZ = quadPosition.z + (quadSize.z / 2);  // Top edge of the quad

        // Pick a random point within the quad's bounds
        float randomX = Random.Range(quadMinX, quadMaxX);  // Random x position within quad
        float randomZ = Random.Range(quadMinZ, quadMaxZ);  // Random z position within quad

        return new Vector3(randomX, ball.position.y, randomZ);  // Return the random point in world space
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 targetPosition = PickRandomTargetWithinQuad();  // Pick a random point on the quad

            // Calculate direction towards the random target, based on position only, not direction from player
            Vector3 directionToTarget = (targetPosition - ball.position).normalized;

            // Set the ball's velocity towards the target position
            other.GetComponent<Rigidbody>().velocity = directionToTarget * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            // Play appropriate animation based on ball's relative position
            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand");
            }
            else
            {
                animator.Play("backhand");
            }
        }
    }
}
