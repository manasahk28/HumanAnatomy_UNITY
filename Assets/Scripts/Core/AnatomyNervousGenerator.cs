using System.Collections.Generic;
using UnityEngine;

namespace HumanAnatomy
{
    public static class AnatomyNervousGenerator
    {
        public static GameObject BuildFullNervousSystem(Transform parent, Shader litShader)
        {
            GameObject nervousRoot = new GameObject("NervousSystem_Realistic");
            nervousRoot.transform.SetParent(parent, false);

            // Educational nervous system color palette
            Material matFrontal     = CreateNerveMaterial(litShader, "Mat_FrontalLobe",    new Color(0.35f, 0.70f, 0.95f)); // Sky Blue
            Material matParietal    = CreateNerveMaterial(litShader, "Mat_ParietalLobe",   new Color(0.95f, 0.45f, 0.45f)); // Coral Pink
            Material matOccipital   = CreateNerveMaterial(litShader, "Mat_OccipitalLobe",  new Color(0.70f, 0.40f, 0.85f)); // Violet
            Material matTemporal    = CreateNerveMaterial(litShader, "Mat_TemporalLobe",   new Color(0.30f, 0.80f, 0.50f)); // Mint Emerald
            Material matCerebellum  = CreateNerveMaterial(litShader, "Mat_Cerebellum",     new Color(0.95f, 0.65f, 0.20f)); // Amber Orange
            Material matBrainstem   = CreateNerveMaterial(litShader, "Mat_Brainstem",      new Color(0.95f, 0.80f, 0.25f)); // Light Gold

            Material matCord        = CreateNerveMaterial(litShader, "Mat_SpinalCord",     new Color(1.00f, 0.88f, 0.20f)); // Bright Electric Gold
            Material matGanglia     = CreateNerveMaterial(litShader, "Mat_SympatheticTrunk",new Color(0.95f, 0.75f, 0.15f)); // Golden Yellow
            Material matArmNerves   = CreateNerveMaterial(litShader, "Mat_ArmNerves",      new Color(1.00f, 0.92f, 0.30f)); // Brilliant Nerve Yellow
            Material matLegNerves   = CreateNerveMaterial(litShader, "Mat_LegNerves",      new Color(1.00f, 0.85f, 0.25f)); // Deep Nerve Gold

            // 1. Brain with Distinguishable Major Regions
            BuildBrainMajorRegions(nervousRoot.transform, matFrontal, matParietal, matOccipital, matTemporal, matCerebellum, matBrainstem);

            // 2. Spinal Cord & Cauda Equina
            BuildSpinalCordSystem(nervousRoot.transform, matCord, matGanglia);

            // 3. Peripheral Nerves & Brachial Plexus into Arms
            BuildArmNervePathways(nervousRoot.transform, matArmNerves);

            // 4. Lumbosacral Plexus & Major Leg Nerves (Sciatic, Femoral, Tibial, Peroneal)
            BuildLegNervePathways(nervousRoot.transform, matLegNerves);

            return nervousRoot;
        }

