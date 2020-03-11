using System.Collections.Generic;
using System.Linq;

namespace Server.Game
{
    public interface IPoloGameEngine
    {
        AddPlayerOperationResult AddPlayer(string playerId);
        bool IsGameReadyToStart { get; }
        bool SetPlayerNumber(string playerId, int playerValue);
        GameResulsForPlayer GameResultForPlayer(string playerId);
    }

    public class PoloGameEngine : IPoloGameEngine
    {
        private readonly Dictionary<string, Player> _players;
        public const int MaxOnlineClients = 2;
        private int winningValue = 0;

        public PoloGameEngine()
        {
            _players = new Dictionary<string, Player>(); //todo change to concurent dictionary?
        }

        public AddPlayerOperationResult AddPlayer(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return AddPlayerOperationResult.CannotAddPlayerWhichClientIdIsNotSet;

            if (_players.ContainsKey(playerId))
                return AddPlayerOperationResult.ClientAlreadyExists;

            if (_players.Count >= MaxOnlineClients)
                return AddPlayerOperationResult.CannotAddMorePlayers;

            _players.Add(playerId, new Player(playerId));
            return AddPlayerOperationResult.PlayerAdded;
        }

        public bool SetPlayerNumber(string playerId, int playerValue)
        {
            if (_players.ContainsKey(playerId))
            {
                _players[playerId].SetPlayerScore(playerValue);
                winningValue = _players.Values.Max(player => player.PlayerScore);
                return true;
            }

            return false;
        }

        public GameResulsForPlayer GameResultForPlayer(string playerId)
        {
            if (!_players.ContainsKey(playerId))
                return GameResulsForPlayer.ClientDisconected;
            if (!IsGameReadyToStart)
                return GameResulsForPlayer.GameNotStarted;

            var playerScore = _players[playerId].PlayerScore;

            if (playerScore == winningValue)
                return GameResulsForPlayer.Won;
            else
                return GameResulsForPlayer.Lost;
        }

        public bool IsGameReadyToStart => _players?.Count == MaxOnlineClients;
    }
}