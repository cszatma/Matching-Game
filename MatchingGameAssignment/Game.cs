namespace MatchingGameAssignment
{
    public enum GameModes { Singleplayer, Multiplayer };
    public enum GameDifficulty { Easy, Hard };

    class Game : object
    {
        GameModes gameMode;
        GameDifficulty difficulty;
        Player player1;
        Player player2;
        
        int multiplier = 50;
        Player currentPlayer; //Determines who's turn it is in 2 player mode
        int cardsMatched = 0;

        GameTime time;

        public GameModes GameMode
        {
            get { return gameMode; }
        }

        public GameDifficulty Difficulty
        {
            get { return difficulty; }
        }

        public Player Player1
        {
            get { return player1; }
        }

        public Player Player2
        {
            get { return player2; }
        }

        public int Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public Player CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        public GameTime Time
        {
            get { return time; }
        }

        public int CardsMatched
        {
            get { return cardsMatched; }
            set { cardsMatched = value; }
        }


        public Game(GameModes gameMode, GameDifficulty difficulty)
        {
            this.gameMode = gameMode;
            this.difficulty = difficulty;
            time = new GameTime();
            player1 = new Player();
            player2 = gameMode == GameModes.Multiplayer ? new Player() : null;
            currentPlayer = player1;
        }

        public void IncrementStats(bool shouldIncrementScore)
        {
            if (GameMode == GameModes.Singleplayer)
            {
                CurrentPlayer.IncrementStats(shouldIncrementScore, Multiplier);
                Multiplier = Multiplier > 1 ? Multiplier-- : 1;
                return;
            }
            CurrentPlayer.IncrementStats(shouldIncrementScore, null);
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }
    }

}
