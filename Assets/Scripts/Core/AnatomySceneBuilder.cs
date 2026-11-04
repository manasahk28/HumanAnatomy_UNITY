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
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            AnatomyUIManager uiManager = canvasObj.AddComponent<AnatomyUIManager>();

            // Title Banner
            GameObject titleObj = CreateTextElement("TitleText", canvasObj.transform,
                "<b>HUMAN ANATOMY 3D — SKELETAL, MUSCULAR & NERVOUS SYSTEMS</b>", 17, TextAlignmentOptions.TopLeft,
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -25), new Vector2(620, 45));

            // System Filter Buttons Panel
            GameObject filterPanel = CreateUIPanel("FilterPanel", canvasObj.transform,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -35), new Vector2(920, 45),
                new Color(0.1f, 0.12f, 0.16f, 0.85f));

            HorizontalLayoutGroup hlg = filterPanel.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.padding = new RectOffset(6, 6, 6, 6);

            string[] labels = { "All Systems", "Skeleton Only", "Muscles Only", "Nervous Only", "Brain & Cord", "Arm Nerves", "Leg Nerves" };
            string[] modes = { "All", "Skeleton", "Muscles", "Nervous", "BrainCord", "ArmNerves", "LegNerves" };
            for (int i = 0; i < labels.Length; i++)
            {
                string m = modes[i];
                Button btn = CreateButton($"Btn_{m}", filterPanel.transform, labels[i], 12,
                    new Color(0.2f, 0.25f, 0.35f, 0.9f));
                btn.onClick.AddListener(() => {
                    AnatomyExplorerManager mgr = FindAnyObjectByType<AnatomyExplorerManager>();
                    if (mgr != null) mgr.FilterMusculoskeletal(m);
                });
            }

            // Exploded View & Reset Camera Buttons
            GameObject controlPanel = CreateUIPanel("ControlPanel", canvasObj.transform,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-15, -35), new Vector2(260, 45),
                new Color(0.1f, 0.12f, 0.16f, 0.85f));
            HorizontalLayoutGroup chlg = controlPanel.AddComponent<HorizontalLayoutGroup>();
            chlg.spacing = 8;
            chlg.childControlHeight = true;
            chlg.childControlWidth = true;
            chlg.padding = new RectOffset(6, 6, 6, 6);

            Button explodeBtn = CreateButton("Btn_Explode", controlPanel.transform, "Explode", 14, new Color(0.35f, 0.45f, 0.7f));
            Button resetBtn = CreateButton("Btn_ResetCam", controlPanel.transform, "Reset View", 14, new Color(0.35f, 0.45f, 0.7f));
            uiManager.explodedButton = explodeBtn;
            uiManager.resetCamButton = resetBtn;

            // Info Card (Right Side)
            GameObject infoCard = CreateUIPanel("InfoPanel", canvasObj.transform,
                new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20, 0), new Vector2(360, 520),
                new Color(0.08f, 0.10f, 0.14f, 0.92f));
            uiManager.infoPanel = infoCard;

            // Info Card Header
            GameObject headerBg = CreateUIPanel("HeaderBg", infoCard.transform,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -35), new Vector2(340, 60),
                new Color(0.16f, 0.22f, 0.32f, 0.95f));

            uiManager.titleText = CreateTextElement("PartTitle", headerBg.transform, "Organ Name", 20, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, 8), new Vector2(240, 26)).GetComponent<TextMeshProUGUI>();

            uiManager.scientificText = CreateTextElement("ScientificName", headerBg.transform, "(Organum)", 13, TextAlignmentOptions.Left,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, -14), new Vector2(240, 20)).GetComponent<TextMeshProUGUI>();
            uiManager.scientificText.color = new Color(0.7f, 0.85f, 1f, 0.8f);

            // Close button on card
            Button closeBtn = CreateButton("CloseBtn", headerBg.transform, "X", 14, new Color(0.6f, 0.2f, 0.2f));
            RectTransform closeRt = closeBtn.GetComponent<RectTransform>();
            closeRt.anchorMin = closeRt.anchorMax = new Vector2(1, 0.5f);
            closeRt.anchoredPosition = new Vector2(-22, 0);
            closeRt.sizeDelta = new Vector2(32, 32);
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
            Button focusBtn = CreateButton("FocusBtn", infoCard.transform, "Focus Camera", 14, new Color(0.2f, 0.55f, 0.8f));
            RectTransform focusRt = focusBtn.GetComponent<RectTransform>();
            focusRt.anchorMin = focusRt.anchorMax = new Vector2(0.3f, 0.08f);
            focusRt.anchoredPosition = Vector2.zero;
            focusRt.sizeDelta = new Vector2(140, 36);
            uiManager.focusButton = focusBtn;

            Button isolateBtn = CreateButton("IsolateBtn", infoCard.transform, "Isolate Part", 14, new Color(0.3f, 0.65f, 0.4f));
            RectTransform isoRt = isolateBtn.GetComponent<RectTransform>();
            isoRt.anchorMin = isoRt.anchorMax = new Vector2(0.72f, 0.08f);
            isoRt.anchoredPosition = Vector2.zero;
            isoRt.sizeDelta = new Vector2(140, 36);
            uiManager.isolateButton = isolateBtn;

            // Bottom Navigation Hint
            CreateTextElement("HintText", canvasObj.transform,
                "<b>Controls:</b> Left Mouse Drag = Orbit | Right Mouse Drag = Pan | Scroll = Zoom | Left Click = Inspect Organ",
                13, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20), new Vector2(800, 30));
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

