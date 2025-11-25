using UnityEngine;
using UnityEngine.InputSystem;

public class BarrierInputHandler : MonoBehaviour
{
    [Header("Referencia al script del grupo")]
    public GroupMoveOnPress groupMoveScript;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Subir.performed += OnSubir;
    }

    private void OnDisable()
    {
        controls.Player.Subir.performed -= OnSubir;
        controls.Disable();
    }

    private void OnSubir(InputAction.CallbackContext context)
    {
        groupMoveScript?.TogglePosition();
    }
}
