using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    private bool playerWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerWon = true;
            Debug.Log("YOU WIN!");
        }
    }

    private void Update()
    {
        if (playerWon && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    private void OnGUI()
    {
        if (playerWon)
        {
            // Black background
            GUI.color = Color.black;

            GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                Texture2D.whiteTexture
            );

            GUI.color = Color.white;

            // YOU WIN text
            GUIStyle winStyle = new GUIStyle();

            winStyle.fontSize = 65;
            winStyle.fontStyle = FontStyle.Bold;
            winStyle.alignment = TextAnchor.MiddleCenter;
            winStyle.normal.textColor = Color.yellow;

            GUI.Label(
                new Rect(0, 0, Screen.width, Screen.height),
                "YOU WIN!",
                winStyle
            );

            // Restart instructions
            GUIStyle restartStyle = new GUIStyle();

            restartStyle.fontSize = 25;
            restartStyle.alignment = TextAnchor.MiddleCenter;
            restartStyle.normal.textColor = Color.white;

            GUI.Label(
                new Rect(0, 70, Screen.width, Screen.height),
                "PRESS ENTER TO TRY AGAIN",
                restartStyle
            );
        }
    }
}

