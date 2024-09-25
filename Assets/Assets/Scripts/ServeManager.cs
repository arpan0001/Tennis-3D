using UnityEngine;

public class ServeManager : MonoBehaviour
{
    [SerializeField] private Transform serveRight;  
    [SerializeField] private Transform serveLeft;   
    [SerializeField] public Transform botServePosition;  
    [SerializeField] private ScoreManager scoreManager;  

    private bool servedRight = true;  
    private bool isBotServing = false;  

    public bool IsBotServing => isBotServing; 

    private void Start()
    {
        
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }
        
        
        scoreManager.OnSetOver += HandleSetOver;
    }

    
    private void HandleSetOver()
    {
        
        isBotServing = !isBotServing;
    }

    
    public Vector3 GetNextServePosition()
    {
        if (isBotServing)
        {
            
            return botServePosition.position;
        }
        else
        {
            
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
        
        scoreManager.OnSetOver -= HandleSetOver;
    }
}
