using UnityEngine;

using DG.Tweening;

namespace SMGCore.Windows {
	public partial class WindowController : MonoBehaviour {
		public Sequence CreateAppearSequence() {
			switch ( ShowAnim ) {
				case ShowAnimType.ScaleUp: {
						return GetAppearSequenceScaleUp();
					}
				case ShowAnimType.SlideDown: {
						return GetAppearSequenceByDirection(Vector2.down);
					}
				case ShowAnimType.SlideUp: {
						return GetAppearSequenceByDirection(Vector2.up);
					}
				case ShowAnimType.SlideLeft: {
						return GetAppearSequenceByDirection(Vector2.left);
					}
				case ShowAnimType.SlideRight: {
						return GetAppearSequenceByDirection(Vector2.right);
					}
				default: {
						return DOTween.Sequence();
					}
			}
		}

		public Sequence CreateDisappearSequence() {
			switch ( HideAnim ) {
				case HideAnimType.ScaleDown: {
						return GetDisappearSequenceScaleDown();
					}
				case HideAnimType.MoveDown: {
						return GetDisappearSequenceByDirection(Vector2.down);
					}
				case HideAnimType.MoveUp: {
						return GetDisappearSequenceByDirection(Vector2.up);
					}
				case HideAnimType.MoveLeft: {
						return GetDisappearSequenceByDirection(Vector2.left);
					}
				case HideAnimType.MoveRight: {
						return GetDisappearSequenceByDirection(Vector2.right);
					}
				default: {
						return DOTween.Sequence();
					}
			}
		}

		public Sequence GetAppearSequenceByDirection(Vector2 dir) {
			var sequence = DOTween.Sequence();
			_canvasGroup.alpha = 0;
			sequence.Append(_canvasGroup.DOFade(1, WindowConstants.ShowFadeTime));
			_transform.localScale = Vector3.one;
			var currentPosition = _transform.anchoredPosition;
			_transform.anchoredPosition = currentPosition + dir * WindowConstants.ShowOffset;
			sequence.Insert(0, _transform.DOAnchorPos(currentPosition, WindowConstants.ShowMoveTime));
			sequence.Append(_transform.DOShakeAnchorPos(WindowConstants.ShowShakePosTime, (-dir) * WindowConstants.ShakeForce, 5, 0, true));
			sequence.Append(_transform.DOPunchScale(dir * WindowConstants.ShowScaleDown, WindowConstants.ShowShakeScaleTime, 5).SetEase(Ease.InOutBounce));
			return sequence;
		}

		public Sequence GetAppearSequenceScaleUp() {
			_transform.localScale = Vector3.zero;
			var sequence = DOTween.Sequence();
			_canvasGroup.alpha = 0;
			sequence.Append(_canvasGroup.DOFade(1, WindowConstants.ShowFadeTime));
			sequence.Insert(0.1f, _transform.DOScale(1, WindowConstants.ShowScaleUpTime));
			return sequence;
		}

		public Sequence GetDisappearSequenceScaleDown() {
			var sequence = DOTween.Sequence();
			sequence.AppendInterval(WindowConstants.HideFadeDelay);
			sequence.Append(_canvasGroup.DOFade(0, WindowConstants.HideFadeTime));
			sequence.Insert(0, _transform.DOScale(WindowConstants.HideScaleUp, WindowConstants.HideScaleUpTime));
			sequence.Insert(WindowConstants.HideScaleUpTime, _transform.DOScale(0, WindowConstants.HideScaleDownTime));
			return sequence;
		}

		public Sequence GetDisappearSequenceByDirection(Vector2 dir) {
			var sequence = DOTween.Sequence();
			sequence.Append(_canvasGroup.DOFade(0, WindowConstants.HideMoveFadeTime));
			_transform.localScale = Vector3.one;
			var nextAnchoredPosition = _transform.anchoredPosition + dir * WindowConstants.HideOffset;
			sequence.Insert(0, _transform.DOAnchorPos(nextAnchoredPosition, WindowConstants.HideMoveFadeTime));
			return sequence;
		}

		float GetCallbackTime() {
			switch ( ShowAnim ) {
				case ShowAnimType.ScaleUp:
					return WindowConstants.ShowFadeTime;
				case ShowAnimType.None:
					return 0;
				case ShowAnimType.SlideDown:
					return WindowConstants.ShowMoveTime * 0.5f;
				default:
					return WindowConstants.ShowMoveTime;
			}
		}
	}
}