        #region 1. Brain with Distinguishable Regions
        private static void BuildBrainMajorRegions(Transform parent,
            Material matFrontal, Material matParietal, Material matOccipital,
            Material matTemporal, Material matCerebellum, Material matBrainstem)
        {
            GameObject brainGroup = new GameObject("Brain_Complete");
            brainGroup.transform.SetParent(parent, false);
            brainGroup.transform.localPosition = new Vector3(0, 1.76f, 0);

            // A. Frontal Lobes
            GameObject frontalObj = CreateCompositeNerve(brainGroup.transform, "Nervous_FrontalLobe",
                new Vector3(0, 0.05f, 0.045f), matFrontal,
                "Frontal Lobes (Left & Right)", "Lobi Frontales",
                "Largest cerebral lobes responsible for executive cognitive control, voluntary motor initiation, language expression, and personality.",
                "Contains primary motor cortex (precentral gyrus), premotor cortex, and Broca's motor speech area.",
                new Vector3(0, 0.5f, 0.2f),
                "Coordinates complex decision making and emotional regulation.",
                "Damaged in frontal lobe dementia or Phineas Gage-type traumatic injuries.");

            AddPrimitiveChild(frontalObj.transform, PrimitiveType.Sphere, "Frontal_Left",
                new Vector3(-0.045f, 0, 0), new Vector3(0.085f, 0.09f, 0.10f), Quaternion.identity, matFrontal);
            AddPrimitiveChild(frontalObj.transform, PrimitiveType.Sphere, "Frontal_Right",
                new Vector3(0.045f, 0, 0), new Vector3(0.085f, 0.09f, 0.10f), Quaternion.identity, matFrontal);

            // B. Parietal Lobes
            GameObject parietalObj = CreateCompositeNerve(brainGroup.transform, "Nervous_ParietalLobe",
                new Vector3(0, 0.065f, -0.035f), matParietal,
                "Parietal Lobes (Left & Right)", "Lobi Parietales",
                "Primary somatosensory integration center processing somatic sensations (touch, pain, temperature, proprioception) and spatial coordinates.",
                "Houses the primary somatosensory cortex (postcentral gyrus) and sensory homunculus representation.",
                new Vector3(0, 0.55f, -0.1f),
                "Integrates multimodal sensory cues into a cohesive spatial map.",
                "Lesions in non-dominant parietal lobe produce hemispatial neglect syndrome.");

            AddPrimitiveChild(parietalObj.transform, PrimitiveType.Sphere, "Parietal_Left",
                new Vector3(-0.045f, 0, 0), new Vector3(0.08f, 0.085f, 0.09f), Quaternion.identity, matParietal);
            AddPrimitiveChild(parietalObj.transform, PrimitiveType.Sphere, "Parietal_Right",
                new Vector3(0.045f, 0, 0), new Vector3(0.08f, 0.085f, 0.09f), Quaternion.identity, matParietal);

            // C. Occipital Lobes
            GameObject occipitalObj = CreateCompositeNerve(brainGroup.transform, "Nervous_OccipitalLobe",
                new Vector3(0, 0.015f, -0.09f), matOccipital,
                "Occipital Lobes (Visual Cortex)", "Lobi Occipitales",
                "Posterior cerebral processing hub dedicated to visual perception, color discrimination, motion tracking, and spatial object recognition.",
                "Surrounds calcarine sulcus hosting the primary visual cortex (Brodmann Area 17 / Striate Cortex).",
                new Vector3(0, 0.4f, -0.3f),
                "Receives retinotopic visual inputs from optic radiation via lateral geniculate nucleus.",
                "Damage causes cortical blindness or visual agnosia.");

            AddPrimitiveChild(occipitalObj.transform, PrimitiveType.Sphere, "Occipital_Left",
                new Vector3(-0.035f, 0, 0), new Vector3(0.065f, 0.07f, 0.065f), Quaternion.identity, matOccipital);
            AddPrimitiveChild(occipitalObj.transform, PrimitiveType.Sphere, "Occipital_Right",
                new Vector3(0.035f, 0, 0), new Vector3(0.065f, 0.07f, 0.065f), Quaternion.identity, matOccipital);

            // D. Temporal Lobes
            GameObject temporalObj = CreateCompositeNerve(brainGroup.transform, "Nervous_TemporalLobe",
                new Vector3(0, -0.01f, 0.01f), matTemporal,
                "Temporal Lobes (Left & Right)", "Lobi Temporales",
                "Lateral cerebral lobes managing auditory processing, language comprehension (Wernicke's area), and memory encoding.",
                "Houses the hippocampus, amygdala, and primary auditory cortex (transverse temporal gyri of Heschl).",
                new Vector3(0, 0.3f, 0),
                "Hippocampal formations inside medial temporal lobes consolidate short-term to long-term memory.",
                "Temporal lobe epilepsy can trigger déjà vu and auditory auras.");

            AddPrimitiveChild(temporalObj.transform, PrimitiveType.Sphere, "Temporal_Left",
                new Vector3(-0.08f, 0, 0), new Vector3(0.065f, 0.065f, 0.095f), Quaternion.identity, matTemporal);
            AddPrimitiveChild(temporalObj.transform, PrimitiveType.Sphere, "Temporal_Right",
                new Vector3(0.08f, 0, 0), new Vector3(0.065f, 0.065f, 0.095f), Quaternion.identity, matTemporal);

            // E. Cerebellum
            GameObject cerebellumObj = CreateCompositeNerve(brainGroup.transform, "Nervous_Cerebellum",
                new Vector3(0, -0.06f, -0.065f), matCerebellum,
                "Cerebellum (Little Brain)", "Cerebellum",
                "Dorsal hindbrain structure maintaining motor coordination, precision rhythm, equilibrium, and procedural motor learning.",
                "Contains over 50% of the entire brain's neurons packed densely across cerebellar folia.",
                new Vector3(0, 0.2f, -0.3f),
                "Composed of two lateral cerebellar hemispheres and central vermis.",
                "Damage results in intention tremor, dysmetria, and cerebellar ataxia.");

            AddPrimitiveChild(cerebellumObj.transform, PrimitiveType.Sphere, "Cerebellum_Left",
                new Vector3(-0.045f, 0, 0), new Vector3(0.075f, 0.065f, 0.075f), Quaternion.identity, matCerebellum);
            AddPrimitiveChild(cerebellumObj.transform, PrimitiveType.Sphere, "Cerebellum_Right",
                new Vector3(0.045f, 0, 0), new Vector3(0.075f, 0.065f, 0.075f), Quaternion.identity, matCerebellum);

            // F. Brainstem (Pons & Medulla Oblongata)
            GameObject brainstemObj = CreateCompositeNerve(brainGroup.transform, "Nervous_Brainstem",
                new Vector3(0, -0.08f, -0.01f), matBrainstem,
                "Brainstem (Midbrain, Pons, Medulla)", "Truncus Encephali",
                "Vital neurological stalk connecting cerebral cortex to spinal cord; controls autonomous vegetative homeostasis.",
                "Houses reticular activating system (consciousness) and autonomic centers for respiration, heart rate, and blood pressure.",
                new Vector3(0, 0.1f, 0),
                "Gives origin to 10 of the 12 pairs of cranial nerves (III through XII).",
                "Medulla oblongata features motor decussation where corticospinal tract fibers cross.");

            AddPrimitiveChild(brainstemObj.transform, PrimitiveType.Cylinder, "Brainstem_PonsMedulla",
                Vector3.zero, new Vector3(0.035f, 0.06f, 0.035f), Quaternion.Euler(-10, 0, 0), matBrainstem);
        }
        #endregion

