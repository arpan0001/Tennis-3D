using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    public string hitter;

    int playerScore;
    int botScore;

    [SerializeField] Text playerScoreText;
    [SerializeField] Text botScoreText;

    public bool playing = true;

    private Rigidbody rb;
    private Player player;
    private bool ballTossed = false;
    private float lastTapTime = 0f;
    public float tossForce = 5f; // Force applied to the ball when tossing
    public float doubleTapThreshold = 0.3f; // Time threshold for double tap detection

    [SerializeField] Transform serveRight;  // Serve right position
    [SerializeField] Transform serveLeft;   // Serve left position
    private bool servedRight = true;        // To track the current serve direction

    private void Start()
    {
        initialPos = transform.position;
        playerScore = 0;
        botScore = 0;
        rb = GetComponent<Rigidbody>();

        // Ensure there's a Player in the scene
        player = FindObjectOfType<Player>();

        // Initialize score UI
        updateScores();
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
            TossBall();
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
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    // Double-tap detected
                    lastTapTime = 0; // Reset for next detection
                    TossBall();
                    return true;
                }
                lastTapTime = Time.time;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collisions with Wall or Out
        if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Out"))
        {
            rb.velocity = Vector3.zero; // Stop ball's movement
            StartCoroutine(ResetPositionAfterDelay(1f)); // Reset position after a delay

            // Reset the player's position
            player.Reset();

            // Update the score
            if (playing)
            {
                if (hitter == "player")
                {
                    playerScore++;
                }
                else if (hitter == "bot")
                {
                    botScore++;
                }
                playing = false;
                updateScores(); // Update the score UI
            }
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Adjust the ball's position based on the serve direction
        if (servedRight)
        {
            transform.position = serveRight.position + new Vector3(0.2f, 1, 0); // Serve from the right
        }
        else
        {
            transform.position = serveLeft.position + new Vector3(0.2f, 1, 0);  // Serve from the left
        }

        // Toggle the serve direction
        servedRight = !servedRight;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballTossed = false; // Allow the ball to be tossed again

        // Notify the player to reset its position
        if (player != null)
        {
            player.ResetPlayerPosition();
        }

        playing = true; // Ball is ready to be in play again
    }

    private void TossBall()
    {
        // Stop any current movement before applying a new force
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * tossForce, ForceMode.Impulse); // Apply the upward toss force
        ballTossed = true; // Indicate that the ball has been tossed

        // Enable hitting the ball after toss
        if (player != null)
        {
            player.SetCanHitBall(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Out") && playing)
        {
            if (hitter == "player")
            {
                playerScore++;
            }
            else if (hitter == "bot")
            {
                botScore++;
            }
            playing = false;
            updateScores();
        }
    }

    void updateScores()
    {
        // Update the UI with the current scores
        if (playerScoreText != null && botScoreText != null)
        {
            playerScoreText.text = "Player : " + playerScore;
            botScoreText.text = "Bot : " + botScore;
        }
    }
}
