using UnityEngine;
using UnityEngine.UI;

using SMGCore.Utils;

namespace SMGCore.Windows {
	public sealed class UICanvas : MonoBehaviour {
		bool _isLayerAssigned = false;

		public Transform Root { get; set; }

		Canvas Canvas { get; set; }

		void Start() {
			TrySetCanvasLayer();
		}

		public UICanvas Init() {
			Root   = gameObject.AddComponent<RectTransform>();
			Canvas = gameObject.AddComponent<Canvas>();

			SetupCanvas(Canvas);
			SetupRaycaster(gameObject.AddComponent<GraphicRaycaster>());
			SetupScaler(gameObject.AddComponent<CanvasScaler>());

			if ( Application.isPlaying ) {
				DontDestroyOnLoad(gameObject);
			}

			return this;
		}

		public void UpdateCanvasCamera() {
			Canvas.worldCamera = Camera.main;
		}

		public bool HasActiveWindow<T>() where T : MonoBehaviour {
			return FindObjectOfType<T>();
		}

		public GameObject InstantiateNew(GameObject prefab) {
			if ( !prefab ) {
				Debug.LogError("UICanvas.InstantiateNew: prefab is null.");
				return null;

			}
			return Instantiate(prefab, Root, false);
		}

		public void Attach(WindowController window) {
			if ( !window ) {
				Debug.LogError("UICanvas.Attach: window is null.");
				return;
			}
			var trans = window.CachedTransform;
			RectTransformUtils.UpdateLocalPosition(trans, Vector2.zero);
		}

		public void CheckLayers(WindowController newWindow, WindowController oldWindow) {
			if ( !oldWindow ) {
				return;
			}
			if ( !newWindow ) {
				Debug.LogError("UICanvas.CheckLayers: newWindow is null.");
				return;
			}
			var oldOrderMin = 0;
			if ( oldWindow.OverrideCanvas ) {
				oldOrderMin = oldWindow.OverrideCanvas.sortingOrder;
			}
			var oldOrderMax = oldWindow.MaxOrderInLayer;
			var newOrder = 0;
			if ( newWindow.OverrideCanvas ) {
				newOrder = newWindow.OverrideCanvas.sortingOrder;
			}
			if ( newOrder < oldOrderMin ) {
				Debug.LogWarningFormat(
					"UICanvas.CheckLayers: New window order = {0} < current min window order = {1} (new: {2}, old: {3})",
					newOrder, oldOrderMin, newWindow, oldWindow
				);
			}
			if ( newOrder < oldOrderMax ) {
				Debug.LogWarningFormat(
					"UICanvas.CheckLayers: New window order = {0} < current max window order = {1} (new: {2}, old: {3})",
					newOrder, oldOrderMax, newWindow, oldWindow
				);
			}
		}

		public void SetupLayers(WindowController window, WindowBackground background, bool hasWindowsWithBackground, bool force) {
			if ( !window ) {
				Debug.LogError("UICanvas.SetupLayers: window is null");
				return;
			}
			if ( background ) {
				if ( window.WithBackground ) {
					background.Show(force, window.BgColor);
					MoveToFirstPlane(background);
					SetupOrderInLayer(window, background);
				} else if ( !hasWindowsWithBackground ) {
					background.Resetup();
				}
			}
			MoveToFirstPlane(window);
		}

		public void MoveToFirstPlane(WindowController window) {
			if ( !window ) {
				Debug.LogError("UICanvas.MoveToFirstPlane: window is null.");
				return;
			}
			window.CachedTransform.SetAsLastSibling();
		}

		public void MoveToFirstPlane(WindowBackground background) {
			if ( !background ) {
				Debug.LogError("UICanvas.MoveToFirstPlane: background is null");
				return;
			}
			background.CachedTransform.SetAsLastSibling();
		}

		public void HideBackground(WindowBackground background, bool force) {
			if ( background ) {
				background.Hide(force);
			}
		}

		public void TrySetCanvasLayer() {
			if ( _isLayerAssigned ) {
				return;
			}

			var targetLayer = SortingLayer.NameToID("UI");
			var targetOrder = 1;

			Canvas.sortingLayerID = targetLayer;
			Canvas.sortingOrder   = targetOrder;

			if ( (Canvas.sortingLayerID == targetLayer) && (Canvas.sortingOrder == targetOrder) ) {
				_isLayerAssigned = true;
			}
		}

		void SetupCanvas(Canvas canvas) {
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.planeDistance = 100;
			canvas.additionalShaderChannels =
				AdditionalCanvasShaderChannels.TexCoord1 |
				AdditionalCanvasShaderChannels.TexCoord2 |
				AdditionalCanvasShaderChannels.Tangent |
				AdditionalCanvasShaderChannels.Normal;
		}

		void SetupRaycaster(GraphicRaycaster raycaster) {
			raycaster.ignoreReversedGraphics = true;
		}

		void SetupScaler(CanvasScaler scaler) {
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 0f;
			scaler.referencePixelsPerUnit = 100.0f;
		}

		void SetupOrderInLayer(WindowController window, WindowBackground background) {
			if ( !window ) {
				Debug.LogError("UICanvas.SetupOrderInLayer: window is null.");
				return;
			}
			var windowOrder = 0;
			if ( window.OverrideCanvas ) {
				windowOrder = window.OverrideCanvas.sortingOrder;
			}
			if ( !background ) {
				Debug.LogError("UICanvas.SetupOrderInLayer: background is null.");
				return;
			}
			if ( background.OverrideCanvas ) {
				background.OverrideCanvas.overrideSorting = windowOrder > 0;
				background.OverrideCanvas.sortingOrder    = windowOrder - 1;
			}
		}
	}
}
