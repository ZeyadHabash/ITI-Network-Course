using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Mirror_RPS
{
    public enum RPSMove
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 3,
    }

    public class NetworkPlayer : NetworkBehaviour
    {
        // SyncVar => value changes server-side are replicated for all clients
        // hook => method for *clients* to call when this SyncVar changes 
        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        private string playerName;

        [SyncVar(hook = nameof(OnPlayerMoveChanged))]
        private RPSMove playerMove;

        [SyncVar(hook = nameof(OnIsWaitingChanged))]
        private bool isWaiting;

        // Properties
        public RPSMove PlayerMove => playerMove;


        private void Start()
        {
            GameNetworkManager.Instance.AddPlayer(this);
        }

        private void OnDestroy()
        {
            GameNetworkManager.Instance.RemovePlayer(netId);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            // Debug.Log(isLocalPlayer
            //     ? $"Player {GameNetworkManager.Instance.LocalPlayerName} is connected with NetID {netId}, ConnectionID {netIdentity.connectionToClient.connectionId}"
            //     : $"Other Player Connected with NetID {netId}, ConnectionID {netIdentity.connectionToClient.connectionId}");
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            // Call Server RPC to set playername
            CmdUpdatePlayerName(GameNetworkManager.Instance.LocalPlayerName);
        }

        void OnPlayerNameChanged(string oldName, string newName)
        {
            playerName = newName;
            // update UI
            GameplayUIManager.Instance.UpdateName(newName, netIdentity.isLocalPlayer);
        }

        void OnPlayerMoveChanged(RPSMove oldMove, RPSMove newMove)
        {
            playerMove = newMove;
            Debug.Log($"{playerName}'s move: {playerMove}");
        }

        void OnIsWaitingChanged(bool oldIsWaiting, bool newIsWaiting)
        {
            isWaiting = newIsWaiting;
        }

        [Command]
        void CmdUpdatePlayerName(string newName)
        {
            playerName = newName;
        }

        [Command]
        public void CmdUpdatePlayerMove(int newMove)
        {
            playerMove = (RPSMove)newMove;
            GameNetworkManager.Instance.CalculateResult();
        }

        [TargetRpc]
        public void TargetUpdateResult(RPSResult result, RPSMove p1Move, RPSMove p2Move)
        {
            GameplayUIManager.Instance.DisplayResult(p1Move, p2Move, result);
            Invoke(nameof(CmdResetGame), 3);
        }

        [Command]
        public void CmdResetGame()
        {
            // set these here bec syncvars need to be set on the server 
            isWaiting = true; 
            playerMove = RPSMove.None;
            
            List<NetworkPlayer> allPlayers = GameNetworkManager.Instance.GetAllPlayers();
            bool allPlayersReady = allPlayers.TrueForAll(x => x.isWaiting);
            if (allPlayersReady)
            {
                ResetGame();
            }
        }

        [ClientRpc]
        public void ResetGame()
        {
            GameplayUIManager.Instance.ResetGameUI();
            isWaiting = false;
        }
    }
}