using UnityEngine;

public class ServeManager : MonoBehaviour
{
    [SerializeField] private Transform serveRight;  // Right serve position for player
    [SerializeField] private Transform serveLeft;   // Left serve position for player
    [SerializeField] public Transform botServePosition;  // Serve position for bot
    [SerializeField] private ScoreManager scoreManager;  // Reference to ScoreManager

    private bool servedRight = true;  // Tracks which side was served last
    private bool isBotServing = false;  // Tracks whether it's the bot's serve

    public bool IsBotServing => isBotServing; // Public property to check if the bot is serving

    private void Start()
    {
        // Ensure we have a reference to the ScoreManager
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }
        
        // Subscribe to the set over event from ScoreManager
        scoreManager.OnSetOver += HandleSetOver;
    }

    // Method to be called when a set is over
    private void HandleSetOver()
    {
        // Alternate between bot and player serving after each set
        isBotServing = !isBotServing;
    }

    // Method to get the next serve position
    public Vector3 GetNextServePosition()
    {
        if (isBotServing)
        {
            // Bot serve, no need to alternate between right/left
            return botServePosition.position;
        }
        else
        {
            // Player serve alternates between right and left sides
            if (servedRight)
            {
                servedRight = false;
                return serveRight.position + new Vector3(0.2f, 1, 0);
            }
            else
            {
                servedRight = true;
                return serveLeft.position + new Vector3(0.2f, 1, 0);
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to avoid memory leaks
        scoreManager.OnSetOver -= HandleSetOver;
    }
}
