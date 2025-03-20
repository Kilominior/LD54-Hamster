using UnityEngine;
using UnityEngine.UI;

public class OnScreenControlPanel: MonoBehaviour
{
    [SerializeField]
    private PlayerInputHandler inputHandler;

    [SerializeField]
    private OnScreenVirtualStick moveStick;

    [SerializeField]
    private OnScreenVirtualStick aimStick;

    [SerializeField]
    private Button jumpButton;

    [SerializeField]
    private Button interactButton;

    [SerializeField]
    private Button subInteractButton;

    private void Start()
    {
        ActionBinding();
    }

    private void OnDestroy()
    {
        ActionUnbinding();
    }

    private void ActionBinding()
    {
        moveStick.onDraggingAmount += inputHandler.OnMovePerformed;
        moveStick.onDragEnd += inputHandler.OnMoveCanceled;

        aimStick.onDragBegin += inputHandler.OnAimTriggerPerformed;
        aimStick.onDraggingAmount += inputHandler.OnAimPerformedByPushAmount;
        aimStick.onDragEnd += inputHandler.OnAimTriggerCanceled;

        jumpButton.onClick.AddListener(inputHandler.OnJumpPerformed);
        interactButton.onClick.AddListener(inputHandler.OnInteractPerformed);
        subInteractButton.onClick.AddListener(inputHandler.OnSubInteractPerformed);
    }

    private void ActionUnbinding()
    {
        moveStick.onDraggingAmount -= inputHandler.OnMovePerformed;
        moveStick.onDragEnd -= inputHandler.OnMoveCanceled;

        aimStick.onDragBegin -= inputHandler.OnAimTriggerPerformed;
        aimStick.onDraggingAmount -= inputHandler.OnAimPerformedByPushAmount;
        aimStick.onDragEnd -= inputHandler.OnAimTriggerCanceled;
    }
}
