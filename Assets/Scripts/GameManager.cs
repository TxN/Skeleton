using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SMGCore {
	public class GameManager : ManualSingleton<GameManager> {
		const float RARE_UPDATE_INTERVAL = 1f;
		Dictionary<string, BaseStateController> _controllers = new Dictionary<string, BaseStateController>();

		public void Initialize() {
			CreateControllers();
			Init();
			PostInit();
			Load();
			PostLoad();
			StartCoroutine(RareUpdate());
		}

		void CreateControllers() {
			//Здесь добавляются все игровые контроллеры.
		}

		void Start() {
			Initialize();
		}

		void Update() {
			UpdateControllers();
		}

		void LateUpdate() {
			LateUpdateControllers();
		}

		void OnDestroy() {
			Reset();
			StopAllCoroutines();
		}

		public void Reset() {
			foreach ( var pair in _controllers ) {
				try {
					pair.Value.Reset();
				} catch ( Exception e ) {
					Debug.LogError(e);
				}
			}
		}

		public void Init() {
			foreach ( var pair in _controllers ) {
				try {
					pair.Value.Init();
				} catch ( Exception e ) {
					Debug.LogError(e);
				}
			}
		}

		public void PostInit() {
			foreach ( var pair in _controllers ) {
				try {
					pair.Value.PostInit();
				} catch ( Exception e ) {
					Debug.LogError(e);
				}
			}
		}

		public void Load() {
			foreach ( var pair in _controllers ) {
				var controller = pair.Value;
				controller.Load();
			}
		}

		public void PostLoad() {
			foreach ( var pair in _controllers ) {
				pair.Value.PostLoad();
			}
		}

		public void Save() {
			foreach ( var pair in _controllers ) {
				pair.Value.Save();
			}
		}

		public void UpdateControllers() {
			foreach ( var pair in _controllers ) {
				pair.Value.Update();
			}
		}

		public void LateUpdateControllers() {
			foreach ( var pair in _controllers ) {
				pair.Value.LateUpdate();
			}
		}

		public void RareUpdateControllers() {
			foreach ( var pair in _controllers ) {
				pair.Value.RareUpdate();
			}
		}

		IEnumerator RareUpdate() {
			while ( true ) {
				RareUpdateControllers();
				yield return new WaitForSeconds(RARE_UPDATE_INTERVAL);
			}
		}
	}
}
