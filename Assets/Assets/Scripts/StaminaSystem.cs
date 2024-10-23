using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public Slider staminaSlider;    
    public Slider staminaSlider2;   
    public Slider staminaSlider3;   
   
    public float staminaDrainRate = 0.2f;  // Adjusted to slow down stamina drain
    public float staminaRegenRate = 50f; 

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
        // Optional: You can add stamina regeneration logic here if needed.
    }

    public bool CanMove()
    {
        return ReactToUnity.instance._Energy > 0;
    }

    public void DrainStamina(float stamina)
    {
        float actualStaminaDrain = stamina * staminaDrainRate;  // Drain stamina based on the defined rate
        reactToUnity?.UseEnergy_Unity(Energy: (int)actualStaminaDrain);
    }

    public void SetStamina()
    {
        staminaSlider.value = ReactToUnity.instance._Energy;
        staminaSlider2.value = ReactToUnity.instance._Energy;
        staminaSlider3.value = ReactToUnity.instance._Energy;
    }
}
