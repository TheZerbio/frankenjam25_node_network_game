namespace Script
{
    public class GameManger
    {
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