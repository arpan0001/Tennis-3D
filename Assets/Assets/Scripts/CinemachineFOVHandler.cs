using UnityEngine;
using Cinemachine;

public class CinemachineFOVHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;

    [Header("FOV Settings")]
    [SerializeField] private float landscapeFOV = 60f; // FOV for landscape mode
    [SerializeField] private float portraitFOV = 50f;  // FOV for portrait mode

    private bool isLandscape;

    void Start()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        }
        
        UpdateFOV(); // Set the initial FOV based on the current orientation
    }

    void Update()
    {
        CheckOrientationChange();
    }

    private void CheckOrientationChange()
    {
        bool currentIsLandscape = Screen.width > Screen.height;

        if (currentIsLandscape != isLandscape)
        {
            isLandscape = currentIsLandscape;
            UpdateFOV();
        }
    }

    private void UpdateFOV()
    {
        if (isLandscape)
        {
            cinemachineCamera.m_Lens.FieldOfView = landscapeFOV;
        }
        else
        {
            cinemachineCamera.m_Lens.FieldOfView = portraitFOV;
        }

        Debug.Log($"Screen Orientation: {(isLandscape ? "Landscape" : "Portrait")} | FOV: {cinemachineCamera.m_Lens.FieldOfView}");
    }
}
