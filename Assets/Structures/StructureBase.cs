using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Structures
{
    public class StructureBase : UnitBase
    {
        //public GameObject placementPoint;

        public MeshRenderer model;
        // Start is called before the first frame update
        int Health;

        void Start()
        {
            isStructure = true;
        }
        
    }
}
