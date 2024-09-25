using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{
    float speed = 40;
    Animator animator;
    public Transform ball;
    public Transform aimTarget;

    public Transform[] targets;

    float force = 13;
    Vector3 targetPosition;

    ShotManager shotManager;
    ServeManager serveManager; 
    Ball ballScript;           

    public float detectionRange = 10f; 

    private int topspinCount = 0; 
    private int shotCounter = 0;  

    public Transform botServePosition; 
    public float serveTossHeight = 10f; 
    public float serveDelay = 1.5f; 
    private bool isServing = false; 

    
    private SoundManager soundManager;

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        serveManager = FindObjectOfType<ServeManager>(); 
        ballScript = ball.GetComponent<Ball>(); 

        
        soundManager = FindObjectOfType<SoundManager>(); 
    }

    void Update()
    {
        
        botServePosition.gameObject.SetActive(!serveManager.IsBotServing);

        if (IsBallWithinRange() && !isServing) 
        {
            Move();
        }

        
        if (IsBallAtServePosition() && !isServing && serveManager.IsBotServing)
        {
            StartCoroutine(HandleBotServe()); 
        }
    }

    bool IsBallWithinRange()
    {
        float distanceToBall = Vector3.Distance(transform.position, ball.position);
        return distanceToBall <= detectionRange; 
    }

    bool IsBallAtServePosition()
    {
        
        float distanceToServePosition = Vector3.Distance(ball.position, botServePosition.position);
        return distanceToServePosition < 0.5f; 
    }

    void Move()
    {
        
        targetPosition.x = ball.position.x;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    Vector3 PickTarget()
    {
        int randomValue = Random.Range(0, targets.Length);
        return targets[randomValue].position;
    }

    Shot PickShot()
    {
        
        if (topspinCount < 4)
        {
            topspinCount++;
            return shotManager.topSpin; 
        }
        else
        {
            topspinCount = 0; 
            return shotManager.flat; 
        }
    }

    
    IEnumerator HandleBotServe()
    {
        isServing = true; 

        
        targetPosition = botServePosition.position; 
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; 
        }

        
        ball.position = botServePosition.position; 
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, serveTossHeight, 0); 

        yield return new WaitForSeconds(serveDelay); 

        
        Vector3 ballDir = ball.position - transform.position;
        Shot currentShot = PickShot(); 
        Vector3 dir = PickTarget() - transform.position; 
        ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0); // Apply velocity to the ball

        
        if (soundManager != null)
        {
            soundManager.PlayHitSoundWithDelay(0f); 
        }

        
        if (ballDir.x >= 0)
        {
            animator.Play("forehand");
        }
        else
        {
            animator.Play("backhand");
        }

        ballScript.hitter = "bot"; 

        isServing = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !isServing) 
        {
            shotCounter++; 

            
            if (shotCounter == 7)
            {
                shotCounter = 0; 
                Debug.Log("Bot missed the ball!"); 
                return; 
            }

            
            Shot currentShot = PickShot(); 
            Vector3 dir = PickTarget() - transform.position; 
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0); // Apply velocity to the ball

            
            if (soundManager != null)
            {
                soundManager.PlayHitSoundWithDelay(0f); 
            }

            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand");
            }
            else
            {
                animator.Play("backhand");
            }

            ballScript.hitter = "bot"; 
        }
    }
}
