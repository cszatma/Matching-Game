namespace MatchingGameAssignment
{
    class GameTime
    {
        int minutes;
        int seconds;

        public int Minutes
        {
            get { return minutes; }
        }

        public int Seconds
        {
            get { return seconds; }
        }

        public GameTime()
        {
            minutes = 0;
            seconds = 0;
        }

        public void IncrementTime()
        {
            if (seconds == 59)
            {
                minutes++;
                seconds = 0;
                return;
            }
            seconds++;
        }

        public override string ToString()
        {
            return minutes.ToString() + ":" + seconds.ToString("D2");
        }
    }
}
