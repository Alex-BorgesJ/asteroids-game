using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    public AudioClip destroySound;

    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;
    public float maxLifetime = 30f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Adiciona o componente de som
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Define sprite aleatório
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);

        // Ajusta escala e massa baseado no tamanho
        transform.localScale = Vector3.one * size;
        rb.mass = size;

        // Destrói após um tempo de vida máximo
        Destroy(gameObject, maxLifetime);
    }

    public void SetTrajectory(Vector2 direction)
    {
        rb.AddForce(direction * movementSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {

            // Toca o som de destruição
            if (destroySound != null)
            {
                float volume = Mathf.Lerp(0.07f, 0.4f, Mathf.InverseLerp(minSize, maxSize, size));
                AudioSource.PlayClipAtPoint(destroySound, transform.position, volume);
            }

            // Divide se for grande o suficiente
            if ((size * 0.5f) >= minSize)
            {
                CreateSplit();
                CreateSplit();
            }

            GameManager.Instance.OnAsteroidDestroyed(this);

            // Destroi esse asteroide
            Destroy(gameObject);
        }
    }

    private Asteroid CreateSplit()
    {
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;

        half.SetTrajectory(Random.insideUnitCircle.normalized);

        return half;
    }
}
