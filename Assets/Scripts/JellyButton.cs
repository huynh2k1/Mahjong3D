using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class JellyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Button _mBtn;
    private Vector3 _scale;
    [Header("Settings")][SerializeField] private bool resetSizeOnEnable = true;

    [Header("LightSweep")]
    [SerializeField]
    private RectTransform lightSweep;

    [SerializeField] private float sweepDuration = 0.75f;
    [SerializeField] private float delayEachLoop = 2.5f;
    [SerializeField] Vector2 _lightSweepStartPos;

    [Header("Auto scale frequently")]
    [SerializeField]
    private bool scaleFrequently = false;
    [SerializeField] private Vector2 targetScale = new Vector2(1.2f, 1.2f);
    [SerializeField] private float delayScale = 2f;
    [SerializeField] private float scaleDuration = 0.5f;


    [Space]
    [Header("Scale button on click")]
    [SerializeField]
    private bool scaleOnClick = true;

    [SerializeField] private float durationScale = 0.2f;
    [SerializeField] private float scales = -0.1f;
    [SerializeField] private Ease easeScaleClick = Ease.InOutQuad;

    //[SerializeField] private Sound sound = Sound.Button;


    private RectTransform rectTransform;
    private Sequence s;

    public static event Action playSmallSound;
    public static event Action playMediumSound;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _scale = transform.localScale;
        initLightSweepPos();
    }

    private void OnEnable()
    {
        if (lightSweep != null)
        {
            Vector2 desPos = new Vector2(_lightSweepStartPos.x * -1, _lightSweepStartPos.y);
            s = DOTween.Sequence();
            // s.SetDelay(1f);
            // DOVirtual.DelayedCall(1f, () =>
            // {
            ButtonRewardAnim(s, desPos);
            // });
        }

        ScaleFrequently();

        if (resetSizeOnEnable)
        {
            transform.localScale = _scale;
        }
    }

    private void ScaleFrequently()
    {
        if (scaleFrequently)
            transform.DOScale(targetScale, scaleDuration).SetDelay(delayScale)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    transform.DOScale(new Vector3(1, 1, 1), scaleDuration).SetUpdate(true)
                        .OnComplete(ScaleFrequently);
                });
    }

    private void ButtonRewardAnim(Sequence sequence, Vector2 desPos)
    {
        sequence.Append(lightSweep.DOAnchorPos(desPos, sweepDuration).SetEase(Ease.Linear));
        sequence.AppendInterval(delayEachLoop);
        sequence.SetLoops(-1, LoopType.Restart);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //SoundManager.instance.PlaySound(sound);
        if (scaleOnClick)
            transform.DOScale(new Vector3(_scale.x + scales, _scale.y + scales, _scale.z + scales), durationScale)
                .SetUpdate(true).SetEase(easeScaleClick);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (scaleOnClick)
            transform.DOScale(new Vector3(_scale.x, _scale.y, _scale.z), durationScale).SetUpdate(true)
                .SetEase(easeScaleClick);
    }


    private void initLightSweepPos()
    {
        if (lightSweep != null)
        {
            //lightSweep.anchoredPosition = _lightSweepStartPos;
            _lightSweepStartPos = lightSweep.anchoredPosition;
        }
    }

    private void OnDisable()
    {
        s.Kill();
        transform.DOKill();
        initLightSweepPos();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundCtrl.I.PlaySound(TypeSound.CLICK);
    }
}
