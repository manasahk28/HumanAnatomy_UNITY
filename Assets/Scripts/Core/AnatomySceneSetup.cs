using UnityEngine;

namespace HumanAnatomy
{
    [DefaultExecutionOrder(-100)]
    public class AnatomySceneSetup : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitializeScene()
        {
            // If already initialized, return
            if (FindAnyObjectByType<AnatomyExplorerManager>() != null) return;

            Debug.Log("<color=cyan>[HumanAnatomy]</color> Auto-initializing Human Anatomy Environment...");

            // 1. Setup Camera
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f); // Professional dark slate medical background
                cam.fieldOfView = 50f;
                cam.nearClipPlane = 0.1f;
                cam.farClipPlane = 100f;

                OrbitCameraController orbit = cam.GetComponent<OrbitCameraController>();
                if (orbit == null) orbit = cam.gameObject.AddComponent<OrbitCameraController>();
                orbit.distance = 2.5f;
                orbit.minDistance = 0.4f;
                orbit.maxDistance = 5.0f;
                orbit.targetOffset = new Vector3(0, 0.95f, 0);
            }

            // 2. Setup Explorer Manager
            GameObject mgrObj = new GameObject("HumanAnatomy_Manager");
            AnatomyExplorerManager manager = mgrObj.AddComponent<AnatomyExplorerManager>();
            if (cam != null) manager.mainCamera = cam;

            // 3. Setup Scene Builder and generate body
            AnatomySceneBuilder builder = mgrObj.AddComponent<AnatomySceneBuilder>();
            builder.buildOnStart = true;
            builder.includeMuscles = true;
            builder.includeNervous = true;
            builder.BuildAnatomyModel();

            // 4. Ensure optimal lighting for 3D anatomy inspection
            SetupSceneLighting();
        }

        private static void SetupSceneLighting()
        {
            // Key Light
            Light[] lights = FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            Light keyLight = null;
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    keyLight = l;
                    break;
                }
            }

            if (keyLight != null)
            {
                keyLight.transform.rotation = Quaternion.Euler(45f, 40f, 0f);
                keyLight.color = new Color(1f, 0.98f, 0.95f);
                keyLight.intensity = 1.3f;
            }

            // Back/Rim Light for silhouette contrast
            GameObject rimObj = new GameObject("Rim_Light");
            Light rimLight = rimObj.AddComponent<Light>();
            rimLight.type = LightType.Directional;
            rimLight.transform.rotation = Quaternion.Euler(-30f, -140f, 0f);
            rimLight.color = new Color(0.7f, 0.85f, 1f);
            rimLight.intensity = 0.8f;
        }
    }
}

