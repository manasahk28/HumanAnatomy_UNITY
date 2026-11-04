using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HumanAnatomy
{
    public class AnatomyExplorerManager : MonoBehaviour
    {
        public static AnatomyExplorerManager Instance { get; private set; }

        [Header("References")]
        public OrbitCameraController cameraController;
        public Camera mainCamera;

        [Header("State")]
        public AnatomySystemType activeFilter = AnatomySystemType.All;
        public bool isExploded = false;
        public bool showGhostSilhouette = true;
        public bool isSkeletalVisible = true;
        public bool isMuscularVisible = true;
        public bool isNervousVisible = true;

        [Header("Raycast Settings")]
        public LayerMask anatomyLayer = ~0;

        private List<AnatomyPart> allParts = new List<AnatomyPart>();
        private AnatomyPart hoveredPart = null;
        private AnatomyPart selectedPart = null;
        private float explodedLerp = 0f;
        private bool isExplodedControlledBySlider = false;

        public event Action<AnatomyPart> OnPartSelected;
        public event Action OnPartDeselected;
        public event Action<AnatomySystemType> OnFilterChanged;
        public event Action<float> OnExplodeProgressChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (mainCamera == null) mainCamera = Camera.main;
            if (cameraController == null && mainCamera != null)
            {
                cameraController = mainCamera.GetComponent<OrbitCameraController>();
            }
        }

        private void Start()
        {
            RefreshPartsList();
        }

        public void RefreshPartsList()
        {
            allParts.Clear();
            allParts.AddRange(FindObjectsByType<AnatomyPart>(FindObjectsInactive.Include));
            ApplyFilter(activeFilter);
        }

        private void Update()
        {
            HandleExplodedTransition();
            HandleRaycasting();
        }

        private void HandleExplodedTransition()
        {
            if (isExplodedControlledBySlider) return;

            float target = isExploded ? 1f : 0f;
            if (Mathf.Abs(explodedLerp - target) > 0.001f)
            {
                explodedLerp = Mathf.MoveTowards(explodedLerp, target, Time.deltaTime * 2.5f);
                foreach (var part in allParts)
                {
                    if (part != null) part.SetExplodedProgress(explodedLerp);
                }
                OnExplodeProgressChanged?.Invoke(explodedLerp);
            }
        }

        private void HandleRaycasting()
        {
            if (mainCamera == null) return;
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Don't interact with 3D scene if clicking on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                ClearHover();
                return;
            }

            Vector2 mousePos = mouse.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, anatomyLayer))
            {
                AnatomyPart hitPart = hit.collider.GetComponentInParent<AnatomyPart>();
                if (hitPart != null && hitPart.data.systemType != AnatomySystemType.Silhouette)
                {
                    if (hoveredPart != hitPart)
                    {
                        ClearHover();
                        hoveredPart = hitPart;
                        hoveredPart.SetHovered(true);
                        AnatomyTooltipUI.Instance?.ShowTooltip(hoveredPart);
                        AnatomyAudioFeedback.Instance?.PlayHoverSound();
                    }

                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        SelectPart(hitPart);
                    }
                    return;
                }
            }

            ClearHover();

            // Clicking in empty space deselects
            if (mouse.leftButton.wasPressedThisFrame)
            {
                DeselectPart();
            }
        }

        private void ClearHover()
        {
            if (hoveredPart != null)
            {
                hoveredPart.SetHovered(false);
                hoveredPart = null;
                AnatomyTooltipUI.Instance?.HideTooltip();
            }
        }

        public void SelectPart(AnatomyPart part)
        {
            if (selectedPart == part) return;

            if (selectedPart != null)
            {
                selectedPart.SetSelected(false);
            }

            selectedPart = part;

            if (selectedPart != null)
            {
                selectedPart.SetSelected(true);
                AnatomyAudioFeedback.Instance?.PlaySelectSound();
                OnPartSelected?.Invoke(selectedPart);
            }
            else
            {
                OnPartDeselected?.Invoke();
            }
        }

        public void DeselectPart()
        {
            if (selectedPart != null)
            {
                selectedPart.SetSelected(false);
                selectedPart = null;
                OnPartDeselected?.Invoke();
            }
        }

        public void FocusOnSelected()
        {
            if (selectedPart != null && cameraController != null)
            {
                Bounds b = selectedPart.GetBounds();
                float dist = Mathf.Max(b.size.x, b.size.y, b.size.z) * 2.8f;
                cameraController.FocusOn(b.center, Mathf.Clamp(dist, 0.6f, 2.0f));
                AnatomyAudioFeedback.Instance?.PlayWhooshSound();
            }
        }

        public void ResetCamera()
        {
            if (cameraController != null)
            {
                cameraController.ResetView();
                AnatomyAudioFeedback.Instance?.PlayWhooshSound();
            }
        }

        public void ToggleExplodedView()
        {
            isExplodedControlledBySlider = false;
            isExploded = !isExploded;
            AnatomyAudioFeedback.Instance?.PlayExplodeSound();
        }

        public void SetExplodeProgress(float progress)
        {
            isExplodedControlledBySlider = true;
            explodedLerp = Mathf.Clamp01(progress);
            isExploded = explodedLerp > 0.5f;

            foreach (var part in allParts)
            {
                if (part != null) part.SetExplodedProgress(explodedLerp);
            }
            OnExplodeProgressChanged?.Invoke(explodedLerp);
        }

        public void SetGhostSilhouette(bool show)
        {
            showGhostSilhouette = show;
            foreach (var part in allParts)
            {
                if (part != null && part.data.systemType == AnatomySystemType.Silhouette)
                {
                    part.SetVisibility(showGhostSilhouette);
                }
            }
        }

        public void SetSystemVisible(AnatomySystemType systemType, bool visible)
        {
            if (systemType == AnatomySystemType.Skeletal) isSkeletalVisible = visible;
            else if (systemType == AnatomySystemType.Muscular) isMuscularVisible = visible;
            else if (systemType == AnatomySystemType.Nervous) isNervousVisible = visible;

            UpdateAllSystemsVisibility();
            AnatomyAudioFeedback.Instance?.PlayFilterSound();
        }

        public void UpdateAllSystemsVisibility()
        {
            foreach (var part in allParts)
            {
                if (part == null) continue;

                if (part.data.systemType == AnatomySystemType.Silhouette)
                {
                    part.SetVisibility(showGhostSilhouette);
                    continue;
                }

                bool visible = true;
                if (part.data.systemType == AnatomySystemType.Skeletal) visible = isSkeletalVisible;
                else if (part.data.systemType == AnatomySystemType.Muscular) visible = isMuscularVisible;
                else if (part.data.systemType == AnatomySystemType.Nervous) visible = isNervousVisible;

                part.SetVisibility(visible);
            }
        }

        public bool SearchAndSelectPart(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                foreach (var part in allParts)
                {
                    if (part != null) part.SetDimmed(false);
                }
                return false;
            }

            query = query.Trim().ToLower();
            AnatomyPart bestMatch = null;

            foreach (var part in allParts)
            {
                if (part == null || part.data.systemType == AnatomySystemType.Silhouette) continue;

                string pName = part.data.partName?.ToLower() ?? "";
                string sName = part.data.scientificName?.ToLower() ?? "";
                string goName = part.gameObject.name.ToLower();

                bool isMatch = pName.Contains(query) || sName.Contains(query) || goName.Contains(query);
                if (isMatch)
                {
                    if (bestMatch == null) bestMatch = part;
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else
                {
                    part.SetDimmed(true);
                }
            }

            if (bestMatch != null)
            {
                SelectPart(bestMatch);
                FocusOnSelected();
                return true;
            }

            return false;
        }

        public void ApplyFilter(AnatomySystemType systemType)
        {
            activeFilter = systemType;
            AnatomyAudioFeedback.Instance?.PlayFilterSound();

            foreach (var part in allParts)
            {
                if (part == null) continue;

                if (part.data.systemType == AnatomySystemType.Silhouette)
                {
                    part.SetVisibility(showGhostSilhouette);
                    continue;
                }

                if (activeFilter == AnatomySystemType.All)
                {
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else if (part.data.systemType == activeFilter)
                {
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else
                {
                    part.SetVisibility(true);
                    part.SetDimmed(true);
                }
            }

            OnFilterChanged?.Invoke(activeFilter);
        }

        public void FilterSkeletalRegion(string regionKeyword)
        {
            AnatomyAudioFeedback.Instance?.PlayFilterSound();
            foreach (var part in allParts)
            {
                if (part == null) continue;
                if (string.IsNullOrEmpty(regionKeyword) || regionKeyword == "All")
                {
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = false;
                    if (regionKeyword == "Axial")
                        match = n.Contains("skull") || n.Contains("spine") || n.Contains("mandible") || n.Contains("sacrum") || n.Contains("cord");
                    else if (regionKeyword == "Thorax")
                        match = n.Contains("rib") || n.Contains("sternum");
                    else if (regionKeyword == "Upper")
                        match = n.Contains("clavicle") || n.Contains("scapula") || n.Contains("humerus") || n.Contains("radius") || n.Contains("ulna") || n.Contains("hand");
                    else if (regionKeyword == "Lower")
                        match = n.Contains("pelvis") || n.Contains("femur") || n.Contains("patella") || n.Contains("tibia") || n.Contains("fibula") || n.Contains("foot");

                    if (match)
                    {
                        part.SetVisibility(true);
                        part.SetDimmed(false);
                    }
                    else
                    {
                        part.SetVisibility(true);
                        part.SetDimmed(true);
                    }
                }
            }
        }

        public void FilterMusculoskeletal(string mode)
        {
            AnatomyAudioFeedback.Instance?.PlayFilterSound();
            foreach (var part in allParts)
            {
                if (part == null) continue;
                if (mode == "All")
                {
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else if (mode == "Skeleton")
                {
                    bool isSkel = part.data.systemType == AnatomySystemType.Skeletal;
                    part.SetVisibility(isSkel);
                    part.SetDimmed(false);
                }
                else if (mode == "Muscles")
                {
                    bool isMusc = part.data.systemType == AnatomySystemType.Muscular;
                    part.SetVisibility(isMusc);
                    part.SetDimmed(false);
                }
                else if (mode == "UpperMuscles")
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = n.Contains("pectoralis") || n.Contains("deltoid") || n.Contains("biceps") || n.Contains("triceps") || n.Contains("forearm") || n.Contains("trapezius") || n.Contains("latissimus");
                    part.SetVisibility(true);
                    part.SetDimmed(!match);
                }
                else if (mode == "LowerMuscles")
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = n.Contains("abdominis") || n.Contains("oblique") || n.Contains("gluteal") || n.Contains("quadriceps") || n.Contains("hamstring") || n.Contains("gastrocnemius");
                    part.SetVisibility(true);
                    part.SetDimmed(!match);
                }
                else if (mode == "Nervous")
                {
                    bool isNerv = part.data.systemType == AnatomySystemType.Nervous;
                    part.SetVisibility(isNerv);
                    part.SetDimmed(false);
                }
                else if (mode == "BrainCord")
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = n.Contains("brain") || n.Contains("lobe") || n.Contains("cerebellum") || n.Contains("cord") || n.Contains("cauda") || n.Contains("sympathetic");
                    part.SetVisibility(true);
                    part.SetDimmed(!match);
                }
                else if (mode == "ArmNerves")
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = n.Contains("plexus") || n.Contains("radial") || n.Contains("median") || n.Contains("ulnar");
                    part.SetVisibility(true);
                    part.SetDimmed(!match);
                }
                else if (mode == "LegNerves")
                {
                    string n = part.gameObject.name.ToLower();
                    bool match = n.Contains("femoral") || n.Contains("sciatic") || n.Contains("tibial") || n.Contains("fibular");
                    part.SetVisibility(true);
                    part.SetDimmed(!match);
                }
            }
        }

        public void IsolateCurrentSelected()
        {
            if (selectedPart == null) return;
            AnatomyAudioFeedback.Instance?.PlayFilterSound();

            foreach (var part in allParts)
            {
                if (part == null) continue;
                if (part == selectedPart)
                {
                    part.SetVisibility(true);
                    part.SetDimmed(false);
                }
                else
                {
                    part.SetVisibility(false);
                }
            }
        }
    }
}
