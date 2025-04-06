using System;
using QFramework;
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

    [SerializeField]
    private Transform hideTrans;

    private CanvasGroup cg;
    private CanvasGroup interactionCg;
    private LayoutElement subInteractElement;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        interactionCg = interactButton.transform.parent.GetComponent<CanvasGroup>();
        subInteractElement = subInteractButton.GetComponent<LayoutElement>();
        DisableInteractButton();

#if UNITY_ANDROID || UNITY_IOS
        EventRegister();
        ActionBinding();
        Show();
#else
        Hide();
#endif
    }

#if UNITY_ANDROID || UNITY_IOS
    private void OnDestroy()
    {
        ActionUnbinding();
    }
#endif

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<ShowInteractHintEvent>(OnHintShow).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<HideInteractHintEvent>(OnHintHide).UnRegisterWhenGameObjectDestroyed(this);
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

    private void OnHintShow(ShowInteractHintEvent @event)
    {
        EnableInteractButton(CheckIsBothButton(@event.hint));
    }

    private void OnHintHide(HideInteractHintEvent @event)
    {
        DisableInteractButton();
    }

    private bool CheckIsBothButton(ControlSchemeHint hint)
    {
        if(hint.transform.GetChild(0).childCount == 1) return false;
        return true;
    }

    public void EnableInteractButton(bool bothButton)
    {
        if(bothButton)
        {
            ShowSubInteractButton();
        }
        else
        {
            HideSubInteractButton();
        }
        interactionCg.Show();
    }

    public void DisableInteractButton()
    {
        interactionCg.Hide();
        HideSubInteractButton();
    }

    private void ShowSubInteractButton()
    {
        subInteractElement.ignoreLayout = false;
    }

    private void HideSubInteractButton()
    {
        subInteractElement.ignoreLayout = true;
        subInteractButton.transform.position = hideTrans.position;
    }

    public void Show()
    {
        cg.Show();
    }

    public void Hide()
    {
        cg.Hide();
    }
}
