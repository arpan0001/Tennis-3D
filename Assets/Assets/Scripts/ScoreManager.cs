using UnityEngine;
using TMPro; // Import the TextMeshPro namespace
using UnityEngine.UI; // Import for Button functionality

public class ScoreManager : MonoBehaviour
{
    public delegate void SetOverAction();
    public event SetOverAction OnSetOver;  // Event to notify ServeManager when a set is over

    int[] tennisScores = { 0, 15, 30, 40 };
    int playerScoreIndex = 0;
    int botScoreIndex = 0;

    int playerSets = 0;
    int botSets = 0;
    int setsToWin = 2;

    // Arrays to hold TextMeshProUGUI references for different orientations
    [SerializeField] TextMeshProUGUI[] playerScoreTexts;
    [SerializeField] TextMeshProUGUI[] botScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet1ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet1ScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet2ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet2ScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet3ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet3ScoreTexts;
    [SerializeField] TextMeshProUGUI[] winnerTexts;

    // References to the Try Again buttons
    [SerializeField] Button tryAgainButton1; // First Try Again button
    [SerializeField] Button tryAgainButton2; // Second Try Again button

    void Start()
    {
        // Hide both Try Again buttons at the start of the game
        tryAgainButton1.gameObject.SetActive(false);
        tryAgainButton2.gameObject.SetActive(false);

        // Add listeners for both buttons to trigger TryAgain method
        tryAgainButton1.onClick.AddListener(TryAgain);
        tryAgainButton2.onClick.AddListener(TryAgain);
    }

    public void UpdateScore(string hitter, bool netCollision = false)
    {
        if (netCollision)
        {
            // If the ball hits the net, the score goes to the opponent
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
            // Regular score update logic
            if (hitter == "player")
            {
                UpdatePlayerScore();
            }
            else if (hitter == "bot")
            {
                UpdateBotScore();
            }
        }

        UpdateUI();
        CheckForMatchWinner();
    }

    private void UpdatePlayerScore()
    {
        if (playerScoreIndex < 3)
        {
            playerScoreIndex++;
        }
        else
        {
            // Player wins the game, save set score and reset scores
            SaveSetScore("player");
            ResetScores();
        }
    }

    private void UpdateBotScore()
    {
        if (botScoreIndex < 3)
        {
            botScoreIndex++;
        }
        else
        {
            // Bot wins the game, save set score and reset scores
            SaveSetScore("bot");
            ResetScores();
        }
    }

    private void SaveSetScore(string winner)
    {
        TextMeshProUGUI[] playerSetTexts = playerSets == 0 ? playerSet1ScoreTexts :
                                           playerSets == 1 ? playerSet2ScoreTexts :
                                           playerSets == 2 ? playerSet3ScoreTexts : playerSet3ScoreTexts;

        TextMeshProUGUI[] botSetTexts = playerSets == 0 ? botSet1ScoreTexts :
                                        playerSets == 1 ? botSet2ScoreTexts : botSet3ScoreTexts;

        // Update the UI with the current score for both players and bot
        UpdateAllUI(playerSetTexts, tennisScores[playerScoreIndex].ToString());
        UpdateAllUI(botSetTexts, tennisScores[botScoreIndex].ToString());

        // Increment sets based on who won
        if (winner == "player")
        {
            playerSets++;
        }
        else if (winner == "bot")
        {
            botSets++;
        }

        // Trigger the OnSetOver event to notify ServeManager
        OnSetOver?.Invoke();
    }

    private void CheckForMatchWinner()
    {
        if (playerSets >= setsToWin && playerSets > botSets)
        {
            DisplayWinner("Player");
        }
        else if (botSets >= setsToWin && botSets > playerSets)
        {
            DisplayWinner("Bot");
        }
    }

    private void DisplayWinner(string winner)
    {
        UpdateAllUI(winnerTexts, winner + " Wins!");

        // Show both Try Again buttons
        tryAgainButton1.gameObject.SetActive(true);
        tryAgainButton2.gameObject.SetActive(true);
    }

    private void ResetScores()
    {
        playerScoreIndex = 0;
        botScoreIndex = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        string playerScore = "Player: " + tennisScores[playerScoreIndex];
        string botScore = "Bot: " + tennisScores[botScoreIndex];

        UpdateAllUI(playerScoreTexts, playerScore);
        UpdateAllUI(botScoreTexts, botScore);
    }

    private void UpdateAllUI(TextMeshProUGUI[] uiElements, string text)
    {
        foreach (TextMeshProUGUI uiElement in uiElements)
        {
            if (uiElement != null)
            {
                uiElement.text = text;
            }
        }
    }

    // Method to reset all scores and start a new game
    public void TryAgain()
    {
        // Reset scores
        playerSets = 0;
        botSets = 0;
        playerScoreIndex = 0;
        botScoreIndex = 0;

        // Update the UI
        UpdateUI();
        
        // Hide both Try Again buttons
        tryAgainButton1.gameObject.SetActive(false);
        tryAgainButton2.gameObject.SetActive(false);

        // Hide the winner text
        ClearWinnerText();

        // Optionally reset the set score UI as well
        ResetSetScoresUI();
    }

    private void ClearWinnerText()
    {
        // Clear all winner texts to hide the message
        UpdateAllUI(winnerTexts, "");
    }

    private void ResetSetScoresUI()
    {
        // Reset all set score texts
        UpdateAllUI(playerSet1ScoreTexts, "0");
        UpdateAllUI(botSet1ScoreTexts, "0");
        UpdateAllUI(playerSet2ScoreTexts, "0");
        UpdateAllUI(botSet2ScoreTexts, "0");
        UpdateAllUI(playerSet3ScoreTexts, "0");
        UpdateAllUI(botSet3ScoreTexts, "0");
    }
}
