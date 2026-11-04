using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomySkeletalGenerator
    {
        public static GameObject BuildFullSkeleton(Transform parent, Shader litShader)
        {
            GameObject skeletonRoot = new GameObject("SkeletalSystem_Realistic");
            skeletonRoot.transform.SetParent(parent, false);

            // Distinct educational color palette for each anatomical bone region
            Material matCranium     = CreateBoneMaterial(litShader, "Mat_Cranium",     new Color(0.39f, 0.71f, 0.96f)); // Sky Blue
            Material matFace        = CreateBoneMaterial(litShader, "Mat_Face",        new Color(0.30f, 0.82f, 0.88f)); // Aquamarine
            Material matMandible    = CreateBoneMaterial(litShader, "Mat_Mandible",    new Color(0.50f, 0.87f, 0.92f)); // Cyan
            Material matOrbit       = CreateBoneMaterial(litShader, "Mat_Orbit",       new Color(0.12f, 0.25f, 0.35f)); // Deep Socket
            Material matCervical    = CreateBoneMaterial(litShader, "Mat_Cervical",    new Color(0.73f, 0.41f, 0.78f)); // Lavender
            Material matThoracic    = CreateBoneMaterial(litShader, "Mat_Thoracic",    new Color(0.49f, 0.34f, 0.76f)); // Purple
            Material matLumbar      = CreateBoneMaterial(litShader, "Mat_Lumbar",      new Color(0.37f, 0.21f, 0.69f)); // Deep Violet
            Material matSacrum      = CreateBoneMaterial(litShader, "Mat_Sacrum",      new Color(0.67f, 0.28f, 0.74f)); // Orchid
            Material matSpinalCord  = CreateBoneMaterial(litShader, "Mat_SpinalCord",  new Color(1.00f, 0.84f, 0.31f)); // Nerve Gold
            Material matSternum     = CreateBoneMaterial(litShader, "Mat_Sternum",     new Color(0.94f, 0.33f, 0.31f)); // Coral Red
            Material matRibs        = CreateBoneMaterial(litShader, "Mat_Ribs",        new Color(1.00f, 0.44f, 0.26f)); // Tangerine Orange
            Material matClavicle    = CreateBoneMaterial(litShader, "Mat_Clavicle",    new Color(0.15f, 0.65f, 0.60f)); // Teal
            Material matScapula     = CreateBoneMaterial(litShader, "Mat_Scapula",     new Color(0.30f, 0.71f, 0.67f)); // Sea Green
            Material matHumerus     = CreateBoneMaterial(litShader, "Mat_Humerus",     new Color(1.00f, 0.65f, 0.15f)); // Amber Orange
            Material matRadius      = CreateBoneMaterial(litShader, "Mat_Radius",      new Color(0.61f, 0.80f, 0.40f)); // Lime
            Material matUlna        = CreateBoneMaterial(litShader, "Mat_Ulna",        new Color(0.40f, 0.73f, 0.42f)); // Emerald Green
            Material matHand        = CreateBoneMaterial(litShader, "Mat_Hand",        new Color(1.00f, 0.72f, 0.30f)); // Warm Gold
            Material matPelvis      = CreateBoneMaterial(litShader, "Mat_Pelvis",      new Color(0.93f, 0.25f, 0.48f)); // Rose / Crimson
            Material matFemur       = CreateBoneMaterial(litShader, "Mat_Femur",       new Color(0.26f, 0.65f, 0.96f)); // Dodger Blue
            Material matPatella     = CreateBoneMaterial(litShader, "Mat_Patella",     new Color(1.00f, 0.93f, 0.35f)); // Bright Yellow
            Material matTibia       = CreateBoneMaterial(litShader, "Mat_Tibia",       new Color(0.15f, 0.78f, 0.85f)); // Turquoise
            Material matFibula      = CreateBoneMaterial(litShader, "Mat_Fibula",      new Color(0.50f, 0.80f, 0.77f)); // Soft Mint
            Material matFoot        = CreateBoneMaterial(litShader, "Mat_Foot",        new Color(0.62f, 0.66f, 0.85f)); // Periwinkle

            // A. Skull with detailed facial structure
            BuildDetailedSkull(skeletonRoot.transform, matCranium, matFace, matMandible, matOrbit);

            // B. Vertebral column and spinal cord region
            BuildVertebralColumnAndSpine(skeletonRoot.transform, matCervical, matThoracic, matLumbar, matSacrum, matSpinalCord);

            // C. Rib cage and sternum
            BuildRibCageAndSternum(skeletonRoot.transform, matSternum, matRibs);

            // D. Pelvis
            BuildPelvis(skeletonRoot.transform, matPelvis);

            // E. Shoulder bones (Pectoral Girdle: Clavicles & Scapulae)
            BuildShoulderBones(skeletonRoot.transform, matClavicle, matScapula);

            // F. Upper Limbs (Humerus, Radius, Ulna)
            BuildUpperLimbs(skeletonRoot.transform, matHumerus, matRadius, matUlna);

            // G. Hand bones and finger structures
            BuildHands(skeletonRoot.transform, matHand);

            // H. Lower Limbs (Femur, Patella, Tibia, Fibula)
            BuildLowerLimbs(skeletonRoot.transform, matFemur, matPatella, matTibia, matFibula);

            // I. Foot bones
            BuildFeet(skeletonRoot.transform, matFoot);

            return skeletonRoot;
        }

        #region A. Detailed Skull
        private static void BuildDetailedSkull(Transform parent, Material craniumMat, Material faceMat, Material mandibleMat, Material orbitMat)
        {
            GameObject skullGroup = new GameObject("Skull_Complete");
            skullGroup.transform.SetParent(parent, false);

            // 1. Neurocranium (Frontal, Parietal, Occipital, Temporal cranial dome)
            GameObject craniumObj = CreateCompositeBone(skullGroup.transform, "Skull_Neurocranium",
                new Vector3(0, 1.76f, 0), craniumMat,
                "Neurocranium (Cranial Vault)", "Calvaria",
                "Rigid protective enclosure sheltering the cerebral hemispheres, cerebellum, and brainstem.",
                "Formed by 8 cranial bones united by coronal, sagittal, and lambdoid sutures.",
                new Vector3(0, 0.45f, 0),
                "Houses the cerebrum and protects against traumatic impact.",
                "Foramen magnum at skull base allows spinal cord transition.");

            // Parietal and Frontal cranial vault
            AddPrimitiveChild(craniumObj.transform, PrimitiveType.Sphere, "Cranial_Vault",
                new Vector3(0, 0.04f, -0.02f), new Vector3(0.24f, 0.25f, 0.27f), Quaternion.identity, craniumMat);

            // Forehead curve
            AddPrimitiveChild(craniumObj.transform, PrimitiveType.Sphere, "Frontal_Forehead",
                new Vector3(0, 0.05f, 0.05f), new Vector3(0.20f, 0.16f, 0.18f), Quaternion.identity, craniumMat);

            // Occipital protuberance (back of head)
            AddPrimitiveChild(craniumObj.transform, PrimitiveType.Sphere, "Occipital_Back",
                new Vector3(0, -0.01f, -0.08f), new Vector3(0.18f, 0.16f, 0.18f), Quaternion.identity, craniumMat);

            // Temporal bones (sides of head)
            AddPrimitiveChild(craniumObj.transform, PrimitiveType.Cube, "Temporal_L",
                new Vector3(-0.10f, 0, -0.02f), new Vector3(0.04f, 0.12f, 0.14f), Quaternion.Euler(0, 0, -10), craniumMat);
            AddPrimitiveChild(craniumObj.transform, PrimitiveType.Cube, "Temporal_R",
                new Vector3(0.10f, 0, -0.02f), new Vector3(0.04f, 0.12f, 0.14f), Quaternion.Euler(0, 0, 10), craniumMat);


            // 2. Facial Skeleton (Viscerocranium: Orbits, Zygomatic cheek arches, Nasal, Maxilla)
            GameObject facialObj = CreateCompositeBone(skullGroup.transform, "Skull_FacialBones",
                new Vector3(0, 1.70f, 0.07f), faceMat,
                "Facial Skeleton (Viscerocranium)", "Skeleton Faciale",
                "Anterior skull framework supporting eyes, nasal passageway, cheeks, and upper dentition.",
                "Comprises 14 facial bones: maxillae, zygomatics, nasals, lacrimals, palatines, vomer, and conchae.",
                new Vector3(0, 0.35f, 0.25f),
                "Bilateral orbital sockets shelter the eyes and oculomotor apparatus.",
                "Zygomatic arches (cheekbones) withstand intense bite forces.");

            // Nasal Bridge
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Cube, "Nasal_Bridge",
                new Vector3(0, 0.03f, 0.045f), new Vector3(0.035f, 0.065f, 0.035f), Quaternion.Euler(15, 0, 0), faceMat);

            // Left & Right Zygomatic Arches (Cheekbones)
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Cube, "Zygomatic_L",
                new Vector3(-0.075f, 0.01f, 0.01f), new Vector3(0.04f, 0.05f, 0.09f), Quaternion.Euler(10, -15, 0), faceMat);
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Cube, "Zygomatic_R",
                new Vector3(0.075f, 0.01f, 0.01f), new Vector3(0.04f, 0.05f, 0.09f), Quaternion.Euler(10, 15, 0), faceMat);

            // Left & Right Orbital Sockets (Eye Cavities)
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Sphere, "Orbit_L",
                new Vector3(-0.042f, 0.03f, 0.035f), new Vector3(0.042f, 0.042f, 0.035f), Quaternion.identity, orbitMat);
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Sphere, "Orbit_R",
                new Vector3(0.042f, 0.03f, 0.035f), new Vector3(0.042f, 0.042f, 0.035f), Quaternion.identity, orbitMat);

            // Maxilla (Upper Jaw & Palate)
            AddPrimitiveChild(facialObj.transform, PrimitiveType.Cube, "Maxilla_Jaw",
                new Vector3(0, -0.04f, 0.03f), new Vector3(0.095f, 0.05f, 0.075f), Quaternion.identity, faceMat);


            // 3. Mandible (Lower Jawbone)
            GameObject mandibleObj = CreateCompositeBone(skullGroup.transform, "Skull_Mandible",
                new Vector3(0, 1.62f, 0.05f), mandibleMat,
                "Mandible (Lower Jaw)", "Mandibula",
                "Strongest and largest bone of the human face; forms the lower jaw and articulates mastication.",
                "Articulates with temporal bones at bilateral temporomandibular joints (TMJ).",
                new Vector3(0, 0.2f, 0.35f),
                "Possesses mental protuberance (chin) unique to Homo sapiens.",
                "Anchors inferior dentition and major chewing musculature.");

            // Chin mental protuberance
            AddPrimitiveChild(mandibleObj.transform, PrimitiveType.Cube, "Mandible_Chin",
                new Vector3(0, -0.02f, 0.045f), new Vector3(0.075f, 0.045f, 0.065f), Quaternion.identity, mandibleMat);

            // Bilateral Mandibular Rami ascending to TMJ condyles
            AddPrimitiveChild(mandibleObj.transform, PrimitiveType.Cube, "Ramus_L",
                new Vector3(-0.055f, 0.025f, -0.015f), new Vector3(0.022f, 0.075f, 0.04f), Quaternion.Euler(15, -15, 0), mandibleMat);
            AddPrimitiveChild(mandibleObj.transform, PrimitiveType.Cube, "Ramus_R",
                new Vector3(0.055f, 0.025f, -0.015f), new Vector3(0.022f, 0.075f, 0.04f), Quaternion.Euler(15, 15, 0), mandibleMat);
        }
        #endregion

        #region B. Vertebral Column & Spinal Cord
        private static void BuildVertebralColumnAndSpine(Transform parent,
            Material cervicalMat, Material thoracicMat, Material lumbarMat, Material sacrumMat, Material cordMat)
        {
            GameObject spineGroup = new GameObject("Spine_And_Cord");
            spineGroup.transform.SetParent(parent, false);

            // 1. Cervical Spine (C1-C7)
            BuildVertebraSection(spineGroup.transform, "Spine_Cervical", 7,
                new Vector3(0, 1.57f, -0.02f), new Vector3(0, 1.48f, -0.03f),
                0.055f, 0.045f, cervicalMat,
                "Cervical Vertebrae (C1-C7)", "Vertebrae Cervicales",
                "Superior 7 vertebrae supporting head mobility, flexion, extension, and rotation.",
                "Features transverse foramina transmitting vertebral arteries upward to the brain.",
                new Vector3(0, 0, -0.3f),
                "C1 (Atlas) lacks a body and cradles occipital condyles.",
                "C2 (Axis) features dens (odontoid process) serving as rotational pivot.");

            // 2. Thoracic Spine (T1-T12)
            BuildVertebraSection(spineGroup.transform, "Spine_Thoracic", 12,
                new Vector3(0, 1.47f, -0.035f), new Vector3(0, 1.18f, -0.055f),
                0.065f, 0.055f, thoracicMat,
                "Thoracic Vertebrae (T1-T12)", "Vertebrae Thoracicae",
                "Intermediate 12 vertebrae articulating with 12 pairs of ribs to enclose thoracic cage.",
                "Possesses costal facets for rib head and tubercle articulation; exhibits physiological kyphosis.",
                new Vector3(0, 0, -0.38f),
                "Long, downward-slanting spinous processes overlap like roof tiles.",
                "Provides sturdy thoracic rigidity to protect heart and lungs.");

            // 3. Lumbar Spine (L1-L5)
            BuildVertebraSection(spineGroup.transform, "Spine_Lumbar", 5,
                new Vector3(0, 1.17f, -0.055f), new Vector3(0, 1.02f, -0.03f),
                0.080f, 0.065f, lumbarMat,
                "Lumbar Vertebrae (L1-L5)", "Vertebrae Lumbales",
                "Heaviest 5 vertebrae supporting upper body weight and trunk flexibility.",
                "Characterized by massive kidney-shaped vertebral bodies and sturdy quadrangular spinous processes.",
                new Vector3(0, 0, -0.32f),
                "L4-L5 and L5-S1 intervertebral discs withstand greatest mechanical compressive loads.",
                "Natural lordotic curvature balances body center of gravity above pelvis.");

            // 4. Sacrum & Coccyx
            GameObject sacrumObj = CreateCompositeBone(spineGroup.transform, "Spine_Sacrum_Coccyx",
                new Vector3(0, 0.94f, -0.05f), sacrumMat,
                "Sacrum and Coccyx", "Os Sacrum et Coccygis",
                "Shield-shaped triangular bone formed by 5 fused sacral and 4 fused coccygeal vertebrae.",
                "Locks firmly between iliac bones at bilateral sacroiliac joints, distributing axial load to pelvis.",
                new Vector3(0, -0.15f, -0.25f),
                "Sacral canal houses the cauda equina nerve roots.",
                "Four pairs of anterior and posterior sacral foramina transmit sacral nerves.");

            AddPrimitiveChild(sacrumObj.transform, PrimitiveType.Cube, "Sacrum_Body",
                new Vector3(0, 0.02f, 0), new Vector3(0.12f, 0.11f, 0.055f), Quaternion.Euler(15, 0, 0), sacrumMat);
            AddPrimitiveChild(sacrumObj.transform, PrimitiveType.Cube, "Coccyx_Tip",
                new Vector3(0, -0.05f, -0.015f), new Vector3(0.045f, 0.04f, 0.03f), Quaternion.Euler(25, 0, 0), sacrumMat);

            // 5. Spinal Cord Region
            GameObject cordObj = CreateCompositeBone(spineGroup.transform, "Nervous_SpinalCord",
                new Vector3(0, 1.30f, -0.035f), cordMat,
                "Spinal Cord (Neural Canal)", "Medulla Spinalis",
                "Cylindrical central nerve bundle traversing vertebral canal from brainstem to conus medullaris.",
                "Transmits afferent sensory and efferent motor information; mediates autonomous somatic reflexes.",
                new Vector3(0, 0, -0.55f),
                "Gives rise to 31 bilateral pairs of spinal nerves.",
                "Terminates near L1-L2 vertebra level, continuing as cauda equina fibers.");

            AddPrimitiveChild(cordObj.transform, PrimitiveType.Cylinder, "SpinalCord_Tube",
                Vector3.zero, new Vector3(0.024f, 0.28f, 0.024f), Quaternion.identity, cordMat);
        }

        private static void BuildVertebraSection(Transform parent, string name, int count,
            Vector3 startPos, Vector3 endPos, float width, float depth, Material mat,
            string partName, string latin, string func, string desc, Vector3 explodeOffset, params string[] facts)
        {
            GameObject section = CreateCompositeBone(parent, name, Vector3.zero, mat,
                partName, latin, func, desc, explodeOffset, facts);

            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0f;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);

                // Vertebral body
                AddPrimitiveChild(section.transform, PrimitiveType.Cylinder, $"{name}_V{i}",
                    pos, new Vector3(width * 0.7f, 0.012f, width * 0.7f), Quaternion.identity, mat);

                // Spinous process
                AddPrimitiveChild(section.transform, PrimitiveType.Cube, $"{name}_Spine{i}",
                    pos + new Vector3(0, -0.005f, -depth * 0.4f), new Vector3(width * 0.25f, 0.018f, depth * 0.5f),
                    Quaternion.Euler(-18, 0, 0), mat);
            }
        }
        #endregion

        #region C. Rib Cage & Sternum
        private static void BuildRibCageAndSternum(Transform parent, Material sternumMat, Material ribsMat)
        {
            GameObject ribCage = new GameObject("RibCage_And_Sternum");
            ribCage.transform.SetParent(parent, false);

            // 1. Sternum (Breastbone)
            GameObject sternumObj = CreateCompositeBone(ribCage.transform, "Skeletal_Sternum",
                new Vector3(0, 1.34f, 0.11f), sternumMat,
                "Sternum (Breastbone)", "Sternum",
                "Flat elongated bone situated anteriorly along thoracic midline, anchoring anterior rib cage.",
                "Composed of 3 segments: manubrium, body (gladiolus), and xiphoid process.",
                new Vector3(0, 0, 0.45f),
                "Sternal angle (Angle of Louis) marks level of second costal cartilage.",
                "Clinically accessible site for red bone marrow aspiration.");

            // Manubrium
            AddPrimitiveChild(sternumObj.transform, PrimitiveType.Cube, "Sternum_Manubrium",
                new Vector3(0, 0.07f, 0), new Vector3(0.075f, 0.045f, 0.022f), Quaternion.identity, sternumMat);
            // Body (Gladiolus)
            AddPrimitiveChild(sternumObj.transform, PrimitiveType.Cube, "Sternum_Body",
                new Vector3(0, 0, 0), new Vector3(0.045f, 0.11f, 0.018f), Quaternion.identity, sternumMat);
            // Xiphoid process
            AddPrimitiveChild(sternumObj.transform, PrimitiveType.Cube, "Sternum_Xiphoid",
                new Vector3(0, -0.07f, 0), new Vector3(0.025f, 0.03f, 0.012f), Quaternion.identity, sternumMat);

            // 2. 12 Rib Pairs
            GameObject ribsObj = CreateCompositeBone(ribCage.transform, "Skeletal_RibCage_Complete",
                new Vector3(0, 1.33f, 0.02f), ribsMat,
                "Rib Cage (12 Pairs of Costae)", "Cavea Thoracis (Costae I-XII)",
                "Bilateral curved thoracic bone arches enclosing thoracic cavity; articulates with spine and sternum.",
                "Divided into 7 true ribs (vertebrosternal), 3 false ribs (vertebrochondral), and 2 floating ribs.",
                new Vector3(0, 0, 0.35f),
                "Intercostal spaces contain muscles, arteries, veins, and nerves for respiration.",
                "Elastic costal cartilages enable volumetric chest expansion during breathing.");

            int ribCount = 12;
            for (int i = 0; i < ribCount; i++)
            {
                float t = (float)i / (ribCount - 1);
                float y = Mathf.Lerp(0.12f, -0.15f, t);
                float widthScale = Mathf.Sin(t * Mathf.PI) * 0.08f + 0.11f;
                float depthScale = Mathf.Sin(t * Mathf.PI) * 0.05f + 0.08f;

                // Left rib arch lateral & anterior
                AddPrimitiveChild(ribsObj.transform, PrimitiveType.Cube, $"Rib_L_{i}_Lat",
                    new Vector3(-widthScale, y, 0), new Vector3(0.015f, 0.012f, depthScale * 1.6f), Quaternion.identity, ribsMat);
                AddPrimitiveChild(ribsObj.transform, PrimitiveType.Cube, $"Rib_L_{i}_Ant",
                    new Vector3(-widthScale * 0.5f, y, depthScale * 0.8f), new Vector3(widthScale * 0.9f, 0.012f, 0.015f), Quaternion.Euler(0, 20, 0), ribsMat);

                // Right rib arch lateral & anterior
                AddPrimitiveChild(ribsObj.transform, PrimitiveType.Cube, $"Rib_R_{i}_Lat",
                    new Vector3(widthScale, y, 0), new Vector3(0.015f, 0.012f, depthScale * 1.6f), Quaternion.identity, ribsMat);
                AddPrimitiveChild(ribsObj.transform, PrimitiveType.Cube, $"Rib_R_{i}_Ant",
                    new Vector3(widthScale * 0.5f, y, depthScale * 0.8f), new Vector3(widthScale * 0.9f, 0.012f, 0.015f), Quaternion.Euler(0, -20, 0), ribsMat);
            }
        }
        #endregion

        #region D. Pelvis
        private static void BuildPelvis(Transform parent, Material pelvisMat)
        {
            GameObject pelvisGroup = new GameObject("Pelvis_Complete");
            pelvisGroup.transform.SetParent(parent, false);

            BuildHipSide(pelvisGroup.transform, true, pelvisMat);
            BuildHipSide(pelvisGroup.transform, false, pelvisMat);
        }

        private static void BuildHipSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistrum" : "Dextrum";

            GameObject hipObj = CreateCompositeBone(parent, $"Skeletal_Pelvis_{sideName}",
                new Vector3(side * 0.11f, 0.94f, 0.01f), mat,
                $"{sideName} Hip Bone (Os Coxae)", $"Os Coxae {latinSide}",
                $"Forms {sideName.ToLower()} lateral and anterior pelvic girdle wall; transfers body weight to lower extremity.",
                "Formed by fusion of ilium (superior blade), ischium (inferoposterior), and pubis (anteromedial).",
                new Vector3(side * 0.3f, -0.1f, 0),
                "Acetabulum cup articulates with femoral head in a deep ball-and-socket joint.",
                "Iliac crest provides essential attachment for abdominal and gluteal muscles.");

            // Iliac crest blade
            AddPrimitiveChild(hipObj.transform, PrimitiveType.Cube, "Iliac_Blade",
                new Vector3(side * 0.03f, 0.05f, 0), new Vector3(0.09f, 0.08f, 0.035f), Quaternion.Euler(0, side * 20, side * 15), mat);

            // Ischium (sitting tuberosity)
            AddPrimitiveChild(hipObj.transform, PrimitiveType.Cube, "Ischium_Body",
                new Vector3(side * 0.02f, -0.04f, -0.03f), new Vector3(0.05f, 0.065f, 0.045f), Quaternion.identity, mat);

            // Pubic bone & symphysis arch
            AddPrimitiveChild(hipObj.transform, PrimitiveType.Cube, "Pubis_Body",
                new Vector3(side * -0.02f, -0.02f, 0.035f), new Vector3(0.055f, 0.05f, 0.045f), Quaternion.Euler(0, side * -25, 0), mat);
        }
        #endregion

        #region E. Shoulder Bones (Pectoral Girdle)
        private static void BuildShoulderBones(Transform parent, Material clavicleMat, Material scapulaMat)
        {
            GameObject shoulderGroup = new GameObject("ShoulderGirdle");
            shoulderGroup.transform.SetParent(parent, false);

            BuildShoulderSide(shoulderGroup.transform, true, clavicleMat, scapulaMat);
            BuildShoulderSide(shoulderGroup.transform, false, clavicleMat, scapulaMat);
        }

        private static void BuildShoulderSide(Transform parent, bool isLeft, Material clavicleMat, Material scapulaMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistra" : "Dextra";

            // 1. Clavicle (Collarbone)
            GameObject clavObj = CreateCompositeBone(parent, $"Skeletal_Clavicle_{sideName}",
                new Vector3(side * 0.10f, 1.48f, 0.06f), clavicleMat,
                $"{sideName} Clavicle (Collarbone)", $"Clavicula {latinSide}",
                "S-shaped horizontal strut bracing upper extremity away from thoracic cage.",
                "Connects upper limb to axial skeleton via sternoclavicular and acromioclavicular joints.",
                new Vector3(side * 0.2f, 0.15f, 0.25f),
                "Most commonly fractured long bone in the human body.",
                "Subclavian artery and brachial plexus pass directly underneath.");

            AddPrimitiveChild(clavObj.transform, PrimitiveType.Cylinder, "Clavicle_Shaft",
                Vector3.zero, new Vector3(0.022f, 0.09f, 0.022f), Quaternion.Euler(0, side * 15, 80 * side), clavicleMat);

            // 2. Scapula (Shoulder Blade)
            GameObject scapObj = CreateCompositeBone(parent, $"Skeletal_Scapula_{sideName}",
                new Vector3(side * 0.14f, 1.38f, -0.09f), scapulaMat,
                $"{sideName} Scapula (Shoulder Blade)", $"Scapula {latinSide}",
                "Triangular flat posterior bone anchoring rotator cuff musculature and shoulder articulation.",
                "Features posterior scapular spine, acromion, coracoid process, and shallow glenoid fossa.",
                new Vector3(side * 0.35f, 0.1f, -0.25f),
                "Glenoid cavity articulates with humeral head with exceptional range of motion.",
                "Glides across thoracic wall along physiological scapulothoracic joint plane.");

            // Triangular blade
            AddPrimitiveChild(scapObj.transform, PrimitiveType.Cube, "Scapula_Blade",
                Vector3.zero, new Vector3(0.095f, 0.13f, 0.016f), Quaternion.Euler(5, side * 22, 0), scapulaMat);

            // Scapular spine & acromion roof
            AddPrimitiveChild(scapObj.transform, PrimitiveType.Cube, "Scapula_Acromion",
                new Vector3(side * 0.03f, 0.05f, -0.01f), new Vector3(0.08f, 0.024f, 0.05f), Quaternion.Euler(0, side * 15, 0), scapulaMat);
        }
        #endregion

        #region F. Upper Limbs (Humerus, Radius, Ulna)
        private static void BuildUpperLimbs(Transform parent, Material humerusMat, Material radiusMat, Material ulnaMat)
        {
            GameObject armGroup = new GameObject("UpperLimbs");
            armGroup.transform.SetParent(parent, false);

            BuildArmSide(armGroup.transform, true, humerusMat, radiusMat, ulnaMat);
            BuildArmSide(armGroup.transform, false, humerusMat, radiusMat, ulnaMat);
        }

        private static void BuildArmSide(Transform parent, bool isLeft, Material humerusMat, Material radiusMat, Material ulnaMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistrum" : "Dextrum";
            string latinSideF = isLeft ? "Sinistra" : "Dextra";

            // 1. Humerus (Upper Arm)
            GameObject humerusObj = CreateCompositeBone(parent, $"Skeletal_Humerus_{sideName}",
                new Vector3(side * 0.22f, 1.32f, -0.01f), humerusMat,
                $"{sideName} Humerus", $"Humerus {latinSide}",
                "Long bone of upper arm articulating proximally at shoulder and distally at elbow joint.",
                "Features hemispherical head, greater/lesser tubercles, deltoid tuberosity, and distal condyles.",
                new Vector3(side * 0.35f, 0, 0),
                "Distal trochlea articulates with ulna; capitulum articulates with radial head.",
                "Radial groove accommodates radial nerve along posterior mid-shaft.");

            // Hemispherical Head (fitting glenoid cavity)
            AddPrimitiveChild(humerusObj.transform, PrimitiveType.Sphere, "Humerus_Head",
                new Vector3(side * -0.02f, 0.14f, 0), new Vector3(0.065f, 0.065f, 0.065f), Quaternion.identity, humerusMat);

            // Shaft (Diaphysis)
            AddPrimitiveChild(humerusObj.transform, PrimitiveType.Cylinder, "Humerus_Shaft",
                Vector3.zero, new Vector3(0.040f, 0.13f, 0.040f), Quaternion.Euler(0, 0, side * -4), humerusMat);

            // Distal Epicondyles (Elbow base)
            AddPrimitiveChild(humerusObj.transform, PrimitiveType.Cube, "Humerus_Epicondyles",
                new Vector3(0, -0.14f, 0), new Vector3(0.055f, 0.035f, 0.040f), Quaternion.identity, humerusMat);


            // 2. Radius (Lateral rotating forearm bone)
            GameObject radiusObj = CreateCompositeBone(parent, $"Skeletal_Radius_{sideName}",
                new Vector3(side * 0.26f, 1.03f, 0.01f), radiusMat,
                $"{sideName} Radius", $"Radius {latinSide}",
                "Lateral forearm long bone rotating around ulna to achieve forearm pronation and supination.",
                "Features cylindrical head, radial tuberosity, and broadened distal styloid process.",
                new Vector3(side * 0.45f, 0, 0.1f),
                "Distal articular surface forms radiocarpal joint with scaphoid and lunate wrist bones.",
                "Colles' fracture of distal radius is one of most common orthopedic injuries.");

            // Radial Head (pivoting disc)
            AddPrimitiveChild(radiusObj.transform, PrimitiveType.Cylinder, "Radius_Head",
                new Vector3(0, 0.11f, 0), new Vector3(0.032f, 0.015f, 0.032f), Quaternion.identity, radiusMat);

            // Shaft
            AddPrimitiveChild(radiusObj.transform, PrimitiveType.Cylinder, "Radius_Shaft",
                Vector3.zero, new Vector3(0.028f, 0.10f, 0.028f), Quaternion.identity, radiusMat);

            // Distal Styloid process (wrist articulation)
            AddPrimitiveChild(radiusObj.transform, PrimitiveType.Cube, "Radius_Distal",
                new Vector3(0, -0.11f, 0), new Vector3(0.038f, 0.025f, 0.032f), Quaternion.identity, radiusMat);


            // 3. Ulna (Medial forearm bone with olecranon elbow)
            GameObject ulnaObj = CreateCompositeBone(parent, $"Skeletal_Ulna_{sideName}",
                new Vector3(side * 0.21f, 1.03f, -0.02f), ulnaMat,
                $"{sideName} Ulna", $"Ulna {latinSideF}",
                "Medial forearm stabilizing long bone forming primary hinge component of elbow joint.",
                "Prominent olecranon process hooks into humeral olecranon fossa to prevent hyperextension.",
                new Vector3(side * 0.45f, 0, -0.1f),
                "Trochlear notch forms sturdy hinge articulation with humeral trochlea.",
                "Distal head features small styloid process non-articular with carpal bones directly.");

            // Olecranon process (elbow point)
            AddPrimitiveChild(ulnaObj.transform, PrimitiveType.Cube, "Ulna_Olecranon",
                new Vector3(0, 0.12f, -0.01f), new Vector3(0.035f, 0.04f, 0.045f), Quaternion.identity, ulnaMat);

            // Shaft
            AddPrimitiveChild(ulnaObj.transform, PrimitiveType.Cylinder, "Ulna_Shaft",
                Vector3.zero, new Vector3(0.026f, 0.105f, 0.026f), Quaternion.identity, ulnaMat);

            // Distal head & styloid
            AddPrimitiveChild(ulnaObj.transform, PrimitiveType.Sphere, "Ulna_DistalHead",
                new Vector3(0, -0.11f, 0), new Vector3(0.028f, 0.028f, 0.028f), Quaternion.identity, ulnaMat);
        }
        #endregion

        #region G. Hand Bones & Finger Structures
        private static void BuildHands(Transform parent, Material handMat)
        {
            GameObject handGroup = new GameObject("HandBones");
            handGroup.transform.SetParent(parent, false);

            BuildHandSide(handGroup.transform, true, handMat);
            BuildHandSide(handGroup.transform, false, handMat);
        }

        private static void BuildHandSide(Transform parent, bool isLeft, Material handMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistra" : "Dextra";

            GameObject handObj = CreateCompositeBone(parent, $"Skeletal_Hand_{sideName}",
                new Vector3(side * 0.23f, 0.81f, 0f), handMat,
                $"{sideName} Hand and Finger Bones", $"Ossa Manus {latinSide}",
                "Complex manipulative structure comprising 27 bones: 8 carpals, 5 metacarpals, and 14 phalanges.",
                "Arranged into wrist, palm, and five multi-jointed digital ray columns.",
                new Vector3(side * 0.5f, -0.15f, 0),
                "Carpal bones: scaphoid, lunate, triquetrum, pisiform, trapezium, trapezoid, capitate, hamate.",
                "Thumb (pollex) features opposable saddle carpometacarpal joint enabling human tool grasp.");

            // 1. Carpal block (wrist)
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_Carpals",
                new Vector3(0, 0.06f, 0), new Vector3(0.055f, 0.035f, 0.028f), Quaternion.identity, handMat);

            // 2. Metacarpal palm
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_Metacarpals",
                new Vector3(0, 0.02f, 0), new Vector3(0.058f, 0.055f, 0.020f), Quaternion.identity, handMat);

            // 3. Thumb (opposed ray)
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_Thumb",
                new Vector3(side * 0.04f, 0.015f, 0.015f), new Vector3(0.018f, 0.048f, 0.018f), Quaternion.Euler(0, 0, side * 35), handMat);

            // 4. Index Finger
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_Index",
                new Vector3(side * 0.020f, -0.035f, 0), new Vector3(0.014f, 0.058f, 0.014f), Quaternion.identity, handMat);

            // 5. Middle Finger
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_Middle",
                new Vector3(0, -0.042f, 0), new Vector3(0.014f, 0.065f, 0.014f), Quaternion.identity, handMat);

            // 6. Ring & Little Fingers
            AddPrimitiveChild(handObj.transform, PrimitiveType.Cube, "Hand_RingLittle",
                new Vector3(side * -0.020f, -0.032f, 0), new Vector3(0.025f, 0.052f, 0.014f), Quaternion.identity, handMat);
        }
        #endregion

        #region H. Lower Limbs (Femur, Patella, Tibia, Fibula)
        private static void BuildLowerLimbs(Transform parent, Material femurMat, Material patellaMat, Material tibiaMat, Material fibulaMat)
        {
            GameObject legGroup = new GameObject("LowerLimbs");
            legGroup.transform.SetParent(parent, false);

            BuildLegSide(legGroup.transform, true, femurMat, patellaMat, tibiaMat, fibulaMat);
            BuildLegSide(legGroup.transform, false, femurMat, patellaMat, tibiaMat, fibulaMat);
        }

        private static void BuildLegSide(Transform parent, bool isLeft, Material femurMat, Material patellaMat, Material tibiaMat, Material fibulaMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistrum" : "Dextrum";
            string latinSideF = isLeft ? "Sinistra" : "Dextra";

            // 1. Femur (Thigh bone)
            GameObject femurObj = CreateCompositeBone(parent, $"Skeletal_Femur_{sideName}",
                new Vector3(side * 0.10f, 0.70f, 0.01f), femurMat,
                $"{sideName} Femur (Thigh Bone)", $"Femur {latinSide}",
                "Longest, heaviest, and strongest tubular bone in human body, supporting locomotion and upright posture.",
                "Possesses spherical head angled inward at 125°, greater/lesser trochanters, and distal condyles.",
                new Vector3(side * 0.35f, 0, 0),
                "Can resist compressive forces exceeding 1,000 kg (1 ton).",
                "Distal condyles articulate with tibial plateau to form knee joint.");

            // Head (angled inward to acetabulum)
            AddPrimitiveChild(femurObj.transform, PrimitiveType.Sphere, "Femur_Head",
                new Vector3(side * -0.035f, 0.19f, 0), new Vector3(0.055f, 0.055f, 0.055f), Quaternion.identity, femurMat);

            // Greater trochanter (lateral)
            AddPrimitiveChild(femurObj.transform, PrimitiveType.Cube, "Femur_Trochanter",
                new Vector3(side * 0.032f, 0.18f, -0.01f), new Vector3(0.038f, 0.042f, 0.038f), Quaternion.identity, femurMat);

            // Shaft
            AddPrimitiveChild(femurObj.transform, PrimitiveType.Cylinder, "Femur_Shaft",
                Vector3.zero, new Vector3(0.045f, 0.18f, 0.045f), Quaternion.Euler(0, 0, side * -5), femurMat);

            // Distal Bicondylar Knee Base
            AddPrimitiveChild(femurObj.transform, PrimitiveType.Cube, "Femur_Condyles",
                new Vector3(0, -0.19f, 0), new Vector3(0.068f, 0.042f, 0.055f), Quaternion.identity, femurMat);


            // 2. Patella (Kneecap)
            GameObject patellaObj = CreateCompositeBone(parent, $"Skeletal_Patella_{sideName}",
                new Vector3(side * 0.10f, 0.48f, 0.06f), patellaMat,
                $"{sideName} Patella (Kneecap)", $"Patella {latinSideF}",
                "Largest sesamoid bone in human skeleton, embedded within quadriceps femoris tendon.",
                "Increases mechanical leverage of quadriceps tendon during knee extension by 30%.",
                new Vector3(side * 0.25f, 0, 0.35f),
                "Articular posterior facets glide across femoral patellar groove.",
                "Protects anterior knee joint capsule from traumatic impact.");

            AddPrimitiveChild(patellaObj.transform, PrimitiveType.Sphere, "Patella_Disc",
                Vector3.zero, new Vector3(0.048f, 0.055f, 0.028f), Quaternion.identity, patellaMat);


            // 3. Tibia (Shin bone)
            GameObject tibiaObj = CreateCompositeBone(parent, $"Skeletal_Tibia_{sideName}",
                new Vector3(side * 0.09f, 0.27f, 0f), tibiaMat,
                $"{sideName} Tibia (Shin Bone)", $"Tibia {latinSideF}",
                "Major weight-bearing long bone of lower leg; transmits all body mass down from knee to ankle.",
                "Features broad tibial plateau, anterior tibial crest, and prominent medial malleolus (inner ankle).",
                new Vector3(side * 0.38f, 0, 0),
                "Supports ~85% of lower leg mechanical body load.",
                "Subcutaneous anterior border is palpable along entirety of shin.");

            // Tibial plateau
            AddPrimitiveChild(tibiaObj.transform, PrimitiveType.Cube, "Tibia_Plateau",
                new Vector3(0, 0.18f, 0), new Vector3(0.065f, 0.035f, 0.052f), Quaternion.identity, tibiaMat);

            // Shin shaft
            AddPrimitiveChild(tibiaObj.transform, PrimitiveType.Cylinder, "Tibia_Shaft",
                Vector3.zero, new Vector3(0.042f, 0.16f, 0.042f), Quaternion.identity, tibiaMat);

            // Medial malleolus (inner ankle)
            AddPrimitiveChild(tibiaObj.transform, PrimitiveType.Cube, "Tibia_Malleolus",
                new Vector3(side * -0.018f, -0.17f, 0), new Vector3(0.024f, 0.038f, 0.032f), Quaternion.identity, tibiaMat);


            // 4. Fibula
            GameObject fibulaObj = CreateCompositeBone(parent, $"Skeletal_Fibula_{sideName}",
                new Vector3(side * 0.14f, 0.26f, -0.01f), fibulaMat,
                $"{sideName} Fibula", $"Fibula {latinSideF}",
                "Slender lateral stabilizing bone of leg anchoring muscles and reinforcing lateral ankle mortise.",
                "Non-weight-bearing long bone terminating distally as lateral malleolus (outer ankle).",
                new Vector3(side * 0.48f, 0, 0),
                "Head articulates proximally with lateral tibial condyle below knee joint.",
                "Commonly used as donor vascularized bone graft in reconstructive surgery.");

            AddPrimitiveChild(fibulaObj.transform, PrimitiveType.Cylinder, "Fibula_Shaft",
                Vector3.zero, new Vector3(0.020f, 0.17f, 0.020f), Quaternion.identity, fibulaMat);

            // Lateral malleolus (outer ankle)
            AddPrimitiveChild(fibulaObj.transform, PrimitiveType.Cube, "Fibula_Malleolus",
                new Vector3(0, -0.18f, 0), new Vector3(0.022f, 0.035f, 0.028f), Quaternion.identity, fibulaMat);
        }
        #endregion

        #region I. Foot Bones
        private static void BuildFeet(Transform parent, Material footMat)
        {
            GameObject footGroup = new GameObject("FootBones");
            footGroup.transform.SetParent(parent, false);

            BuildFootSide(footGroup.transform, true, footMat);
            BuildFootSide(footGroup.transform, false, footMat);
        }

        private static void BuildFootSide(Transform parent, bool isLeft, Material footMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistri" : "Dextri";

            GameObject footObj = CreateCompositeBone(parent, $"Skeletal_Foot_{sideName}",
                new Vector3(side * 0.10f, 0.04f, 0.06f), footMat,
                $"{sideName} Foot and Ankle Bones", $"Ossa Pedis {latinSide}",
                "Biomechanical platform comprising 26 bones: 7 tarsals (including calcaneus & talus), 5 metatarsals, 14 phalanges.",
                "Arranged into medial longitudinal, lateral longitudinal, and transverse shock-absorbing arches.",
                new Vector3(side * 0.4f, -0.15f, 0.3f),
                "Talus bone bears full body weight and articulates with tibial and fibular mortise.",
                "Calcaneus (heel bone) is largest tarsal bone, anchoring Achilles tendon (tendo calcaneus).");

            // Calcaneus (heel bone)
            AddPrimitiveChild(footObj.transform, PrimitiveType.Cube, "Calcaneus_Heel",
                new Vector3(0, 0.01f, -0.06f), new Vector3(0.055f, 0.045f, 0.085f), Quaternion.Euler(-10, 0, 0), footMat);

            // Talus dome (ankle joint)
            AddPrimitiveChild(footObj.transform, PrimitiveType.Cube, "Talus_Dome",
                new Vector3(0, 0.035f, 0.01f), new Vector3(0.055f, 0.040f, 0.055f), Quaternion.identity, footMat);

            // Metatarsals (anterior arch)
            AddPrimitiveChild(footObj.transform, PrimitiveType.Cube, "Metatarsals_Arch",
                new Vector3(side * 0.005f, 0, 0.075f), new Vector3(0.075f, 0.025f, 0.095f), Quaternion.Euler(8, 0, 0), footMat);

            // Toes phalanges
            AddPrimitiveChild(footObj.transform, PrimitiveType.Cube, "Toes_Phalanges",
                new Vector3(side * 0.005f, -0.008f, 0.14f), new Vector3(0.070f, 0.018f, 0.045f), Quaternion.identity, footMat);
        }
        #endregion

        #region Helpers
        private static GameObject CreateCompositeBone(
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
                systemType = AnatomySystemType.Skeletal,
                keyFunction = function,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat.color;
            part.hoverColor = Color.white;
            part.selectedColor = new Color(1.0f, 0.95f, 0.2f); // Golden glow selection
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

            // Make sure the child collider passes clicks up to the parent AnatomyPart
            return child;
        }

        private static Material CreateBoneMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.45f);
            mat.SetFloat("_Cull", 0); // Double-sided rendering to ensure 100% visibility
            return mat;
        }
        #endregion
    }
}
