using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    [SelectionBase]
    public class AnatomyPart : MonoBehaviour
    {
        [Header("Anatomy Metadata")]
        public AnatomyPartData data = new AnatomyPartData();

        [Header("Visual Settings")]
        public Color baseColor = Color.white;
        public Color hoverColor = Color.white;
        public Color selectedColor = new Color(1.0f, 0.95f, 0.2f);
        public Color dimmedColor = new Color(0.25f, 0.25f, 0.25f, 0.25f);

        [Header("Animation Options")]
        public bool hasHeartbeatPulse = false;
        public float pulseSpeed = 1.2f;
        public float pulseMagnitude = 0.04f;

        [Header("Exploded View")]
        public Vector3 explodedOffset = Vector3.zero;

        private List<MeshRenderer> allRenderers = new List<MeshRenderer>();
        private List<Material> runtimeMaterials = new List<Material>();
        private Vector3 initialLocalPosition;
        private Vector3 initialLocalScale;

        private bool isHovered = false;
        private bool isSelected = false;
        private bool isDimmed = false;
        private float explodedProgress = 0f;

        public bool IsSelected => isSelected;
        public bool IsHovered => isHovered;
        public Vector3 InitialLocalPosition => initialLocalPosition;

        private void Awake()
        {
            initialLocalPosition = transform.localPosition;
            initialLocalScale = transform.localScale;
            InitializeMaterials();
        }

        private void Start()
        {
            // If child meshes were added after Awake, refresh
            if (allRenderers.Count == 0)
            {
                InitializeMaterials();
            }
        }

        public void InitializeMaterials()
        {
            allRenderers.Clear();
            allRenderers.AddRange(GetComponentsInChildren<MeshRenderer>(true));

            foreach (var r in allRenderers)
            {
                if (r == null) continue;
                Material sourceMat = r.sharedMaterial;
                Material runtimeMat;
                if (sourceMat != null)
                {
                    runtimeMat = new Material(sourceMat);
                }
                else
                {
                    Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
                    if (litShader == null) litShader = Shader.Find("Standard");
                    runtimeMat = new Material(litShader);
                }

                runtimeMat.name = $"{r.gameObject.name}_RuntimeMat";
                runtimeMat.color = baseColor;
                runtimeMat.SetColor("_BaseColor", baseColor);
                runtimeMat.SetFloat("_Cull", 0); // Double-sided rendering so it's always visible!
                r.material = runtimeMat;
                runtimeMaterials.Add(runtimeMat);
            }
        }

        private void Update()
        {
            // Heartbeat or subtle living motion
            if (hasHeartbeatPulse)
            {
                float pulse = 1f + Mathf.Sin(Time.time * Mathf.PI * 2f * pulseSpeed) * pulseMagnitude;
                transform.localScale = initialLocalScale * pulse;
            }

            // Handle exploded view transition smoothly
            Vector3 targetPos = initialLocalPosition + (explodedOffset * explodedProgress);
            if (Vector3.Distance(transform.localPosition, targetPos) > 0.001f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 6f);
            }
        }

        public void SetExplodedProgress(float t)
        {
            explodedProgress = Mathf.Clamp01(t);
        }

        public void SetHovered(bool hovered)
        {
            if (isHovered == hovered) return;
            isHovered = hovered;
            UpdateVisualState();
        }

        public void SetSelected(bool selected)
        {
            if (isSelected == selected) return;
            isSelected = selected;
            UpdateVisualState();
        }

        public void SetDimmed(bool dimmed)
        {
            if (isDimmed == dimmed) return;
            isDimmed = dimmed;
            UpdateVisualState();
        }

        public void SetVisibility(bool visible)
        {
            foreach (var r in allRenderers)
            {
                if (r != null) r.enabled = visible;
            }

            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var col in colliders)
            {
                if (col != null) col.enabled = visible;
            }
        }

        private void UpdateVisualState()
        {
            Color targetCol;
            Color emitCol;

            if (isSelected)
            {
                targetCol = selectedColor;
                emitCol = selectedColor * 0.45f;
            }
            else if (isHovered)
            {
                targetCol = hoverColor;
                emitCol = hoverColor * 0.35f;
            }
            else if (isDimmed)
            {
                targetCol = dimmedColor;
                emitCol = Color.black;
            }
            else
            {
                targetCol = baseColor;
                emitCol = Color.black;
            }

            foreach (var mat in runtimeMaterials)
            {
                if (mat == null) continue;
                mat.color = targetCol;
                mat.SetColor("_BaseColor", targetCol);

                if (emitCol != Color.black)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", emitCol);
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        public Bounds GetBounds()
        {
            if (allRenderers.Count > 0)
            {
                Bounds b = allRenderers[0].bounds;
                for (int i = 1; i < allRenderers.Count; i++)
                {
                    if (allRenderers[i] != null) b.Encapsulate(allRenderers[i].bounds);
                }
                return b;
            }
            return new Bounds(transform.position, Vector3.one * 0.2f);
        }

        private void OnDestroy()
        {
            foreach (var m in runtimeMaterials)
            {
                if (m != null) Destroy(m);
            }
            runtimeMaterials.Clear();
        }
    }
}
