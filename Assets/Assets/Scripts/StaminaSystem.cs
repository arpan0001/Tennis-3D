using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 10f; // Stamina lost per second while moving
    public float staminaRegenRate = 20f; // Stamina regained per second when regenerating

    private bool canMove = true;

    private void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        // Update the slider UI to match the current stamina level
        staminaSlider.value = currentStamina;

        // Stop player movement when stamina reaches 0
        if (currentStamina <= 0)
        {
            canMove = false;
        }

        // Regenerate stamina while the "I" key is being held down
        if (Input.GetKey(KeyCode.I) && currentStamina < maxStamina)
        {
            RegenerateStamina();
        }
    }

    // Method to check if the player is allowed to move
    public bool CanMove()
    {
        return canMove && currentStamina > 0;
    }

    // Method to drain stamina
    public void DrainStamina()
    {
        if (currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canMove = false; // Player stops moving
            }
        }
    }

    // Method to regenerate stamina
    public void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                canMove = true; // Allow player movement once stamina regenerates
            }
        }
    }
}
