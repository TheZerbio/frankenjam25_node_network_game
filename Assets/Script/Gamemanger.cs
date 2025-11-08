using UnityEngine;

namespace Script
{
    public class GameManger : MonoBehaviour
    {
        #region IDs
        private static int _nextNodeId = 0;
        private static int _nextEdgeId = 0;
        private static int _nextWorkerId = 0;
        
        public static int GetNextNodeId()
        {
            return _nextNodeId++;
        }
        
        public static int GetNextEdgeId()
        {
            return _nextEdgeId++;
        }
        
        public static int GetNextWorkerId()
        {
            return _nextEdgeId++;
        }
        #endregion
        
        private static GameManger _instance;
        public Graph.Graph PlayerOneGraph;
        public Graph.Graph PlayerTwoGraph;
        
        public static GameManger GetInstance()
        {
            return _instance ??= new GameManger();
        }

        private GameManger()
        {
            PlayerOneGraph = new Graph.Graph();
            PlayerTwoGraph = new Graph.Graph();
        }
        
    }
}