using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/Floating On-Screen Stick")]
public class FloatingOnScreenStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 fixedResetPosition; 

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        m_JoystickTransform.anchoredPosition = m_PointerDownPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_DragPos);
        var delta = m_DragPos - m_PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        m_JoystickTransform.anchoredPosition = m_PointerDownPos + delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        m_JoystickTransform.anchoredPosition = fixedResetPosition;
        SendValueToControl(Vector2.zero);
    }

    private void Start()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;

        
        if (fixedResetPosition == Vector2.zero)
        {
            fixedResetPosition = m_StartPos; 
        }
    }

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [SerializeField]
    private float m_MovementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    [SerializeField]
    private RectTransform m_JoystickTransform;

    private Vector2 m_StartPos;
    private Vector2 m_PointerDownPos;
    private Vector2 m_DragPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}