using UnityEngine;

public class Banana : MonoBehaviour
{
    public int score = 100;
    public float decayTime = 20f;
    public float maxDecayTime = 20f;

    private Player player;
    private BananaSpawner bananaSpawner;
    private SpriteRenderer sr;

    public AudioClip bananaSound;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        bananaSpawner = GameObject.Find("BananaSpawner").GetComponent<BananaSpawner>();
        sr = GetComponent<SpriteRenderer>();
        maxDecayTime = decayTime;

        audioSource = GameObject.Find("SFXAudioSource").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (decayTime > 0f)
            decayTime -= Time.deltaTime;
        else
            Destroy(gameObject);

        //sr.color = new Color(sr.color.r, sr.color.r, sr.color.r, decayTime / maxDecayTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.AddScore(score);
            audioSource.PlayOneShot(bananaSound);
            bananaSpawner.Spawn();
            bananaSpawner.spawnTimer = maxDecayTime / 10f;

            if (player.currentHealth < player.maxHealth)
                player.currentHealth++;

            Destroy(gameObject);
        }
    }
}
