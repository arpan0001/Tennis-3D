using UnityEngine;

public class OrientationCanvasSwitcher : MonoBehaviour
{
    public GameObject portraitCanvas;
    public GameObject landscapeCanvas;

    private ScreenOrientation lastOrientation;

    void Start()
    {
        UpdateCanvasBasedOnPlatformAndOrientation();
    }

    void Update()
    {
        // If running on a PC platform, don't change canvases based on orientation
        if (IsRunningOnPC())
            return;

        if (Screen.orientation != lastOrientation)
        {
            UpdateCanvasBasedOnPlatformAndOrientation();
        }
    }

    private void UpdateCanvasBasedOnPlatformAndOrientation()
    {
        lastOrientation = Screen.orientation;

        if (IsRunningOnPC())
        {
            // For PC, you could set one canvas as default or manage it differently
            portraitCanvas.SetActive(false);
            landscapeCanvas.SetActive(true);
        }
        else
        {
            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                portraitCanvas.SetActive(true);
                landscapeCanvas.SetActive(false);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                portraitCanvas.SetActive(false);
                landscapeCanvas.SetActive(true);
            }
        }
    }

    // Checks if the current platform is a PC (Windows, Mac, Linux)
    private bool IsRunningOnPC()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer ||
               Application.platform == RuntimePlatform.OSXPlayer ||
               Application.platform == RuntimePlatform.LinuxPlayer;
    }
}
