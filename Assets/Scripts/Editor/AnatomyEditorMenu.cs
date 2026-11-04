#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HumanAnatomy.Editor
{
    public static class AnatomyEditorMenu
    {
        [MenuItem("Human Anatomy/Build Anatomy Scene Hierarchy", false, 1)]
        public static void BuildSceneHierarchy()
        {
            // 1. Camera Setup
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
                cam.fieldOfView = 50f;

                OrbitCameraController orbit = cam.GetComponent<OrbitCameraController>();
                if (orbit == null) orbit = cam.gameObject.AddComponent<OrbitCameraController>();
                orbit.distance = 2.5f;
                orbit.minDistance = 0.4f;
                orbit.maxDistance = 5.0f;
                orbit.targetOffset = new Vector3(0, 0.95f, 0);
                EditorUtility.SetDirty(cam.gameObject);
            }

            // 2. Manager & Builder
            GameObject mgrObj = GameObject.Find("HumanAnatomy_Manager");
            if (mgrObj == null)
            {
                mgrObj = new GameObject("HumanAnatomy_Manager");
                Undo.RegisterCreatedObjectUndo(mgrObj, "Create Anatomy Manager");
            }

            AnatomyExplorerManager manager = mgrObj.GetComponent<AnatomyExplorerManager>();
            if (manager == null) manager = mgrObj.AddComponent<AnatomyExplorerManager>();
            if (cam != null) manager.mainCamera = cam;

            AnatomyAudioFeedback audio = mgrObj.GetComponent<AnatomyAudioFeedback>();
            if (audio == null) audio = mgrObj.AddComponent<AnatomyAudioFeedback>();

            AnatomySceneBuilder builder = mgrObj.GetComponent<AnatomySceneBuilder>();
            if (builder == null) builder = mgrObj.AddComponent<AnatomySceneBuilder>();
            builder.includeMuscles = true;
            builder.includeNervous = true;
            builder.includeCirculatory = true;
            builder.includeRespiratory = true;
            builder.includeDigestive = true;

            builder.BuildAnatomyModel();

            // Focus Scene View directly onto the model
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Frame(new Bounds(new Vector3(0, 0.95f, 0), new Vector3(1.5f, 2.0f, 1.5f)), false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("<color=green>[HumanAnatomy]</color> Scene hierarchy built and saved successfully! The Scene view is now framed on the model.");
        }

        [MenuItem("Human Anatomy/Clear Generated Anatomy", false, 2)]
        public static void ClearGeneratedAnatomy()
        {
            GameObject mgrObj = GameObject.Find("HumanAnatomy_Manager");
            if (mgrObj != null)
            {
                Undo.DestroyObjectImmediate(mgrObj);
            }

            GameObject canvasObj = GameObject.Find("AnatomyCanvas");
            if (canvasObj != null)
            {
                Undo.DestroyObjectImmediate(canvasObj);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=yellow>[HumanAnatomy]</color> Generated anatomy objects cleared.");
        }
    }
}
#endif

