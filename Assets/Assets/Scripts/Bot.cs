using System.Collections;
using System.Collections.Generic;
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

    public float detectionRange = 10f; // Range within which the bot starts following the ball

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
    }

    void Update()
    {
        if (IsBallWithinRange()) // Only calculate position and speed if ball is within range
        {
            Move();
        }
    }

    bool IsBallWithinRange()
    {
        float distanceToBall = Vector3.Distance(transform.position, ball.position);
        return distanceToBall <= detectionRange; // Check if ball is within detection range
    }

    void Move()
    {
        // Calculate movement only if the ball is within range
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
        int randomValue = Random.Range(0, 2); // Updated range to avoid always picking topSpin
        if (randomValue == 0)
            return shotManager.topSpin;
        else
            return shotManager.flat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Shot currentShot = PickShot();
            Vector3 dir = PickTarget() - transform.position;
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand");
            }
            else
            {
                animator.Play("backhand");
            }

            ball.GetComponent<Ball>().hitter = "bot";
        }
    }
}
