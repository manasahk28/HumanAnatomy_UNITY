using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace HumanAnatomy
{
    public class AnatomySceneBuilder : MonoBehaviour
    {
        [Header("Setup On Awake")]
        public bool buildOnStart = true;
        public bool skeletonOnly = false;
        public bool includeMuscles = true;
        public bool includeNervous = true;

        private void Start()
        {
            if (buildOnStart)
            {
                BuildAnatomyModel();
            }
        }

        [ContextMenu("Build Anatomy Model")]
        public void BuildAnatomyModel()
        {
            // Clean up any existing generated model
            Transform existing = transform.Find("HumanBodyRoot");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            // Clean up existing canvas in editor mode so UI updates take effect
            if (!Application.isPlaying)
            {
                GameObject oldCanvas = GameObject.Find("AnatomyCanvas");
                if (oldCanvas != null) DestroyImmediate(oldCanvas);
            }

            GameObject root = new GameObject("HumanBodyRoot");
            root.transform.SetParent(transform, false);

            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            // 1. Build the detailed color-coded skeletal system
            BuildSkeletalSystem(root.transform, litShader);

            // 2. Build the full muscular system
            if (includeMuscles)
            {
                BuildMuscularSystem(root.transform, litShader);
            }

            // 3. Build the full nervous system
            if (includeNervous)
            {
                BuildNervousSystem(root.transform, litShader);
            }

            // Ensure Audio Feedback component exists on manager
            if (GetComponent<AnatomyAudioFeedback>() == null)
            {
                gameObject.AddComponent<AnatomyAudioFeedback>();
            }

            // Ensure UI exists
            BuildUI();

            // Refresh Explorer Manager
            AnatomyExplorerManager explorer = FindAnyObjectByType<AnatomyExplorerManager>();
            if (explorer != null)
            {
                explorer.RefreshPartsList();
            }
        }

        #region Muscular System
        private void BuildMuscularSystem(Transform parent, Shader shader)
        {
            AnatomyMuscularGenerator.BuildFullMuscularSystem(parent, shader);
        }
        #endregion

        #region Nervous System
        private void BuildNervousSystem(Transform parent, Shader shader)
        {
            AnatomyNervousGenerator.BuildFullNervousSystem(parent, shader);
        }
        #endregion

        #region 1. Ghost Silhouette
        private void BuildGhostSilhouette(Transform parent, Shader shader)
        {
            GameObject group = new GameObject("GhostSilhouette");
            group.transform.SetParent(parent, false);

            Material ghostMat = new Material(shader);
            ghostMat.name = "GhostBodyMat";
            // Set transparency on URP Lit
            ghostMat.SetFloat("_Surface", 1); // Transparent
            ghostMat.SetFloat("_Blend", 0); // Alpha
            ghostMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            ghostMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            ghostMat.SetInt("_ZWrite", 0);
            ghostMat.renderQueue = 3000;
            Color ghostCol = new Color(0.35f, 0.65f, 0.95f, 0.10f);
            ghostMat.color = ghostCol;
            ghostMat.SetColor("_BaseColor", ghostCol);

            // Head silhouette
            CreatePart(group.transform, PrimitiveType.Sphere, "Body_Head",
                new Vector3(0, 1.76f, 0.01f), new Vector3(0.24f, 0.28f, 0.26f),
                ghostMat, AnatomySystemType.Silhouette, "Human Body Outline", "Homo Sapiens",
                "Provides structural enclosure and spatial reference for all internal organs.",
                "Outer epidermal envelope.", Vector3.zero, false);

            // Neck silhouette
            CreatePart(group.transform, PrimitiveType.Cylinder, "Body_Neck",
                new Vector3(0, 1.55f, -0.01f), new Vector3(0.13f, 0.11f, 0.13f),
                ghostMat, AnatomySystemType.Silhouette, "Neck Outline", "Collum",
                "Connects head to torso.", "", Vector3.zero, false);

            // Torso silhouette
            CreatePart(group.transform, PrimitiveType.Capsule, "Body_Torso",
                new Vector3(0, 1.20f, 0.01f), new Vector3(0.42f, 0.58f, 0.28f),
                ghostMat, AnatomySystemType.Silhouette, "Torso Outline", "Truncus",
                "Houses thoracic and abdominal cavities.", "", Vector3.zero, false);

            // Arms silhouettes
            CreatePart(group.transform, PrimitiveType.Capsule, "Body_LeftArm",
                new Vector3(-0.25f, 1.15f, -0.01f), new Vector3(0.10f, 0.60f, 0.10f),
                ghostMat, AnatomySystemType.Silhouette, "Left Arm", "Brachium Sinistrum", "", "", Vector3.zero, false);

            CreatePart(group.transform, PrimitiveType.Capsule, "Body_RightArm",
                new Vector3(0.25f, 1.15f, -0.01f), new Vector3(0.10f, 0.60f, 0.10f),
                ghostMat, AnatomySystemType.Silhouette, "Right Arm", "Brachium Dextrum", "", "", Vector3.zero, false);

            // Legs silhouettes
            CreatePart(group.transform, PrimitiveType.Capsule, "Body_LeftLeg",
                new Vector3(-0.11f, 0.48f, 0.01f), new Vector3(0.14f, 0.86f, 0.14f),
                ghostMat, AnatomySystemType.Silhouette, "Left Leg", "Membrum Inferius", "", "", Vector3.zero, false);

            CreatePart(group.transform, PrimitiveType.Capsule, "Body_RightLeg",
                new Vector3(0.11f, 0.48f, 0.01f), new Vector3(0.14f, 0.86f, 0.14f),
                ghostMat, AnatomySystemType.Silhouette, "Right Leg", "Membrum Inferius", "", "", Vector3.zero, false);
        }
        #endregion

        #region 2. Skeletal System
        private void BuildSkeletalSystem(Transform parent, Shader shader)
        {
            // Call the full anatomically proportioned skeletal generator
            AnatomySkeletalGenerator.BuildFullSkeleton(parent, shader);
        }
        #endregion

        #region 4. Circulatory System
        private void BuildCirculatorySystem(Transform parent, Shader shader)
        {
            GameObject group = new GameObject("CirculatorySystem");
            group.transform.SetParent(parent, false);

            Material heartMat = CreateColoredMaterial(shader, "HeartMat", new Color(0.85f, 0.15f, 0.22f));
            Material arteryMat = CreateColoredMaterial(shader, "ArteryMat", new Color(0.92f, 0.22f, 0.22f));
            Material veinMat = CreateColoredMaterial(shader, "VeinMat", new Color(0.20f, 0.45f, 0.85f));

            // Heart
            AnatomyPart heartPart = CreatePart(group.transform, PrimitiveType.Sphere, "Circulatory_Heart",
                new Vector3(-0.04f, 1.34f, 0.02f), new Vector3(0.14f, 0.16f, 0.13f),
                heartMat, AnatomySystemType.Circulatory, "Human Heart", "Cor",
                "Muscular organ maintaining systemic and pulmonary blood circulation via continuous cyclical contractions.",
                "Pumps roughly 5 liters of blood per minute through a vascular network extending over 100,000 kilometers.",
                new Vector3(-0.25f, 0.1f, 0.45f), true,
                "Four chambers: Right & Left Atria, Right & Left Ventricles.",
                "Has intrinsic cardiac conduction pacing (Sinoatrial node).");
            heartPart.hasHeartbeatPulse = true;
            heartPart.pulseMagnitude = 0.05f;
            heartPart.pulseSpeed = 1.1f;

            // Aorta
            CreatePart(group.transform, PrimitiveType.Cylinder, "Circulatory_Aorta",
                new Vector3(-0.02f, 1.38f, -0.01f), new Vector3(0.03f, 0.14f, 0.03f),
                arteryMat, AnatomySystemType.Circulatory, "Aortic Arch & Descending Aorta", "Aorta",
                "Main trunk of systemic arterial circulation conveying oxygenated blood from the left cardiac ventricle.",
                "Withstands blood pressure peaks averaging 120 mmHg, utilizing elastic recoil (Windkessel effect).",
                new Vector3(-0.1f, 0.15f, 0.35f), true,
                "Arches superiorly before descending along the anterior aspect of the thoracic spine.",
                "Branches supply the head, arms, thoracic viscera, and abdominal organs.");

            // Vena Cava
            CreatePart(group.transform, PrimitiveType.Cylinder, "Circulatory_VenaCava",
                new Vector3(0.03f, 1.33f, -0.01f), new Vector3(0.032f, 0.18f, 0.032f),
                veinMat, AnatomySystemType.Circulatory, "Vena Cava (Superior & Inferior)", "Vena Cava",
                "Primary venous channels collecting deoxygenated blood from the upper and lower body to the right atrium.",
                "Low-pressure vascular conduits reliant on muscle pump action and thoracic pressure gradients for venous return.",
                new Vector3(0.1f, 0.15f, 0.35f), true,
                "Superior Vena Cava drains head, neck, and upper limbs.",
                "Inferior Vena Cava is the widest vein in the human body.");
        }
        #endregion

        #region 5. Respiratory System
        private void BuildRespiratorySystem(Transform parent, Shader shader)
        {
            GameObject group = new GameObject("RespiratorySystem");
            group.transform.SetParent(parent, false);

            Material lungMat = CreateColoredMaterial(shader, "LungMat", new Color(0.92f, 0.55f, 0.60f));
            Material airwayMat = CreateColoredMaterial(shader, "AirwayMat", new Color(0.85f, 0.75f, 0.85f));

            // Trachea (Windpipe)
            CreatePart(group.transform, PrimitiveType.Cylinder, "Respiratory_Trachea",
                new Vector3(0, 1.52f, 0.01f), new Vector3(0.04f, 0.12f, 0.04f),
                airwayMat, AnatomySystemType.Respiratory, "Trachea (Windpipe)", "Trachea",
                "Cartilaginous airway tube conducting air from larynx downward into right and left mainstem bronchi.",
                "Reinforced by 16-20 C-shaped hyaline cartilage rings to prevent collapse during inspiration.",
                new Vector3(0, 0.1f, 0.35f), true,
                "Lined with pseudostratified ciliated columnar epithelium to sweep debris upward.",
                "Bifurcates at the carina into primary bronchi.");

            // Right Lung (3 lobes)
            CreatePart(group.transform, PrimitiveType.Capsule, "Respiratory_RightLung",
                new Vector3(0.13f, 1.33f, 0.01f), new Vector3(0.14f, 0.22f, 0.15f),
                lungMat, AnatomySystemType.Respiratory, "Right Lung", "Pulmo Dexter",
                "Primary organ of gas exchange; oxygenates venous blood and eliminates carbon dioxide waste.",
                "Heavier and wider than the left lung, divided into 3 distinct lobes: Superior, Middle, and Inferior.",
                new Vector3(0.45f, 0.1f, 0.2f), true,
                "Contains hundreds of millions of microscopic alveoli providing ~70 m² surface area.",
                "Receives deoxygenated blood from the pulmonary trunk.");

            // Left Lung (2 lobes + cardiac notch)
            CreatePart(group.transform, PrimitiveType.Capsule, "Respiratory_LeftLung",
                new Vector3(-0.13f, 1.33f, 0.01f), new Vector3(0.13f, 0.21f, 0.14f),
                lungMat, AnatomySystemType.Respiratory, "Left Lung", "Pulmo Sinister",
                "Facilitates external pulmonary respiration alongside the right lung.",
                "Features a medial cardiac notch to accommodate heart positioning, having 2 lobes: Superior and Inferior.",
                new Vector3(-0.45f, 0.1f, 0.2f), true,
                "Divided by an oblique fissure into two functional lobes.",
                "Enclosed within the visceral and parietal pleural cavity.");
        }
        #endregion

        #region 6. Digestive System
        private void BuildDigestiveSystem(Transform parent, Shader shader)
        {
            GameObject group = new GameObject("DigestiveSystem");
            group.transform.SetParent(parent, false);

            Material liverMat = CreateColoredMaterial(shader, "LiverMat", new Color(0.62f, 0.24f, 0.18f));
            Material stomachMat = CreateColoredMaterial(shader, "StomachMat", new Color(0.88f, 0.48f, 0.38f));
            Material bowelMat = CreateColoredMaterial(shader, "BowelMat", new Color(0.82f, 0.62f, 0.48f));

            // Liver
            CreatePart(group.transform, PrimitiveType.Cube, "Digestive_Liver",
                new Vector3(0.08f, 1.15f, 0.03f), new Vector3(0.20f, 0.13f, 0.16f),
                liverMat, AnatomySystemType.Digestive, "Liver", "Hepar",
                "Metabolic factory synthesizing plasma proteins, detoxifying biochemicals, and producing digestive bile.",
                "The largest internal visceral organ (approx. 1.5 kg), capable of cellular tissue regeneration.",
                new Vector3(0.35f, -0.1f, 0.35f), true,
                "Receives dual blood supply from hepatic artery and hepatic portal vein.",
                "Stores glycogen, vitamin reserves, and trace iron.");

            // Stomach
            CreatePart(group.transform, PrimitiveType.Sphere, "Digestive_Stomach",
                new Vector3(-0.07f, 1.13f, 0.04f), new Vector3(0.15f, 0.12f, 0.13f),
                stomachMat, AnatomySystemType.Digestive, "Stomach", "Gaster",
                "Muscular reservoir initiating mechanical churn and enzymatic digestion of ingested nutrients.",
                "Secretes gastric juice consisting of hydrochloric acid (pH 1.5 to 2.0) and pepsinogen.",
                new Vector3(-0.35f, -0.1f, 0.35f), true,
                "Features mucosal rugae folds that expand to hold 1 to 2 liters of food.",
                "Empties semi-liquid chyme into the duodenum via pyloric sphincter.");

            // Intestinal Tract (Small & Large Intestine)
            CreatePart(group.transform, PrimitiveType.Cube, "Digestive_Intestines",
                new Vector3(0, 0.98f, 0.03f), new Vector3(0.24f, 0.16f, 0.17f),
                bowelMat, AnatomySystemType.Digestive, "Intestinal Tract", "Intestinum (Tenue et Crassum)",
                "Principal site for nutrient digestion, carbohydrate/lipid absorption, electrolyte recovery, and water reabsorption.",
                "Small intestine stretches approximately 6 meters; large intestine (colon) measures roughly 1.5 meters.",
                new Vector3(0, -0.3f, 0.35f), true,
                "Villi and microvilli expand nutrient absorptive surface to ~30 m².",
                "Houses trillions of symbiotic gut microbiome bacteria essential for health.");
        }
        #endregion

        #region Helper Creators
        private AnatomyPart CreatePart(
            Transform parent, PrimitiveType primitiveType, string objName,
            Vector3 localPos, Vector3 localScale, Material mat,
            AnatomySystemType systemType, string partName, string scientificName,
            string keyFunction, string description, Vector3 explodedOffset, bool addCollider = true,
            params string[] clinicalFacts)
        {
            GameObject obj = GameObject.CreatePrimitive(primitiveType);
            obj.name = objName;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;

            if (mat != null)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = mat;
            }

            Collider col = obj.GetComponent<Collider>();
            if (!addCollider && col != null)
            {
                DestroyImmediate(col);
            }

            AnatomyPart part = obj.AddComponent<AnatomyPart>();
            part.data = new AnatomyPartData
            {
                partName = partName,
                scientificName = scientificName,
                systemType = systemType,
                keyFunction = keyFunction,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat != null ? mat.color : Color.white;
            part.explodedOffset = explodedOffset;

            return part;
        }

        private Material CreateColoredMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            return mat;
        }
        #endregion

        #region UI Builder
        private void BuildUI()
        {
            if (FindAnyObjectByType<AnatomyUIManager>() != null) return;

            // Ensure EventSystem
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // Create Canvas
            GameObject canvasObj = new GameObject("AnatomyCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            AnatomyUIManager uiManager = canvasObj.AddComponent<AnatomyUIManager>();

            // 1. TOP HEADER BAR
            GameObject topHeader = CreateUIPanel("TopHeaderBar", canvasObj.transform,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, -32), new Vector2(0, 56),
                new Color(0.08f, 0.10f, 0.14f, 0.92f));

            // App Title
            CreateTextElement("AppTitle", topHeader.transform,
                "<b>HUMAN ANATOMY 3D</b>", 17, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(25, 0), new Vector2(240, 36));

            // Search Bar in Top Bar
            GameObject searchBox = CreateSearchBox("SearchInput", topHeader.transform,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(400, 0), new Vector2(280, 36));
            uiManager.searchInputField = searchBox.GetComponent<TMP_InputField>();

            // System Quick Toggles (Skeletal, Muscular, Nervous)
            GameObject layerGroup = CreateUIPanel("LayerToggles", topHeader.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50, 0), new Vector2(330, 36),
                Color.clear);
            HorizontalLayoutGroup lhlg = layerGroup.AddComponent<HorizontalLayoutGroup>();
            lhlg.spacing = 6;
            lhlg.childControlHeight = true;
            lhlg.childControlWidth = true;
            lhlg.childForceExpandHeight = true;
            lhlg.childForceExpandWidth = true;

            Button btnSkel = CreateToggleButton("Toggle_Skel", layerGroup.transform, "Skeletal", new Color(0.18f, 0.28f, 0.42f), new Color(0.35f, 0.78f, 1f));
            Button btnMusc = CreateToggleButton("Toggle_Musc", layerGroup.transform, "Muscular", new Color(0.42f, 0.18f, 0.22f), new Color(1f, 0.35f, 0.35f));
            Button btnNerv = CreateToggleButton("Toggle_Nerv", layerGroup.transform, "Nervous", new Color(0.40f, 0.34f, 0.15f), new Color(1f, 0.85f, 0.25f));

            uiManager.toggleSkeletalButton = btnSkel;
            uiManager.skeletalIndicator = btnSkel.transform.Find("Indicator")?.GetComponent<Image>();
            uiManager.toggleMuscularButton = btnMusc;
            uiManager.muscularIndicator = btnMusc.transform.Find("Indicator")?.GetComponent<Image>();
            uiManager.toggleNervousButton = btnNerv;
            uiManager.nervousIndicator = btnNerv.transform.Find("Indicator")?.GetComponent<Image>();

            // Right side Top Controls: Explode Slider + Audio Button + Reset Button
            GameObject rightControls = CreateUIPanel("TopRightControls", topHeader.transform,
                new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20, 0), new Vector2(420, 38),
                Color.clear);
            HorizontalLayoutGroup rhlg = rightControls.AddComponent<HorizontalLayoutGroup>();
            rhlg.spacing = 10;
            rhlg.childControlHeight = true;
            rhlg.childControlWidth = false;
            rhlg.childForceExpandHeight = true;
            rhlg.childForceExpandWidth = false;

            // Explode Slider Container
            GameObject sliderContainer = CreateUIPanel("ExplodeContainer", rightControls.transform,
                Vector2.zero, Vector2.one, Vector2.zero, new Vector2(210, 36),
                new Color(0.12f, 0.15f, 0.20f, 0.85f));
            LayoutElement sliderLe = sliderContainer.AddComponent<LayoutElement>();
            sliderLe.preferredWidth = 210;

            CreateTextElement("ExplodeLabel", sliderContainer.transform, "Explode:", 12, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(8, 0), new Vector2(50, 24));

            GameObject sliderObj = CreateSliderElement("Slider", sliderContainer.transform,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(110, 0), new Vector2(95, 18));
            uiManager.explodeSlider = sliderObj.GetComponent<Slider>();

            TextMeshProUGUI valText = CreateTextElement("ValText", sliderContainer.transform, "0%", 11, TextAlignmentOptions.Right,
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-6, 0), new Vector2(35, 20)).GetComponent<TextMeshProUGUI>();
            uiManager.explodeValueText = valText;

            // Audio Mute Button
            Button muteBtn = CreateButton("Btn_Mute", rightControls.transform, "Audio: ON", 12, new Color(0.22f, 0.32f, 0.44f));
            LayoutElement muteLe = muteBtn.gameObject.AddComponent<LayoutElement>();
            muteLe.preferredWidth = 90;
            uiManager.muteButton = muteBtn;
            uiManager.muteButtonText = muteBtn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();

            // Reset Camera Button
            Button resetBtn = CreateButton("Btn_ResetCam", rightControls.transform, "Reset View", 12, new Color(0.22f, 0.32f, 0.44f));
            LayoutElement resetLe = resetBtn.gameObject.AddComponent<LayoutElement>();
            resetLe.preferredWidth = 85;
            uiManager.resetCamButton = resetBtn;

            // 2. LEFT PRESET FILTERS PANEL
            GameObject filterPanel = CreateUIPanel("FilterPanel", canvasObj.transform,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20, -90), new Vector2(180, 290),
                new Color(0.08f, 0.10f, 0.14f, 0.90f));

            CreateTextElement("FilterHeader", filterPanel.transform, "<b>SYSTEM PRESETS</b>", 12, TextAlignmentOptions.Center,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -16), new Vector2(160, 20));

            GameObject filterList = CreateUIPanel("FilterList", filterPanel.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -12), new Vector2(160, 240),
                Color.clear);
            VerticalLayoutGroup fvlg = filterList.AddComponent<VerticalLayoutGroup>();
            fvlg.spacing = 6;
            fvlg.childControlHeight = true;
            fvlg.childControlWidth = true;
            fvlg.childForceExpandHeight = true;
            fvlg.childForceExpandWidth = true;

            string[] labels = { "All Systems", "Skeleton Only", "Muscles Only", "Nervous Only", "Brain & Cord", "Arm Nerves", "Leg Nerves" };
            string[] modes = { "All", "Skeleton", "Muscles", "Nervous", "BrainCord", "ArmNerves", "LegNerves" };
            for (int i = 0; i < labels.Length; i++)
            {
                string m = modes[i];
                Button btn = CreateButton($"Btn_{m}", filterList.transform, labels[i], 11, new Color(0.18f, 0.22f, 0.30f, 0.95f));
                btn.onClick.AddListener(() => {
                    AnatomyExplorerManager mgr = FindAnyObjectByType<AnatomyExplorerManager>();
                    if (mgr != null) mgr.FilterMusculoskeletal(m);
                });
            }

            // 3. RIGHT INFO CARD
            GameObject infoCard = CreateUIPanel("InfoPanel", canvasObj.transform,
                new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20, 0), new Vector2(360, 520),
                new Color(0.08f, 0.10f, 0.14f, 0.94f));
            uiManager.infoPanel = infoCard;

            // Info Card Header
            GameObject headerBg = CreateUIPanel("HeaderBg", infoCard.transform,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -35), new Vector2(340, 60),
                new Color(0.16f, 0.22f, 0.32f, 0.95f));

            uiManager.titleText = CreateTextElement("PartTitle", headerBg.transform, "Organ Name", 19, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, 8), new Vector2(240, 26)).GetComponent<TextMeshProUGUI>();

            uiManager.scientificText = CreateTextElement("ScientificName", headerBg.transform, "(Organum)", 13, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, -14), new Vector2(240, 20)).GetComponent<TextMeshProUGUI>();
            uiManager.scientificText.color = new Color(0.7f, 0.85f, 1f, 0.8f);

            // Close button on card
            Button closeBtn = CreateButton("CloseBtn", headerBg.transform, "X", 14, new Color(0.6f, 0.2f, 0.2f));
            RectTransform closeRt = closeBtn.GetComponent<RectTransform>();
            closeRt.anchorMin = closeRt.anchorMax = new Vector2(1, 0.5f);
            closeRt.anchoredPosition = new Vector2(-22, 0);
            closeRt.sizeDelta = new Vector2(30, 30);
            uiManager.closeInfoButton = closeBtn;

            // System Badge
            uiManager.systemBadgeText = CreateTextElement("SystemBadge", infoCard.transform, "Skeletal System", 13, TextAlignmentOptions.Left,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -85), new Vector2(330, 24)).GetComponent<TextMeshProUGUI>();
            uiManager.systemBadgeText.color = new Color(0.4f, 0.85f, 1f);

            // Function Text
            uiManager.functionText = CreateTextElement("FunctionText", infoCard.transform, "Function details...", 13, TextAlignmentOptions.TopLeft,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -135), new Vector2(330, 70)).GetComponent<TextMeshProUGUI>();

            // Description Text
            uiManager.descriptionText = CreateTextElement("DescriptionText", infoCard.transform, "Description...", 12, TextAlignmentOptions.TopLeft,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -225), new Vector2(330, 100)).GetComponent<TextMeshProUGUI>();
            uiManager.descriptionText.color = new Color(0.85f, 0.85f, 0.85f);

            // Clinical Facts Text
            uiManager.factsText = CreateTextElement("FactsText", infoCard.transform, "Clinical Facts...", 12, TextAlignmentOptions.TopLeft,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -345), new Vector2(330, 110)).GetComponent<TextMeshProUGUI>();
            uiManager.factsText.color = new Color(1f, 0.9f, 0.6f);

            // Action Buttons
            Button focusBtn = CreateButton("FocusBtn", infoCard.transform, "Focus Camera", 13, new Color(0.2f, 0.55f, 0.8f));
            RectTransform focusRt = focusBtn.GetComponent<RectTransform>();
            focusRt.anchorMin = focusRt.anchorMax = new Vector2(0.3f, 0.08f);
            focusRt.anchoredPosition = Vector2.zero;
            focusRt.sizeDelta = new Vector2(140, 36);
            uiManager.focusButton = focusBtn;

            Button isolateBtn = CreateButton("IsolateBtn", infoCard.transform, "Isolate Part", 13, new Color(0.3f, 0.65f, 0.4f));
            RectTransform isoRt = isolateBtn.GetComponent<RectTransform>();
            isoRt.anchorMin = isoRt.anchorMax = new Vector2(0.72f, 0.08f);
            isoRt.anchoredPosition = Vector2.zero;
            isoRt.sizeDelta = new Vector2(140, 36);
            uiManager.isolateButton = isolateBtn;

            // 4. INTERACTIVE TOOLTIP
            CreateTooltipUI(canvasObj.transform);

            // 5. BOTTOM NAVIGATION HINT
            CreateTextElement("HintText", canvasObj.transform,
                "<b>Controls:</b> Left Mouse Drag = Orbit | Right Mouse Drag = Pan | Scroll = Zoom | Left Click = Select & Inspect | Hover = Tooltip",
                12, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 18), new Vector2(900, 26));
        }

        private void CreateTooltipUI(Transform canvasTransform)
        {
            GameObject tooltipObj = new GameObject("AnatomyTooltip", typeof(RectTransform));
            tooltipObj.transform.SetParent(canvasTransform, false);
            RectTransform rt = tooltipObj.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(230, 78);

            CanvasGroup cg = tooltipObj.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            Image bg = tooltipObj.AddComponent<Image>();
            bg.color = new Color(0.06f, 0.08f, 0.12f, 0.94f);

            Outline outline = tooltipObj.AddComponent<Outline>();
            outline.effectColor = new Color(0.35f, 0.55f, 0.85f, 0.5f);
            outline.effectDistance = new Vector2(1, -1);

            AnatomyTooltipUI tooltip = tooltipObj.AddComponent<AnatomyTooltipUI>();
            tooltip.tooltipRect = rt;
            tooltip.canvasGroup = cg;

            // Title
            GameObject nameObj = CreateTextElement("PartName", tooltipObj.transform, "Part Name", 14, TextAlignmentOptions.Left,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(10, -14), new Vector2(-20, 20));
            tooltip.nameText = nameObj.GetComponent<TextMeshProUGUI>();

            // System
            GameObject sysObj = CreateTextElement("SystemBadge", tooltipObj.transform, "● Skeletal System", 11, TextAlignmentOptions.Left,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(10, -34), new Vector2(-20, 18));
            tooltip.systemText = sysObj.GetComponent<TextMeshProUGUI>();

            // Function
            GameObject fnObj = CreateTextElement("Function", tooltipObj.transform, "Primary function info...", 11, TextAlignmentOptions.Left,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(10, -54), new Vector2(-20, 18));
            fnObj.GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0.85f, 0.9f);
            tooltip.functionText = fnObj.GetComponent<TextMeshProUGUI>();
        }

        private GameObject CreateSearchBox(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject searchObj = new GameObject(name, typeof(RectTransform));
            searchObj.transform.SetParent(parent, false);
            RectTransform rt = searchObj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image bg = searchObj.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.23f, 0.95f);

            Outline ol = searchObj.AddComponent<Outline>();
            ol.effectColor = new Color(0.25f, 0.35f, 0.5f, 0.6f);
            ol.effectDistance = new Vector2(1, -1);

            TMP_InputField input = searchObj.AddComponent<TMP_InputField>();

            // Text Area
            GameObject textArea = new GameObject("TextArea", typeof(RectTransform));
            textArea.transform.SetParent(searchObj.transform, false);
            RectTransform areaRt = textArea.GetComponent<RectTransform>();
            areaRt.anchorMin = Vector2.zero;
            areaRt.anchorMax = Vector2.one;
            areaRt.offsetMin = new Vector2(10, 2);
            areaRt.offsetMax = new Vector2(-10, -2);
            textArea.AddComponent<RectMask2D>();

            // Placeholder Text
            GameObject placeholderObj = CreateTextElement("Placeholder", textArea.transform, "Search anatomy...", 12, TextAlignmentOptions.Left,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            TextMeshProUGUI placeholderTmp = placeholderObj.GetComponent<TextMeshProUGUI>();
            placeholderTmp.color = new Color(0.6f, 0.65f, 0.75f, 0.6f);
            RectTransform prt = placeholderObj.GetComponent<RectTransform>();
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;

            // Main Text
            GameObject textObj = CreateTextElement("Text", textArea.transform, "", 12, TextAlignmentOptions.Left,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            TextMeshProUGUI textTmp = textObj.GetComponent<TextMeshProUGUI>();
            textTmp.color = Color.white;
            RectTransform trt = textObj.GetComponent<RectTransform>();
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            input.textViewport = areaRt;
            input.textComponent = textTmp;
            input.placeholder = placeholderTmp;

            return searchObj;
        }

        private GameObject CreateSliderElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject sliderObj = new GameObject(name, typeof(RectTransform));
            sliderObj.transform.SetParent(parent, false);
            RectTransform rt = sliderObj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;

            // Background
            GameObject bgObj = new GameObject("Background", typeof(RectTransform));
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.35f);
            bgRt.anchorMax = new Vector2(1, 0.65f);
            bgRt.offsetMin = bgRt.offsetMax = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.25f, 0.32f);

            // Fill Area
            GameObject fillArea = new GameObject("FillArea", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform faRt = fillArea.GetComponent<RectTransform>();
            faRt.anchorMin = new Vector2(0, 0.35f);
            faRt.anchorMax = new Vector2(1, 0.65f);
            faRt.offsetMin = new Vector2(5, 0);
            faRt.offsetMax = new Vector2(-5, 0);

            GameObject fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.GetComponent<RectTransform>();
            fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.35f, 0.65f, 1f);

            // Handle Slide Area
            GameObject handleArea = new GameObject("HandleSlideArea", typeof(RectTransform));
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform haRt = handleArea.GetComponent<RectTransform>();
            haRt.anchorMin = Vector2.zero;
            haRt.anchorMax = Vector2.one;
            haRt.offsetMin = new Vector2(10, 0);
            haRt.offsetMax = new Vector2(-10, 0);

            GameObject handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(16, 16);
            Image handleImg = handle.AddComponent<Image>();
            handleImg.color = Color.white;

            slider.fillRect = fillRt;
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;
            slider.direction = Slider.Direction.LeftToRight;

            return sliderObj;
        }

        private Button CreateToggleButton(string name, Transform parent, string label, Color bgColor, Color dotColor)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform));
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();

            // Dot Indicator
            GameObject dotObj = new GameObject("Indicator", typeof(RectTransform));
            dotObj.transform.SetParent(btnObj.transform, false);
            RectTransform dotRt = dotObj.GetComponent<RectTransform>();
            dotRt.anchorMin = dotRt.anchorMax = new Vector2(0, 0.5f);
            dotRt.anchoredPosition = new Vector2(12, 0);
            dotRt.sizeDelta = new Vector2(8, 8);
            Image dotImg = dotObj.AddComponent<Image>();
            dotImg.color = dotColor;

            // Label
            GameObject textObj = CreateTextElement("Label", btnObj.transform, label, 11, TextAlignmentOptions.Left,
                new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.offsetMin = new Vector2(24, 0);
            textRt.offsetMax = Vector2.zero;

            return btn;
        }

        private GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = panel.AddComponent<Image>();
            img.color = color;
            return panel;
        }

        private GameObject CreateTextElement(string name, Transform parent, string text, float fontSize, TextAlignmentOptions alignment,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform));
            textObj.transform.SetParent(parent, false);
            RectTransform rt = textObj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = Color.white;
            return textObj;
        }

        private Button CreateButton(string name, Transform parent, string label, float fontSize, Color normalColor)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform));
            btnObj.transform.SetParent(parent, false);
            RectTransform rt = btnObj.GetComponent<RectTransform>();

            Image img = btnObj.AddComponent<Image>();
            img.color = normalColor;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = normalColor;
            cb.highlightedColor = normalColor * 1.25f;
            cb.pressedColor = normalColor * 0.8f;
            btn.colors = cb;

            GameObject textObj = CreateTextElement("Label", btnObj.transform, label, fontSize, TextAlignmentOptions.Center,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            return btn;
        }
        #endregion
    }
}

