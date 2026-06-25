using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace Mirror_RPS
{
    public enum RPSResult
    {
        Draw = 0,
        Win = 1,
        Lose = 2
    }

    public class GameNetworkManager : NetworkManager
    {
        #region Properties

        // Singleton
        public static GameNetworkManager Instance { get; private set; }

        // Network Bools
        public bool IsClient { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsHost => IsClient && IsServer;

        // Other
        public string LocalPlayerName { get; set; }

        public Dictionary<uint, NetworkPlayer> networkPlayers = new Dictionary<uint, NetworkPlayer>();

        public NetworkPlayer LocalPlayer => networkPlayers?.First(x => x.Value.isLocalPlayer).Value;
        public NetworkPlayer OpponentPlayer => networkPlayers?.First(x => !x.Value.isLocalPlayer).Value;

        #endregion

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        #region Connection

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        #endregion

        #region Players

        public void AddPlayer(NetworkPlayer player)
        {
            if (networkPlayers.Count >= 2)
            {
                Debug.LogWarning("Can't add more than 2 players");
                return;
            }

            networkPlayers.TryAdd(player.netId, player);
        }

        public void RemovePlayer(uint playerId)
        {
            networkPlayers.Remove(playerId);
        }

        public List<NetworkPlayer> GetAllPlayers()
        {
            return networkPlayers.Values.ToList();
        }

        #endregion

        [Server]
        public void CalculateResult()
        {
            if (networkPlayers.Count < 2)
                return;

            RPSMove p1Move = networkPlayers.ElementAt(0).Value.PlayerMove;
            RPSMove p2Move = networkPlayers.ElementAt(1).Value.PlayerMove;

            if (p1Move == RPSMove.None || p2Move == RPSMove.None)
                return;

            RPSResult p1Result;
            RPSResult p2Result;

            bool p1Wins = false;

            if (p1Move == p2Move)
                p1Result = p2Result = RPSResult.Draw;
            else
            {
                switch (p1Move)
                {
                    case RPSMove.Rock:
                        p1Wins = p2Move == RPSMove.Scissors;
                        break;
                    case RPSMove.Scissors:
                        p1Wins = p2Move == RPSMove.Rock;
                        break;
                    case RPSMove.Paper:
                        p1Wins = p2Move == RPSMove.Rock;
                        break;
                }

                if (p1Wins)
                {
                    p1Result = RPSResult.Win;
                    p2Result = RPSResult.Lose;
                }
                else
                {
                    p1Result = RPSResult.Lose;
                    p2Result = RPSResult.Win;
                }
            }

            networkPlayers.ElementAt(0).Value.TargetUpdateResult(p1Result, p1Move, p2Move);
            networkPlayers.ElementAt(1).Value.TargetUpdateResult(p2Result, p2Move, p1Move);
            
        }
    }
}