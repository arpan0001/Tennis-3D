using System.Collections;
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

    [SerializeField] Transform serveRight;
    [SerializeField] Transform serveLeft;
    private bool servedRight = true;

    // Reference to ScoreManager
    private ScoreManager scoreManager;

    // References to the GameObjects you want to disable on double tap
    [SerializeField] private GameObject objectToDisable1;
    [SerializeField] private GameObject objectToDisable2;

    private void Start()
    {
        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();

        player = FindObjectOfType<Player>();

        // Find the ScoreManager instance
        scoreManager = FindObjectOfType<ScoreManager>();
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
        if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Out"))
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(ResetPositionAfterDelay(1f));

            player.Reset();

            if (playing)
            {
                // Use the ScoreManager to update the score
                scoreManager.UpdateScore(hitter);
                playing = false;
            }
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (servedRight)
        {
            transform.position = serveRight.position + new Vector3(0.2f, 1, 0);
        }
        else
        {
            transform.position = serveLeft.position + new Vector3(0.2f, 1, 0);
        }

        servedRight = !servedRight;

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
}
