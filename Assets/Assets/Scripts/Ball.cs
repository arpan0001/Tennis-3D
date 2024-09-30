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

    private ServeManager serveManager; // Reference to ServeManager
    private ScoreManager scoreManager;
    private SoundManager soundManager;

    [SerializeField] private GameObject objectToDisable1;
    [SerializeField] private GameObject objectToDisable2;
    [SerializeField] private GameObject objectToDisableOnSpace1; // New field for the first object to disable
    // New field for the third object to disable

    private void Start()
    {
        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
        scoreManager = FindObjectOfType<ScoreManager>();
        serveManager = FindObjectOfType<ServeManager>();  // Find ServeManager instance
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        if (!serveManager.IsBotServing)  // Ensure only player can toss when not bot's turn
        {
            if (Input.GetKeyDown(KeyCode.Space) && !ballTossed)
            {
                TossBall();
                DisableObjectsOnSpace(); // Call the method to disable the objects
            }

            if (TouchInputDetected() && !ballTossed)
            {
                TossBall();
            }
        }
    }

    private void DisableObjectsOnSpace()
    {
        // Disable the specified objects when space is pressed
        if (objectToDisableOnSpace1 != null)
        {
            objectToDisableOnSpace1.SetActive(false);
        }
        
    }

    private bool TouchInputDetected()
    {
        if (serveManager.IsBotServing)  // Prevent toss when bot is serving
        {
            return false;
        }

        if (Vector3.Distance(transform.position, serveManager.botServePosition.position) < 0.5f)
        {
            return false; 
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !ballTossed)
            {
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    lastTapTime = 0;

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
        if (collision.collider is BoxCollider)
        {
            if (soundManager != null)
            {
                soundManager.PlayCollisionSound(); 
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
            playing = false; 
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
            playing = false; 
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

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
