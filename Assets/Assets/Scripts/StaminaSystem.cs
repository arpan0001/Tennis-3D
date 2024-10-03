using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;    
    public Slider staminaSlider2;   
    public Slider staminaSlider3;   
    //public float maxStamina = 100f;
    //public float currentStamina;
    public float staminaDrainRate = 10f; 
    public float staminaRegenRate = 20f; 

    private bool canMove = true;
    private ReactToUnity reactToUnity;

    private void Start()
    {
        reactToUnity = ReactToUnity.instance; 

        ReactToUnity.OnUpdateEnergy += SetStamina;
        ReactToUnity.OnOutOfEnergy += SetStamina;

        ReactToUnity.instance._Energy = ReactToUnity.instance._maxEnergy;
        
        staminaSlider.maxValue = ReactToUnity.instance._maxEnergy;
        staminaSlider.value = ReactToUnity.instance._Energy;

        staminaSlider2.maxValue = ReactToUnity.instance._maxEnergy;
        staminaSlider2.value = ReactToUnity.instance._Energy;

        staminaSlider3.maxValue = ReactToUnity.instance._maxEnergy;
        staminaSlider3.value = ReactToUnity.instance._Energy;
    }

    private void Update()
    {
        
    }

    public bool CanMove()
    {
        return canMove && ReactToUnity.instance._Energy > 0;
    }

    public void DrainStamina(float stamina)
    {
        reactToUnity?.UseEnergy_Unity(Energy:(int)stamina);
    }

    public void SetStamina()
    {
        staminaSlider.value = ReactToUnity.instance._Energy;
        staminaSlider2.value = ReactToUnity.instance._Energy;
        staminaSlider3.value = ReactToUnity.instance._Energy; 
        if (ReactToUnity.instance._Energy <= 0)
        {
            canMove = false;
            
        }

    }

    // public void RegenerateStamina()
    // {
    //     if (currentStamina < maxStamina)
    //     {
    //         currentStamina += staminaRegenRate * Time.deltaTime;

    //         if (currentStamina >= maxStamina)
    //         {
    //             currentStamina = maxStamina;
    //             canMove = true;
    //         }
    //     }
    // }
}
