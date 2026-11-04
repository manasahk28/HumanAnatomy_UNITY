using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomyMuscularGenerator
    {
        public static GameObject BuildFullMuscularSystem(Transform parent, Shader litShader)
        {
            GameObject muscleRoot = new GameObject("MuscularSystem_Realistic");
            muscleRoot.transform.SetParent(parent, false);

            // Educational muscle color palette
            Material matPec         = CreateMuscleMaterial(litShader, "Mat_Pectoralis",       new Color(0.88f, 0.22f, 0.26f)); // Crimson
            Material matDeltoid     = CreateMuscleMaterial(litShader, "Mat_Deltoid",          new Color(0.95f, 0.42f, 0.22f)); // Coral Amber
            Material matBiceps      = CreateMuscleMaterial(litShader, "Mat_Biceps",           new Color(0.96f, 0.32f, 0.22f)); // Vivid Red-Orange
            Material matTriceps     = CreateMuscleMaterial(litShader, "Mat_Triceps",          new Color(0.72f, 0.18f, 0.26f)); // Deep Burgundy
            Material matForearm     = CreateMuscleMaterial(litShader, "Mat_ForearmMuscles",   new Color(0.82f, 0.44f, 0.32f)); // Warm Terracotta
            Material matAbs         = CreateMuscleMaterial(litShader, "Mat_RectusAbdominis",  new Color(0.90f, 0.20f, 0.30f)); // Ruby Scarlet
            Material matObliques    = CreateMuscleMaterial(litShader, "Mat_ExternalObliques", new Color(0.86f, 0.36f, 0.24f)); // Russet
            Material matTrapezius   = CreateMuscleMaterial(litShader, "Mat_Trapezius",        new Color(0.66f, 0.22f, 0.38f)); // Royal Maroon
            Material matLats        = CreateMuscleMaterial(litShader, "Mat_LatissimusDorsi",  new Color(0.58f, 0.18f, 0.24f)); // Deep Mahogany
            Material matGluteal     = CreateMuscleMaterial(litShader, "Mat_Gluteals",         new Color(0.82f, 0.24f, 0.38f)); // Crimson Rose
            Material matQuads       = CreateMuscleMaterial(litShader, "Mat_Quadriceps",       new Color(0.93f, 0.26f, 0.22f)); // Bright Vermilion
            Material matHamstrings  = CreateMuscleMaterial(litShader, "Mat_Hamstrings",       new Color(0.74f, 0.20f, 0.25f)); // Garnet Red
            Material matCalf        = CreateMuscleMaterial(litShader, "Mat_CalfMuscles",      new Color(0.88f, 0.28f, 0.26f)); // Cardinal Red

            // 1. Anterior Thorax: Pectoralis Major
            BuildPectoralisMajor(muscleRoot.transform, matPec);

            // 2. Shoulder Cap: Deltoids
            BuildDeltoids(muscleRoot.transform, matDeltoid);

            // 3. Upper Arm: Biceps and Triceps
            BuildArmMuscles(muscleRoot.transform, matBiceps, matTriceps);

            // 4. Forearm Muscles
            BuildForearmMuscles(muscleRoot.transform, matForearm);

            // 5. Abdominal Wall: Rectus Abdominis
            BuildRectusAbdominis(muscleRoot.transform, matAbs);

            // 6. Abdominal Flanks: External Obliques
            BuildExternalObliques(muscleRoot.transform, matObliques);

            // 7. Upper Back & Neck: Trapezius
            BuildTrapezius(muscleRoot.transform, matTrapezius);

            // 8. Mid & Lower Back: Latissimus Dorsi
            BuildLatissimusDorsi(muscleRoot.transform, matLats);

            // 9. Pelvis / Buttocks: Gluteal Muscles
            BuildGluteals(muscleRoot.transform, matGluteal);

            // 10. Anterior Thigh: Quadriceps Femoris
            BuildQuadriceps(muscleRoot.transform, matQuads);

            // 11. Posterior Thigh: Hamstrings
            BuildHamstrings(muscleRoot.transform, matHamstrings);

            // 12. Posterior Leg / Calf: Gastrocnemius and Soleus
            BuildCalves(muscleRoot.transform, matCalf);

            return muscleRoot;
        }

        #region 1. Pectoralis Major
        private static void BuildPectoralisMajor(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_PectoralisMajor");
            group.transform.SetParent(parent, false);

            BuildPecSide(group.transform, true, mat);
            BuildPecSide(group.transform, false, mat);
        }

        private static void BuildPecSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject pecObj = CreateCompositeMuscle(parent, $"Muscular_PectoralisMajor_{sideName}",
                new Vector3(side * 0.09f, 1.37f, 0.12f), mat,
                $"{sideName} Pectoralis Major", $"Musculus Pectoralis Major {latinSide}",
                "Powerful thick fan-shaped muscle covering upper anterior chest wall.",
                "Originates from medial clavicle, sternum, and 1st-6th costal cartilages; inserts onto lateral crest of greater tubercle of humerus.",
                new Vector3(side * 0.25f, 0.1f, 0.35f),
                "Primary Action: Adduction, internal rotation, and forward flexion of arm.",
                "Innervation: Medial and lateral pectoral nerves (C5–T1).",
                "Forms anterior axillary fold framing armpit.");

            // Medial sternal head
            AddPrimitiveChild(pecObj.transform, PrimitiveType.Cube, "Pec_SternalHead",
                new Vector3(side * -0.01f, -0.01f, 0), new Vector3(0.12f, 0.13f, 0.045f), Quaternion.Euler(0, side * 15, 0), mat);

            // Clavicular superior head radiating to shoulder
            AddPrimitiveChild(pecObj.transform, PrimitiveType.Cube, "Pec_ClavicularHead",
                new Vector3(side * 0.04f, 0.05f, 0), new Vector3(0.10f, 0.07f, 0.040f), Quaternion.Euler(0, side * 25, side * 15), mat);
        }
        #endregion

        #region 2. Deltoids
        private static void BuildDeltoids(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Deltoids");
            group.transform.SetParent(parent, false);

            BuildDeltoidSide(group.transform, true, mat);
            BuildDeltoidSide(group.transform, false, mat);
        }

        private static void BuildDeltoidSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinisteus" : "Dexter";

            GameObject deltObj = CreateCompositeMuscle(parent, $"Muscular_Deltoid_{sideName}",
                new Vector3(side * 0.23f, 1.43f, -0.01f), mat,
                $"{sideName} Deltoid", $"Musculus Deltoideus {latinSide}",
                "Triangular multipennate muscle cap wrapping the glenohumeral shoulder articulation.",
                "Originates from lateral clavicle, acromion, and spine of scapula; converges into deltoid tuberosity on lateral humeral shaft.",
                new Vector3(side * 0.38f, 0.15f, 0),
                "Primary Action: Prime mover of shoulder abduction (15° to 90°); anterior head flexes, posterior head extends.",
                "Innervation: Axillary nerve (C5–C6).",
                "Common intramuscular injection site.");

            // Lateral multipennate head
            AddPrimitiveChild(deltObj.transform, PrimitiveType.Sphere, "Deltoid_Lateral",
                new Vector3(side * 0.02f, 0, 0), new Vector3(0.085f, 0.13f, 0.085f), Quaternion.Euler(0, 0, side * -10), mat);

            // Anterior head
            AddPrimitiveChild(deltObj.transform, PrimitiveType.Sphere, "Deltoid_Anterior",
                new Vector3(side * 0.01f, 0.02f, 0.035f), new Vector3(0.065f, 0.11f, 0.065f), Quaternion.identity, mat);

            // Posterior head
            AddPrimitiveChild(deltObj.transform, PrimitiveType.Sphere, "Deltoid_Posterior",
                new Vector3(side * 0.01f, 0.01f, -0.035f), new Vector3(0.065f, 0.11f, 0.065f), Quaternion.identity, mat);
        }
        #endregion

        #region 3. Biceps and Triceps
        private static void BuildArmMuscles(Transform parent, Material bicepsMat, Material tricepsMat)
        {
            GameObject group = new GameObject("Muscles_Biceps_Triceps");
            group.transform.SetParent(parent, false);

            BuildArmMuscleSide(group.transform, true, bicepsMat, tricepsMat);
            BuildArmMuscleSide(group.transform, false, bicepsMat, tricepsMat);
        }

        private static void BuildArmMuscleSide(Transform parent, bool isLeft, Material bicepsMat, Material tricepsMat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            // A. Biceps Brachii (Anterior Compartment)
            GameObject bicepsObj = CreateCompositeMuscle(parent, $"Muscular_Biceps_{sideName}",
                new Vector3(side * 0.23f, 1.30f, 0.025f), bicepsMat,
                $"{sideName} Biceps Brachii", $"Musculus Biceps Brachii {latinSide}",
                "Two-headed fusiform muscle spanning anterior upper arm between shoulder and elbow.",
                "Originates via long head (supraglenoid tubercle) and short head (coracoid process); inserts into radial tuberosity.",
                new Vector3(side * 0.35f, 0, 0.25f),
                "Primary Action: Most powerful forearm supinator; strong elbow flexor.",
                "Innervation: Musculocutaneous nerve (C5–C6).",
                "Long head tendon passes through intertubercular bicipital groove.");

            AddPrimitiveChild(bicepsObj.transform, PrimitiveType.Capsule, "Biceps_Belly",
                Vector3.zero, new Vector3(0.062f, 0.11f, 0.062f), Quaternion.Euler(0, 0, side * -4), bicepsMat);

            // B. Triceps Brachii (Posterior Compartment)
            GameObject tricepsObj = CreateCompositeMuscle(parent, $"Muscular_Triceps_{sideName}",
                new Vector3(side * 0.23f, 1.30f, -0.045f), tricepsMat,
                $"{sideName} Triceps Brachii", $"Musculus Triceps Brachii {latinSide}",
                "Three-headed muscle occupying entire posterior compartment of upper arm.",
                "Originates from infraglenoid tubercle (long head) and posterior humeral shaft (lateral & medial heads); inserts onto olecranon process of ulna.",
                new Vector3(side * 0.35f, 0, -0.25f),
                "Primary Action: Sole major extensor of elbow joint; long head stabilizes abducted shoulder.",
                "Innervation: Radial nerve (C6–C8).",
                "Antagonist to biceps brachii and brachialis.");

            AddPrimitiveChild(tricepsObj.transform, PrimitiveType.Capsule, "Triceps_Belly",
                Vector3.zero, new Vector3(0.068f, 0.13f, 0.068f), Quaternion.Euler(0, 0, side * -4), tricepsMat);
        }
        #endregion

        #region 4. Forearm Muscles
        private static void BuildForearmMuscles(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Forearm");
            group.transform.SetParent(parent, false);

            BuildForearmSide(group.transform, true, mat);
            BuildForearmSide(group.transform, false, mat);
        }

        private static void BuildForearmSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistrum" : "Dextrum";

            GameObject forearmObj = CreateCompositeMuscle(parent, $"Muscular_Forearm_{sideName}",
                new Vector3(side * 0.25f, 1.02f, 0), mat,
                $"{sideName} Forearm Muscle Compartment", $"Musculi Antebrachii {latinSide}",
                "Dense complex of anterior flexor-pronator and posterior extensor-supinator muscle groups.",
                "Originates from medial/lateral humeral epicondyles; extends tendons across carpal tunnel and extensor retinaculum to hand digits.",
                new Vector3(side * 0.45f, 0, 0),
                "Primary Action: Controls wrist articulation, forearm pronation/supination, and delicate finger grip dexterity.",
                "Innervation: Median, Ulnar, and Radial nerves.",
                "Includes brachioradialis, flexor carpi radialis, extensor digitorum.");

            // Proximal muscular taper
            AddPrimitiveChild(forearmObj.transform, PrimitiveType.Capsule, "Forearm_ProximalBelly",
                new Vector3(0, 0.03f, 0), new Vector3(0.075f, 0.11f, 0.075f), Quaternion.identity, mat);

            // Distal tendinous taper
            AddPrimitiveChild(forearmObj.transform, PrimitiveType.Cylinder, "Forearm_DistalTendons",
                new Vector3(0, -0.07f, 0), new Vector3(0.052f, 0.06f, 0.052f), Quaternion.identity, mat);
        }
        #endregion

        #region 5. Rectus Abdominis
        private static void BuildRectusAbdominis(Transform parent, Material mat)
        {
            GameObject absObj = CreateCompositeMuscle(parent, "Muscular_RectusAbdominis",
                new Vector3(0, 1.14f, 0.10f), mat,
                "Rectus Abdominis (Abdominals)", "Musculus Rectus Abdominis",
                "Paired vertical muscle strap separated by linea alba and partitioned by tendinous intersections (the 'six-pack').",
                "Originates from pubic crest and pubic symphysis; inserts into 5th–7th costal cartilages and xiphoid process.",
                new Vector3(0, 0, 0.4f),
                "Primary Action: Flexes thoracic and lumbar vertebral column; raises intra-abdominal pressure during forced expiration.",
                "Innervation: Lower thoracic intercostal and subcostal nerves (T7–T12).",
                "Enclosed inside aponeurotic rectus sheath.");

            // Left column (4 segmented blocks)
            for (int i = 0; i < 4; i++)
            {
                float y = Mathf.Lerp(0.08f, -0.08f, i / 3f);
                AddPrimitiveChild(absObj.transform, PrimitiveType.Cube, $"Abs_L_{i}",
                    new Vector3(-0.045f, y, 0), new Vector3(0.055f, 0.042f, 0.028f), Quaternion.identity, mat);
                AddPrimitiveChild(absObj.transform, PrimitiveType.Cube, $"Abs_R_{i}",
                    new Vector3(0.045f, y, 0), new Vector3(0.055f, 0.042f, 0.028f), Quaternion.identity, mat);
            }
        }
        #endregion

        #region 6. External Obliques
        private static void BuildExternalObliques(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_ExternalObliques");
            group.transform.SetParent(parent, false);

            BuildObliqueSide(group.transform, true, mat);
            BuildObliqueSide(group.transform, false, mat);
        }

        private static void BuildObliqueSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject oblObj = CreateCompositeMuscle(parent, $"Muscular_ExternalOblique_{sideName}",
                new Vector3(side * 0.14f, 1.13f, 0.04f), mat,
                $"{sideName} External Oblique", $"Musculus Obliquus Externus {latinSide}",
                "Broad flat superficial muscle forming lateral abdominal wall with downward/forward diagonal fiber orientation.",
                "Originates from external surfaces of 5th–12th ribs; inserts onto iliac crest and anterior abdominal aponeurosis.",
                new Vector3(side * 0.35f, 0, 0.25f),
                "Primary Action: Bilaterally flexes trunk; unilaterally flexes and rotates trunk to opposite side.",
                "Innervation: Lower intercostal nerves and subcostal nerve (T7–T12).",
                "Inferior aponeurotic border folds back upon itself to form inguinal ligament (Poupart's ligament).");

            AddPrimitiveChild(oblObj.transform, PrimitiveType.Cube, "Oblique_Flank",
                Vector3.zero, new Vector3(0.065f, 0.17f, 0.09f), Quaternion.Euler(0, side * 35, side * 10), mat);
        }
        #endregion

        #region 7. Trapezius
        private static void BuildTrapezius(Transform parent, Material mat)
        {
            GameObject trapObj = CreateCompositeMuscle(parent, "Muscular_Trapezius",
                new Vector3(0, 1.48f, -0.08f), mat,
                "Trapezius (Upper Back & Neck)", "Musculus Trapezius",
                "Large diamond/kite-shaped superficial muscle covering posterior neck and superior trunk.",
                "Originates from external occipital protuberance, nuchal ligament, and C7–T12 spinous processes; inserts into lateral clavicle, acromion, and spine of scapula.",
                new Vector3(0, 0.15f, -0.35f),
                "Primary Action: Elevates, retracts, rotates, and depresses scapula; extends and laterally flexes neck.",
                "Innervation: Spinal accessory nerve (Cranial Nerve XI) and C3–C4 cervical plexus fibers.",
                "Divided into upper (descending), middle (transverse), and lower (ascending) functional fibers.");

            // Upper cervical/neck trapezial crest
            AddPrimitiveChild(trapObj.transform, PrimitiveType.Cube, "Trap_Cervical",
                new Vector3(0, 0.09f, 0.02f), new Vector3(0.14f, 0.09f, 0.035f), Quaternion.Euler(15, 0, 0), mat);

            // Middle broad scapular sheet
            AddPrimitiveChild(trapObj.transform, PrimitiveType.Cube, "Trap_MiddleBlade",
                new Vector3(0, 0.01f, 0), new Vector3(0.28f, 0.11f, 0.035f), Quaternion.identity, mat);

            // Lower thoracic descending wedge
            AddPrimitiveChild(trapObj.transform, PrimitiveType.Cube, "Trap_LowerThoracic",
                new Vector3(0, -0.09f, -0.01f), new Vector3(0.16f, 0.11f, 0.030f), Quaternion.Euler(-10, 0, 0), mat);
        }
        #endregion

        #region 8. Latissimus Dorsi
        private static void BuildLatissimusDorsi(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_LatissimusDorsi");
            group.transform.SetParent(parent, false);

            BuildLatSide(group.transform, true, mat);
            BuildLatSide(group.transform, false, mat);
        }

        private static void BuildLatSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject latObj = CreateCompositeMuscle(parent, $"Muscular_LatissimusDorsi_{sideName}",
                new Vector3(side * 0.13f, 1.23f, -0.08f), mat,
                $"{sideName} Latissimus Dorsi", $"Musculus Latissimus Dorsi {latinSide}",
                "Broadest muscle of human back; triangular sheet creating the athletic 'V-taper' silhouette.",
                "Originates from T7–T12 spinous processes, thoracolumbar fascia, and iliac crest; wraps around to insert onto intertubercular groove of humerus.",
                new Vector3(side * 0.35f, 0, -0.3f),
                "Primary Action: Powerful arm extension, adduction, and internal rotation ('swimmer's muscle').",
                "Innervation: Thoracodorsal nerve (C6–C8).",
                "Crucial power generator for climbing, rowing, and pull-up biomechanics.");

            AddPrimitiveChild(latObj.transform, PrimitiveType.Cube, "Lat_BackWing",
                Vector3.zero, new Vector3(0.12f, 0.19f, 0.040f), Quaternion.Euler(0, side * -20, side * 15), mat);
        }
        #endregion

        #region 9. Gluteal Muscles
        private static void BuildGluteals(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Gluteals");
            group.transform.SetParent(parent, false);

            BuildGluteSide(group.transform, true, mat);
            BuildGluteSide(group.transform, false, mat);
        }

        private static void BuildGluteSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject gluteObj = CreateCompositeMuscle(parent, $"Muscular_Gluteal_{sideName}",
                new Vector3(side * 0.13f, 0.90f, -0.07f), mat,
                $"{sideName} Gluteal Complex (Maximus & Medius)", $"Musculus Gluteus {latinSide}",
                "Heaviest and most massive muscle complex in human anatomy, essential for upright bipedal standing.",
                "Originates from posterior ilium, sacrum, and coccyx; inserts into gluteal tuberosity of femur and iliotibial (IT) tract.",
                new Vector3(side * 0.3f, -0.1f, -0.3f),
                "Primary Action: Powerful hip extension (running, stair climbing, rising from chair); gluteus medius abducts and levels pelvis.",
                "Innervation: Inferior and superior gluteal nerves (L4–S2).",
                "Hypertrophy distinguishes hominid bipedal posture from all other primates.");

            // Gluteus Maximus (Posterior power mass)
            AddPrimitiveChild(gluteObj.transform, PrimitiveType.Sphere, "Gluteus_Maximus",
                new Vector3(0, 0, 0), new Vector3(0.14f, 0.16f, 0.13f), Quaternion.Euler(15, side * 15, 0), mat);

            // Gluteus Medius (Anterolateral stabilizer)
            AddPrimitiveChild(gluteObj.transform, PrimitiveType.Sphere, "Gluteus_Medius",
                new Vector3(side * 0.03f, 0.07f, 0.03f), new Vector3(0.09f, 0.09f, 0.09f), Quaternion.identity, mat);
        }
        #endregion

        #region 10. Quadriceps
        private static void BuildQuadriceps(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Quadriceps");
            group.transform.SetParent(parent, false);

            BuildQuadSide(group.transform, true, mat);
            BuildQuadSide(group.transform, false, mat);
        }

        private static void BuildQuadSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject quadObj = CreateCompositeMuscle(parent, $"Muscular_Quadriceps_{sideName}",
                new Vector3(side * 0.11f, 0.68f, 0.05f), mat,
                $"{sideName} Quadriceps Femoris", $"Musculus Quadriceps Femoris {latinSide}",
                "Massive four-headed muscle filling anterior thigh compartment.",
                "Composed of Rectus Femoris, Vastus Lateralis, Vastus Medialis, and Vastus Intermedius; converges onto patella and tibial tuberosity via patellar tendon.",
                new Vector3(side * 0.35f, 0, 0.3f),
                "Primary Action: Sole major extensor of knee joint; rectus femoris also assists hip flexion.",
                "Innervation: Femoral nerve (L2–L4).",
                "Vastus medialis oblique (VMO) stabilizes patellar tracking during final extension.");

            // Rectus Femoris (central bipennate column)
            AddPrimitiveChild(quadObj.transform, PrimitiveType.Capsule, "Quad_RectusFemoris",
                new Vector3(0, 0.02f, 0.01f), new Vector3(0.07f, 0.19f, 0.07f), Quaternion.identity, mat);

            // Vastus Lateralis (powerful outer sweep)
            AddPrimitiveChild(quadObj.transform, PrimitiveType.Capsule, "Quad_VastusLateralis",
                new Vector3(side * 0.045f, 0, -0.01f), new Vector3(0.075f, 0.18f, 0.075f), Quaternion.Euler(0, 0, side * -5), mat);

            // Vastus Medialis (teardrop inner bulge)
            AddPrimitiveChild(quadObj.transform, PrimitiveType.Capsule, "Quad_VastusMedialis",
                new Vector3(side * -0.035f, -0.06f, 0), new Vector3(0.065f, 0.12f, 0.065f), Quaternion.Euler(0, 0, side * 8), mat);
        }
        #endregion

        #region 11. Hamstrings
        private static void BuildHamstrings(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Hamstrings");
            group.transform.SetParent(parent, false);

            BuildHamstringSide(group.transform, true, mat);
            BuildHamstringSide(group.transform, false, mat);
        }

        private static void BuildHamstringSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinistrum" : "Dextrum";

            GameObject hamObj = CreateCompositeMuscle(parent, $"Muscular_Hamstrings_{sideName}",
                new Vector3(side * 0.11f, 0.68f, -0.05f), mat,
                $"{sideName} Hamstring Muscle Group", $"Musculi Ischiocrurales {latinSide}",
                "Three-muscle posterior thigh group: Biceps Femoris, Semitendinosus, and Semimembranosus.",
                "Originates predominantly from ischial tuberosity of pelvis; inserts onto head of fibula and medial tibial condyle.",
                new Vector3(side * 0.35f, 0, -0.3f),
                "Primary Action: Primary knee flexor; secondary hip extensor during sprinting and walking stride.",
                "Innervation: Sciatic nerve (Tibial & Common Fibular branches, L5–S2).",
                "Hamstring strain injuries frequently occur during eccentric deceleration in high-speed sprinting.");

            // Lateral head: Biceps Femoris
            AddPrimitiveChild(hamObj.transform, PrimitiveType.Capsule, "Ham_BicepsFemoris",
                new Vector3(side * 0.025f, 0, 0), new Vector3(0.062f, 0.18f, 0.062f), Quaternion.identity, mat);

            // Medial head: Semitendinosus / Semimembranosus
            AddPrimitiveChild(hamObj.transform, PrimitiveType.Capsule, "Ham_Semitendinosus",
                new Vector3(side * -0.025f, 0, 0), new Vector3(0.060f, 0.18f, 0.060f), Quaternion.identity, mat);
        }
        #endregion

        #region 12. Gastrocnemius & Soleus (Calf)
        private static void BuildCalves(Transform parent, Material mat)
        {
            GameObject group = new GameObject("Muscles_Calves");
            group.transform.SetParent(parent, false);

            BuildCalfSide(group.transform, true, mat);
            BuildCalfSide(group.transform, false, mat);
        }

        private static void BuildCalfSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            GameObject calfObj = CreateCompositeMuscle(parent, $"Muscular_GastrocnemiusSoleus_{sideName}",
                new Vector3(side * 0.10f, 0.28f, -0.045f), mat,
                $"{sideName} Gastrocnemius and Soleus (Calf)", $"Musculus Triceps Surae {latinSide}",
                "Two-part posterior leg complex uniting into the calcaneal tendon (Achilles tendon).",
                "Gastrocnemius originates from femoral condyles (two superficial heads); deep soleus originates from posterior tibia/fibula; unites into Achilles tendon inserting onto calcaneus.",
                new Vector3(side * 0.35f, -0.1f, -0.25f),
                "Primary Action: Prime mover of foot plantar flexion; crucial for propulsion in running, jumping, and bipedal gait.",
                "Innervation: Tibial nerve (S1–S2).",
                "Soleus is heavily rich in slow-twitch fibers for postural anti-gravity endurance.");

            // Gastrocnemius Medial Head
            AddPrimitiveChild(calfObj.transform, PrimitiveType.Sphere, "Gastroc_MedialHead",
                new Vector3(side * -0.022f, 0.06f, 0), new Vector3(0.052f, 0.11f, 0.052f), Quaternion.identity, mat);

            // Gastrocnemius Lateral Head
            AddPrimitiveChild(calfObj.transform, PrimitiveType.Sphere, "Gastroc_LateralHead",
                new Vector3(side * 0.022f, 0.06f, 0), new Vector3(0.048f, 0.10f, 0.048f), Quaternion.identity, mat);

            // Deep Soleus belly
            AddPrimitiveChild(calfObj.transform, PrimitiveType.Capsule, "Soleus_Belly",
                new Vector3(0, 0.01f, 0.01f), new Vector3(0.068f, 0.13f, 0.050f), Quaternion.identity, mat);

            // Achilles Tendon
            AddPrimitiveChild(calfObj.transform, PrimitiveType.Cylinder, "Achilles_Tendon",
                new Vector3(0, -0.09f, -0.005f), new Vector3(0.024f, 0.06f, 0.024f), Quaternion.identity, mat);
        }
        #endregion

        #region Helpers
        private static GameObject CreateCompositeMuscle(
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
                systemType = AnatomySystemType.Muscular,
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

            return child;
        }

        private static Material CreateMuscleMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.50f);
            mat.SetFloat("_Cull", 0); // Double-sided rendering
            return mat;
        }
        #endregion
    }
}

