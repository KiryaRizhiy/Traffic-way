using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InterfaceAnimation : MonoBehaviour
{
    public List<Clip> animations;
    public Tween currentAnimation;
    void Start()
    {
        foreach (Clip _c in animations)
        {
            if(_c.playCondidion == PlayConditions.None ||
                (_c.playCondidion == PlayConditions.IsGarageOpened && Engine.meta.GarageOpened)
                )
            {
                if (_c.animType == AnimationTypes.CanvasGroupFadeIn)
                {
                    if (_c.playType == PlayTypes.OnStart)
                        currentAnimation = GetComponent<CanvasGroup>().DOFade(1f, _c.duration).SetDelay(_c.delay);
                    if (_c.playType == PlayTypes.Loop)
                    {
                        Sequence _sq = DOTween.Sequence();
                        currentAnimation = DOTween.Sequence().PrependInterval(_c.delay)
                            .Append(_sq.Append(GetComponent<CanvasGroup>().DOFade(1f, _c.duration))
                            .AppendInterval(_c.actionPause)
                            .Append(GetComponent<CanvasGroup>().DOFade(GetComponent<CanvasGroup>().alpha, _c.duration))
                            .AppendInterval(_c.loopPasue)
                            .SetLoops(_c.loops));
                    }
                }
                if (_c.animType == AnimationTypes.ZoomInZoomOut)
                {
                    if (_c.playType == PlayTypes.OnStart)
                        currentAnimation = transform.DOScale(_c.scale, _c.duration).SetDelay(_c.delay);
                    if (_c.playType == PlayTypes.Loop)
                    {
                        Sequence _sq = DOTween.Sequence();
                        currentAnimation = DOTween.Sequence().PrependInterval(_c.delay)
                            .Append(_sq.Append(transform.DOScale(_c.scale, _c.duration))
                            .AppendInterval(_c.actionPause)
                            .Append(transform.DOScale(1f, _c.duration))
                            .AppendInterval(_c.loopPasue)
                            .SetLoops(_c.loops)); ;
                    }
                }
                if (_c.animType == AnimationTypes.MoveUp)
                {
                    RectTransform _rt = GetComponent<RectTransform>();
                    if (_c.playType == PlayTypes.OnStart)
                    {
                        currentAnimation = DOTween.Sequence()
                            .Append(_rt.DOAnchorMax(new Vector2(_rt.anchorMax.x, _c.relatedDistanceMax), _c.duration).SetDelay(_c.delay))
                            .Join(_rt.DOAnchorMin(new Vector2(_rt.anchorMin.x, _c.relatedDistanceMin), _c.duration).SetDelay(_c.delay));
                    }
                    if (_c.playType == PlayTypes.Loop)
                    {
                        Sequence _sq = DOTween.Sequence();
                        currentAnimation = DOTween.Sequence().PrependInterval(_c.delay)
                            .Append(_sq
                            .Append(_rt.DOAnchorMax(new Vector2(_rt.anchorMax.x, _c.relatedDistanceMax), _c.duration))
                            .Join(_rt.DOAnchorMin(new Vector2(_rt.anchorMin.x, _c.relatedDistanceMin), _c.duration))
                            .AppendInterval(_c.actionPause)
                            .Append(_rt.DOAnchorMax(new Vector2(_rt.anchorMax.x, _rt.anchorMax.y), _c.duration))
                            .Join(_rt.DOAnchorMin(new Vector2(_rt.anchorMin.x, _rt.anchorMin.y), _c.duration))
                            .AppendInterval(_c.loopPasue)
                            .SetLoops(_c.loops));
                    }
                }
                //if (_c.animType == AnimationTypes.Jump)
                //{
                //    RectTransform _rt = GetComponent<RectTransform>();
                //    if (_c.playType == PlayTypes.OnStart)
                //        _rt.DOJumpAnchorPos(_rt.anchoredPosition,_c.scale,1,_c.duration).SetDelay(_c.delay);
                //    if (_c.playType == PlayTypes.Loop)
                //    {
                //        Sequence _sq = DOTween.Sequence();
                //        DOTween.Sequence().PrependInterval(_c.delay)
                //            .Append(_sq.Append(transform.DOScale(_c.scale, _c.duration))
                //            .AppendInterval(_c.actionPause)
                //            .Append(transform.DOScale(1f, _c.duration))
                //            .AppendInterval(_c.loopPasue)
                //            .SetLoops(_c.loops)); ;
                //    }
                //}
            }
        }
    }


    [System.Serializable]
    public class Clip
    {
        public AnimationTypes animType;
        public PlayTypes playType;
        public PlayConditions playCondidion;
        public float scale;
        public float relatedDistanceMin;
        public float relatedDistanceMax;
        public float delay;
        public float actionPause;
        public float loopPasue;
        public float duration;
        public int loops;
    }
}
public enum AnimationTypes { CanvasGroupFadeIn, ZoomInZoomOut, MoveUp}
public enum PlayTypes { OnStart , Loop}
public enum PlayConditions { None, IsGarageOpened}
