using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    public GameObject ProjectileObject;

    public void Fire()
    {
        Instantiate(ProjectileObject, transform.position, transform.rotation);
    }
}