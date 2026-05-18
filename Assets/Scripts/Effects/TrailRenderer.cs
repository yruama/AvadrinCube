using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Affiche un trait entre la position du joueur et un point de référence (visuel).
/// </summary>
public class TrailRenderer : MonoBehaviour
{
    public LineRenderer lr;
    private GameObject _player;
	// Use this for initialization
	void Start ()
    {
        Transform playerTransform = GameRegistry.Instance.GetPlayerTransform();
        if (playerTransform == null)
        {
            Debug.LogError("Player not found in GameRegistry");
            enabled = false;
            return;
        }
        _player = playerTransform.gameObject;
    }
    
	// Update is called once per frame
	void Update ()
    {
		if (_player != null)
        {
            Vector3 pos = new Vector3(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y, _player.transform.position.z - transform.position.z);
            lr.SetPosition(0, _player.transform.position);
            lr.SetPosition(1, transform.position);
        }
	}
}
