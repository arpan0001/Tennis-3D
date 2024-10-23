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
            // Player hit the net, bot should score
            UpdateBotScore();
        }
        else if (hitter == "bot")
        {
            // Bot hit the net, player should score
            UpdatePlayerScore();
        }
    }
    else
    {
        if (hitter == "player")
        {
            // Player wins the point
            UpdatePlayerScore();
        }
        else if (hitter == "bot")
        {
            // Bot wins the point
            UpdateBotScore();
        }
    }

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
        
        reactToUnity?.OnGameOver();  // Notify ReactToUnity that the game is over

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
        UpdateUI();

        tryAgainButton.gameObject.SetActive(false);
        winnerText.text = "";
    }
}
