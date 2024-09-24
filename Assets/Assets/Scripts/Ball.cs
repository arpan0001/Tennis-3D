using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    public string hitter;
    public bool playing = true;
    private Rigidbody rb;
    private Player player;
    private bool ballTossed = false;
    private float lastTapTime = 0f;
    public float tossForce = 5f;
    public float doubleTapThreshold = 0.3f;

    // Reference to ServeManager
    private ServeManager serveManager;

    // Reference to ScoreManager
    private ScoreManager scoreManager;

    // Reference to SoundManager
    private SoundManager soundManager;

    // References to the GameObjects you want to disable on double tap
    [SerializeField] private GameObject objectToDisable1;
    [SerializeField] private GameObject objectToDisable2;

    private void Start()
    {
        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
        scoreManager = FindObjectOfType<ScoreManager>();
        serveManager = FindObjectOfType<ServeManager>(); // Find ServeManager in the scene
        soundManager = FindObjectOfType<SoundManager>(); // Find SoundManager in the scene
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !ballTossed)
        {
            TossBall();
        }

        if (TouchInputDetected() && !ballTossed)
        {
            TossBall();
        }
    }

    private bool TouchInputDetected()
    {
        // Check if the ball is at the bot's serve position
        if (Vector3.Distance(transform.position, serveManager.botServePosition.position) < 0.5f)
        {
            return false; // Disable double tap if ball is at bot's serve position
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !ballTossed)
            {
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    lastTapTime = 0;

                    // Disable the GameObjects on double tap
                    if (objectToDisable1 != null)
                    {
                        objectToDisable1.SetActive(false);
                    }
                    if (objectToDisable2 != null)
                    {
                        objectToDisable2.SetActive(false);
                    }

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
        // Play collision sound when ball collides with a BoxCollider
        if (collision.collider is BoxCollider)
        {
            if (soundManager != null)
            {
                soundManager.PlayCollisionSound(); // Play the collision sound
            }
        }

        if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Out"))
        {
            HandleBallOut();
        }
        else if (collision.transform.CompareTag("Net"))
        {
            HandleBallNetCollision();
        }
    }

    private void HandleBallOut()
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(ResetPositionAfterDelay(1f));
        player.Reset();

        if (playing)
        {
            scoreManager.UpdateScore(hitter);
            playing = false; // Set playing to false when a point is scored
        }
    }

    private void HandleBallNetCollision()
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(ResetPositionAfterDelay(1f));
        player.Reset();

        if (playing)
        {
            scoreManager.UpdateScore(hitter, netCollision: true);
            playing = false; // Set playing to false when a point is scored
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Get the next serve position from ServeManager
        transform.position = serveManager.GetNextServePosition();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballTossed = false;

        if (player != null)
        {
            player.ResetPlayerPosition();
        }

        playing = true;
        lastTapTime = 0;
    }

    private void TossBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * tossForce, ForceMode.Impulse);
        ballTossed = true;

        if (player != null)
        {
            player.SetCanHitBall(true);
        }
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballTossed = false;

        if (player != null)
        {
            player.SetCanHitBall(false);
        }
    }
}
