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

        [Header("Action Buttons")]
        public Button focusButton;
        public Button isolateButton;
        public Button closeInfoButton;
        public Button explodedButton;
        public Button resetCamButton;
        public Button silhouetteToggleButton;

        [Header("Enhanced UI Controls")]
        public TMP_InputField searchInputField;
        public Slider explodeSlider;
        public TextMeshProUGUI explodeValueText;
        public Button muteButton;
        public TextMeshProUGUI muteButtonText;

        [Header("System Toggle Buttons")]
        public Button toggleSkeletalButton;
        public Button toggleMuscularButton;
        public Button toggleNervousButton;
        public Image skeletalIndicator;
        public Image muscularIndicator;
        public Image nervousIndicator;

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
                explorerManager.OnExplodeProgressChanged += UpdateExplodeSliderValue;
            }

            // Wire up info panel action buttons
            if (focusButton != null) focusButton.onClick.AddListener(() => explorerManager?.FocusOnSelected());
            if (isolateButton != null) isolateButton.onClick.AddListener(() => explorerManager?.IsolateCurrentSelected());
            if (closeInfoButton != null) closeInfoButton.onClick.AddListener(() => explorerManager?.DeselectPart());
            if (explodedButton != null) explodedButton.onClick.AddListener(() => explorerManager?.ToggleExplodedView());
            if (resetCamButton != null) resetCamButton.onClick.AddListener(() => explorerManager?.ResetCamera());

            // Wire up search bar
            if (searchInputField != null)
            {
                searchInputField.onValueChanged.AddListener(OnSearchChanged);
                searchInputField.onSubmit.AddListener(OnSearchSubmit);
            }

            // Wire up explode slider
            if (explodeSlider != null)
            {
                explodeSlider.onValueChanged.AddListener(OnExplodeSliderChanged);
            }

            // Wire up mute button
            if (muteButton != null)
            {
                muteButton.onClick.AddListener(OnMuteButtonClicked);
                UpdateMuteButtonVisual();
            }

            // Wire up system toggle buttons
            if (toggleSkeletalButton != null)
            {
                toggleSkeletalButton.onClick.AddListener(() => {
                    if (explorerManager != null)
                    {
                        explorerManager.SetSystemVisible(AnatomySystemType.Skeletal, !explorerManager.isSkeletalVisible);
                        UpdateSystemIndicators();
                    }
                });
            }

            if (toggleMuscularButton != null)
            {
                toggleMuscularButton.onClick.AddListener(() => {
                    if (explorerManager != null)
                    {
                        explorerManager.SetSystemVisible(AnatomySystemType.Muscular, !explorerManager.isMuscularVisible);
                        UpdateSystemIndicators();
                    }
                });
            }

            if (toggleNervousButton != null)
            {
                toggleNervousButton.onClick.AddListener(() => {
                    if (explorerManager != null)
                    {
                        explorerManager.SetSystemVisible(AnatomySystemType.Nervous, !explorerManager.isNervousVisible);
                        UpdateSystemIndicators();
                    }
                });
            }

            UpdateSystemIndicators();
            HidePartInfo();
        }

        private void OnDestroy()
        {
            if (explorerManager != null)
            {
                explorerManager.OnPartSelected -= DisplayPartInfo;
                explorerManager.OnPartDeselected -= HidePartInfo;
                explorerManager.OnExplodeProgressChanged -= UpdateExplodeSliderValue;
            }
        }

        public void DisplayPartInfo(AnatomyPart part)
        {
            if (infoPanel != null) infoPanel.SetActive(true);
            if (part == null || part.data == null) return;

            if (titleText != null) titleText.text = part.data.partName;
            if (scientificText != null) scientificText.text = $"({part.data.scientificName})";
            if (systemBadgeText != null)
            {
                string colorHex = part.data.systemType switch
                {
                    AnatomySystemType.Skeletal => "#5AC8FA",
                    AnatomySystemType.Muscular => "#FF453A",
                    AnatomySystemType.Nervous => "#FFD60A",
                    _ => "#FFFFFF"
                };
                systemBadgeText.text = $"<color={colorHex}>●</color> {part.data.systemType} System";
            }
            if (functionText != null) functionText.text = $"<b>Function:</b> {part.data.keyFunction}";
            if (descriptionText != null) descriptionText.text = part.data.description;

            if (factsText != null)
            {
                if (part.data.clinicalFacts != null && part.data.clinicalFacts.Length > 0)
                {
                    string factsStr = "<b>Clinical Facts:</b>\n";
                    foreach (var f in part.data.clinicalFacts)
                    {
                        factsStr += $"• {f}\n";
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

        private void OnSearchChanged(string text)
        {
            if (explorerManager != null)
            {
                explorerManager.SearchAndSelectPart(text);
            }
        }

        private void OnSearchSubmit(string text)
        {
            if (explorerManager != null)
            {
                explorerManager.SearchAndSelectPart(text);
            }
        }

        private void OnExplodeSliderChanged(float val)
        {
            if (explorerManager != null)
            {
                explorerManager.SetExplodeProgress(val);
            }
            if (explodeValueText != null)
            {
                explodeValueText.text = $"{Mathf.RoundToInt(val * 100f)}%";
            }
        }

        private void UpdateExplodeSliderValue(float val)
        {
            if (explodeSlider != null && Mathf.Abs(explodeSlider.value - val) > 0.01f)
            {
                explodeSlider.SetValueWithoutNotify(val);
            }
            if (explodeValueText != null)
            {
                explodeValueText.text = $"{Mathf.RoundToInt(val * 100f)}%";
            }
        }

        private void OnMuteButtonClicked()
        {
            if (AnatomyAudioFeedback.Instance != null)
            {
                AnatomyAudioFeedback.Instance.ToggleMute();
                UpdateMuteButtonVisual();
            }
        }

        private void UpdateMuteButtonVisual()
        {
            if (muteButtonText != null)
            {
                bool muted = AnatomyAudioFeedback.Instance != null && AnatomyAudioFeedback.Instance.isMuted;
                muteButtonText.text = muted ? "Audio: OFF" : "Audio: ON";
            }
        }

        private void UpdateSystemIndicators()
        {
            if (explorerManager == null) return;

            if (skeletalIndicator != null)
            {
                skeletalIndicator.color = explorerManager.isSkeletalVisible
                    ? new Color(0.35f, 0.78f, 1f, 1f)
                    : new Color(0.3f, 0.3f, 0.35f, 0.5f);
            }

            if (muscularIndicator != null)
            {
                muscularIndicator.color = explorerManager.isMuscularVisible
                    ? new Color(1f, 0.35f, 0.35f, 1f)
                    : new Color(0.3f, 0.3f, 0.35f, 0.5f);
            }

            if (nervousIndicator != null)
            {
                nervousIndicator.color = explorerManager.isNervousVisible
                    ? new Color(1f, 0.85f, 0.2f, 1f)
                    : new Color(0.3f, 0.3f, 0.35f, 0.5f);
            }
        }
    }
}
