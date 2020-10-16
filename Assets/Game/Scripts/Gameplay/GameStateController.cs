using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public GameObject playerPrefab;
    
    public Transform playerSpawn;
    public PlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        GameStateController[] controllers = FindObjectsOfType<GameStateController>();
        foreach (GameStateController controller in controllers) {
            if (controller != this) {
                DestroyImmediate(controller);
            }
        }

        player = FindObjectOfType<PlayerController>();
        if (!player) {
            GameObject spawnedPlayer = Instantiate(playerPrefab);
            player.transform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);
            player = spawnedPlayer.GetComponent<PlayerController>();
        }

        DontDestroyOnLoad(player.gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
