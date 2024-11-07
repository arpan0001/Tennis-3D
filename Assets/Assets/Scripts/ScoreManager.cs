using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public delegate void GameOverAction();
    public event GameOverAction OnGameOver;

    public int playerScore = 0;
    public int botScore = 0;
    public int pointsToWin = 10;

    [SerializeField] TextMeshProUGUI playerScoreText;
    [SerializeField] TextMeshProUGUI botScoreText;
    [SerializeField] TextMeshProUGUI winnerText;

    [SerializeField] Button tryAgainButton;
    [SerializeField] GameObject toggleObject; 

    private ReactToUnity reactToUnity;

    void Start()
    {
        reactToUnity = ReactToUnity.instance;
        ReactToUnity.OnUpdateQuizScore += UpdateQuizScore; 
        tryAgainButton.gameObject.SetActive(false);
        tryAgainButton.onClick.AddListener(TryAgain);
        UpdateUI();
    }

  public void UpdateScore(string hitter, bool netCollision = false, bool outOfBounds = false)
 {
    // Check if the point is due to a net collision
    if (netCollision)
    {
        // Award point to the opponent of the hitter
        if (hitter == "player")
        {
            UpdateBotScore();
        }
        else if (hitter == "bot")
        {
            UpdatePlayerScore();
        }
    }
    // Check if the point is due to the ball going out of bounds
    else if (outOfBounds)
    {
        // Award point to the opponent of the hitter
        if (hitter == "player")
        {
            UpdateBotScore();
        }
        else if (hitter == "bot")
        {
            UpdatePlayerScore();
        }
    }
    // Otherwise, award point to the hitter for a successful play
    else
    {
        if (hitter == "player")
        {
            UpdatePlayerScore();
        }
        else if (hitter == "bot")
        {
            UpdateBotScore();
        }
    }

    // Consume energy for each score update
    reactToUnity.UseEnergy_Unity(1);

    // Update the game UI to reflect the new score
    UpdateUI();

    // Check if the game has a winner
    CheckForWinner();

    // Check for any additional score conditions
    CheckScoreConditions();
 }


    private void UpdatePlayerScore()
    {
        playerScore++;
    }

    private void UpdateBotScore()
    {
        botScore++;
    }

    private void CheckForWinner()
    {
        if (playerScore >= pointsToWin)
        {
            DisplayWinner("Player");
        }
        else if (botScore >= pointsToWin)
        {
            DisplayWinner("Bot");
        }
    }

    private void DisplayWinner(string winner)
    {
        winnerText.text = winner + " Wins!";
        
        reactToUnity?.OnGameOver();  

        tryAgainButton.gameObject.SetActive(true);
        OnGameOver?.Invoke();
    }

    private void UpdateUI()
    {
        playerScoreText.text = "Player: " + playerScore;
        botScoreText.text = "Bot: " + botScore;
    }

    public void TryAgain()
    {
        playerScore = 0;
        botScore = 0;
       
        reactToUnity.SetEnergy_Unity(reactToUnity._maxEnergy);  

        UpdateUI();
        tryAgainButton.gameObject.SetActive(false);
        winnerText.text = "";
    }

    private void CheckScoreConditions()
    {
        if (playerScore % 3 == 0 && playerScore != 0)
        {
            toggleObject.SetActive(true); 
        }
        if (botScore % 2 == 0 && botScore != 0)
        {
            toggleObject.SetActive(false); 
        }
    }

    public void UpdateQuizScore(int amount)
    {
        if (amount > 0) 
        {
            playerScore += amount;
            UpdateUI();
            CheckScoreConditions(); 
        }
    }
}
