using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Vector2 axisInput = _playerController.moveAction.ReadValue<Vector2>();
        axisInput *= _moveSpeed;

        if (_playerController.CanMove)
        {
            _playerController.Controller.Move(new Vector3(axisInput.x, _playerController.JumpVelocity, axisInput.y) * Time.deltaTime);
        }
    }
    
    public void SetParent(Transform newParent)
    {
       transform.parent = newParent;

    }

    public void ClearParent()
    {
         transform.parent = null;
    }
}
