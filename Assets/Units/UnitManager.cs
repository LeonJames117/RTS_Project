using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Utility_Scripts;
namespace Units
{
    public class UnitManager : MonoBehaviour
    {
        readonly Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
        PathRequest _currentRequest;
        static UnitManager _instance;
        PathfindingManager _pathfinding;
        [FormerlySerializedAs("Following_Path")] public bool followingPath;
        [FormerlySerializedAs("All_Units")] public List<UnitBase> allUnits = new List<UnitBase>();
        [FormerlySerializedAs("Selected_Units")] public List<UnitBase> selectedUnits = new List<UnitBase>();
        
        
        bool _currentlyProcessingPath;
        private void Awake()
        {
            _instance = this;
            _pathfinding=GetComponent<PathfindingManager>();
        }
    
        public static void RequestPath(Vector3 start, Vector3 target, SharedTypes.UnitType unitType, Action<Vector3[], bool> callback)
        {
            
            PathRequest newRequest = new PathRequest(start,target,unitType,callback);
            _instance._requestQueue.Enqueue(newRequest);
            _instance.Process_Next();
        }

        void Process_Next()
        {
            if(!_currentlyProcessingPath && _requestQueue.Count > 0)
            {
                _currentRequest = _requestQueue.Dequeue();// Also removes item from queue
                _currentlyProcessingPath = true;
                _pathfinding.Start_Pathfinding(_currentRequest.Start, _currentRequest.Target,_currentRequest.UnitType);
            }
        }

        public void Finished_Path_Processing(Vector3[] path, bool success)
        {
            _currentRequest.Callback(path,success);
            print("Path processing finished");
            _currentlyProcessingPath=false;
            Process_Next();
        }

        void Update()
        {
            if (!_currentlyProcessingPath && _requestQueue.Count > 0)
            {
                Process_Next();
            }
            //print("Currently " + _requestQueue.Count + " requests queued");
        }

        public struct PathRequest
        {
            public readonly Vector3 Start;
            public readonly Vector3 Target;
            public readonly Action<Vector3[],bool> Callback;
            public readonly SharedTypes.UnitType UnitType;
            public PathRequest(Vector3 start,Vector3 end, SharedTypes.UnitType unitType, Action<Vector3[],bool> callback)
            {
                Start = start;
                Target = end;
                Callback = callback;
                UnitType = unitType;
            }
        }
    }
}
