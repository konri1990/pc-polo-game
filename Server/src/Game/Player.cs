namespace Server.Game
{
    public class Player
    {
        public string ClientId { get; }
        public int PlayerScore { get; private set; }
        public Player(string clientId) =>
            (ClientId) = (clientId);

        public void SetPlayerScore(int number){
            if(number > 0) {
                PlayerScore = number;
            }
        }
    }
}