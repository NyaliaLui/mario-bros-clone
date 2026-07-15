using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An on-screen UI button that drives PlayerInputReader's virtual input.
/// Attach to a UI Image (raycastTarget on) under a Canvas with a GraphicRaycaster.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Action { Left, Right, Jump }
    [SerializeField] private Action action;

    private PlayerInputReader reader;

    void Awake() { reader = FindFirstObjectByType<PlayerInputReader>(); }

    public void OnPointerDown(PointerEventData e) { Set(true); }
    public void OnPointerUp(PointerEventData e) { Set(false); }
    void OnDisable() { Set(false); } // release when hidden so input doesn't stick

    private void Set(bool pressed)
    {
        if (reader == null) reader = FindFirstObjectByType<PlayerInputReader>();
        if (reader == null) return;
        switch (action)
        {
            case Action.Left: reader.SetTouchLeft(pressed); break;
            case Action.Right: reader.SetTouchRight(pressed); break;
            case Action.Jump: reader.SetTouchJump(pressed); break;
        }
    }
}
