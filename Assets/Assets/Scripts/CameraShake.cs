using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] private float shakeDuration = 0.5f; // Duration of the shake
    [SerializeField] private float shakeAmplitude = 1f; // Amplitude of the shake
    [SerializeField] private float shakeFrequency = 2f; // Frequency of the shake

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // Add noise component if not already present
        if (noise == null)
        {
            noise = virtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        noise.m_AmplitudeGain = 0f; // Start with no shake
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    private System.Collections.IEnumerator Shake()
    {
        noise.m_AmplitudeGain = shakeAmplitude; 
        noise.m_FrequencyGain = shakeFrequency; 

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null; 
        }

        noise.m_AmplitudeGain = 0f; 
    }
}
