using System;
using TMPro;
using UnityEngine;

namespace Mirror_RPS
{
    public class GameplayUIManager : MonoBehaviour
    {
        public static GameplayUIManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI txt_playerName;
        [SerializeField] private TextMeshProUGUI txt_opponentName;

        [SerializeField] private CanvasGroup cnvsgrp_actions;

        [SerializeField] private TextMeshProUGUI txt_playerMove;
        [SerializeField] private TextMeshProUGUI txt_opponentMove;
        [SerializeField] private TextMeshProUGUI txt_result;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cnvsgrp_actions.interactable = !GameNetworkManager.Instance.IsServer;
        }

        public void UpdateName(string playerName, bool isMe)
        {
            if (isMe)
            {
                txt_playerName.text = playerName;
            }
            else
            {
                txt_opponentName.text = playerName;
            }
        }

        public void OnButtonClick(int move)
        {
            GameNetworkManager.Instance.LocalPlayer.CmdUpdatePlayerMove(move);
            cnvsgrp_actions.interactable = false;
        }

        public void DisplayResult(RPSMove playerMove, RPSMove opponentMove, RPSResult result)
        {
            txt_playerMove.text = playerMove.ToString();
            txt_opponentMove.text = opponentMove.ToString();
            txt_result.text = result.ToString();
            
        }

        public void ResetGameUI()
        {
            txt_playerMove.text = string.Empty;
            txt_opponentMove.text = string.Empty;
            txt_result.text = string.Empty;
            
            cnvsgrp_actions.interactable = true;
        }
    }
}