using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class ScoreManager : MonoBehaviour
{
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
                                           playerSets == 1 ? playerSet2ScoreTexts : playerSet3ScoreTexts;

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
    }

    private void ResetScores()
    {
        playerScoreIndex = 0;
        botScoreIndex = 0;
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
}
