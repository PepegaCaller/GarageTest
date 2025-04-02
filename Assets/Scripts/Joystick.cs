using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle;
    public RectTransform background;
    public float handleLimit = 1f;  

    private Vector2 _inputVector = Vector2.zero;
    private Vector2 _startPosition;

    void Start()
    {
        _startPosition = handle.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( background,
                                                                 eventData.position,
                                                                 eventData.pressEventCamera,
                                                                 out position);

        position = Vector2.ClampMagnitude(position, handleLimit * (background.sizeDelta.x / 2));
        handle.anchoredPosition = position;
        _inputVector = position / (background.sizeDelta.x / 2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick();
    }

    public Vector2 GetInput()
    {
        return _inputVector;
    }

    public void ResetJoystick()
    {
        handle.anchoredPosition = Vector2.zero;
        _inputVector = Vector2.zero;
    }

}