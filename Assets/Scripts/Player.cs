using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 moveVector;        // Stores joystick movement input
    public Transform aimTarget;
    float speed = 3f;          // Movement speed
    float force = 10f;         // Force applied to the ball
    bool hitting;              // Indicates if the player is hitting

    private void Update()
    {
        // Get keyboard input for movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Check if the player is hitting
        if (Input.GetKeyDown(KeyCode.F))
        {
            hitting = true;
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            hitting = false;
        }

        // Combine joystick and keyboard input
        Vector3 movementInput = new Vector3(moveVector.x + h, 0, moveVector.y + v);

        // Handle aimTarget movement when hitting
        if (hitting)
        {
            Vector3 aimMovement = new Vector3(movementInput.x, 0, 0) * speed * 2 * Time.deltaTime;
            aimTarget.Translate(aimMovement);  // Move aim target horizontally
        }
        // Handle player movement when not hitting
        else if (movementInput.x != 0 || movementInput.z != 0)
        {
            Vector3 movement = new Vector3(movementInput.x, 0, movementInput.z) * speed * Time.deltaTime;
            transform.Translate(movement);  // Move player using combined input
        }
    }

    // Handle collision with the ball
   private void OnTriggerEnter(Collider other)
 {
    if (other.CompareTag("Ball"))
    {
        Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();
        
        if (ballRigidbody != null)
        {
            // Calculate direction from the player to the aimTarget
            Vector3 dir = aimTarget.position - transform.position;
            // Apply force to the ball in the direction of the aimTarget, with an upward boost
            ballRigidbody.velocity = dir.normalized * force + new Vector3(0, 10, 0);
        }
    }
 }


    // New input system callback for joystick movement
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }
}
