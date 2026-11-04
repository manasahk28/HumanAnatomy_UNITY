using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomyDigestiveGenerator
    {
        public static GameObject BuildFullDigestiveSystem(Transform parent, Shader litShader)
        {
            GameObject digRoot = new GameObject("DigestiveSystem_Realistic");
            digRoot.transform.SetParent(parent, false);

            // Educational digestive color palette
            Material matEsophagus   = CreateDigMaterial(litShader, "Mat_Esophagus",       new Color(0.85f, 0.52f, 0.55f)); // Muscular Flesh Pink
            Material matStomach     = CreateDigMaterial(litShader, "Mat_Stomach",         new Color(0.86f, 0.42f, 0.32f)); // Warm Coral Amber
            Material matLiver       = CreateDigMaterial(litShader, "Mat_Liver",           new Color(0.55f, 0.20f, 0.16f)); // Hepatic Mahogany
            Material matGallbladder = CreateDigMaterial(litShader, "Mat_Gallbladder",     new Color(0.24f, 0.62f, 0.22f)); // Biliary Emerald Green
            Material matPancreas    = CreateDigMaterial(litShader, "Mat_Pancreas",        new Color(0.92f, 0.72f, 0.28f)); // Glandular Ochre Yellow
            Material matSmallIntest = CreateDigMaterial(litShader, "Mat_SmallIntestine",  new Color(0.88f, 0.58f, 0.46f)); // Mucosal Peachy Tan
            Material matLargeIntest = CreateDigMaterial(litShader, "Mat_LargeIntestine",  new Color(0.66f, 0.48f, 0.36f)); // Colonic Clay Ochre

            // 1. Esophagus
            BuildEsophagus(digRoot.transform, matEsophagus);

            // 2. Stomach (J-shaped: Cardia, Fundus, Body, Greater/Lesser Curvature, Pylorus)
            BuildStomach(digRoot.transform, matStomach);

            // 3. Liver (Right & Left Lobes, Visceral Demarcation)
            BuildLiver(digRoot.transform, matLiver);

            // 4. Gallbladder & Biliary Ducts
            BuildGallbladder(digRoot.transform, matGallbladder);

            // 5. Pancreas (Head in Duodenal Loop, Body & Tail)
            BuildPancreas(digRoot.transform, matPancreas);

            // 6. Small Intestine (Duodenum C-loop, Convoluted Jejunum & Ileum)
            BuildSmallIntestine(digRoot.transform, matSmallIntest);

            // 7. Large Intestine / Colon & Appendix (Cecum, Ascending, Transverse, Descending, Sigmoid)
            BuildLargeIntestine(digRoot.transform, matLargeIntest);

            return digRoot;
        }

        #region 1. Esophagus
        private static void BuildEsophagus(Transform parent, Material mat)
        {
            GameObject esophObj = CreateCompositeDig(parent, "Digestive_Esophagus",
                new Vector3(0, 1.45f, -0.015f), mat,
                "Esophagus", "Oesophagus",
                "Muscular transit tube (25 cm) propelling swallowed bolus from pharynx through the posterior mediastinum into the stomach via coordinated peristalsis.",
                "Traverses esophageal hiatus of diaphragm at T10 level. Features upper esophageal sphincter (striated muscle) and lower esophageal sphincter (physiological smooth muscle zone).",
                new Vector3(0, 0.2f, -0.3f),
                "Gastroesophageal reflux disease (GERD) results from lower esophageal sphincter (LES) incompetence.",
                "Barrett's esophagus is metaplasia from stratified squamous to intestinal columnar epithelium, predisposing to adenocarcinoma.",
                "Esophageal varices develop in portal hypertension, posing severe risk of life-threatening hemorrhage.");

            AddPrimitiveChild(esophObj.transform, PrimitiveType.Cylinder, "Esophagus_Tube",
                new Vector3(0, 0, 0), new Vector3(0.022f, 0.17f, 0.022f), Quaternion.identity, mat);
        }
        #endregion

        #region 2. Stomach
        private static void BuildStomach(Transform parent, Material mat)
        {
            GameObject stomRoot = new GameObject("Digestive_Stomach_Complete");
            stomRoot.transform.SetParent(parent, false);
            stomRoot.transform.localPosition = new Vector3(-0.045f, 1.26f, 0.035f);

            GameObject stomObj = CreateCompositeDig(stomRoot.transform, "Digestive_Stomach",
                Vector3.zero, mat,
                "Stomach (Cardia, Fundus, Body & Pylorus)", "Gaster / Ventriculus",
                "J-shaped muscular hollow reservoir performing mechanical churn and initial enzymatic digestion of dietary protein into acidic liquid chyme.",
                "Features 4 anatomical regions: Cardia, Fundus (dome beneath left hemidiaphragm), Body (corpus), and Pyloric antrum/canal guarded by the pyloric sphincter. Possesses 3 smooth muscle layers (longitudinal, circular, oblique).",
                new Vector3(-0.45f, 0.15f, 0.35f),
                "Parietal cells secrete hydrochloric acid (HCl, pH 1.5-2.0) and intrinsic factor required for vitamin B12 absorption in the terminal ileum.",
                "Chief cells synthesize inactive pepsinogen, cleaved to active protease pepsin in acidic pH.",
                "Helicobacter pylori infection causes chronic gastritis and peptic ulcer disease.");

            // A. Fundus (Dome superiorly on left)
            AddPrimitiveChild(stomObj.transform, PrimitiveType.Sphere, "Stomach_Fundus",
                new Vector3(-0.022f, 0.038f, -0.01f), new Vector3(0.065f, 0.065f, 0.060f), Quaternion.Euler(0, 0, 20f), mat);

            // B. Body (Corpus - Greater curvature convexity on left)
            AddPrimitiveChild(stomObj.transform, PrimitiveType.Sphere, "Stomach_Body",
                new Vector3(-0.015f, -0.005f, 0), new Vector3(0.082f, 0.095f, 0.075f), Quaternion.Euler(0, -15f, 35f), mat);

            // C. Pyloric Antrum & Canal (Sweeping right across midline to duodenum)
            AddPrimitiveChild(stomObj.transform, PrimitiveType.Cylinder, "Stomach_Pylorus",
                new Vector3(0.035f, -0.032f, 0.008f), new Vector3(0.032f, 0.055f, 0.032f), Quaternion.Euler(0, 0, -68f), mat);
        }
        #endregion

        #region 3. Liver
        private static void BuildLiver(Transform parent, Material mat)
        {
            GameObject liverRoot = new GameObject("Digestive_Liver_Complete");
            liverRoot.transform.SetParent(parent, false);
            liverRoot.transform.localPosition = new Vector3(0.045f, 1.25f, 0.025f);

            GameObject liverObj = CreateCompositeDig(liverRoot.transform, "Digestive_Liver",
                Vector3.zero, mat,
                "Liver (Right & Left Lobes)", "Hepar",
                "Largest internal organ and primary metabolic factory (~1.5 kg). Synthesizes bile salts, plasma proteins (albumin, clotting factors), detoxifies metabolites, and stores glycogen.",
                "Divided into large Right Lobe and flatter Left Lobe by the falciform ligament. Receives dual blood supply: 75% deoxygenated nutrient-rich venous blood via Hepatic Portal Vein and 25% oxygenated blood via Hepatic Artery.",
                new Vector3(0.45f, 0.2f, 0.35f),
                "Has extraordinary regenerative capacity; resects up to 70% can regenerate to full functional volume within weeks.",
                "Cirrhosis produces portal hypertension leading to ascites, splenomegaly, and portosystemic encephalopathy.",
                "Couinaud classification divides liver into 8 functionally independent vascular segments.");

            // Right Lobe (Massive convex dome fitting under right diaphragm)
            AddPrimitiveChild(liverObj.transform, PrimitiveType.Sphere, "Liver_RightLobe",
                new Vector3(0.025f, 0.015f, 0), new Vector3(0.125f, 0.105f, 0.115f), Quaternion.Euler(-10f, 15f, -15f), mat);

            // Left Lobe (Extending across midline anterior to stomach)
            AddPrimitiveChild(liverObj.transform, PrimitiveType.Sphere, "Liver_LeftLobe",
                new Vector3(-0.065f, 0.005f, 0.015f), new Vector3(0.085f, 0.065f, 0.075f), Quaternion.Euler(-5f, -20f, 10f), mat);

            // Inferior visceral border
            AddPrimitiveChild(liverObj.transform, PrimitiveType.Sphere, "Liver_InferiorMargin",
                new Vector3(0.01f, -0.035f, 0.015f), new Vector3(0.11f, 0.045f, 0.08f), Quaternion.identity, mat);
        }
        #endregion

        #region 4. Gallbladder & Biliary System
        private static void BuildGallbladder(Transform parent, Material mat)
        {
            GameObject gbObj = CreateCompositeDig(parent, "Digestive_Gallbladder",
                new Vector3(0.042f, 1.205f, 0.045f), mat,
                "Gallbladder & Biliary Ducts", "Vesica Biliaris / Fellea",
                "Pear-shaped muscular sac (30-50 mL) nestled on visceral fossa of right liver lobe, concentrating and storing hepatic bile.",
                "Contracts in response to Cholecystokinin (CCK) released by duodenal I-cells upon fatty food ingestion, expelling concentrated bile through cystic duct into Common Bile Duct (CBD).",
                new Vector3(0.35f, -0.1f, 0.45f),
                "Cholelithiasis (gallstones) forms from cholesterol supersaturation or bilirubin precipitation.",
                "Murphy's sign (inspiratory arrest on deep palpation of right subcostal margin) indicates acute cholecystitis.",
                "Common Bile Duct unites with Main Pancreatic Duct at hepatopancreatic ampulla (ampulla of Vater), guarded by sphincter of Oddi.");

            // Gallbladder fundus & body
            AddPrimitiveChild(gbObj.transform, PrimitiveType.Sphere, "Gallbladder_Body",
                new Vector3(0, 0, 0), new Vector3(0.024f, 0.042f, 0.024f), Quaternion.Euler(35f, -15f, -20f), mat);
            // Cystic Duct & CBD
            AddPrimitiveChild(gbObj.transform, PrimitiveType.Cylinder, "Biliary_Duct",
                new Vector3(-0.012f, 0.018f, -0.01f), new Vector3(0.006f, 0.035f, 0.006f), Quaternion.Euler(-25f, 30f, 35f), mat);
        }
        #endregion

        #region 5. Pancreas
        private static void BuildPancreas(Transform parent, Material mat)
        {
            GameObject pancObj = CreateCompositeDig(parent, "Digestive_Pancreas",
                new Vector3(-0.015f, 1.20f, -0.005f), mat,
                "Pancreas (Head, Body & Tail)", "Pancreas",
                "Elongated retroperitoneal dual gland combining exocrine digestive enzyme secretion with endocrine glucose regulation.",
                "Head is cradled snugly within the C-loop of the duodenum; elongated body and tail cross anterior to spine, terminating at splenic hilum.",
                new Vector3(0, 0.1f, -0.35f),
                "Exocrine acinar cells secrete digestive enzymes: pancreatic lipase (fats), amylase (starches), and inactive trypsinogen/chymotrypsinogen (proteins).",
                "Ductal epithelial cells secrete abundant alkaline bicarbonate fluid (pH 8.0) neutralizing acidic gastric chyme.",
                "Endocrine Islets of Langerhans contain beta cells (insulin) and alpha cells (glucagon); autoimmune beta cell destruction causes Type 1 Diabetes Mellitus.");

            // Pancreatic Head (In duodenal C-loop)
            AddPrimitiveChild(pancObj.transform, PrimitiveType.Sphere, "Pancreas_Head",
                new Vector3(0.030f, -0.008f, 0.008f), new Vector3(0.038f, 0.045f, 0.032f), Quaternion.identity, mat);
            // Pancreatic Body
            AddPrimitiveChild(pancObj.transform, PrimitiveType.Cylinder, "Pancreas_Body",
                new Vector3(-0.015f, 0.008f, 0), new Vector3(0.024f, 0.075f, 0.022f), Quaternion.Euler(0, 0, 78f), mat);
            // Pancreatic Tail (reaching toward spleen)
            AddPrimitiveChild(pancObj.transform, PrimitiveType.Sphere, "Pancreas_Tail",
                new Vector3(-0.062f, 0.022f, -0.008f), new Vector3(0.022f, 0.022f, 0.022f), Quaternion.identity, mat);
        }
        #endregion

        #region 6. Small Intestine
        private static void BuildSmallIntestine(Transform parent, Material mat)
        {
            GameObject siRoot = new GameObject("Digestive_SmallIntestine_Complete");
            siRoot.transform.SetParent(parent, false);
            siRoot.transform.localPosition = new Vector3(0, 1.10f, 0.025f);

            GameObject siObj = CreateCompositeDig(siRoot.transform, "Digestive_SmallIntestine",
                Vector3.zero, mat,
                "Small Intestine (Duodenum, Jejunum, Ileum)", "Intestinum Tenue",
                "Primary site of chemical enzymatic digestion and nutrient absorption (~6 meters in living adult).",
                "Consists of 3 contiguous segments: Duodenum (25 cm C-loop receiving bile and pancreatic juice), Jejunum (~2.5 m, thick-walled with prominent plicae circulares), and Ileum (~3.5 m, contains Peyer's patches lymphoid tissue, ending at ileocecal valve).",
                new Vector3(0, -0.2f, 0.45f),
                "Mucosal villi and microvilli expand internal absorptive surface area to over 250 square meters (~tennis court).",
                "Celiac disease triggers autoimmune enteropathy destroying small bowel villi upon gluten ingestion.",
                "Terminal ileum specifically absorbs conjugated bile acids and intrinsic factor-bound Vitamin B12.");

            // Duodenum C-Loop
            AddPrimitiveChild(siObj.transform, PrimitiveType.Cylinder, "Duodenum_Loop",
                new Vector3(0.042f, 0.085f, -0.005f), new Vector3(0.026f, 0.085f, 0.026f), Quaternion.Euler(0, 0, 25f), mat);

            // Convoluted Jejunal and Ileal Loops (Central Abdomen)
            Vector3[] loopOffsets = {
                new Vector3(-0.035f, 0.035f, 0.015f),
                new Vector3(0.025f, 0.025f, 0.020f),
                new Vector3(-0.020f, -0.010f, 0.025f),
                new Vector3(0.030f, -0.020f, 0.018f),
                new Vector3(-0.040f, -0.045f, 0.012f),
                new Vector3(0.015f, -0.055f, 0.015f)
            };

            for (int i = 0; i < loopOffsets.Length; i++)
            {
                AddPrimitiveChild(siObj.transform, PrimitiveType.Capsule, $"Intestinal_Loop_{i}",
                    loopOffsets[i], new Vector3(0.040f, 0.065f, 0.038f),
                    Quaternion.Euler(Random.Range(-25f, 25f), Random.Range(-30f, 30f), Random.Range(35f, 145f)), mat);
            }
        }
        #endregion

        #region 7. Large Intestine / Colon & Appendix
        private static void BuildLargeIntestine(Transform parent, Material mat)
        {
            GameObject liRoot = new GameObject("Digestive_LargeIntestine_Complete");
            liRoot.transform.SetParent(parent, false);
            liRoot.transform.localPosition = new Vector3(0, 1.10f, 0.015f);

            GameObject liObj = CreateCompositeDig(liRoot.transform, "Digestive_LargeIntestine",
                Vector3.zero, mat,
                "Large Intestine (Colon, Cecum, Appendix & Rectum)", "Intestinum Crassum",
                "Terminal gastrointestinal conduit (~1.5 m) framing small intestine; absorbs water and electrolytes, ferments indigestible fiber via symbiotic microbiome, and forms stool.",
                "Characterized by 3 longitudinal smooth muscle ribbons (taeniae coli), sacculations (haustra), and fat tags (appendices epiploicae).",
                new Vector3(0, -0.15f, 0.4f),
                "Vermiform appendix extends from cecum; luminal obstruction causes acute appendicitis (McBurney's point tenderness).",
                "Colorectal adenocarcinoma arises predominantly from adenomatous polyps; screened by screening colonoscopy.",
                "Diverticulosis occurs from herniation of colonic mucosa through circular muscle layer at sites of penetrating nutrient vessels.");

            // A. Cecum & Appendix (Right lower quadrant / Iliac fossa)
            AddPrimitiveChild(liObj.transform, PrimitiveType.Sphere, "Colon_Cecum",
                new Vector3(0.082f, -0.11f, 0), new Vector3(0.055f, 0.065f, 0.052f), Quaternion.identity, mat);
            // Vermiform Appendix
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Appendix",
                new Vector3(0.095f, -0.145f, 0.008f), new Vector3(0.008f, 0.045f, 0.008f), Quaternion.Euler(0, 0, -35f), mat);

            // B. Ascending Colon (Right flank up to hepatic flexure)
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Ascending",
                new Vector3(0.082f, -0.01f, 0), new Vector3(0.048f, 0.16f, 0.048f), Quaternion.identity, mat);

            // C. Transverse Colon (Spanning across anterior abdomen beneath stomach)
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Transverse",
                new Vector3(0, 0.075f, 0.025f), new Vector3(0.045f, 0.17f, 0.045f), Quaternion.Euler(0, 0, 90f), mat);

            // D. Descending Colon (Left flank from splenic flexure down to pelvic brim)
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Descending",
                new Vector3(-0.085f, -0.01f, 0), new Vector3(0.048f, 0.16f, 0.048f), Quaternion.identity, mat);

            // E. Sigmoid Colon & Rectum (S-shaped curve into pelvis)
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Sigmoid",
                new Vector3(-0.045f, -0.115f, -0.01f), new Vector3(0.045f, 0.095f, 0.045f), Quaternion.Euler(0, 0, 45f), mat);
            AddPrimitiveChild(liObj.transform, PrimitiveType.Cylinder, "Colon_Rectum",
                new Vector3(0, -0.155f, -0.02f), new Vector3(0.042f, 0.065f, 0.042f), Quaternion.identity, mat);
        }
        #endregion

        #region Helpers
        private static GameObject CreateCompositeDig(
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
                systemType = AnatomySystemType.Digestive,
                keyFunction = function,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat.color;
            part.hoverColor = Color.white;
            part.selectedColor = new Color(1.0f, 0.65f, 0.20f); // Warm Amber Orange selection
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

        private static Material CreateDigMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.55f);
            mat.SetFloat("_Cull", 0); // Double-sided rendering
            return mat;
        }
        #endregion
    }
}