        #region 2. Spinal Cord System
        private static void BuildSpinalCordSystem(Transform parent, Material cordMat, Material gangliaMat)
        {
            GameObject cordGroup = new GameObject("SpinalCord_And_Roots");
            cordGroup.transform.SetParent(parent, false);

            // A. Main Spinal Cord & Enlargements
            GameObject mainCordObj = CreateCompositeNerve(cordGroup.transform, "Nervous_MainSpinalCord",
                new Vector3(0, 1.34f, -0.035f), cordMat,
                "Spinal Cord (Cervical to Lumbar)", "Medulla Spinalis",
                "Primary cylindrical conduction cable transmitting sensory afferents and motor efferents between brain and body.",
                "Features cervical enlargement (C4–T1, upper limb plexus) and lumbar enlargement (T11–L1, lower limb plexus).",
                new Vector3(0, 0, -0.45f),
                "Terminates at L1–L2 vertebral level at conus medullaris.",
                "Gives rise to 31 pairs of spinal nerves traversing intervertebral foramina.");

            // Cervical to thoracic cord
            AddPrimitiveChild(mainCordObj.transform, PrimitiveType.Cylinder, "Cord_Cervicothoracic",
                new Vector3(0, 0.12f, 0), new Vector3(0.022f, 0.20f, 0.022f), Quaternion.identity, cordMat);

            // Thoracic to lumbar cord
            AddPrimitiveChild(mainCordObj.transform, PrimitiveType.Cylinder, "Cord_Thoracolumbar",
                new Vector3(0, -0.15f, 0), new Vector3(0.020f, 0.18f, 0.020f), Quaternion.identity, cordMat);

            // Conus Medullaris tip
            AddPrimitiveChild(mainCordObj.transform, PrimitiveType.Sphere, "Cord_ConusMedullaris",
                new Vector3(0, -0.32f, 0.005f), new Vector3(0.020f, 0.035f, 0.020f), Quaternion.identity, cordMat);


            // B. Cauda Equina (Horse's Tail Nerve Roots)
            GameObject caudaObj = CreateCompositeNerve(cordGroup.transform, "Nervous_CaudaEquina",
                new Vector3(0, 0.96f, -0.032f), cordMat,
                "Cauda Equina (Lumbar & Sacral Nerve Roots)", "Cauda Equina",
                "Sheaf of lumbar, sacral, and coccygeal nerve roots descending vertically within the lumbar cistern below conus medullaris.",
                "Enclosed in dural sac with cerebrospinal fluid (CSF); prime location for clinical lumbar puncture (spinal tap at L3/L4 or L4/L5).",
                new Vector3(0, -0.1f, -0.35f),
                "Cauda equina syndrome is a surgical emergency characterized by saddle anesthesia and bladder/bowel incontinence.",
                "Supplies pelvic floor, sphincters, and all lower extremity musculature.");

            for (int i = -2; i <= 2; i++)
            {
                AddPrimitiveChild(caudaObj.transform, PrimitiveType.Cylinder, $"Cauda_Root_{i}",
                    new Vector3(i * 0.006f, 0, 0), new Vector3(0.007f, 0.10f, 0.007f), Quaternion.Euler(0, 0, i * 4), cordMat);
            }


            // C. Sympathetic Trunk (Autonomic Chain & Paravertebral Ganglia)
            GameObject sympatheticObj = CreateCompositeNerve(cordGroup.transform, "Nervous_SympatheticTrunk",
                new Vector3(0, 1.30f, -0.02f), gangliaMat,
                "Sympathetic Trunk (Autonomic Nervous System)", "Truncus Sympathicus",
                "Bilateral paired chain of paravertebral sympathetic ganglia running alongside entire vertebral column.",
                "Coordinates fight-or-flight sympathetic physiological responses: accelerates heart rate, dilates bronchioles, inhibits digestion.",
                new Vector3(0, 0, -0.35f),
                "Extends from base of skull to coccyx (uniting at ganglion impar).",
                "Transmits preganglionic and postganglionic sympathetic visceral fibers.");

            for (int i = 0; i < 8; i++)
            {
                float y = Mathf.Lerp(0.18f, -0.18f, i / 7f);
                // Left chain
                AddPrimitiveChild(sympatheticObj.transform, PrimitiveType.Sphere, $"SympGanglion_L_{i}",
                    new Vector3(-0.035f, y, 0), new Vector3(0.012f, 0.012f, 0.012f), Quaternion.identity, gangliaMat);
                // Right chain
                AddPrimitiveChild(sympatheticObj.transform, PrimitiveType.Sphere, $"SympGanglion_R_{i}",
                    new Vector3(0.035f, y, 0), new Vector3(0.012f, 0.012f, 0.012f), Quaternion.identity, gangliaMat);
            }
        }
        #endregion

