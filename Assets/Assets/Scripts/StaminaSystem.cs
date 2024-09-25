using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;    
    public Slider staminaSlider2;   
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 10f; 
    public float staminaRegenRate = 20f; 

    private bool canMove = true;

    private void Start()
    {
        currentStamina = maxStamina;

        
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
        staminaSlider2.maxValue = maxStamina;
        staminaSlider2.value = currentStamina;
    }

    private void Update()
    {
       
        staminaSlider.value = currentStamina;
        staminaSlider2.value = currentStamina;

        if (currentStamina <= 0)
        {
            canMove = false;
        }

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
            currentStamina -= staminaDrainRate * Time.deltaTime;
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
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                canMove = true;
            }
        }
    }
}
