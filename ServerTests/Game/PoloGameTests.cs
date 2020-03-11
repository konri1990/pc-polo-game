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
            for(int i = 0; i<PoloGameEngine.MaxOnlineClients; i++) {
                _gameEngine.AddPlayer($"Player{i}");
            }

            _gameEngine.AddPlayer("NextPlayer").Should().Be(AddPlayerOperationResult.CannotAddMorePlayers);
        }

        [Fact]
        public void Given_Game_When_AllPlayersAreOnline_Then_GameIsReadyToStart()
        {
            for(int i = 0; i<PoloGameEngine.MaxOnlineClients; i++) {
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
        public void Given_Game_When_OnePlayerSendTheGreatestValue_Then_GetWinnerAndLostPlayer()
        {
            var playerOne = "Player1";
            var playerTwo = "Player2";
            _gameEngine.AddPlayer(playerOne);
            _gameEngine.AddPlayer(playerTwo);

            _gameEngine.SetPlayerNumber(playerOne, 2);
            _gameEngine.SetPlayerNumber(playerTwo, 3);

            _gameEngine.GameResultForPlayer(playerOne).Should().Be(GameResulsForPlayer.Lost);
            _gameEngine.GameResultForPlayer(playerTwo).Should().Be(GameResulsForPlayer.Won);
        }

        [Fact]
        public void Given_Game_When_PlayerIsNotOnline_Then_ClientDisconnected()
        {
            var playerOne = "Player1";
            var playerTwo = "Player2";
            _gameEngine.AddPlayer(playerOne);

            _gameEngine.SetPlayerNumber(playerOne, 2);

            _gameEngine.GameResultForPlayer(playerOne).Should().Be(GameResulsForPlayer.GameNotStarted);
            _gameEngine.GameResultForPlayer(playerTwo).Should().Be(GameResulsForPlayer.ClientDisconected);
        }
    }
}
