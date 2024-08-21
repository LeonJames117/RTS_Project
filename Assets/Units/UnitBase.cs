using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility_Scripts;

namespace Units
{
    public class UnitBase : MonoBehaviour
    {
        // Start is called before the first frame update
        public bool isStructure;
        public GameObject selectionGraphic;
        public List<Vector3> orderQueue = new List<Vector3>();
        public Vector3 target;
        public bool selectable;
        public SharedTypes.UnitType unitType;
        public UnitManager uMan;
        void Start()
        {
            uMan.allUnits.Add(this);
            selectionGraphic.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
