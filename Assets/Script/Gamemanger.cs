namespace Script
{
    public class Gamemanger
    {
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
    }
}