using Xunit;
using FluentAssertions;
using Server.Game;

namespace ServerTests.Runner
{
    public class PoloGameTests
    {
        private readonly IPoloGameEngine _gameEngine;
        public PoloGameTests()
        {
            _gameEngine = new PoloGameEngine();
        }

        [Fact]
        public void Given_InGameAlreadyMaxPlayers_When_AddNextClient_Then_ReturnStatusNotAllowedForMorePlayers()
        {
            for(int i = 0; i<PoloGameEngine.MaxAvailableClients; i++) {
                _gameEngine.AddPlayer($"Player{i}");
            }

            _gameEngine.AddPlayer("NextPlayer").Should().Be(AddPlayerOperationResult.NotAllowedForMorePlayers);
        }

        [Fact]
        public void Given_Game_When_AllPlayersAreOnline_Then_GameIsReadyToStart()
        {
            for(int i = 0; i<PoloGameEngine.MaxAvailableClients; i++) {
                _gameEngine.AddPlayer($"Player{i}");
            }

            _gameEngine.IsGameReadyToStart.Should().Be(true);
        }

        [Fact]
        public void Given_Game_When_NotEnoughPlayers_Then_GameCannotStart()
        {
            _gameEngine.AddPlayer("Player");

            _gameEngine.IsGameReadyToStart.Should().Be(false);
        }

        [Fact]
        public void Given_Game_When_OnePlayerSendTheGreatestValue_Then_GetWinnerPlayer()
        {
            _gameEngine.AddPlayer("Player1");
            _gameEngine.AddPlayer("Player2");

            _gameEngine.SetPlayerNumber("Player1", 2);
            _gameEngine.SetPlayerNumber("Player2", 3);

            // _gameEngine.GetWinner().Should().Be("Player1");
            // _gameEngine.GetWinner().Should().Be("Player1");
        }
    }
}
