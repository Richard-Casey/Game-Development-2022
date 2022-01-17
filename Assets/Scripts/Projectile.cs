using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    // declare the LaunchSpeed variable as public, so it is exposed to the editor.
    public float LaunchSpeed = 3.0f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(LaunchSpeed, 0.0f);
    }
}