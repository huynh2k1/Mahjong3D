using DG.Tweening;
using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int ID;
    public int indexMat = 1;
    public Vector3Int gridPos;
    public GameObject model;
    [SerializeField] ParticleSystem _effect;
    [SerializeField] Color normalColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color inCorrectColor;
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    public Action<Block> OnBlockClick;

    private void Awake()
    {
        if(meshRenderer == null)
            meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        if(boxCollider == null)
        {
            boxCollider = transform.GetComponent<BoxCollider>();
        }
        Normal();
    }

    private void OnMouseDown()
    {
        if (GameManager.I.CurState != GameState.Play)
            return;
        LevelControl.I.SelectBlock(this);   
    }

    public void Normal()
    {
        meshRenderer.materials[indexMat].color = normalColor;
    }

    public void Hover()
    {
        meshRenderer.materials[indexMat].color = hoverColor;
    }

    public void Shake(float duration = 0.1f, float strength = 0.05f)
    {
        transform.DOKill(); // Ngăn animation cũ chồng lên
        Vector3 pos = transform.position;
        meshRenderer.materials[indexMat].color = inCorrectColor;

        // Shake theo vị trí (X,Z), khóa trục Y
        transform.DOShakePosition(duration,
            new Vector3(strength, 0, strength),
            vibrato: 10,
            randomness: 90,
            fadeOut: true).OnComplete(() =>
            {
                Normal();
                transform.position = pos;
            });
    }

    public void Destroy()
    {
        model.transform.DOKill();
        _effect.Play();
        boxCollider.enabled = false;
        model.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            model.SetActive(false);
        });
    }
}
