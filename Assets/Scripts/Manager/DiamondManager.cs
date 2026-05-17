using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiamondManager : MonoBehaviour
{
    public Material diamondMaterial;
    public GameObject diamondParent;
    public GameObject star;

    private int _nbDiamond = 0;
    private int _nbStar = 0;

    public void OnGetDiamond() {
        GameObject diamond = diamondParent.transform.GetChild(_nbDiamond).gameObject;
        diamond.GetComponent<Renderer>().material = diamondMaterial;

        DOTween.To(() => diamond.transform.localScale, x => diamond.transform.localScale = x, Vector3.one * 1.25f, 0.25f).OnComplete(() => {
            DOTween.To(() => diamond.transform.localScale, x => diamond.transform.localScale = x, Vector3.one, 0.25f);
        });

        _nbDiamond += 1;
    }

    public void OnGetStar() {
        DOTween.To(() => star.transform.localScale, x => star.transform.localScale = x, Vector3.one * 1.25f, 0.25f).OnComplete(() => {
            DOTween.To(() => star.transform.localScale, x => star.transform.localScale = x, Vector3.one, 0.25f);
        });

        _nbStar += 1;
        
        star.SetActive(true);
    }

    public int nbDiamond
    {
        get
        {
            return _nbDiamond;
        }

        set
        {
            _nbDiamond = value;
        }
    }
}
