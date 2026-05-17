using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSwitchTimer : MonoBehaviour
{
    private LineRenderer lr;
    private GameObject _target;
        
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (_target != null)
        {
            lr.SetPosition(0, _target.transform.position);
            lr.SetPosition(1, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
        }
    }

    public void SetTarget(GameObject go) {
        _target = go;
    }
}
