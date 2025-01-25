using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;

namespace Com.Nongmo.SeohaMakesGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            GameObject.Find("Leave button").GetComponent<Button>().onClick.AddListener(LeaveRoom);
        }

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
    }
}