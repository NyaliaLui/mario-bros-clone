using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shows the on-screen touch controls only on touch-capable devices
/// (mobile builds or any platform with an active touchscreen).
/// </summary>
public class TouchControlsVisibility : MonoBehaviour
{
    [SerializeField] private GameObject container;

    void Awake()
    {
        bool touch = Application.isMobilePlatform || Touchscreen.current != null;
        if (container != null) container.SetActive(touch);
    }
}
