using System.Linq;
using businesslogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class InfoUI : MonoBehaviour {
        [SerializeField] private GameObject infoPlayer;
        [SerializeField] private Transform contentFather;

        private void OnEnable() {
            foreach (Transform child in contentFather) {
                Destroy(child.gameObject);
            }
            infoPlayer.gameObject.SetActive(true);
            foreach (var playerId in GameManager.Instance.GetPlayersId()) {
                GameObject newPlayer = Instantiate(infoPlayer, contentFather);
                newPlayer.transform.SetParent(contentFather);
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().enabled = true;
                newPlayer.transform.Find("PlayerInfo").GetComponent<TMP_Text>().enabled = true;
                newPlayer.transform.Find("PlayerInfo").GetComponent<ContentSizeFitter>().enabled = true;
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(playerId), 255);
                newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().text = GameManager.Instance.GetEnemyNameById(playerId);
                if(playerId.Equals(Player.Instance.PlayerId))
                    newPlayer.transform.Find("PlayerName").GetComponent<TMP_Text>().text += " (You)";

                newPlayer.transform.Find("PlayerInfo").GetComponent<TMP_Text>().text = "Armies Number: " + GameManager.Instance.AllTerritories
                                      .Where(territory => territory.player_id.Equals(playerId))
                                      .Sum(territory => territory.num_tanks) + "\n\n"
                                  + "Territories Number: " + GameManager.Instance.AllTerritories
                                      .Count(territory => territory.player_id.Equals(playerId));
            }
            infoPlayer.gameObject.SetActive(false);
        }
    }
}