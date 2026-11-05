using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomyRespiratoryGenerator
    {
        public static GameObject BuildFullRespiratorySystem(Transform parent, Shader litShader)
        {
            GameObject respRoot = new GameObject("RespiratorySystem_Realistic");
            respRoot.transform.SetParent(parent, false);

            // Educational respiratory color palette
            Material matTrachea    = CreateRespMaterial(litShader, "Mat_TracheaCartilage", new Color(0.75f, 0.88f, 0.95f)); // Hyaline Cartilage
            Material matBronchi    = CreateRespMaterial(litShader, "Mat_BronchialTree",     new Color(0.30f, 0.75f, 0.85f)); // Cyan Branching Airway
            Material matRightLung  = CreateRespMaterial(litShader, "Mat_RightLung",          new Color(0.92f, 0.58f, 0.62f)); // Pulmonary Rose Pink
            Material matLeftLung   = CreateRespMaterial(litShader, "Mat_LeftLung",           new Color(0.88f, 0.52f, 0.58f)); // Pulmonary Rose
            Material matPleura     = CreateRespMaterial(litShader, "Mat_PleuralParenchyma",  new Color(0.95f, 0.65f, 0.70f)); // Vascular Parenchyma

            // 1. Larynx, Trachea, and Cartilage Rings
            BuildTracheaAndLarynx(respRoot.transform, matTrachea);

            // 2. Bronchial Tree (Carina, Primary, Secondary & Tertiary Bronchi)
            BuildBronchialTree(respRoot.transform, matBronchi);

            // 3. Right Lung (3 Distinct Anatomical Lobes: Superior, Middle, Inferior)
            BuildRightLung(respRoot.transform, matRightLung);

            // 4. Left Lung (2 Lobes: Superior with Cardiac Notch & Lingula, Inferior)
            BuildLeftLung(respRoot.transform, matLeftLung);

            return respRoot;
        }

        #region 1. Larynx & Trachea
        private static void BuildTracheaAndLarynx(Transform parent, Material mat)
        {
            GameObject tracheaObj = CreateCompositeResp(parent, "Respiratory_Trachea_Larynx",
                new Vector3(0, 1.50f, 0.015f), mat,
                "Larynx & Trachea", "Larynx & Trachea",
                "Primary conductive airway conveying atmospheric air from upper respiratory tract down through the neck into thoracic cavity.",
                "Reinforced by 16-20 C-shaped hyaline cartilage rings connected posteriorly by trachealis muscle. Spans from cricoid cartilage (C6) to the carina bifurcation (T4/T5).",
                new Vector3(0, 0.25f, 0.35f),
                "Thyroid cartilage forms the visible laryngeal prominence ('Adam's apple').",
                "Tracheostomy or cricothyroidotomy performed here during acute upper airway obstruction.",
                "Lined with pseudostratified ciliated columnar epithelium maintaining mucociliary escalator.");

            // Larynx / Thyroid Cartilage
            AddPrimitiveChild(tracheaObj.transform, PrimitiveType.Cylinder, "Larynx_ThyroidCartilage",
                new Vector3(0, 0.075f, 0.005f), new Vector3(0.038f, 0.035f, 0.035f), Quaternion.identity, mat);
            // Cricoid Cartilage
            AddPrimitiveChild(tracheaObj.transform, PrimitiveType.Cylinder, "Larynx_CricoidRing",
                new Vector3(0, 0.045f, 0.002f), new Vector3(0.032f, 0.018f, 0.032f), Quaternion.identity, mat);

            // Tracheal Main Column
            AddPrimitiveChild(tracheaObj.transform, PrimitiveType.Cylinder, "Trachea_MainStem",
                new Vector3(0, -0.025f, -0.005f), new Vector3(0.028f, 0.095f, 0.028f), Quaternion.identity, mat);

            // Cartilage Rings (Visual ridges)
            for (int i = 0; i < 5; i++)
            {
                float y = 0.02f - i * 0.022f;
                AddPrimitiveChild(tracheaObj.transform, PrimitiveType.Cylinder, $"Cartilage_Ring_{i}",
                    new Vector3(0, y, -0.003f), new Vector3(0.030f, 0.006f, 0.030f), Quaternion.identity, mat);
            }
        }
        #endregion

        #region 2. Bronchial Tree
        private static void BuildBronchialTree(Transform parent, Material mat)
        {
            GameObject bronchObj = CreateCompositeResp(parent, "Respiratory_BronchialTree",
                new Vector3(0, 1.40f, -0.005f), mat,
                "Bronchial Tree & Carina", "Arbor Bronchialis",
                "Extensive branching network of airways distributing tidal airflow into bronchopulmonary segments of both lungs.",
                "Originates at the sensitive tracheal carina (T4/T5). Right main bronchus is wider, shorter (2.5 cm), and more vertical (25°), while left main bronchus is narrower, longer (5 cm), and more horizontal (45°).",
                new Vector3(0, 0.2f, 0.45f),
                "Aspirated foreign objects predominantly lodge in the more vertical right main bronchus.",
                "Bronchial smooth muscle is innervated by parasympathetic vagus (bronchoconstriction) and sympathetic beta-2 receptors (bronchodilation).",
                "Target of inhaled beta-2 agonists (albuterol) during acute asthma attacks.");

            // Carina bifurcation node
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Sphere, "Carina_Node",
                new Vector3(0, 0.035f, 0), new Vector3(0.026f, 0.022f, 0.026f), Quaternion.identity, mat);

            // Right Main Bronchus (More vertical, steeper angle)
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Right_Main_Bronchus",
                new Vector3(0.025f, 0.015f, -0.005f), new Vector3(0.018f, 0.045f, 0.018f), Quaternion.Euler(0, 0, -32f), mat);
            // Right Secondary Lobar Bronchi
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Right_Superior_Lobar",
                new Vector3(0.045f, 0.035f, 0), new Vector3(0.013f, 0.035f, 0.013f), Quaternion.Euler(0, 15f, -65f), mat);
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Right_Middle_Lobar",
                new Vector3(0.052f, 0.005f, 0.012f), new Vector3(0.012f, 0.035f, 0.012f), Quaternion.Euler(-25f, 0, -45f), mat);
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Right_Inferior_Lobar",
                new Vector3(0.045f, -0.025f, -0.01f), new Vector3(0.014f, 0.045f, 0.014f), Quaternion.Euler(15f, 0, -25f), mat);

            // Left Main Bronchus (More horizontal, longer path around heart)
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Left_Main_Bronchus",
                new Vector3(-0.032f, 0.018f, -0.008f), new Vector3(0.016f, 0.058f, 0.016f), Quaternion.Euler(0, 0, 48f), mat);
            // Left Secondary Lobar Bronchi
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Left_Superior_Lobar",
                new Vector3(-0.055f, 0.035f, 0.005f), new Vector3(0.013f, 0.038f, 0.013f), Quaternion.Euler(0, -15f, 65f), mat);
            AddPrimitiveChild(bronchObj.transform, PrimitiveType.Cylinder, "Left_Inferior_Lobar",
                new Vector3(-0.052f, -0.022f, -0.012f), new Vector3(0.014f, 0.048f, 0.014f), Quaternion.Euler(20f, 0, 35f), mat);
        }
        #endregion

        #region 3. Right Lung (3 Lobes)
        private static void BuildRightLung(Transform parent, Material mat)
        {
            GameObject lungRoot = new GameObject("Respiratory_RightLung_Complete");
            lungRoot.transform.SetParent(parent, false);
            lungRoot.transform.localPosition = new Vector3(0.11f, 1.34f, 0.01f);

            // A. Right Superior Lobe
            GameObject supObj = CreateCompositeResp(lungRoot.transform, "RightLung_SuperiorLobe",
                new Vector3(0, 0.065f, 0), mat,
                "Right Lung — Superior Lobe", "Lobus Superior Pulmonis Dextri",
                "Uppermost lobe of the right lung reaching above the first rib into root of the neck (cervical pleura).",
                "Separated from middle lobe by the horizontal fissure and from inferior lobe by the oblique fissure. Comprises apical, anterior, and posterior bronchopulmonary segments.",
                new Vector3(0.45f, 0.25f, 0.25f),
                "Apical lung tumors (Pancoast tumors) compress the sympathetic trunk, producing Horner's syndrome (ptosis, miosis, anhidrosis).",
                "Common site for reactivation pulmonary tuberculosis due to high ventilation-perfusion ratio.");

            AddPrimitiveChild(supObj.transform, PrimitiveType.Sphere, "R_Sup_Apex",
                new Vector3(0, 0.02f, 0), new Vector3(0.088f, 0.095f, 0.082f), Quaternion.Euler(0, 0, -8f), mat);

            // B. Right Middle Lobe
            GameObject midObj = CreateCompositeResp(lungRoot.transform, "RightLung_MiddleLobe",
                new Vector3(0.005f, -0.008f, 0.025f), mat,
                "Right Lung — Middle Lobe", "Lobus Medius Pulmonis Dextri",
                "Wedge-shaped anterior lobe bounded superiorly by the horizontal fissure and inferiorly by the oblique fissure.",
                "Comprises lateral and medial bronchopulmonary segments. Projects against anterior chest wall between 4th and 6th costal cartilages.",
                new Vector3(0.55f, 0, 0.45f),
                "Right middle lobe syndrome is recurrent atelectasis or bronchiectasis caused by lymph node compression of its narrow lobar bronchus.");

            AddPrimitiveChild(midObj.transform, PrimitiveType.Sphere, "R_Mid_Body",
                new Vector3(0, 0, 0), new Vector3(0.075f, 0.065f, 0.068f), Quaternion.Euler(-12f, 0, -5f), mat);

            // C. Right Inferior Lobe
            GameObject infObj = CreateCompositeResp(lungRoot.transform, "RightLung_InferiorLobe",
                new Vector3(-0.005f, -0.065f, -0.015f), mat,
                "Right Lung — Inferior Lobe", "Lobus Inferior Pulmonis Dextri",
                "Largest, most posterior lobe of the right lung, resting on the convex right dome of the diaphragm directly above the liver.",
                "Separated from superior and middle lobes by the oblique fissure. Comprises superior, medial basal, anterior basal, lateral basal, and posterior basal segments.",
                new Vector3(0.5f, -0.25f, -0.2f),
                "Most frequent location of community-acquired aspiration pneumonia in recumbent or hospitalized patients.",
                "Auscultated predominantly over the posterior thoracic wall below the scapular spine.");

            AddPrimitiveChild(infObj.transform, PrimitiveType.Sphere, "R_Inf_Base",
                new Vector3(0, 0, 0), new Vector3(0.098f, 0.115f, 0.092f), Quaternion.Euler(10f, 0, -5f), mat);
        }
        #endregion

        #region 4. Left Lung (2 Lobes, Cardiac Notch & Lingula)
        private static void BuildLeftLung(Transform parent, Material mat)
        {
            GameObject lungRoot = new GameObject("Respiratory_LeftLung_Complete");
            lungRoot.transform.SetParent(parent, false);
            lungRoot.transform.localPosition = new Vector3(-0.11f, 1.34f, 0.01f);

            // A. Left Superior Lobe with Cardiac Notch & Lingula
            GameObject supObj = CreateCompositeResp(lungRoot.transform, "LeftLung_SuperiorLobe",
                new Vector3(0, 0.045f, 0.01f), mat,
                "Left Lung — Superior Lobe (Cardiac Notch & Lingula)", "Lobus Superior Pulmonis Sinistri",
                "Upper lobe of left lung characterized by the deep anterior Cardiac Notch accommodating the left ventricular apex of the heart.",
                "Features the tongue-shaped Lingula on its anteroinferior margin (developmental homologue of the right middle lobe). Separated from inferior lobe by the oblique fissure.",
                new Vector3(-0.45f, 0.25f, 0.25f),
                "Left lung volume is roughly 10% smaller than right lung to accommodate cardiac silhouette.",
                "Cardiac notch allows direct pericardiocentesis access without traversing lung parenchyma.");

            // Superior apex
            AddPrimitiveChild(supObj.transform, PrimitiveType.Sphere, "L_Sup_Apex",
                new Vector3(0, 0.035f, 0), new Vector3(0.082f, 0.092f, 0.078f), Quaternion.Euler(0, 0, 8f), mat);

            // Cardiac notch concavity contour and Lingula
            AddPrimitiveChild(supObj.transform, PrimitiveType.Sphere, "L_Lingula_Projection",
                new Vector3(0.022f, -0.035f, 0.022f), new Vector3(0.045f, 0.055f, 0.042f), Quaternion.Euler(-15f, 15f, 12f), mat);

            // B. Left Inferior Lobe
            GameObject infObj = CreateCompositeResp(lungRoot.transform, "LeftLung_InferiorLobe",
                new Vector3(0, -0.060f, -0.015f), mat,
                "Left Lung — Inferior Lobe", "Lobus Inferior Pulmonis Sinistri",
                "Posterior basal lobe of the left lung resting on the left dome of the diaphragm above the stomach and spleen.",
                "Separated from the superior lobe by the steep oblique fissure running from T3/T4 level posteriorly down to 6th costochondral junction.",
                new Vector3(-0.5f, -0.25f, -0.2f),
                "Houses extensive alveolar capillary network facilitating O2/CO2 passive gas exchange across the 0.5-micrometer alveolar-capillary membrane.",
                "Total human alveolar surface area approximates 70 to 100 square meters (half a tennis court).");

            AddPrimitiveChild(infObj.transform, PrimitiveType.Sphere, "L_Inf_Base",
                new Vector3(0, 0, 0), new Vector3(0.092f, 0.110f, 0.088f), Quaternion.Euler(10f, 0, 8f), mat);
        }
        #endregion

        #region Helpers
        private static GameObject CreateCompositeResp(
            Transform parent, string name, Vector3 localPos, Material mat,
            string partName, string latin, string function, string description,
            Vector3 explodeOffset, params string[] clinicalFacts)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;

            AnatomyPart part = obj.AddComponent<AnatomyPart>();
            part.data = new AnatomyPartData
            {
                partName = partName,
                scientificName = latin,
                systemType = AnatomySystemType.Respiratory,
                keyFunction = function,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat.color;
            part.hoverColor = Color.white;
            part.selectedColor = new Color(0.25f, 0.95f, 0.75f); // Brilliant Mint/Teal selection
            part.explodedOffset = explodeOffset;

            return obj;
        }

        private static GameObject AddPrimitiveChild(
            Transform parent, PrimitiveType primitiveType, string childName,
            Vector3 localPos, Vector3 localScale, Quaternion localRot, Material mat)
        {
            GameObject child = GameObject.CreatePrimitive(primitiveType);
            child.name = childName;
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPos;
            child.transform.localScale = localScale;
            child.transform.localRotation = localRot;

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = mat;

            return child;
        }

        private static Material CreateRespMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.45f);
            mat.SetFloat("_Cull", 0); // Double-sided rendering
            return mat;
        }
        #endregion
    }
}

