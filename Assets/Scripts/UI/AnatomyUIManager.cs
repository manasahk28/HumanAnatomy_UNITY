using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HumanAnatomy
{
    public class AnatomyUIManager : MonoBehaviour
    {
        [Header("Manager Reference")]
        public AnatomyExplorerManager explorerManager;

        [Header("UI Panels")]
        public GameObject infoPanel;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI scientificText;
        public TextMeshProUGUI systemBadgeText;
        public TextMeshProUGUI functionText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI factsText;

        [Header("Buttons")]
        public Button focusButton;
        public Button isolateButton;
        public Button closeInfoButton;
        public Button explodedButton;
        public Button resetCamButton;
        public Button silhouetteToggleButton;

        private void Start()
        {
            if (explorerManager == null)
            {
                explorerManager = AnatomyExplorerManager.Instance;
            }

            if (explorerManager != null)
            {
                explorerManager.OnPartSelected += DisplayPartInfo;
                explorerManager.OnPartDeselected += HidePartInfo;
            }

            // Wire up buttons if assigned
            if (focusButton != null) focusButton.onClick.AddListener(() => explorerManager?.FocusOnSelected());
            if (isolateButton != null) isolateButton.onClick.AddListener(() => explorerManager?.IsolateCurrentSelected());
            if (closeInfoButton != null) closeInfoButton.onClick.AddListener(() => explorerManager?.DeselectPart());
            if (explodedButton != null) explodedButton.onClick.AddListener(() => explorerManager?.ToggleExplodedView());
            if (resetCamButton != null) resetCamButton.onClick.AddListener(() => explorerManager?.ResetCamera());

            HidePartInfo();
        }

        private void OnDestroy()
        {
            if (explorerManager != null)
            {
                explorerManager.OnPartSelected -= DisplayPartInfo;
                explorerManager.OnPartDeselected -= HidePartInfo;
            }
        }

        public void DisplayPartInfo(AnatomyPart part)
        {
            if (infoPanel != null) infoPanel.SetActive(true);
            if (part == null || part.data == null) return;

            if (titleText != null) titleText.text = part.data.partName;
            if (scientificText != null) scientificText.text = $"({part.data.scientificName})";
            if (systemBadgeText != null) systemBadgeText.text = $"{part.data.systemType} System";
            if (functionText != null) functionText.text = $"<b>Function:</b> {part.data.keyFunction}";
            if (descriptionText != null) descriptionText.text = part.data.description;

            if (factsText != null)
            {
                if (part.data.clinicalFacts != null && part.data.clinicalFacts.Length > 0)
                {
                    string factsStr = "<b>Clinical Facts:</b>\n";
                    foreach (var f in part.data.clinicalFacts)
                    {
                        factsStr += $"- {f}\n";
                    }
                    factsText.text = factsStr;
                }
                else
                {
                    factsText.text = "";
                }
            }
        }

        public void HidePartInfo()
        {
            if (infoPanel != null) infoPanel.SetActive(false);
        }

        public void OnFilterButtonClick(int systemIndex)
        {
            if (explorerManager != null)
            {
                explorerManager.ApplyFilter((AnatomySystemType)systemIndex);
            }
        }
    }
}

