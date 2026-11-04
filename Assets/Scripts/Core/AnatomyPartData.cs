using System;
using UnityEngine;

namespace HumanAnatomy
{
    [Serializable]
    public class AnatomyPartData
    {
        public string partName = "Organ";
        public string scientificName = "Organum";
        public AnatomySystemType systemType = AnatomySystemType.Skeletal;
        public string keyFunction = "Main anatomical function";
        [TextArea(3, 6)]
        public string description = "Detailed description of this anatomical structure.";
        public string[] clinicalFacts = new string[0];
    }
}

