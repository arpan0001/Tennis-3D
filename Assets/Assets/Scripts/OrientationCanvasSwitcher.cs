using UnityEngine;

public class OrientationCanvasSwitcher : MonoBehaviour
{
    public GameObject portraitCanvas;
    public GameObject landscapeCanvas;
    public GameObject pcCanvas; // New public GameObject for PC canvas

    private ScreenOrientation lastOrientation;

    void Start()
    {
        UpdateCanvasBasedOnPlatformAndOrientation();
    }

    void Update()
    {
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
            // Show the PC canvas and hide the others
            pcCanvas.SetActive(true);
            portraitCanvas.SetActive(false);
            landscapeCanvas.SetActive(false);
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

    private bool IsRunningOnPC()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer ||
               Application.platform == RuntimePlatform.OSXPlayer ||
               Application.platform == RuntimePlatform.LinuxPlayer;
    }
}
