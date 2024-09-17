using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    public string hitter;

    int[] tennisScores = { 0, 15, 30, 40 };
    int playerScoreIndex = 0;
    int botScoreIndex = 0;

    int playerSets = 0; 
    int botSets = 0;    
    int setsToWin = 2;  

    
    [SerializeField] Text playerScoreText;
    [SerializeField] Text botScoreText;
    [SerializeField] Text playerSet1ScoreText;
    [SerializeField] Text botSet1ScoreText;
    [SerializeField] Text playerSet2ScoreText;
    [SerializeField] Text botSet2ScoreText;
    [SerializeField] Text playerSet3ScoreText;
    [SerializeField] Text botSet3ScoreText;
    [SerializeField] Text winnerText; 

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

    private void Start()
    {
        initialPos = transform.position;
        playerScoreIndex = 0;
        botScoreIndex = 0;
        rb = GetComponent<Rigidbody>();

        
        player = FindObjectOfType<Player>();

        
        UpdateScores();
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
            if (touch.phase == TouchPhase.Began)
            {
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    
                    lastTapTime = 0; 
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
                UpdateScore();
                playing = false;
                UpdateScores(); 
                CheckForMatchWinner(); 
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

    private void UpdateScore()
    {
        if (hitter == "player")
        {
            if (playerScoreIndex < 3) 
            {
                playerScoreIndex++;
            }
            else 
            {
                SaveSetScore();
                playerScoreIndex = 0; 
                botScoreIndex = 0;
            }
        }
        else if (hitter == "bot")
        {
            if (botScoreIndex < 3) 
            {
                botScoreIndex++;
            }
            else 
            {
                SaveSetScore();
                playerScoreIndex = 0; 
                botScoreIndex = 0;
            }
        }
    }

    private void SaveSetScore()
    {
       
        if (playerSets == 0)
        {
            playerSet1ScoreText.text = tennisScores[playerScoreIndex].ToString();
            botSet1ScoreText.text = tennisScores[botScoreIndex].ToString();
        }
        else if (playerSets == 1)
        {
            playerSet2ScoreText.text = tennisScores[playerScoreIndex].ToString();
            botSet2ScoreText.text = tennisScores[botScoreIndex].ToString();
        }
        else if (playerSets == 2)
        {
            playerSet3ScoreText.text = tennisScores[playerScoreIndex].ToString();
            botSet3ScoreText.text = tennisScores[botScoreIndex].ToString();
        }

        if (playerScoreIndex == 0) playerSets++;
        if (botScoreIndex == 0) botSets++;
    }

   private void CheckForMatchWinner()
 {
    
    if (playerSets >= setsToWin && playerSets > botSets)
    {
        DisplayWinner("Bot");
    }
    else if (botSets >= setsToWin && botSets > playerSets)
    {
        DisplayWinner("Player");
    }
    
    else if (playerSets == 3 || botSets == 3)
    {
        if (playerSets > botSets)
        {
            DisplayWinner("Bot");
        }
        else if (botSets > playerSets)
        {
            DisplayWinner("{Player}");
        }
    }
 }

 private void DisplayWinner(string winner)
 {
    if (winnerText != null)
    {
        winnerText.text = winner + " Wins!";
    }
    
    playing = false;
 }


    void UpdateScores()
    {
        
        if (playerScoreText != null && botScoreText != null)
        {
            playerScoreText.text = "Player : " + tennisScores[playerScoreIndex];
            botScoreText.text = "Bot : " + tennisScores[botScoreIndex];
        }
    }
}
