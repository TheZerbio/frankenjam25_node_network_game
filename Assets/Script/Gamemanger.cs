namespace Script
{
    public class GameManger
    {
        #region IDs
        private static int _nextNodeId = 0;
        private static int _nextEdgeId = 0;
        
        public static int GetNextNodeId()
        {
            return _nextNodeId++;
        }
        
        public static int GetNextEdgeId()
        {
            return _nextEdgeId++;
        }
        #endregion
        
        private static GameManger _instance;
        public Graph.Graph playerOneGraph;
        public Graph.Graph playerTwoGraph;
        
        public static GameManger GetInstance()
        {
            return _instance ??= new GameManger();
        }

        private GameManger()
        {
            playerOneGraph = new Graph.Graph();
            playerTwoGraph = new Graph.Graph();
        }
        
    }
}