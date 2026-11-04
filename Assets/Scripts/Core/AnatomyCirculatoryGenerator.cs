using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomyCirculatoryGenerator
    {
        public static GameObject BuildFullCirculatorySystem(Transform parent, Shader litShader)
        {
            GameObject circRoot = new GameObject("CirculatorySystem_Realistic");
            circRoot.transform.SetParent(parent, false);

            // Educational vascular color palette
            Material matMyoLV     = CreateVascularMaterial(litShader, "Mat_Heart_LV",       new Color(0.82f, 0.15f, 0.22f)); // Deep Myocardial Crimson
            Material matMyoRV     = CreateVascularMaterial(litShader, "Mat_Heart_RV",       new Color(0.78f, 0.20f, 0.28f)); // Anterior Ventricle Rose
            Material matAtriumRA  = CreateVascularMaterial(litShader, "Mat_Heart_RA",       new Color(0.65f, 0.25f, 0.45f)); // Venous Atrium
            Material matAtriumLA  = CreateVascularMaterial(litShader, "Mat_Heart_LA",       new Color(0.75f, 0.20f, 0.35f)); // Oxygenated Atrium
            Material matArtery    = CreateVascularMaterial(litShader, "Mat_SystemicArteries",new Color(0.92f, 0.16f, 0.20f)); // Vivid Arterial Scarlet
            Material matVein      = CreateVascularMaterial(litShader, "Mat_SystemicVeins",   new Color(0.18f, 0.45f, 0.88f)); // Cobalt Venous Blue
            Material matPulmArt   = CreateVascularMaterial(litShader, "Mat_PulmonaryArtery", new Color(0.28f, 0.42f, 0.78f)); // Deoxygenated Artery Blue
            Material matPulmVein  = CreateVascularMaterial(litShader, "Mat_PulmonaryVein",   new Color(0.88f, 0.20f, 0.26f)); // Oxygenated Vein Red
            Material matCoronaryA = CreateVascularMaterial(litShader, "Mat_CoronaryArtery",  new Color(1.00f, 0.28f, 0.22f)); // Golden Red
            Material matCoronaryV = CreateVascularMaterial(litShader, "Mat_CoronaryVein",    new Color(0.12f, 0.38f, 0.82f)); // Deep Sapphire

            // 1. Four-Chambered Heart
            BuildFourChamberedHeart(circRoot.transform, matMyoLV, matMyoRV, matAtriumRA, matAtriumLA);

            // 2. Coronary Circulation (LAD, LCx, RCA, Cardiac Veins & Coronary Sinus)
            BuildCoronaryCirculation(circRoot.transform, matCoronaryA, matCoronaryV);

            // 3. Great Vessels of the Heart (Aortic Arch, Pulmonary Trunk & Veins, Venae Cavae)
            BuildGreatVessels(circRoot.transform, matArtery, matVein, matPulmArt, matPulmVein);

            // 4. Head & Neck Vessels (Carotids & Jugulars)
            BuildHeadNeckVasculature(circRoot.transform, matArtery, matVein);

            // 5. Upper Limb Vessels (Subclavian, Axillary, Brachial, Radial, Ulnar, Cephalic, Basilic)
            BuildUpperLimbVasculature(circRoot.transform, matArtery, matVein);

            // 6. Thoracic & Abdominal Vessels (Descending Aorta, IVC, Celiac, Mesenteric, Renals)
            BuildAbdominalVasculature(circRoot.transform, matArtery, matVein);

            // 7. Lower Limb Vessels (Iliacs, Femorals, Great Saphenous, Popliteal, Tibials)
            BuildLowerLimbVasculature(circRoot.transform, matArtery, matVein);

            return circRoot;
        }

        #region 1. Four-Chambered Heart
        private static void BuildFourChamberedHeart(Transform parent,
            Material matLV, Material matRV, Material matRA, Material matLA)
        {
            GameObject heartRoot = new GameObject("Heart_FourChambers");
            heartRoot.transform.SetParent(parent, false);
            // Center of heart in thorax: slightly left of midline, T5-T8 level
            heartRoot.transform.localPosition = new Vector3(-0.02f, 1.34f, 0.02f);
            heartRoot.transform.localRotation = Quaternion.Euler(15f, -22f, 8f); // Anatomical cardiac tilt

            // --- A. Left Ventricle (Apex & Main Systemic Pump) ---
            GameObject lvObj = CreateCompositeVessel(heartRoot.transform, "Heart_LeftVentricle",
                new Vector3(-0.035f, -0.025f, 0.015f), matLV,
                "Left Ventricle", "Ventriculus Sinister Cordis",
                "Thick muscular chamber pumping oxygenated blood into the ascending aorta at high pressure (120 mmHg) to sustain systemic circulation.",
                "Forms the cardiac apex (5th intercostal space). Myocardial wall thickness is roughly 3x that of the right ventricle.",
                new Vector3(-0.35f, 0.15f, 0.45f),
                "Generates the cardiac apical impulse (point of maximal impulse / PMI).",
                "Hypertrophies pathologically in chronic hypertension and aortic stenosis.",
                "Primary site of transmural myocardial infarction.");
            lvObj.GetComponent<AnatomyPart>().hasHeartbeatPulse = true;
            lvObj.GetComponent<AnatomyPart>().pulseMagnitude = 0.045f;
            lvObj.GetComponent<AnatomyPart>().pulseSpeed = 1.15f;

            // LV Cone & Apex
            AddPrimitiveChild(lvObj.transform, PrimitiveType.Sphere, "LV_Body",
                new Vector3(0, 0, 0), new Vector3(0.082f, 0.105f, 0.085f), Quaternion.identity, matLV);
            AddPrimitiveChild(lvObj.transform, PrimitiveType.Sphere, "LV_Apex",
                new Vector3(-0.015f, -0.048f, 0.012f), new Vector3(0.055f, 0.065f, 0.055f), Quaternion.identity, matLV);

            // --- B. Right Ventricle (Pulmonary Pump) ---
            GameObject rvObj = CreateCompositeVessel(heartRoot.transform, "Heart_RightVentricle",
                new Vector3(0.022f, -0.018f, 0.032f), matRV,
                "Right Ventricle", "Ventriculus Dexter Cordis",
                "Anterior crescentic chamber receiving deoxygenated blood from right atrium and propelling it into the low-pressure pulmonary circuit (25 mmHg).",
                "Forms the majority of the anterior (sternocostal) heart surface. Contains the tricuspid valve, papillary muscles, and moderator band.",
                new Vector3(0.15f, 0.15f, 0.5f),
                "Ejects blood through the infundibulum (conus arteriosus) into pulmonary trunk.",
                "Susceptible to acute right ventricular failure in massive pulmonary embolism.");
            rvObj.GetComponent<AnatomyPart>().hasHeartbeatPulse = true;
            rvObj.GetComponent<AnatomyPart>().pulseMagnitude = 0.04f;
            rvObj.GetComponent<AnatomyPart>().pulseSpeed = 1.15f;

            AddPrimitiveChild(rvObj.transform, PrimitiveType.Sphere, "RV_Body",
                new Vector3(0, 0, 0), new Vector3(0.076f, 0.092f, 0.072f), Quaternion.identity, matRV);
            AddPrimitiveChild(rvObj.transform, PrimitiveType.Cylinder, "RV_Infundibulum",
                new Vector3(-0.01f, 0.045f, -0.01f), new Vector3(0.034f, 0.045f, 0.034f), Quaternion.Euler(-18f, 0, 15f), matRV);

            // --- C. Right Atrium (Venous Reservoir) ---
            GameObject raObj = CreateCompositeVessel(heartRoot.transform, "Heart_RightAtrium",
                new Vector3(0.045f, 0.038f, 0.015f), matRA,
                "Right Atrium", "Atrium Dextrum Cordis",
                "Receives systemic deoxygenated blood from Superior & Inferior Vena Cava and cardiac venous return via the Coronary Sinus.",
                "Houses the Sinoatrial (SA) node at the junction with SVC (natural pacemaker) and the Atrioventricular (AV) node in Koch's triangle.",
                new Vector3(0.35f, 0.25f, 0.35f),
                "Features the right auricle containing comb-like pectinate muscles.",
                "Septum features the fossa ovalis, remnant of embryonic foramen ovale.",
                "Normal central venous pressure ranges from 2 to 8 mmHg.");
            raObj.GetComponent<AnatomyPart>().hasHeartbeatPulse = true;
            raObj.GetComponent<AnatomyPart>().pulseMagnitude = 0.03f;
            raObj.GetComponent<AnatomyPart>().pulseSpeed = 1.15f;

            AddPrimitiveChild(raObj.transform, PrimitiveType.Sphere, "RA_Body",
                new Vector3(0, 0, 0), new Vector3(0.068f, 0.075f, 0.065f), Quaternion.identity, matRA);
            AddPrimitiveChild(raObj.transform, PrimitiveType.Sphere, "RA_Auricle",
                new Vector3(-0.022f, 0.025f, 0.024f), new Vector3(0.035f, 0.035f, 0.025f), Quaternion.identity, matRA);

            // --- D. Left Atrium (Oxygenated Reservoir) ---
            GameObject laObj = CreateCompositeVessel(heartRoot.transform, "Heart_LeftAtrium",
                new Vector3(-0.025f, 0.045f, -0.025f), matLA,
                "Left Atrium", "Atrium Sinistrum Cordis",
                "Forms the anatomical base of the heart posteriorly, receiving freshly oxygenated blood from the four pulmonary veins.",
                "Discharges blood into the left ventricle across the bicuspid (mitral) valve during ventricular diastole.",
                new Vector3(-0.3f, 0.25f, -0.2f),
                "Left auricle (appendage) is the primary origin of thromboembolisms in atrial fibrillation.",
                "Smooth-walled chamber derived embryologically from incorporated pulmonary veins.");
            laObj.GetComponent<AnatomyPart>().hasHeartbeatPulse = true;
            laObj.GetComponent<AnatomyPart>().pulseMagnitude = 0.03f;
            laObj.GetComponent<AnatomyPart>().pulseSpeed = 1.15f;

            AddPrimitiveChild(laObj.transform, PrimitiveType.Sphere, "LA_Body",
                new Vector3(0, 0, 0), new Vector3(0.066f, 0.070f, 0.062f), Quaternion.identity, matLA);
            AddPrimitiveChild(laObj.transform, PrimitiveType.Sphere, "LA_Auricle",
                new Vector3(-0.028f, 0.015f, 0.022f), new Vector3(0.030f, 0.032f, 0.022f), Quaternion.identity, matLA);
        }
        #endregion

        #region 2. Coronary Circulation
        private static void BuildCoronaryCirculation(Transform parent, Material matArtery, Material matVein)
        {
            GameObject coronaryRoot = new GameObject("Coronary_PerfusionSystem");
            coronaryRoot.transform.SetParent(parent, false);
            coronaryRoot.transform.localPosition = new Vector3(-0.02f, 1.34f, 0.02f);
            coronaryRoot.transform.localRotation = Quaternion.Euler(15f, -22f, 8f);

            // --- 1. Left Anterior Descending (LAD) Artery ("Widow-Maker") ---
            GameObject ladObj = CreateCompositeVessel(coronaryRoot.transform, "Coronary_LAD_Artery",
                new Vector3(-0.005f, 0.01f, 0.045f), matArtery,
                "Left Anterior Descending (LAD) Artery", "Ramus Interventricularis Anterior",
                "Crucial branch of the Left Main Coronary Artery supplying the anterior LV wall, cardiac apex, and anterior 2/3 of the interventricular septum.",
                "Courses along the anterior interventricular groove. Also known as the 'widow-maker' due to high mortality when acutely occluded.",
                new Vector3(-0.1f, 0.2f, 0.6f),
                "Supplies both bundle branches of the cardiac conduction system.",
                "Most frequent site of coronary atherosclerosis treated with stent angioplasty or CABG (LIMA to LAD).");

            AddPrimitiveChild(ladObj.transform, PrimitiveType.Cylinder, "LAD_Proximal",
                new Vector3(0, 0.025f, 0), new Vector3(0.008f, 0.035f, 0.008f), Quaternion.Euler(15f, 0, -18f), matArtery);
            AddPrimitiveChild(ladObj.transform, PrimitiveType.Cylinder, "LAD_Mid",
                new Vector3(-0.012f, -0.015f, 0.005f), new Vector3(0.007f, 0.040f, 0.007f), Quaternion.Euler(22f, 0, -22f), matArtery);
            AddPrimitiveChild(ladObj.transform, PrimitiveType.Cylinder, "LAD_Distal",
                new Vector3(-0.026f, -0.055f, 0.012f), new Vector3(0.006f, 0.035f, 0.006f), Quaternion.Euler(30f, 0, -25f), matArtery);
            // Diagonal branches
            AddPrimitiveChild(ladObj.transform, PrimitiveType.Cylinder, "Diagonal_1",
                new Vector3(-0.022f, 0.005f, 0.008f), new Vector3(0.005f, 0.028f, 0.005f), Quaternion.Euler(10f, 0, -55f), matArtery);

            // --- 2. Left Circumflex Artery (LCx) ---
            GameObject lcxObj = CreateCompositeVessel(coronaryRoot.transform, "Coronary_Circumflex_Artery",
                new Vector3(-0.038f, 0.028f, 0.015f), matArtery,
                "Left Circumflex (LCx) Artery", "Ramus Circumflexus Arteriae Coronariae Sinistrae",
                "Winds around the left atrioventricular sulcus to the posterior base, perfusing the lateral and posterolateral walls of the left ventricle.",
                "Gives off obtuse marginal (OM) branches supplying the anterolateral papillary muscle.",
                new Vector3(-0.4f, 0.2f, 0.2f),
                "Supplies SA node in ~40% and AV node in ~10% (left-dominant circulation).",
                "Occlusion leads to lateral wall myocardial infarction.");

            AddPrimitiveChild(lcxObj.transform, PrimitiveType.Cylinder, "LCx_Groove",
                new Vector3(0, 0, 0), new Vector3(0.007f, 0.045f, 0.007f), Quaternion.Euler(25f, -60f, 45f), matArtery);
            AddPrimitiveChild(lcxObj.transform, PrimitiveType.Cylinder, "Obtuse_Marginal",
                new Vector3(-0.015f, -0.025f, -0.005f), new Vector3(0.005f, 0.035f, 0.005f), Quaternion.Euler(15f, -40f, 20f), matArtery);

            // --- 3. Right Coronary Artery (RCA) ---
            GameObject rcaObj = CreateCompositeVessel(coronaryRoot.transform, "Coronary_RCA_Artery",
                new Vector3(0.035f, 0.015f, 0.032f), matArtery,
                "Right Coronary Artery (RCA)", "Arteria Coronaria Dextra",
                "Courses within the right atrioventricular groove to supply the right atrium, right ventricle, SA node (60%), and AV node (90%).",
                "In 85-90% of individuals (right-dominant circulation), continues as the Posterior Descending Artery (PDA) perfusing the inferior LV wall and posterior septum.",
                new Vector3(0.45f, 0.2f, 0.2f),
                "Occlusion leads to inferior STEMI and high-grade AV heart blocks.",
                "Gives off acute marginal branches along RV border.");

            AddPrimitiveChild(rcaObj.transform, PrimitiveType.Cylinder, "RCA_Proximal",
                new Vector3(0, 0.025f, 0), new Vector3(0.008f, 0.035f, 0.008f), Quaternion.Euler(-15f, 45f, 25f), matArtery);
            AddPrimitiveChild(rcaObj.transform, PrimitiveType.Cylinder, "RCA_Marginal",
                new Vector3(0.012f, -0.015f, -0.008f), new Vector3(0.007f, 0.042f, 0.007f), Quaternion.Euler(-25f, 60f, 15f), matArtery);
            AddPrimitiveChild(rcaObj.transform, PrimitiveType.Cylinder, "RCA_PDA",
                new Vector3(0.008f, -0.045f, -0.025f), new Vector3(0.006f, 0.038f, 0.006f), Quaternion.Euler(-40f, 15f, -10f), matArtery);

            // --- 4. Coronary Sinus & Great Cardiac Vein ---
            GameObject veinObj = CreateCompositeVessel(coronaryRoot.transform, "Coronary_Sinus_Vein",
                new Vector3(-0.015f, 0.012f, -0.025f), matVein,
                "Coronary Sinus & Great Cardiac Vein", "Sinus Coronarius & Vena Cardiaca Magna",
                "Primary venous drainage system of the heart, collecting ~85% of myocardial deoxygenated blood into the posterior Right Atrium.",
                "Courses in the posterior coronary sulcus, flanked by the left circumflex artery and left atrium.",
                new Vector3(0, 0.2f, -0.45f),
                "Cannulated during cardiac resynchronization therapy (biventricular pacing lead placement).",
                "Guarded by the Thebesian valve at its junction with the right atrium.");

            AddPrimitiveChild(veinObj.transform, PrimitiveType.Cylinder, "CoronarySinus_Trunk",
                new Vector3(0.015f, 0.005f, 0), new Vector3(0.012f, 0.045f, 0.012f), Quaternion.Euler(10f, -80f, 50f), matVein);
            AddPrimitiveChild(veinObj.transform, PrimitiveType.Cylinder, "GreatCardiacVein",
                new Vector3(-0.015f, -0.015f, 0.035f), new Vector3(0.008f, 0.055f, 0.008f), Quaternion.Euler(25f, 0, -22f), matVein);
        }
        #endregion

        #region 3. Great Vessels of the Heart
        private static void BuildGreatVessels(Transform parent,
            Material matArtery, Material matVein, Material matPulmArt, Material matPulmVein)
        {
            GameObject greatRoot = new GameObject("GreatVessels");
            greatRoot.transform.SetParent(parent, false);

            // --- A. Ascending Aorta & Aortic Arch ---
            GameObject aortaObj = CreateCompositeVessel(greatRoot.transform, "Circulatory_AorticArch",
                new Vector3(-0.015f, 1.41f, 0.01f), matArtery,
                "Aortic Arch & Ascending Aorta", "Arcus Aortae & Aorta Ascendens",
                "Chief systemic arterial conduit originating from the left ventricle, arching superiorly and posteriorly over the pulmonary artery at T4.",
                "Gives rise to the three major supra-aortic arterial trunks: Brachiocephalic trunk, Left Common Carotid, and Left Subclavian.",
                new Vector3(-0.1f, 0.35f, 0.3f),
                "Possesses elastic Windkessel properties buffering systolic ejection pressure.",
                "Site of aortic dissection (Stanford Type A / DeBakey I & II).",
                "Baroreceptors in the aortic arch monitor systemic arterial blood pressure.");

            // Ascending Aorta root
            AddPrimitiveChild(aortaObj.transform, PrimitiveType.Cylinder, "Aorta_Ascending",
                new Vector3(0.01f, -0.03f, 0.015f), new Vector3(0.036f, 0.050f, 0.036f), Quaternion.Euler(15f, 0, -12f), matArtery);
            // Arch curve
            AddPrimitiveChild(aortaObj.transform, PrimitiveType.Sphere, "Aorta_ArchCurve",
                new Vector3(0, 0.01f, 0), new Vector3(0.046f, 0.042f, 0.052f), Quaternion.identity, matArtery);

            // Three Supra-aortic Branches
            // 1. Brachiocephalic Trunk
            AddPrimitiveChild(aortaObj.transform, PrimitiveType.Cylinder, "Branch_Brachiocephalic",
                new Vector3(0.018f, 0.042f, 0.012f), new Vector3(0.018f, 0.035f, 0.018f), Quaternion.Euler(-10f, 0, -20f), matArtery);
            // 2. Left Common Carotid
            AddPrimitiveChild(aortaObj.transform, PrimitiveType.Cylinder, "Branch_LeftCarotidRoot",
                new Vector3(-0.004f, 0.045f, 0.005f), new Vector3(0.014f, 0.038f, 0.014f), Quaternion.Euler(0, 0, 5f), matArtery);
            // 3. Left Subclavian
            AddPrimitiveChild(aortaObj.transform, PrimitiveType.Cylinder, "Branch_LeftSubclavianRoot",
                new Vector3(-0.022f, 0.042f, -0.005f), new Vector3(0.014f, 0.036f, 0.014f), Quaternion.Euler(15f, 0, 22f), matArtery);

            // --- B. Pulmonary Trunk & Pulmonary Arteries ---
            GameObject pulmObj = CreateCompositeVessel(greatRoot.transform, "Circulatory_PulmonaryTrunk",
                new Vector3(-0.01f, 1.39f, 0.035f), matPulmArt,
                "Pulmonary Trunk & Arteries", "Truncus Pulmonalis",
                "Arises from right ventricle, coursing anterior and then left of the ascending aorta before bifurcating into Left and Right Pulmonary Arteries.",
                "Only postnatal arteries carrying deoxygenated blood, conveying cardiac output directly to alveolar capillaries for gas exchange.",
                new Vector3(0, 0.25f, 0.5f),
                "Connected to aortic arch via ligamentum arteriosum (remnant of fetal ductus arteriosus).",
                "Saddle pulmonary embolism lodges at the bifurcation, leading to acute obstructive shock.");

            AddPrimitiveChild(pulmObj.transform, PrimitiveType.Cylinder, "Pulm_Trunk",
                new Vector3(0, -0.02f, 0), new Vector3(0.032f, 0.045f, 0.032f), Quaternion.Euler(-20f, 0, 10f), matPulmArt);
            AddPrimitiveChild(pulmObj.transform, PrimitiveType.Cylinder, "Pulm_RightBranch",
                new Vector3(0.035f, 0.01f, -0.02f), new Vector3(0.020f, 0.045f, 0.020f), Quaternion.Euler(15f, 60f, -80f), matPulmArt);
            AddPrimitiveChild(pulmObj.transform, PrimitiveType.Cylinder, "Pulm_LeftBranch",
                new Vector3(-0.035f, 0.01f, -0.01f), new Vector3(0.018f, 0.040f, 0.018f), Quaternion.Euler(10f, -50f, 75f), matPulmArt);

            // --- C. Superior Vena Cava (SVC) ---
            GameObject svcObj = CreateCompositeVessel(greatRoot.transform, "Circulatory_SuperiorVenaCava",
                new Vector3(0.035f, 1.41f, 0.015f), matVein,
                "Superior Vena Cava (SVC)", "Vena Cava Superior",
                "Large venous trunk returning systemic blood from head, neck, upper limbs, and thoracic wall to the superior pole of the right atrium.",
                "Formed by the confluence of the left and right brachiocephalic veins at the level of the first right costal cartilage.",
                new Vector3(0.25f, 0.25f, 0.35f),
                "Receives the azygos vein prior to piercing pericardial reflection.",
                "SVC syndrome occurs when compressed by lung bronchogenic carcinoma or lymphoma.");

            AddPrimitiveChild(svcObj.transform, PrimitiveType.Cylinder, "SVC_Trunk",
                new Vector3(0, 0, 0), new Vector3(0.028f, 0.065f, 0.028f), Quaternion.identity, matVein);
            // Left & Right Brachiocephalic Veins
            AddPrimitiveChild(svcObj.transform, PrimitiveType.Cylinder, "Vein_RightBrachiocephalic",
                new Vector3(0.015f, 0.045f, 0), new Vector3(0.018f, 0.035f, 0.018f), Quaternion.Euler(0, 0, -25f), matVein);
            AddPrimitiveChild(svcObj.transform, PrimitiveType.Cylinder, "Vein_LeftBrachiocephalic",
                new Vector3(-0.025f, 0.048f, 0.005f), new Vector3(0.018f, 0.065f, 0.018f), Quaternion.Euler(0, 0, 68f), matVein);

            // --- D. Pulmonary Veins (Oxygenated Return) ---
            GameObject pvObj = CreateCompositeVessel(greatRoot.transform, "Circulatory_PulmonaryVeins",
                new Vector3(-0.02f, 1.37f, -0.025f), matPulmVein,
                "Pulmonary Veins (4 Vessels)", "Venae Pulmonales",
                "Four venous conduits (Left Superior/Inferior, Right Superior/Inferior) carrying freshly oxygenated blood from lungs into the Left Atrium.",
                "Valveless veins entering the posterosuperior aspect of the left atrium.",
                new Vector3(0, 0.2f, -0.4f),
                "Unique in adult anatomy as veins conveying arterial-grade oxygen-saturated blood (~98% SaO2).",
                "Ectopic foci surrounding pulmonary vein ostia are the predominant source of atrial fibrillation.");

            // Left Pulmonary Veins
            AddPrimitiveChild(pvObj.transform, PrimitiveType.Cylinder, "PV_LeftSuperior",
                new Vector3(-0.045f, 0.015f, 0), new Vector3(0.014f, 0.035f, 0.014f), Quaternion.Euler(0, 30f, 80f), matPulmVein);
            AddPrimitiveChild(pvObj.transform, PrimitiveType.Cylinder, "PV_LeftInferior",
                new Vector3(-0.045f, -0.015f, 0), new Vector3(0.014f, 0.035f, 0.014f), Quaternion.Euler(0, 30f, 80f), matPulmVein);
            // Right Pulmonary Veins
            AddPrimitiveChild(pvObj.transform, PrimitiveType.Cylinder, "PV_RightSuperior",
                new Vector3(0.055f, 0.015f, 0.01f), new Vector3(0.014f, 0.045f, 0.014f), Quaternion.Euler(0, -30f, -80f), matPulmVein);
            AddPrimitiveChild(pvObj.transform, PrimitiveType.Cylinder, "PV_RightInferior",
                new Vector3(0.055f, -0.015f, 0.01f), new Vector3(0.014f, 0.045f, 0.014f), Quaternion.Euler(0, -30f, -80f), matPulmVein);
        }
        #endregion

        #region 4. Head & Neck Vasculature
        private static void BuildHeadNeckVasculature(Transform parent, Material matArtery, Material matVein)
        {
            GameObject neckRoot = new GameObject("Vasculature_HeadAndNeck");
            neckRoot.transform.SetParent(parent, false);

            BuildNeckVesselSide(neckRoot.transform, true, matArtery, matVein);
            BuildNeckVesselSide(neckRoot.transform, false, matArtery, matVein);
        }

        private static void BuildNeckVesselSide(Transform parent, bool isLeft, Material matArtery, Material matVein)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";

            // --- Common & Internal Carotid Artery ---
            GameObject carotidObj = CreateCompositeVessel(parent, $"Circulatory_Carotid_{sideName}",
                new Vector3(side * 0.038f, 1.54f, 0.015f), matArtery,
                $"{sideName} Common & Internal Carotid Artery", $"Arteria Carotis Communis {sideName}",
                "Primary arterial supply to brain, eyes, and face. Ascends anterolaterally in the carotid sheath adjacent to internal jugular vein and vagus nerve.",
                "Bifurcates at C4 (upper border of thyroid cartilage) into internal carotid (brain) and external carotid (facial).",
                new Vector3(side * 0.35f, 0.2f, 0.2f),
                "Carotid sinus at the bifurcation contains baroreceptors sensing arterial pressure.",
                "Carotid endarterectomy performed for high-grade atherosclerotic stenosis to prevent ischemic stroke.");

            AddPrimitiveChild(carotidObj.transform, PrimitiveType.Cylinder, "CommonCarotid",
                new Vector3(0, -0.045f, 0), new Vector3(0.014f, 0.085f, 0.014f), Quaternion.identity, matArtery);
            AddPrimitiveChild(carotidObj.transform, PrimitiveType.Cylinder, "InternalCarotid",
                new Vector3(side * 0.005f, 0.045f, -0.005f), new Vector3(0.011f, 0.080f, 0.011f), Quaternion.identity, matArtery);
            AddPrimitiveChild(carotidObj.transform, PrimitiveType.Cylinder, "ExternalCarotid",
                new Vector3(side * -0.005f, 0.040f, 0.012f), new Vector3(0.010f, 0.075f, 0.010f), Quaternion.Euler(0, 0, side * 15f), matArtery);

            // --- Internal & External Jugular Veins ---
            GameObject jugularObj = CreateCompositeVessel(parent, $"Circulatory_Jugular_{sideName}",
                new Vector3(side * 0.052f, 1.54f, 0.018f), matVein,
                $"{sideName} Internal Jugular Vein", $"Vena Jugularis Interna {sideName}",
                "Major venous channel draining blood from skull, brain dural venous sinuses, and facial structures into the brachiocephalic vein.",
                "Courses within carotid sheath lateral to common carotid artery.",
                new Vector3(side * 0.45f, 0.2f, 0.2f),
                "Standard anatomical landmark for central venous line (CVC) cannulation under ultrasound guidance.",
                "Jugular venous distension (JVD) reflects elevated right atrial pressure in congestive heart failure.");

            AddPrimitiveChild(jugularObj.transform, PrimitiveType.Cylinder, "InternalJugular",
                new Vector3(0, 0, 0), new Vector3(0.018f, 0.165f, 0.018f), Quaternion.identity, matVein);
        }
        #endregion

        #region 5. Upper Limb Vasculature
        private static void BuildUpperLimbVasculature(Transform parent, Material matArtery, Material matVein)
        {
            GameObject armRoot = new GameObject("Vasculature_UpperLimbs");
            armRoot.transform.SetParent(parent, false);

            BuildArmVessels(armRoot.transform, true, matArtery, matVein);
            BuildArmVessels(armRoot.transform, false, matArtery, matVein);
        }

        private static void BuildArmVessels(Transform parent, bool isLeft, Material matArtery, Material matVein)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";

            // --- A. Brachial & Subclavian Artery ---
            GameObject brachialObj = CreateCompositeVessel(parent, $"Circulatory_BrachialArtery_{sideName}",
                new Vector3(side * 0.22f, 1.25f, -0.01f), matArtery,
                $"{sideName} Subclavian, Axillary & Brachial Artery", $"Arteria Brachialis {sideName}",
                "Continuous arterial trunk providing high-flow arterial perfusion to shoulder, arm, forearm, and hand.",
                "Passes over first rib (subclavian), through axilla (axillary), and descends along medial bicipital groove (brachial).",
                new Vector3(side * 0.45f, 0.1f, 0.1f),
                "Standard clinical auscultation site for sphygmomanometer blood pressure measurement.",
                "Bifurcates in cubital fossa into radial and ulnar arteries.");

            // Subclavian arch
            AddPrimitiveChild(brachialObj.transform, PrimitiveType.Cylinder, "Subclavian",
                new Vector3(side * -0.07f, 0.14f, 0), new Vector3(0.015f, 0.08f, 0.015f), Quaternion.Euler(0, 0, side * 75f), matArtery);
            // Axillary & Brachial
            AddPrimitiveChild(brachialObj.transform, PrimitiveType.Cylinder, "Brachial",
                new Vector3(side * 0.02f, -0.04f, 0), new Vector3(0.013f, 0.24f, 0.013f), Quaternion.identity, matArtery);

            // --- B. Forearm Arteries (Radial & Ulnar) ---
            GameObject forearmArtObj = CreateCompositeVessel(parent, $"Circulatory_ForearmArteries_{sideName}",
                new Vector3(side * 0.25f, 0.95f, -0.01f), matArtery,
                $"{sideName} Radial & Ulnar Arteries", $"Arteriae Radialis et Ulnaris {sideName}",
                "Bifurcation conduits supplying deep and superficial forearm muscles and hand palmar arches.",
                "Radial artery runs laterally toward anatomical snuffbox; ulnar artery courses medially with ulnar nerve.",
                new Vector3(side * 0.5f, 0, 0.15f),
                "Radial pulse at wrist is the most common site for clinical pulse palpation.",
                "Modified Allen's test confirms collateral palmar flow prior to radial artery harvest for CABG or arterial blood gas (ABG) sampling.");

            AddPrimitiveChild(forearmArtObj.transform, PrimitiveType.Cylinder, "RadialArtery",
                new Vector3(side * 0.012f, 0, 0), new Vector3(0.009f, 0.22f, 0.009f), Quaternion.identity, matArtery);
            AddPrimitiveChild(forearmArtObj.transform, PrimitiveType.Cylinder, "UlnarArtery",
                new Vector3(side * -0.012f, 0, 0), new Vector3(0.009f, 0.22f, 0.009f), Quaternion.identity, matArtery);

            // --- C. Superficial Arm Veins (Cephalic & Basilic) ---
            GameObject armVeinObj = CreateCompositeVessel(parent, $"Circulatory_ArmVeins_{sideName}",
                new Vector3(side * 0.24f, 1.12f, 0.012f), matVein,
                $"{sideName} Cephalic, Basilic & Median Cubital Veins", $"Venae Cephalica & Basilica {sideName}",
                "Superficial subcutaneous venous drainage network of the upper extremity.",
                "Cephalic vein travels laterally through deltopectoral groove; Basilic vein travels medially, joining brachial to form axillary vein.",
                new Vector3(side * 0.55f, 0, 0.25f),
                "Median cubital vein in cubital fossa is the premier site for routine clinical venipuncture and IV blood draws.",
                "Site of arteriovenous (AV) fistula creation for hemodialysis access.");

            AddPrimitiveChild(armVeinObj.transform, PrimitiveType.Cylinder, "CephalicVein",
                new Vector3(side * 0.022f, 0, 0.01f), new Vector3(0.012f, 0.42f, 0.012f), Quaternion.identity, matVein);
            AddPrimitiveChild(armVeinObj.transform, PrimitiveType.Cylinder, "BasilicVein",
                new Vector3(side * -0.018f, 0, -0.005f), new Vector3(0.012f, 0.42f, 0.012f), Quaternion.identity, matVein);
        }
        #endregion

        #region 6. Abdominal & Thoracic Vasculature
        private static void BuildAbdominalVasculature(Transform parent, Material matArtery, Material matVein)
        {
            GameObject abdoRoot = new GameObject("Vasculature_TrunkAndAbdomen");
            abdoRoot.transform.SetParent(parent, false);

            // --- A. Descending Thoracic & Abdominal Aorta ---
            GameObject descAortaObj = CreateCompositeVessel(abdoRoot.transform, "Circulatory_DescendingAorta",
                new Vector3(-0.015f, 1.15f, -0.02f), matArtery,
                "Descending Thoracic & Abdominal Aorta", "Aorta Descendens",
                "Massive midline arterial trunk descending anterior to vertebrae from T4, traversing the aortic hiatus of diaphragm at T12, bifurcating at L4 into common iliacs.",
                "Supplies all abdominal viscera via unpaired visceral branches (celiac, SMA, IMA) and paired visceral branches (renals, gonadals).",
                new Vector3(-0.1f, 0.1f, 0.35f),
                "Abdominal Aortic Aneurysm (AAA) screening palpates pulsatile periumbilical mass.",
                "Withstands pulsating cardiac output of ~5 liters/min at velocities of ~1 m/s.");

            // Aorta Column
            AddPrimitiveChild(descAortaObj.transform, PrimitiveType.Cylinder, "Aorta_ThoracoAbdominal",
                new Vector3(0, 0, 0), new Vector3(0.026f, 0.36f, 0.026f), Quaternion.identity, matArtery);

            // Celiac Trunk & Mesenteric Roots
            AddPrimitiveChild(descAortaObj.transform, PrimitiveType.Cylinder, "CeliacTrunk",
                new Vector3(0, 0.06f, 0.022f), new Vector3(0.012f, 0.035f, 0.012f), Quaternion.Euler(75f, 0, 0), matArtery);
            AddPrimitiveChild(descAortaObj.transform, PrimitiveType.Cylinder, "SMA_Root",
                new Vector3(0, 0.02f, 0.022f), new Vector3(0.012f, 0.035f, 0.012f), Quaternion.Euler(70f, 0, 0), matArtery);

            // --- B. Inferior Vena Cava (IVC) ---
            GameObject ivcObj = CreateCompositeVessel(abdoRoot.transform, "Circulatory_InferiorVenaCava",
                new Vector3(0.025f, 1.14f, -0.018f), matVein,
                "Inferior Vena Cava (IVC)", "Vena Cava Inferior",
                "Largest vein in the human body, conveying deoxygenated blood from lower limbs, pelvis, kidneys, and abdominal wall to right atrium.",
                "Formed by the junction of common iliac veins at L5; ascends along right side of vertebral column, piercing central tendon of diaphragm at T8.",
                new Vector3(0.15f, 0.1f, 0.35f),
                "IVC filters placed in infrarenal segment to prevent recurrent fatal pulmonary embolism in DVT.",
                "Compressible retroperitoneal reservoir reflecting circulating intravascular volume status.");

            AddPrimitiveChild(ivcObj.transform, PrimitiveType.Cylinder, "IVC_Column",
                new Vector3(0, 0, 0), new Vector3(0.032f, 0.36f, 0.032f), Quaternion.identity, matVein);

            // --- C. Renal Arteries & Veins ---
            GameObject renalObj = CreateCompositeVessel(abdoRoot.transform, "Circulatory_RenalVessels",
                new Vector3(0.005f, 1.16f, -0.02f), matArtery,
                "Renal Arteries & Veins (Left & Right)", "Vasa Renalia",
                "High-perfusion vascular pedicles delivering ~20-25% of cardiac output (~1.2 L/min) to kidneys for metabolic filtration and fluid homeostasis.",
                "Right renal artery is longer and passes posterior to IVC; left renal vein is longer, crossing anterior to aorta beneath SMA.",
                new Vector3(0, 0.2f, 0.4f),
                "Renal artery stenosis triggers secondary renovascular hypertension via renin-angiotensin-aldosterone system (RAAS).",
                "'Nutcracker syndrome' refers to left renal vein compression between aorta and superior mesenteric artery.");

            // Left & Right Renal Arteries
            AddPrimitiveChild(renalObj.transform, PrimitiveType.Cylinder, "RenalArtery_Left",
                new Vector3(-0.065f, 0, 0), new Vector3(0.012f, 0.075f, 0.012f), Quaternion.Euler(0, 0, 85f), matArtery);
            AddPrimitiveChild(renalObj.transform, PrimitiveType.Cylinder, "RenalArtery_Right",
                new Vector3(0.065f, -0.01f, 0), new Vector3(0.012f, 0.080f, 0.012f), Quaternion.Euler(0, 0, -85f), matArtery);

            // Left & Right Renal Veins
            AddPrimitiveChild(renalObj.transform, PrimitiveType.Cylinder, "RenalVein_Left",
                new Vector3(-0.055f, 0.012f, 0.012f), new Vector3(0.016f, 0.075f, 0.016f), Quaternion.Euler(0, 0, 85f), matVein);
            AddPrimitiveChild(renalObj.transform, PrimitiveType.Cylinder, "RenalVein_Right",
                new Vector3(0.055f, 0.005f, 0.012f), new Vector3(0.016f, 0.065f, 0.016f), Quaternion.Euler(0, 0, -85f), matVein);
        }
        #endregion

        #region 7. Lower Limb Vasculature
        private static void BuildLowerLimbVasculature(Transform parent, Material matArtery, Material matVein)
        {
            GameObject legRoot = new GameObject("Vasculature_LowerLimbs");
            legRoot.transform.SetParent(parent, false);

            BuildLegVessels(legRoot.transform, true, matArtery, matVein);
            BuildLegVessels(legRoot.transform, false, matArtery, matVein);
        }

        private static void BuildLegVessels(Transform parent, bool isLeft, Material matArtery, Material matVein)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";

            // --- A. Iliac Vessels (Pelvis) ---
            GameObject iliacObj = CreateCompositeVessel(parent, $"Circulatory_IliacVessels_{sideName}",
                new Vector3(side * 0.055f, 0.90f, 0), matArtery,
                $"{sideName} Common & External Iliac Vessels", $"Vasa Iliaca Communia {sideName}",
                "Bifurcation conduit distributing systemic blood through pelvic cavity and under inguinal ligament into lower extremity.",
                "Common iliac divides into internal iliac (pelvic organs) and external iliac (lower limb).",
                new Vector3(side * 0.35f, 0.1f, 0.2f),
                "External iliac artery becomes the femoral artery as it passes under the midpoint of the inguinal ligament.",
                "Common site of aortoiliac occlusive disease (Leriche syndrome).");

            AddPrimitiveChild(iliacObj.transform, PrimitiveType.Cylinder, "IliacArtery",
                new Vector3(side * 0.012f, 0, 0), new Vector3(0.016f, 0.13f, 0.016f), Quaternion.Euler(0, 0, side * -22f), matArtery);
            AddPrimitiveChild(iliacObj.transform, PrimitiveType.Cylinder, "IliacVein",
                new Vector3(side * -0.010f, -0.005f, 0), new Vector3(0.018f, 0.13f, 0.018f), Quaternion.Euler(0, 0, side * -20f), matVein);

            // --- B. Femoral Artery & Vein (Thigh) ---
            GameObject femoralObj = CreateCompositeVessel(parent, $"Circulatory_FemoralVessels_{sideName}",
                new Vector3(side * 0.095f, 0.62f, 0.015f), matArtery,
                $"{sideName} Femoral Artery & Vein", $"Vasa Femoralia {sideName}",
                "Major vascular bundle of the thigh coursing through femoral triangle and adductor canal.",
                "Artery gives off deep femoral (profunda femoris) branch supplying thigh musculature; vein accompanies artery toward pelvis.",
                new Vector3(side * 0.45f, 0, 0.25f),
                "Femoral artery catheterization is standard entry point for left heart cardiac angiography and stent procedures.",
                "Deep vein thrombosis (DVT) in femoral vein carries high risk of embolization to lungs.");

            AddPrimitiveChild(femoralObj.transform, PrimitiveType.Cylinder, "FemoralArtery",
                new Vector3(side * 0.008f, 0, 0), new Vector3(0.014f, 0.38f, 0.014f), Quaternion.identity, matArtery);
            AddPrimitiveChild(femoralObj.transform, PrimitiveType.Cylinder, "FemoralVein",
                new Vector3(side * -0.008f, 0, 0), new Vector3(0.016f, 0.38f, 0.016f), Quaternion.identity, matVein);

            // --- C. Great Saphenous Vein (Longest Vein) ---
            GameObject saphenousObj = CreateCompositeVessel(parent, $"Circulatory_SaphenousVein_{sideName}",
                new Vector3(side * 0.065f, 0.48f, 0.022f), matVein,
                $"{sideName} Great Saphenous Vein", $"Vena Saphena Magna {sideName}",
                "Longest vein in the human body, ascending subcutaneously along medial aspect of foot, leg, and thigh to terminate at saphenous opening into femoral vein.",
                "Contains 10 to 20 one-way bicuspid valves preventing venous reflux.",
                new Vector3(side * 0.5f, 0, 0.35f),
                "Premier autologous conduit harvested for Coronary Artery Bypass Graft (CABG) surgery.",
                "Valvular incompetence causes saphenofemoral reflux, venous stasis, and lower limb varicose veins.");

            AddPrimitiveChild(saphenousObj.transform, PrimitiveType.Cylinder, "Saphenous_ThighLeg",
                new Vector3(0, 0, 0), new Vector3(0.011f, 0.65f, 0.011f), Quaternion.identity, matVein);

            // --- D. Popliteal & Tibial Vessels (Knee & Leg) ---
            GameObject tibialObj = CreateCompositeVessel(parent, $"Circulatory_TibialVessels_{sideName}",
                new Vector3(side * 0.105f, 0.22f, 0.005f), matArtery,
                $"{sideName} Popliteal, Anterior & Posterior Tibial Vessels", $"Vasa Tibialia {sideName}",
                "Arterial conduits extending through popliteal fossa behind knee, bifurcating into anterior and posterior tibial arteries to supply calf and foot.",
                "Anterior tibial artery crosses ankle joint to become dorsalis pedis artery; posterior tibial artery passes posterior to medial malleolus.",
                new Vector3(side * 0.45f, 0, 0.2f),
                "Dorsalis pedis and posterior tibial pulses are palpated routinely to evaluate peripheral arterial disease (PAD).",
                "Deep popliteal and calf veins are frequent sites of initial thrombus formation in immobilized patients.");

            AddPrimitiveChild(tibialObj.transform, PrimitiveType.Cylinder, "PoplitealArtery",
                new Vector3(0, 0.12f, -0.015f), new Vector3(0.012f, 0.12f, 0.012f), Quaternion.identity, matArtery);
            AddPrimitiveChild(tibialObj.transform, PrimitiveType.Cylinder, "AnteriorTibial",
                new Vector3(side * 0.008f, -0.05f, 0.012f), new Vector3(0.009f, 0.25f, 0.009f), Quaternion.identity, matArtery);
            AddPrimitiveChild(tibialObj.transform, PrimitiveType.Cylinder, "PosteriorTibial",
                new Vector3(side * -0.008f, -0.05f, -0.012f), new Vector3(0.009f, 0.25f, 0.009f), Quaternion.identity, matArtery);
        }
        #endregion

        #region Helpers
        private static GameObject CreateCompositeVessel(
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
                systemType = AnatomySystemType.Circulatory,
                keyFunction = function,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat.color;
            part.hoverColor = Color.white;
            part.selectedColor = new Color(1.0f, 0.35f, 0.45f); // Brilliant Rose selection
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

        private static Material CreateVascularMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.65f); // Glossy wet vascular tissue appearance
            mat.SetFloat("_Cull", 0); // Double-sided rendering
            return mat;
        }
        #endregion
    }
}

