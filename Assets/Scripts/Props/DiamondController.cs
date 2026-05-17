using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class DiamondController : MonoBehaviour
{
    public bool isStar = false;
    public float transitionTime;
    Vector3 _startPosition;

    private Tween _tween;

    private DiamondManager _diamondManager;

    // Start is called before the first frame update
    void Start()
    {
        _diamondManager = GameRegistry.Instance.GetDiamondManager();
        if (_diamondManager == null)
        {
            Debug.LogError("DiamondManager not found in GameRegistry");
            enabled = false;
            return;
        }
        _startPosition = transform.position;
        MoveDiamond();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveDiamond() {
        _tween = DOTween.To(() => transform.position, x => transform.position = x, new Vector3(_startPosition.x, _startPosition.y + 0.07f, _startPosition.z), transitionTime).OnComplete(() => {
            DOTween.To(() => transform.position, x => transform.position = x, new Vector3(_startPosition.x, _startPosition.y - 0.07f, _startPosition.z), transitionTime).OnComplete(() => {
                MoveDiamond();
            });
        });
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_PLAYER)
        {
            _tween.Kill();

            DOTween.To(() => transform.position, x => transform.position = x, new Vector3(_startPosition.x, _startPosition.y + 2f, _startPosition.z), 0.1f).OnComplete(() => {
                DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.one * 1.5f, 0.25f).OnComplete(() => {
                    DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.one * 0, 0.1f).OnComplete(() => {

                        if (isStar == true) _diamondManager.OnGetStar();
                        else _diamondManager.OnGetDiamond();
                        Destroy(gameObject);

                    });
                });
            });


            
        }
    }
}
