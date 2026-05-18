using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fait tourner le skybox en modifiant la propriété _Rotation du shader selon le temps.
/// </summary>
public class WorldRotation : MonoBehaviour
{
    public float speed;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}