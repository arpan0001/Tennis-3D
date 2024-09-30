using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;    
    public Slider staminaSlider2;   
    public Slider staminaSlider3;   // New stamina slider
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 10f; 
    public float staminaRegenRate = 20f; 

    private bool canMove = true;
    private ReactToUnity reactToUnity;

    private void Start()
    {
        reactToUnity = ReactToUnity.instance; 

        // Subscribe to the energy update event
        ReactToUnity.OnUpdateEnergy += RegenerateStamina;

        currentStamina = maxStamina;
        
        // Initialize sliders
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;

        staminaSlider2.maxValue = maxStamina;
        staminaSlider2.value = currentStamina;

        staminaSlider3.maxValue = maxStamina; // Initialize the third slider
        staminaSlider3.value = currentStamina; // Set its initial value
    }

    private void Update()
    {
        // Update stamina slider values
        staminaSlider.value = currentStamina;
        staminaSlider2.value = currentStamina;
        staminaSlider3.value = currentStamina; // Update the third slider

        // Check if stamina is depleted
        if (currentStamina <= 0)
        {
            canMove = false;
        }

        // Regenerate stamina while key is held
        if (Input.GetKey(KeyCode.I) && currentStamina < maxStamina)
        {
            RegenerateStamina();
        }
    }

    public bool CanMove()
    {
        return canMove && currentStamina > 0;
    }

    public void DrainStamina()
    {
        if (currentStamina > 0)
        {
            // Drain stamina over time
            currentStamina -= staminaDrainRate * Time.deltaTime;

            // Call UseEnergy_Unity to deduct energy
            reactToUnity?.UseEnergy_Unity((int)staminaDrainRate);
            
            // Check if stamina has run out
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canMove = false; 
            }
        }
    }

    public void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            // Regenerate stamina over time
            currentStamina += staminaRegenRate * Time.deltaTime;

            // Clamp stamina to max value
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                canMove = true;
            }
        }
    }
}
