using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathMenu : MonoBehaviour
{
    private GameObject player;
    public TMP_Text scoreText;
    public GameObject deathCanvas;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        player.TryGetComponent<Player>(out Player playerScript);

        if (playerScript != null)
            scoreText.text = playerScript.score.ToString();

        if (playerScript.currentHealth <= 0)
            StartCoroutine(DeathDisplay());
    }

    IEnumerator DeathDisplay()
    {
        yield return new WaitForSeconds(5.25f);
        deathCanvas.SetActive(true);
    }

    public void LoadGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
