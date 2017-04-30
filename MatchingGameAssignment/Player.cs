namespace MatchingGameAssignment
{
    class Player
    {
        int score;
        int attempts;

        public int Score
        {
            get { return score; }
        }

        public int Attempts
        {
            get { return attempts; }
        }

        public void IncrementStats(bool shouldIncrementScore, int? amount)
        {
            attempts++;
            if (shouldIncrementScore)
            {
                score += amount.GetValueOrDefault(1);
            }
        }

    }
}
