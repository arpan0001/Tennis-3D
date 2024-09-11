using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //public Transform aimTarget;  // Commented out aimTarget
    public float minX = -5f;  // Minimum X range
    public float maxX = 5f;   // Maximum X range
    float speed = 3f;
    bool hitting;

    public Transform ball;
    public Transform[] targets;  // Array to hold target positions
    Animator animator;

    //Vector3 aimTargetInitialPosition;  // Commented out aimTargetInitialPosition
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
        //aimTargetInitialPosition = aimTarget.position;  // Commented out aimTargetInitialPosition assignment
        shotManager = GetComponent<ShotManager>();
        currentShot = shotManager.topSpin;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");

        // Update aim target based on mouse position
        // Vector3 mousePosition = Input.mousePosition;  // Commented out aimTarget update
        // mousePosition.z = Camera.main.WorldToScreenPoint(aimTarget.position).z;  // Keep z distance
        // Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Clamp the aim target's X position within the defined range
        // float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);

        // Set the aim target position, limiting its movement within the range
        // aimTarget.position = new Vector3(clampedX, aimTarget.position.y, aimTarget.position.z);  // Commented out aimTarget position update

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

        // Move player based on keyboard input and joystick, but not affecting aim target
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

    Vector3 PickTarget()
    {
        if (targets.Length > 0)
        {
            int randomValue = Random.Range(0, targets.Length); 
            return targets[randomValue].position; 
        }
        return transform.position;  // Return current position if no targets are available
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 targetPosition = PickTarget();  // Pick a random target position
            Vector3 dir = targetPosition - transform.position;
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand");
            }
            else
            {
                animator.Play("backhand");
            }

            // aimTarget.position = aimTargetInitialPosition;  // Commented out aimTarget reset
        }
    }
}
