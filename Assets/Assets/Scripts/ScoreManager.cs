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

    private ReactToUnity reactToUnity;

    void Start()
    {
        reactToUnity = ReactToUnity.instance;
        ReactToUnity.OnUpdateQuizScore += UpdateQuizScore; // Subscribe to the event
        tryAgainButton.gameObject.SetActive(false);
        tryAgainButton.onClick.AddListener(TryAgain);
        UpdateUI();
    }

    public void UpdateScore(string hitter, bool netCollision = false)
    {
        if (netCollision)
        {
            if (hitter == "player")
            {
                UpdateBotScore();
            }
            else if (hitter == "bot")
            {
                UpdatePlayerScore();
            }
        }
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

        // Call energy usage after each score
        reactToUnity.UseEnergy_Unity(1);

        UpdateUI();
        CheckForWinner();
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
       
        reactToUnity.SetEnergy_Unity(reactToUnity._maxEnergy);  // Reset energy

        UpdateUI();
        tryAgainButton.gameObject.SetActive(false);
        winnerText.text = "";
    }

    public void UpdateQuizScore(int amount)
    {
        if (amount > 0) // Check if the amount is greater than 0
        {
            playerScore += amount;
            UpdateUI();
        }
    }
}
