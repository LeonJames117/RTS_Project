using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units
{
    public class UnitBase : MonoBehaviour
    {
        // Start is called before the first frame update
        public bool isStructure;
        public GameObject selectionGraphic;
        public List<Vector3> orderQueue = new List<Vector3>();
        public Vector3 target;
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