        #region 3. Major Arm Nerve Pathways (Brachial Plexus)
        private static void BuildArmNervePathways(Transform parent, Material mat)
        {
            GameObject armNerveGroup = new GameObject("Nerves_UpperLimbs");
            armNerveGroup.transform.SetParent(parent, false);

            BuildArmNervesSide(armNerveGroup.transform, true, mat);
            BuildArmNervesSide(armNerveGroup.transform, false, mat);
        }

        private static void BuildArmNervesSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            // A. Brachial Plexus (Roots, Trunks, Cords)
            GameObject plexusObj = CreateCompositeNerve(parent, $"Nervous_BrachialPlexus_{sideName}",
                new Vector3(side * 0.12f, 1.45f, 0.03f), mat,
                $"{sideName} Brachial Plexus", $"Plexus Brachialis {latinSide}",
                "Network of nerve fibers formed by ventral rami of spinal nerves C5–T1 supplying upper extremity.",
                "Organized into Roots (C5-T1), Trunks (Superior, Middle, Inferior), Divisions, and Cords (Lateral, Posterior, Medial).",
                new Vector3(side * 0.25f, 0.1f, 0.2f),
                "Passes between anterior and middle scalene muscles in posterior neck triangle.",
                "Erb-Duchenne palsy involves upper trunk (C5-C6) avulsion ('waiter's tip' posture).");

