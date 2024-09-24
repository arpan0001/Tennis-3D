using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{
    float speed = 40;
    Animator animator;
    public Transform ball;
    public Transform aimTarget;

    public Transform[] targets;

    float force = 13;
    Vector3 targetPosition;

    ShotManager shotManager;
    ServeManager serveManager; // Reference to ServeManager
    Ball ballScript;           // Reference to Ball script

    public float detectionRange = 10f; // Range within which the bot starts following the ball

    private int topspinCount = 0; // Counter for topspin shots
    private int shotCounter = 0;  // Counter for counting the number of successful shots

    public Transform botServePosition; // Bot's serve position
    public float serveTossHeight = 10f; // Height for the ball toss during serve
    public float serveDelay = 1.5f; // Delay before hitting the ball after toss
    private bool isServing = false; // To check if the bot is serving

    // Reference to SoundManager
    private SoundManager soundManager;

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        serveManager = FindObjectOfType<ServeManager>(); // Get ServeManager component
        ballScript = ball.GetComponent<Ball>(); // Get Ball component

        // Get SoundManager component
        soundManager = FindObjectOfType<SoundManager>(); // Assumes SoundManager is present in the scene
    }

    void Update()
    {
        // Disable the bot's serve position if it's the player's turn to serve
        botServePosition.gameObject.SetActive(!serveManager.IsBotServing);

        if (IsBallWithinRange() && !isServing) // Only calculate position and speed if ball is within range
        {
            Move();
        }

        // Handle serving logic
        if (IsBallAtServePosition() && !isServing && serveManager.IsBotServing)
        {
            StartCoroutine(HandleBotServe()); // Start the serve process
        }
    }

    bool IsBallWithinRange()
    {
        float distanceToBall = Vector3.Distance(transform.position, ball.position);
        return distanceToBall <= detectionRange; // Check if ball is within detection range
    }

    bool IsBallAtServePosition()
    {
        // Check if the ball is close to the bot's serve position
        float distanceToServePosition = Vector3.Distance(ball.position, botServePosition.position);
        return distanceToServePosition < 0.5f; // Adjust the threshold as needed
    }

    void Move()
    {
        // Calculate movement only if the ball is within range
        targetPosition.x = ball.position.x;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    Vector3 PickTarget()
    {
        int randomValue = Random.Range(0, targets.Length);
        return targets[randomValue].position;
    }

    Shot PickShot()
    {
        // Bot picks 4 topspins, then 1 flat, then repeats the pattern
        if (topspinCount < 4)
        {
            topspinCount++;
            return shotManager.topSpin; // Pick topspin
        }
        else
        {
            topspinCount = 0; // Reset the counter after picking a flat shot
            return shotManager.flat; // Pick flat shot
        }
    }

    // Coroutine to handle the serve process
    IEnumerator HandleBotServe()
    {
        isServing = true; // Set serving flag to true

        // Move the bot to the serve position
        targetPosition = botServePosition.position; // Set target position to serve position
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // Move until close enough
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Ball toss logic
        ball.position = botServePosition.position; // Set ball to the serve position
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, serveTossHeight, 0); // Toss the ball upwards

        yield return new WaitForSeconds(serveDelay); // Wait before the bot hits the ball

        // Determine direction for the shot
        Vector3 ballDir = ball.position - transform.position;
        Shot currentShot = PickShot(); // Pick the correct shot based on the sequence
        Vector3 dir = PickTarget() - transform.position; // Pick a target
        ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0); // Apply velocity to the ball

        // Play hit sound when the bot hits the ball
        if (soundManager != null)
        {
            soundManager.PlayHitSoundWithDelay(1f); // 1 second delay before playing the hit sound
        }

        // Play appropriate animation for serving
        if (ballDir.x >= 0)
        {
            animator.Play("forehand");
        }
        else
        {
            animator.Play("backhand");
        }

        ballScript.hitter = "bot"; // Set the bot as the hitter

        isServing = false; // Reset serving flag after the serve
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !isServing) // Only allow normal shots if the bot is not serving
        {
            shotCounter++; // Increment shot counter

            // If shot counter reaches 7, bot misses the ball
            if (shotCounter == 7)
            {
                shotCounter = 0; // Reset the counter after missing the ball
                Debug.Log("Bot missed the ball!"); // Debug log to show the bot missed the ball
                return; // Skip the shot to simulate a miss
            }

            // Bot hits the ball
            Shot currentShot = PickShot(); // Pick the correct shot based on the sequence
            Vector3 dir = PickTarget() - transform.position; // Pick a target
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0); // Apply velocity to the ball

            // Play hit sound when the bot hits the ball
            if (soundManager != null)
            {
                soundManager.PlayHitSoundWithDelay(0f); // 1 second delay before playing the hit sound
            }

            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand");
            }
            else
            {
                animator.Play("backhand");
            }

            ballScript.hitter = "bot"; // Set the bot as the hitter
        }
    }
}
