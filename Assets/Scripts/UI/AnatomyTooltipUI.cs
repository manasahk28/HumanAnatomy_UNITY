using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace HumanAnatomy
{
    public class AnatomyTooltipUI : MonoBehaviour
    {
        public static AnatomyTooltipUI Instance { get; private set; }

        [Header("UI Elements")]
        public RectTransform tooltipRect;
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI systemText;
        public TextMeshProUGUI functionText;

        [Header("Settings")]
        public Vector2 cursorOffset = new Vector2(18f, -18f);
        public float smoothSpeed = 16f;
        public float fadeSpeed = 8f;

        private bool isVisible = false;
        private Vector2 targetScreenPos;
        private Canvas rootCanvas;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (tooltipRect == null) tooltipRect = GetComponent<RectTransform>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            rootCanvas = GetComponentInParent<Canvas>();

            HideImmediate();
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 mousePos = mouse.position.ReadValue();

            // Smooth position tracking
            if (isVisible)
            {
                targetScreenPos = mousePos + cursorOffset;
                ClampToScreen();
                tooltipRect.position = Vector2.Lerp(tooltipRect.position, targetScreenPos, Time.deltaTime * smoothSpeed);
            }

            // Smooth Alpha Fade
            float targetAlpha = isVisible ? 1f : 0f;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            }
        }

        public void ShowTooltip(AnatomyPart part)
        {
            if (part == null || part.data == null) return;

            if (nameText != null) nameText.text = $"<b>{part.data.partName}</b>";
            if (systemText != null)
            {
                Color badgeCol = GetSystemColor(part.data.systemType);
                string hexCol = ColorUtility.ToHtmlStringRGB(badgeCol);
                systemText.text = $"<color=#{hexCol}>●</color> {part.data.systemType} System";
            }
            if (functionText != null)
            {
                functionText.text = string.IsNullOrEmpty(part.data.keyFunction) ? part.data.scientificName : part.data.keyFunction;
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                tooltipRect.position = mouse.position.ReadValue() + cursorOffset;
            }

            isVisible = true;
        }

        public void HideTooltip()
        {
            isVisible = false;
        }

        public void HideImmediate()
        {
            isVisible = false;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }

        private void ClampToScreen()
        {
            if (rootCanvas == null) return;
            Vector2 size = tooltipRect.sizeDelta;
            float minX = 10f;
            float maxX = Screen.width - size.x - 10f;
            float minY = size.y + 10f;
            float maxY = Screen.height - 10f;

            targetScreenPos.x = Mathf.Clamp(targetScreenPos.x, minX, maxX);
            targetScreenPos.y = Mathf.Clamp(targetScreenPos.y, minY, maxY);
        }

        private Color GetSystemColor(AnatomySystemType systemType)
        {
            switch (systemType)
            {
                case AnatomySystemType.Skeletal:
                    return new Color(0.35f, 0.75f, 1.0f); // Sky Blue
                case AnatomySystemType.Muscular:
                    return new Color(1.0f, 0.35f, 0.35f); // Crimson
                case AnatomySystemType.Nervous:
                    return new Color(1.0f, 0.85f, 0.25f); // Electric Gold
                case AnatomySystemType.Circulatory:
                    return new Color(1.0f, 0.20f, 0.35f); // Vivid Ruby / Crimson
                default:
                    return Color.white;
            }
        }
    }
}