            AddPrimitiveChild(plexusObj.transform, PrimitiveType.Cylinder, "Plexus_Trunk",
                Vector3.zero, new Vector3(0.018f, 0.06f, 0.018f), Quaternion.Euler(0, side * 20, side * -45), mat);


            // B. Radial Nerve (Posterior Compartment & Spiral Groove)
            GameObject radialObj = CreateCompositeNerve(parent, $"Nervous_RadialNerve_{sideName}",
                new Vector3(side * 0.23f, 1.25f, -0.02f), mat,
                $"{sideName} Radial Nerve", $"Nervus Radialis {latinSide}",
                "Largest branch of brachial plexus (posterior cord, C5–T1); supplies all extensor muscles of upper limb.",
                "Spirals around posterior shaft of humerus in radial groove; bifurcates at elbow into superficial sensory and deep posterior interosseous motor nerve.",
                new Vector3(side * 0.4f, 0, -0.15f),
                "Motor: Triceps brachii, brachioradialis, wrist and finger extensors.",
                "Injury (e.g. humeral mid-shaft fracture or 'Saturday night palsy') leads to classic 'wrist drop'.");

            // Spiral path around humerus
            AddPrimitiveChild(radialObj.transform, PrimitiveType.Cylinder, "Radial_SpiralShaft",
                new Vector3(0, 0.05f, 0), new Vector3(0.012f, 0.10f, 0.012f), Quaternion.Euler(15, side * 15, side * -8), mat);

            // Extension down posterior forearm to back of hand
            AddPrimitiveChild(radialObj.transform, PrimitiveType.Cylinder, "Radial_ForearmBranch",
                new Vector3(side * 0.02f, -0.14f, 0.01f), new Vector3(0.009f, 0.11f, 0.009f), Quaternion.Euler(0, 0, side * -4), mat);


            // C. Median Nerve (Anterior Arm & Carpal Tunnel)
            GameObject medianObj = CreateCompositeNerve(parent, $"Nervous_MedianNerve_{sideName}",
                new Vector3(side * 0.23f, 1.15f, 0.02f), mat,
                $"{sideName} Median Nerve", $"Nervus Medianus {latinSide}",
                "Major anterior peripheral nerve formed by union of lateral and medial cords (C5–T1).",
                "Descends medial to brachial artery; traverses cubital fossa and anterior forearm, passing beneath flexor retinaculum into carpal tunnel.",
                new Vector3(side * 0.4f, 0, 0.2f),
                "Motor: Forearm flexors, thenar muscles (thumb opposition), and lateral two lumbricals.",
                "Compression under flexor retinaculum causes Carpal Tunnel Syndrome (numbness in thumb, index, middle finger).");

            // Upper arm path
            AddPrimitiveChild(medianObj.transform, PrimitiveType.Cylinder, "Median_Brachial",
                new Vector3(side * -0.01f, 0.12f, 0), new Vector3(0.011f, 0.08f, 0.011f), Quaternion.identity, mat);

            // Forearm through carpal tunnel to palm
            AddPrimitiveChild(medianObj.transform, PrimitiveType.Cylinder, "Median_CarpalTunnel",
                new Vector3(side * 0.01f, -0.10f, 0), new Vector3(0.009f, 0.13f, 0.009f), Quaternion.identity, mat);


