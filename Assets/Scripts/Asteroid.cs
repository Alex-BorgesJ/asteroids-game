﻿using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites;

    public float size = 1.0f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50.0f;
    public float maxLifetime = 30.0f;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Assign random properties to make each asteroid feel unique
        this.spriteRenderer.sprite = this.sprites[Random.Range(0, this.sprites.Length)];
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Random.value * 360.0f);

        // Set the scale and mass of the asteroid based on the assigned size so
        // the physics is more realistic
        this.transform.localScale = Vector3.one * this.size;
        this.rigidbody.mass = this.size;

        // Destroy the asteroid after it reaches its max lifetime
        Destroy(this.gameObject, this.maxLifetime);
    }

    public void SetTrajectory(Vector2 direction)
    {
        // The asteroid only needs a force to be added once since they have no
        // drag to make them stop moving
        this.rigidbody.AddForce(direction * this.movementSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            // Check if the asteroid is large enough to split in half
            // (both parts must be greater than the minimum size)
            if ((this.size * 0.5f) >= this.minSize)
            {
                CreateSplit();
                CreateSplit();
            }

            FindObjectOfType<GameManager>().AsteroidDestroyed(this);

            // Destroy the current asteroid since it is either replaced by two
            // new asteroids or small enough to be destroyed by the bullet
            Destroy(this.gameObject);
        }
    }

    private Asteroid CreateSplit()
    {
        // Set the new asteroid poistion to be the same as the current asteroid
        // but with a slight offset so they do not spawn inside each other
        Vector2 position = this.transform.position;
        position += Random.insideUnitCircle * 0.5f;

        // Create the new asteroid at half the size of the current
        Asteroid half = Instantiate(this, position, this.transform.rotation);
        half.size = this.size * 0.5f;

        // Set a random trajectory
        half.SetTrajectory(Random.insideUnitCircle.normalized);

        return half;
    }

}
