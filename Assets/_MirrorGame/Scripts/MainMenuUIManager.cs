using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror_RPS
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField if_playerName;
        [SerializeField] private Button btn_startHost;
        [SerializeField] private Button btn_startClient;

        private void Start()
        {
            ValidateName();
        }

        public void ValidateName()
        {
            if (string.IsNullOrEmpty(if_playerName.text))
            {
                btn_startHost.interactable = false;
                btn_startClient.interactable = false;
            }
            else
            {
                btn_startHost.interactable = true;
                btn_startClient.interactable = true;
                GameNetworkManager.Instance.LocalPlayerName = if_playerName.text;
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}