            // D. Ulnar Nerve ("Funny Bone" Nerve)
            GameObject ulnarObj = CreateCompositeNerve(parent, $"Nervous_UlnarNerve_{sideName}",
                new Vector3(side * 0.21f, 1.10f, -0.015f), mat,
                $"{sideName} Ulnar Nerve", $"Nervus Ulnaris {latinSide}",
                "Medial peripheral nerve originating from medial cord (C8–T1); 'musician's nerve' governing fine intrinsic hand coordination.",
                "Passes posterior to medial epicondyle of humerus in cubital tunnel ('funny bone'), entering hand via Guyon's canal.",
                new Vector3(side * 0.4f, 0, -0.1f),
                "Motor: Flexor carpi ulnaris, hypothenar muscles, all interossei, adductor pollicis, medial two lumbricals.",
                "Severe neuropathy results in characteristic 'claw hand' (Klumpke's deformity) deformity.");

            // Elbow groove
            AddPrimitiveChild(ulnarObj.transform, PrimitiveType.Cylinder, "Ulnar_ElbowPass",
                new Vector3(0, 0.06f, -0.01f), new Vector3(0.010f, 0.07f, 0.010f), Quaternion.Euler(-10, 0, side * 6), mat);

            // Forearm to medial hand
            AddPrimitiveChild(ulnarObj.transform, PrimitiveType.Cylinder, "Ulnar_ForearmGuyon",
                new Vector3(0, -0.12f, 0), new Vector3(0.008f, 0.12f, 0.008f), Quaternion.identity, mat);
        }
        #endregion

        #region 4. Major Leg Nerve Pathways (Lumbosacral Plexus)
        private static void BuildLegNervePathways(Transform parent, Material mat)
        {
            GameObject legNerveGroup = new GameObject("Nerves_LowerLimbs");
            legNerveGroup.transform.SetParent(parent, false);

            BuildLegNervesSide(legNerveGroup.transform, true, mat);
            BuildLegNervesSide(legNerveGroup.transform, false, mat);
        }

