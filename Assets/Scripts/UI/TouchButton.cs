using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// An on-screen UI button that drives PlayerInputReader's virtual input.
/// Attach to a UI Image (raycastTarget on) under a Canvas with a GraphicRaycaster.
/// While pressed, the image tints to a lighter shade of its base color.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Action { Left, Right, Jump }
    [SerializeField] private Action action;
    [SerializeField, Range(0f, 1f)] private float pressedLighten = 0.4f;

    private PlayerInputReader reader;
    private Image image;
    private Color baseColor = Color.white;
    private Color pressedColor = Color.white;

    void Awake()
    {
        reader = FindAnyObjectByType<PlayerInputReader>();
        image = GetComponent<Image>();
        if (image != null)
        {
            baseColor = image.color;
            pressedColor = Color.Lerp(baseColor, Color.white, pressedLighten);
        }
    }

    public void OnPointerDown(PointerEventData e) { Set(true); }
    public void OnPointerUp(PointerEventData e) { Set(false); }
    void OnDisable() { Set(false); } // release when hidden so input doesn't stick

    private void Set(bool pressed)
    {
        if (image != null) image.color = pressed ? pressedColor : baseColor;
        if (reader == null) reader = FindAnyObjectByType<PlayerInputReader>();
        if (reader == null) return;
        switch (action)
        {
            case Action.Left: reader.SetTouchLeft(pressed); break;
            case Action.Right: reader.SetTouchRight(pressed); break;
            case Action.Jump: reader.SetTouchJump(pressed); break;
        }
    }
}
