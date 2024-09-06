using System.Linq;
using businesslogic;
using TMPro;
using UnityEngine;

namespace UI {
    public class InfoUI : MonoBehaviour {
        [SerializeField] private GameObject containerInfo;
        [SerializeField] private Transform contentFather;

        private void OnEnable() {
            contentFather.gameObject.SetActive(true);
            
            foreach (Transform child in contentFather) {
                Destroy(child.gameObject);
            }

            foreach (var playerId in GameManager.Instance.GetPlayersId()) {
                GameObject newPlayer = Instantiate(contentFather.gameObject, containerInfo.transform);
                newPlayer.transform.Find("playerName").GetComponent<TMP_Text>().color = Utils.ColorCode(GameManager.Instance.GetPlayerColor(playerId), 255);
                newPlayer.transform.Find("playerName").GetComponent<TMP_Text>().text = GameManager.Instance.getEnemyNameById(playerId);

                newPlayer.transform.Find("playerInfo").GetComponent<TMP_Text>().text = "Armies Number: " + GameManager.Instance.AllTerritories
                                      .Where(territory => territory.player_id.Equals(playerId))
                                      .Sum(territory => territory.num_tanks) + "\n"
                                  + "Territories Number: " + GameManager.Instance.AllTerritories
                                      .Count(territory => territory.player_id.Equals(playerId));
            }
            
            contentFather.gameObject.SetActive(false);
        }
    }
}