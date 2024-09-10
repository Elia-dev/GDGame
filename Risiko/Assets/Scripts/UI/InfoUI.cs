using System.Linq;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class InfoUI : MonoBehaviour {
        [SerializeField] private GameObject containerInfo;
        [SerializeField] private Transform contentFather;

        private void OnEnable() {
            foreach (Transform child in containerInfo.transform) {
                Destroy(child.gameObject);
            }
            contentFather.gameObject.SetActive(true);
            foreach (var playerId in GameManager.Instance.GetPlayersId()) {
                GameObject newPlayer = Instantiate(contentFather.gameObject, containerInfo.transform);
                newPlayer.transform.SetParent(containerInfo.transform);
                
                // Attiva i componenti necessari
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().enabled = true;
                newPlayer.transform.Find("PlayerInfo").GetComponent<TMP_Text>().enabled = true;
                newPlayer.transform.Find("PlayerInfo").GetComponent<ContentSizeFitter>().enabled = true;
                
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(playerId), 255);
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().text = GameManager.Instance.getEnemyNameById(playerId);
                
                if(playerId.Equals(Player.Instance.PlayerId))
                    newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().text += " (You)";

                newPlayer.transform.Find("PlayerInfo").GetComponent<TMP_Text>().text = "Armies Number: " + GameManager.Instance.AllTerritories
                                      .Where(territory => territory.player_id.Equals(playerId))
                                      .Sum(territory => territory.num_tanks) + "\n\n"
                                  + "Territories Number: " + GameManager.Instance.AllTerritories
                                      .Count(territory => territory.player_id.Equals(playerId));
            }
            contentFather.gameObject.SetActive(false);
        }
    }
}