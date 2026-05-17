using UnityEngine;
using DG.Tweening;

public class DiamondController : MonoBehaviour
{
    public bool isStar;
    public float transitionTime;

    private Vector3 _startPosition;
    private Tween _tween;
    private DiamondManager _diamondManager;

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
        AnimateIdle();
    }

    void AnimateIdle()
    {
        _tween = DOTween.To(() => transform.position, x => transform.position = x,
            new Vector3(_startPosition.x, _startPosition.y + 0.07f, _startPosition.z), transitionTime)
            .OnComplete(() =>
            {
                DOTween.To(() => transform.position, x => transform.position = x,
                    new Vector3(_startPosition.x, _startPosition.y - 0.07f, _startPosition.z), transitionTime)
                    .OnComplete(AnimateIdle);
            });
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.TAG_PLAYER))
            return;

        _tween?.Kill();

        DOTween.To(() => transform.position, x => transform.position = x,
            new Vector3(_startPosition.x, _startPosition.y + 2f, _startPosition.z), 0.1f)
            .OnComplete(() =>
            {
                DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.one * 1.5f, 0.25f)
                    .OnComplete(() =>
                    {
                        DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.zero, 0.1f)
                            .OnComplete(() =>
                            {
                                if (isStar)
                                    _diamondManager.OnGetStar();
                                else
                                    _diamondManager.OnGetDiamond();
                                Destroy(gameObject);
                            });
                    });
            });
    }
}
