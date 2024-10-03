using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public delegate void SetOverAction();
    public event SetOverAction OnSetOver; 

    public int Score; 


    int[] tennisScores = { 0, 15, 30, 40 };
    int playerScoreIndex = 0;
    int botScoreIndex = 0;

    int playerSets = 0;
    int botSets = 0;
    int setsToWin = 2;

    [SerializeField] TextMeshProUGUI[] playerScoreTexts;
    [SerializeField] TextMeshProUGUI[] botScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet1ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet1ScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet2ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet2ScoreTexts;
    [SerializeField] TextMeshProUGUI[] playerSet3ScoreTexts;
    [SerializeField] TextMeshProUGUI[] botSet3ScoreTexts;
    [SerializeField] TextMeshProUGUI[] winnerTexts;

    [SerializeField] Button tryAgainButton1; 
    [SerializeField] Button tryAgainButton2; 

    [SerializeField] Button tryAgainButton3; 

    
    private ReactToUnity reactToUnity;

    void Start()
    {
        reactToUnity = ReactToUnity.instance; 

        tryAgainButton1.gameObject.SetActive(false);
        tryAgainButton2.gameObject.SetActive(false);
        tryAgainButton3.gameObject.SetActive(false);

        tryAgainButton1.onClick.AddListener(TryAgain);
        tryAgainButton2.onClick.AddListener(TryAgain);
        tryAgainButton3.onClick.AddListener(TryAgain);
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

        UpdateAllUI(playerSetTexts, tennisScores[playerScoreIndex].ToString());
        UpdateAllUI(botSetTexts, tennisScores[botScoreIndex].ToString());

        if (winner == "player")
        {
            playerSets++;
        }
        else if (winner == "bot")
        {
            botSets++;
        }

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

        
        reactToUnity?.OnGameOver();

        tryAgainButton1.gameObject.SetActive(true);
        tryAgainButton2.gameObject.SetActive(true);
        tryAgainButton3.gameObject.SetActive(true);

        
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

    public void TryAgain()
    {
        playerSets = 0;
        botSets = 0;
        playerScoreIndex = 0;
        botScoreIndex = 0;

        UpdateUI();
        tryAgainButton1.gameObject.SetActive(false);
        tryAgainButton2.gameObject.SetActive(false);
        tryAgainButton3.gameObject.SetActive(false);

        ClearWinnerText();
        ResetSetScoresUI();
    }

    private void ClearWinnerText()
    {
        UpdateAllUI(winnerTexts, "");
    }

    private void ResetSetScoresUI()
    {
        UpdateAllUI(playerSet1ScoreTexts, "0");
        UpdateAllUI(botSet1ScoreTexts, "0");
        UpdateAllUI(playerSet2ScoreTexts, "0");
        UpdateAllUI(botSet2ScoreTexts, "0");
        UpdateAllUI(playerSet3ScoreTexts, "0");
        UpdateAllUI(botSet3ScoreTexts, "0");
    }
}
