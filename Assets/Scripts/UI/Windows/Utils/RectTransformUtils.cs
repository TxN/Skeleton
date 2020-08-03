using UnityEngine;
using UnityEngine.UI;

namespace SMGCore.Utils {
	public static class RectTransformUtils {
		public static Vector2 GetLocalPosition(RectTransform transform) {
			if ( transform == null ) {
				return Vector2.zero;
			}
			if ( transform.parent == null ) {
				return transform.position;
			}
			return transform.parent.InverseTransformPoint(transform.position);
		}

		public static Vector3 GetLocalPosition(Transform transform) {
			var rectTransform = transform.GetComponent<RectTransform>();
			return rectTransform ? (Vector3)GetLocalPosition(rectTransform) : transform.localPosition;
		}

		public static void UpdateLocalPosition(RectTransform transform, Vector2 position, bool useWorldDelta = true) {
			if ( transform == null ) {
				return;
			}
			if ( transform.parent == null ) {
				transform.position = position;
				return;
			}
			if ( useWorldDelta ) {
				var newGlobal = transform.parent.TransformPoint(position);
				var delta = newGlobal - transform.position;
				transform.anchoredPosition += (Vector2)delta;
			} else {
				transform.anchoredPosition = position;
			}
		}

		public static void UpdateLocalPosition(Image image, Vector2 position) {
			UpdateLocalPosition(image.rectTransform, position);
		}

		public static void UpdateLocalPosition(Transform transform, Vector2 position, bool useWorldDelta = true) {
			var rectTransform = transform.GetComponent<RectTransform>();
			if ( rectTransform ) {
				UpdateLocalPosition(rectTransform, position, useWorldDelta);
			} else {
				transform.localPosition = position;
			}
		}

		public static void AddToLocalPosition(RectTransform transform, Vector2 position) {
			var oldPosition = GetLocalPosition(transform);
			UpdateLocalPosition(transform, oldPosition + position);
		}

		public static void AddToLocalPosition(Image image, Vector2 position) {
			AddToLocalPosition(image.rectTransform, position);
		}

		public static void AddToLocalPosition(Transform transform, Vector2 position) {
			var oldPosition = GetLocalPosition(transform);
			UpdateLocalPosition(transform, oldPosition + (Vector3)position);
		}
	}
}
