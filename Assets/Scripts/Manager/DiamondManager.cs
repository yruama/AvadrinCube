using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Gère l'affichage et le comptage des diamants et étoiles collectés.
/// Ajoute des protections null et des vérifications de bornes pour éviter des exceptions runtime.
/// </summary>
public class DiamondManager : MonoBehaviour
{
    public Material diamondMaterial;
    public GameObject diamondParent;
    public GameObject star;

    private int _nbDiamond = 0;
    private int _nbStar = 0;

    void Start()
    {
        if (diamondParent == null)
            Debug.LogWarning("DiamondManager: diamondParent non assigné.");
        if (diamondMaterial == null)
            Debug.LogWarning("DiamondManager: diamondMaterial non assigné.");
        if (star == null)
            Debug.LogWarning("DiamondManager: star non assigné.");
    }

    /// <summary>
    /// Appelé lorsqu'un diamant est collecté. Met à jour le rendu et incrémente le compteur.
    /// </summary>
    public void OnGetDiamond()
    {
        if (diamondParent == null)
        {
            Debug.LogWarning("OnGetDiamond appelé mais diamondParent est null.");
            return;
        }

        int childCount = diamondParent.transform.childCount;
        if (_nbDiamond < 0 || _nbDiamond >= childCount)
        {
            Debug.LogWarning("Index de diamant hors bornes ou aucun enfant disponible.");
            return;
        }

        GameObject diamond = diamondParent.transform.GetChild(_nbDiamond).gameObject;
        var renderer = diamond.GetComponent<Renderer>();
        if (renderer != null && diamondMaterial != null)
        {
            renderer.material = diamondMaterial;
        }

        DOTween.To(() => diamond.transform.localScale, x => diamond.transform.localScale = x, Vector3.one * 1.25f, 0.25f).OnComplete(() =>
        {
            DOTween.To(() => diamond.transform.localScale, x => diamond.transform.localScale = x, Vector3.one, 0.25f);
        });

        _nbDiamond += 1;
    }

    /// <summary>
    /// Appelé lorsqu'une étoile est collectée. Active l'objet étoile et joue l'animation.
    /// </summary>
    public void OnGetStar()
    {
        if (star == null)
        {
            Debug.LogWarning("OnGetStar appelé mais star est null.");
            return;
        }

        DOTween.To(() => star.transform.localScale, x => star.transform.localScale = x, Vector3.one * 1.25f, 0.25f).OnComplete(() =>
        {
            DOTween.To(() => star.transform.localScale, x => star.transform.localScale = x, Vector3.one, 0.25f);
        });

        _nbStar += 1;
        star.SetActive(true);
    }

    public int nbDiamond
    {
        get { return _nbDiamond; }
        set { _nbDiamond = value; }
    }
}