        private static void BuildLegNervesSide(Transform parent, bool isLeft, Material mat)
        {
            float side = isLeft ? -1f : 1f;
            string sideName = isLeft ? "Left" : "Right";
            string latinSide = isLeft ? "Sinister" : "Dexter";

            // A. Femoral Nerve (Anterior Thigh)
            GameObject femoralObj = CreateCompositeNerve(parent, $"Nervous_FemoralNerve_{sideName}",
                new Vector3(side * 0.10f, 0.78f, 0.05f), mat,
                $"{sideName} Femoral Nerve", $"Nervus Femoralis {latinSide}",
                "Largest branch of lumbar plexus (L2–L4); primary innervation of anterior thigh.",
                "Enters thigh deep to inguinal ligament lateral to femoral artery in femoral triangle; branches into muscular rami and saphenous sensory nerve.",
                new Vector3(side * 0.3f, 0, 0.3f),
                "Motor: Quadriceps femoris (knee extension), sartorius, pectineus.",
                "Sensory: Anterior thigh and medial lower leg/foot (saphenous nerve).",
                "Mediates patellar tendon knee-jerk reflex (L3-L4).");

            AddPrimitiveChild(femoralObj.transform, PrimitiveType.Cylinder, "Femoral_Trunk",
                new Vector3(0, 0.06f, 0), new Vector3(0.014f, 0.08f, 0.014f), Quaternion.identity, mat);

            AddPrimitiveChild(femoralObj.transform, PrimitiveType.Cylinder, "Femoral_Branches",
                new Vector3(side * 0.01f, -0.06f, 0.01f), new Vector3(0.010f, 0.11f, 0.010f), Quaternion.identity, mat);


            // B. Sciatic Nerve (Posterior Thigh - Largest Nerve in Human Body)
            GameObject sciaticObj = CreateCompositeNerve(parent, $"Nervous_SciaticNerve_{sideName}",
                new Vector3(side * 0.11f, 0.72f, -0.045f), mat,
                $"{sideName} Sciatic Nerve", $"Nervus Ischiadicus {latinSide}",
                "Largest and thickest nerve in the human body (~2 cm wide); derived from sacral plexus (L4–S3).",
                "Exits pelvis through greater sciatic foramen inferior to piriformis muscle; descends down posterior thigh before bifurcating at popliteal fossa.",
                new Vector3(side * 0.35f, 0, -0.3f),
                "Bifurcates into Tibial nerve and Common Fibular (Peroneal) nerve near knee.",
                "Motor: Hamstring muscles and all muscles of the lower leg and foot.",
                "Sciatica causes radiating shooting pain, numbness, and tingling along posterior leg.");

            // Thick main sciatic trunk
            AddPrimitiveChild(sciatObj(sciaticObj).transform, PrimitiveType.Cylinder, "Sciatic_MainTrunk",
                Vector3.zero, new Vector3(0.020f, 0.22f, 0.020f), Quaternion.identity, mat);


            // C. Tibial Nerve (Posterior Calf to Sole of Foot)
            GameObject tibialObj = CreateCompositeNerve(parent, $"Nervous_TibialNerve_{sideName}",
                new Vector3(side * 0.10f, 0.28f, -0.035f), mat,
                $"{sideName} Tibial Nerve", $"Nervus Tibialis {latinSide}",
                "Larger terminal branch of sciatic nerve (L4–S3); courses through popliteal fossa down posterior leg.",
                "Passes posterior to medial malleolus through tarsal tunnel to divide into medial and lateral plantar nerves in foot.",
                new Vector3(side * 0.35f, -0.1f, -0.2f),
                "Motor: Gastrocnemius, soleus, tibialis posterior, flexor digitorum longus, and intrinsic foot sole muscles.",
                "Tarsal tunnel syndrome causes burning sole and heel pain.");

            AddPrimitiveChild(tibialObj.transform, PrimitiveType.Cylinder, "Tibial_CalfPath",
                Vector3.zero, new Vector3(0.013f, 0.22f, 0.013f), Quaternion.identity, mat);

            AddPrimitiveChild(tibialObj.transform, PrimitiveType.Cylinder, "Tibial_PlantarFoot",
                new Vector3(0, -0.18f, 0.04f), new Vector3(0.009f, 0.08f, 0.009f), Quaternion.Euler(30, 0, 0), mat);


            // D. Common Fibular (Peroneal) Nerve (Lateral Knee & Foot Dorsum)
            GameObject fibularObj = CreateCompositeNerve(parent, $"Nervous_FibularNerve_{sideName}",
                new Vector3(side * 0.13f, 0.32f, 0.005f), mat,
                $"{sideName} Common Fibular (Peroneal) Nerve", $"Nervus Fibularis Communis {latinSide}",
                "Lateral branch of sciatic nerve (L4–S2) curving superficially around the lateral neck of fibula.",
                "Bifurcates into superficial fibular (fibularis longus/brevis) and deep fibular nerve (anterior compartment muscles).",
                new Vector3(side * 0.45f, 0, 0.1f),
                "Motor: Tibialis anterior (foot dorsiflexion), extensor digitorum longus, extensor hallucis.",
                "Compression or trauma at neck of fibula results in 'Foot Drop' (loss of dorsiflexion and high-stepping gait).");

            AddPrimitiveChild(fibularObj.transform, PrimitiveType.Cylinder, "Fibular_NeckCurve",
                new Vector3(0, 0.10f, 0), new Vector3(0.010f, 0.08f, 0.010f), Quaternion.Euler(10, 0, side * 15), mat);

            AddPrimitiveChild(fibularObj.transform, PrimitiveType.Cylinder, "Fibular_AnteriorLeg",
                new Vector3(side * -0.01f, -0.06f, 0.02f), new Vector3(0.008f, 0.16f, 0.008f), Quaternion.identity, mat);
        }

        private static GameObject sciatObj(GameObject g) => g;
        #endregion

        #region Helpers
        private static GameObject CreateCompositeNerve(
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
                systemType = AnatomySystemType.Nervous,
                keyFunction = function,
                description = description,
                clinicalFacts = clinicalFacts
            };
            part.baseColor = mat.color;
            part.hoverColor = Color.white;
            part.selectedColor = new Color(0.2f, 0.95f, 1f); // Electric Cyan highlight for nerves
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

        private static Material CreateNerveMaterial(Shader shader, string name, Color color)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.60f);
            mat.SetFloat("_Cull", 0); // Double-sided rendering for 100% visibility
            return mat;
        }
        #endregion
    }
}

