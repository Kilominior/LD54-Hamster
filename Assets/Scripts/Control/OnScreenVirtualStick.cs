using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

public class OnScreenVirtualStick: MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private const string kDynamicOriginClickable = "DynamicOriginClickable";

    // 基础依赖
    private RectTransform m_CanvasRect;

    // 可调节参数
    [SerializeField]
    [Min(0)]
    private float m_MovementRange = 50;
    public float movementRange
    {
        get { return m_MovementRange;}
        set { m_MovementRange = value;}
    }

    [SerializeField]
    private VirtualStickMode m_Mode;
    public VirtualStickMode mode
    {
        get => m_Mode;
        set => m_Mode = value;
    }

    [SerializeField]
    [Min(0)]
    private float m_DynamicOriginRange = 100;
    public float dynamicOriginRange
    {
        get => m_DynamicOriginRange;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (m_DynamicOriginRange != value)
            {
                m_DynamicOriginRange = value;
                UpdateDynamicOriginClickableArea();
            }
        }
    }

    public enum VirtualStickMode
    {
        /// <summary>The control's center of origin is fixed in the scene.
        /// The control will begin un-actuated at it's centered position and then move relative to the press motion.</summary>
        RelativePositionWithStaticOrigin,

        /// <summary>The control's center of origin is fixed in the scene.
        /// The control may begin from an actuated position to ensure it is always tracking the current press position.</summary>
        ExactPositionWithStaticOrigin,

        /// <summary>The control's center of origin is determined by the initial press position.
        /// The control will begin unactuated at this center position and then track the current press position.</summary>
        ExactPositionWithDynamicOrigin
    }

    // 操作中回调
    /// <summary>
    /// 开始拖动手柄.
    /// </summary>
    public Action onDragBegin;
    /// <summary>
    /// 拖动手柄中，参数为手柄的推动量，在0~1之间.
    /// </summary>
    public Action<Vector2> onDraggingAmount;
    /// <summary>
    /// 拖动手柄中，参数为两次拖拽间手柄推动量的变化值，在0~1之间
    /// </summary>
    public Action<Vector2> onDraggingDelta;
    /// <summary>
    /// 结束拖动手柄.
    /// </summary>
    public Action onDragEnd;

    // 操作中参数
    // 摇杆的原始所在位置
    private Vector3 m_StartPos;
    // 本次开始拖拽时指针的位置
    private Vector2 m_PointerDownPos;
    // 本次拖拽中上一次记录的指针位置
    private Vector2 m_LastDragAmount;

    void Start()
    {
        m_CanvasRect = transform.parent?.GetComponentInParent<RectTransform>();


        m_StartPos = ((RectTransform)transform).anchoredPosition;
        if (m_Mode != VirtualStickMode.ExactPositionWithDynamicOrigin) return;
        m_PointerDownPos = m_StartPos;
        m_LastDragAmount = Vector2.zero;

        var dynamicOrigin = new GameObject(kDynamicOriginClickable, typeof(Image));
        dynamicOrigin.transform.SetParent(transform);
        var image = dynamicOrigin.GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0);
        var rectTransform = (RectTransform)dynamicOrigin.transform;
        rectTransform.sizeDelta = new Vector2(m_DynamicOriginRange * 2, m_DynamicOriginRange * 2);
        rectTransform.localScale = new Vector3(1, 1, 0);
        rectTransform.anchoredPosition3D = Vector3.zero;

        // image.sprite = SpriteUtilities.CreateCircleSprite(16, new Color32(255, 255, 255, 255));
        // image.alphaHitTestMinimumThreshold = 0.5f;
    }

    private void BeginInteraction(Vector2 pointerPosition, Camera uiCamera)
    {
        switch (m_Mode)
        {
            case VirtualStickMode.RelativePositionWithStaticOrigin:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasRect, pointerPosition, uiCamera, out m_PointerDownPos);
                break;
            case VirtualStickMode.ExactPositionWithStaticOrigin:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasRect, pointerPosition, uiCamera, out m_PointerDownPos);
                UpdateStickPosition(pointerPosition, uiCamera);
                break;
            case VirtualStickMode.ExactPositionWithDynamicOrigin:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasRect, pointerPosition, uiCamera, out var pointerDown);
                m_PointerDownPos = ((RectTransform)transform).anchoredPosition = pointerDown;
                break;
        }
        m_LastDragAmount = Vector2.zero;
        onDragBegin?.Invoke();
    }

    private void EndInteraction()
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        m_PointerDownPos = m_StartPos;
        m_LastDragAmount = Vector2.zero;
        onDragEnd?.Invoke();
    }

    private void UpdateStickPosition(Vector2 pointerPosition, Camera uiCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasRect, pointerPosition, uiCamera, out var position);
        var dragDistance = position - m_PointerDownPos;

        switch (m_Mode)
        {
            case VirtualStickMode.RelativePositionWithStaticOrigin:
                dragDistance = Vector2.ClampMagnitude(dragDistance, m_MovementRange);
                ((RectTransform)transform).anchoredPosition = (Vector2)m_StartPos + dragDistance;
                break;

            case VirtualStickMode.ExactPositionWithStaticOrigin:
                dragDistance = position - (Vector2)m_StartPos;
                dragDistance = Vector2.ClampMagnitude(dragDistance, m_MovementRange);
                ((RectTransform)transform).anchoredPosition = (Vector2)m_StartPos + dragDistance;
                break;

            case VirtualStickMode.ExactPositionWithDynamicOrigin:
                dragDistance = Vector2.ClampMagnitude(dragDistance, m_MovementRange);
                ((RectTransform)transform).anchoredPosition = m_PointerDownPos + dragDistance;
                break;
        }

        var dragAmount = new Vector2(dragDistance.x / m_MovementRange, dragDistance.y / m_MovementRange);
        onDraggingAmount?.Invoke(dragAmount);

        var deltaAmount = dragAmount - m_LastDragAmount;
        if(deltaAmount != Vector2.zero)
            onDraggingDelta?.Invoke(deltaAmount);
        m_LastDragAmount = dragAmount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateStickPosition(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BeginInteraction(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndInteraction();
    }

    private void UpdateDynamicOriginClickableArea()
    {
        var dynamicOriginTransform = transform.Find(kDynamicOriginClickable);
        if (dynamicOriginTransform)
        {
            var rectTransform = (RectTransform)dynamicOriginTransform;
            rectTransform.sizeDelta = new Vector2(m_DynamicOriginRange * 2, m_DynamicOriginRange * 2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = ((RectTransform)transform.parent).localToWorldMatrix;

        var startPos = ((RectTransform)transform).anchoredPosition;
        if (Application.isPlaying)
            startPos = m_StartPos;

        Gizmos.color = new Color32(84, 173, 219, 255);

        var center = startPos;
        if (Application.isPlaying && m_Mode == VirtualStickMode.ExactPositionWithDynamicOrigin)
            center = m_PointerDownPos;

        DrawGizmoCircle(center, m_MovementRange);

        if (m_Mode != VirtualStickMode.ExactPositionWithDynamicOrigin) return;

        Gizmos.color = new Color32(158, 84, 219, 255);
        DrawGizmoCircle(startPos, m_DynamicOriginRange);
    }

    private void DrawGizmoCircle(Vector2 center, float radius)
    {
        for (var i = 0; i < 32; i++)
        {
            var radians = i / 32f * Mathf.PI * 2;
            var nextRadian = (i + 1) / 32f * Mathf.PI * 2;
            Gizmos.DrawLine(
                new Vector3(center.x + Mathf.Cos(radians) * radius, center.y + Mathf.Sin(radians) * radius, 0),
                new Vector3(center.x + Mathf.Cos(nextRadian) * radius, center.y + Mathf.Sin(nextRadian) * radius, 0));
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(OnScreenVirtualStick))]
    internal class OnScreenStickEditor : UnityEditor.Editor
    {
        private AnimBool m_ShowDynamicOriginOptions;

        private SerializedProperty mode;
        private SerializedProperty m_MovementRange;
        private SerializedProperty m_DynamicOriginRange;

        public void OnEnable()
        {
            m_ShowDynamicOriginOptions = new AnimBool(false);

            mode = serializedObject.FindProperty(nameof(OnScreenVirtualStick.m_Mode));
            m_MovementRange = serializedObject.FindProperty(nameof(OnScreenVirtualStick.m_MovementRange));
            m_DynamicOriginRange = serializedObject.FindProperty(nameof(OnScreenVirtualStick.m_DynamicOriginRange));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_MovementRange);
            EditorGUILayout.PropertyField(mode);

            m_ShowDynamicOriginOptions.target = ((OnScreenVirtualStick)target).m_Mode ==
                VirtualStickMode.ExactPositionWithDynamicOrigin;
            if (EditorGUILayout.BeginFadeGroup(m_ShowDynamicOriginOptions.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_DynamicOriginRange);
                if (EditorGUI.EndChangeCheck())
                {
                    ((OnScreenVirtualStick)target).UpdateDynamicOriginClickableArea();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
