namespace Server.Game
{
    public enum AddPlayerOperationResult {
        PlayerAdded,
        CannotAddMorePlayers,
        ClientAlreadyExists,
        CannotAddPlayerWhichClientIdIsNotSet
    }

    public enum GameResulsForPlayer {
        GameNotStarted,
        Won,
        Lost,
        ClientDisconected
    }
}