using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    private Rigidbody rb;
    private Player player;
    private bool ballTossed = false;

    public float tossForce = 5f; // Force applied to the ball when tossing

    // Variables for double-tap detection
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f; // Maximum time between taps to consider as a double-tap

    private void Start()
    {
        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();

        // Ensure there's a Player in the scene
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        // Handle toss input from both keyboard and mobile
        if (Input.GetKeyDown(KeyCode.Space) && !ballTossed)
        {
            TossBall();
        }

        // Handle mobile touch input
        if (TouchInputDetected() && !ballTossed)
        {
            // Check if the touch is a double-tap
            if (IsDoubleTap())
            {
                TossBall();
            }
        }
    }

    private bool TouchInputDetected()
    {
        // Check if there are any touches on the screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch
            if (touch.phase == TouchPhase.Began)
            {
                return true; // Return true if touch started
            }
        }
        return false;
    }

    private bool IsDoubleTap()
    {
        float currentTime = Time.time;
        bool isDoubleTap = (currentTime - lastTapTime) < doubleTapThreshold;
        lastTapTime = currentTime;
        return isDoubleTap;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check for collision with objects tagged as "Wall"
        if (collision.transform.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero; // Stop ball's movement
            StartCoroutine(ResetPositionAfterDelay(1f)); // Reset position after a delay
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the ball's position, velocity, and angular velocity
        transform.position = initialPos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballTossed = false; // Allow the ball to be tossed again

        // Notify the player to reset its position
        if (player != null)
        {
            player.ResetPlayerPosition();
        }
    }

    private void TossBall()
    {
        // Stop any current movement before applying a new force
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * tossForce, ForceMode.Impulse); // Apply the upward toss force
        ballTossed = true; // Indicate that the ball has been tossed
    }
}
