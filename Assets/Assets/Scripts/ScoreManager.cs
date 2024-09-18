using UnityEngine;
using UnityEngine.UI;
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

    public void UpdateScore(string hitter)
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
                ResetScores();
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
                ResetScores();
            }
        }

        UpdateUI();
        CheckForMatchWinner();
    }

    private void SaveSetScore()
    {
        TextMeshProUGUI[] playerSetTexts = playerSets == 0 ? playerSet1ScoreTexts :
                                           playerSets == 1 ? playerSet2ScoreTexts : playerSet3ScoreTexts;

        TextMeshProUGUI[] botSetTexts = playerSets == 0 ? botSet1ScoreTexts :
                                        playerSets == 1 ? botSet2ScoreTexts : botSet3ScoreTexts;

        UpdateAllUI(playerSetTexts, tennisScores[playerScoreIndex].ToString());
        UpdateAllUI(botSetTexts, tennisScores[botScoreIndex].ToString());

        if (playerScoreIndex == 0) playerSets++;
        if (botScoreIndex == 0) botSets++;
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
