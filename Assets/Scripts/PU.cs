using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PU : UI
{
    public override UIType Type => throw new NotImplementedException();

    [Header("References")]
    [SerializeField] private Button btnClose;
    [SerializeField] protected Image mask;
    [SerializeField] protected CanvasGroup mainGroup;
    [SerializeField] protected GameObject main;

    [Header("Tween Settings")]
    [SerializeField] private float targetMaskAlpha = 0.9f;
    [SerializeField] private float tweenDuration = 0.4f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    #region Unity Methods
    protected virtual void Awake()
    {
        Initialize();
        btnClose?.onClick.AddListener(Hide);
    }
    #endregion

    #region Initialization
    protected virtual void Initialize()
    {
        if (mask != null)
        {
            var color = mask.color;
            color.a = 0;
            mask.color = color;
            mask.raycastTarget = false;
        }

        if (main != null)
            main.SetActive(false);

        if (mainGroup != null)
            mainGroup.blocksRaycasts = false;
    }
    #endregion

    #region Public Methods
    public override void Show()
    {
        FadeMask(true);
        AnimateMain(true);
    }

    public override void Hide()
    {
        FadeMask(false);
        AnimateMain(false);
    }
    #endregion

    #region Animation
    protected virtual void AnimateMain(bool isShowing, Action onComplete = null)
    {
        if (main == null || mainGroup == null) return;

        main.transform.DOKill();
        mainGroup.blocksRaycasts = false;

        if (isShowing)
        {
            main.SetActive(true);
            main.transform
                .DOScale(Vector3.one, tweenDuration)
                .From(0.6f)
                .SetUpdate(true)
                .SetEase(showEase)
                .OnComplete(() =>
                {
                    mainGroup.blocksRaycasts = true;
                    onComplete?.Invoke();
                });
        }
        else
        {
            main.transform
                .DOScale(0.4f, tweenDuration)
                .From(1f)
                .SetUpdate(true)
                .SetEase(hideEase)
                .OnComplete(() =>
                {
                    main.SetActive(false);
                    onComplete?.Invoke();
                    Time.timeScale = 1f;
                });
        }
    }

    protected virtual void FadeMask(bool isShowing)
    {
        if (mask == null) return;

        mask.DOKill();
        if (isShowing)
        {
            mask.raycastTarget = true;
            mask.DOFade(targetMaskAlpha, tweenDuration + 0.2f)
                .From(0f)
                .SetUpdate(true)
                .SetEase(Ease.Linear);
        }
        else
        {
            mask.DOFade(0f, tweenDuration)
                .SetUpdate(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => mask.raycastTarget = false);
        }
    }
    #endregion
}
