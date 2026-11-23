using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnemyHitPlayer : MonoBehaviour
{
    public GameObject losePanel;

    private bool gameOver = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver)
        {
            if (Keyboard.current != null &&
                (Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            {
                RestartGame();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayer(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayer(other.gameObject);
    }

    private void CheckPlayer(GameObject other)
    {
        if (gameOver)
            return;

        if (other.CompareTag("Player"))
        {
            gameOver = true;

            Debug.Log("YOU LOSE!");

            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}


