using System;
using System.Collections.Generic;

namespace Server.Game
{
    public interface IPoloGameEngine
    {
        AddPlayerOperationResult AddPlayer(string player);
        bool IsGameReadyToStart { get; }
        void SetPlayerNumber(string player, int playerValue);
    }
    public class PoloGameEngine : IPoloGameEngine
    {
        private readonly List<string> _players;
        public const int MaxAvailableClients = 2;

        public PoloGameEngine()
        {
            _players = new List<string>(); //todo change to concurent dictionary?
        }

        public AddPlayerOperationResult AddPlayer(string Player)
        {
            if(_players.Count >= MaxAvailableClients)
                return AddPlayerOperationResult.NotAllowedForMorePlayers;

            _players.Add(Player);
            return AddPlayerOperationResult.PlayerAdded;
        }

        public void SetPlayerNumber(string player, int playerValue)
        {
            if(_players.Contains(player))
                return;
            throw new Exception("Client not found."); //probably client disconnected
        }

        public bool IsGameReadyToStart => _players?.Count == MaxAvailableClients;
    }
}