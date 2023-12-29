using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Example 20 */
public class CE20SceneManager : CSceneManager {
	/** 상태 */
	private enum EState {
		NONE = -1,
		PLAY,
		MATCHING,
		[HideInInspector] MAX_VAL
	}

	/** UI 정보 */
	[System.Serializable]
	public struct STUIsInfo {
		public GameObject m_oUIs;
		public GameObject m_oObjs;
	}

	#region 변수
	[SerializeField] private STUIsInfo m_oPlayUIsInfo;
	[SerializeField] private STUIsInfo m_oMatchingUIsInfo;

	private EState m_eState = EState.MATCHING;
	private CE20UIsHandler m_oPlayUIsHandler = null;
	private CE20UIsHandler m_oMatchingUIsHandler = null;
	private CE20Engine m_oEngine = null;
	#endregion // 변수

	#region 프로퍼티
	public override string SceneName => KDefine.G_SCENE_N_E20;
	public CE20Engine Engine => m_oEngine;
	#endregion // 프로퍼티

	#region 함수
	/** 초기화 */
	public override void Awake() {
		base.Awake();
		CScheduleManager.Inst.AddComponent(this);

		// 엔진을 설정한다
		m_oEngine = this.GetComponentInChildren<CE20Engine>();

		// UI 처리자를 설정한다
		m_oPlayUIsHandler = this.GetComponentInChildren<CE20PlayUIsHandler>();
		m_oMatchingUIsHandler = this.GetComponentInChildren<CE20MatchingUIsHandler>();
	}

	/** 초기화 */
	public override void Start() {
		base.Start();
		this.SetIsDirtyUpdateState(true);

		// 엔진을 설정한다
		m_oEngine.Init(CE20Engine.MakeParams(m_oPlayUIsInfo.m_oObjs, this, m_oPlayUIsHandler as CE20PlayUIsHandler));

		// UI 처리자를 설정한다 {
		var stGridSize = new Vector2Int(3, 3);

		(m_oPlayUIsHandler as CE20PlayUIsHandler).Init(CE20PlayUIsHandler.MakeParams(this, stGridSize));
		(m_oMatchingUIsHandler as CE20MatchingUIsHandler).Init(CE20MatchingUIsHandler.MakeParams(this));
		// UI 처리자를 설정한다 }

#if UNITY_EDITOR
		CE20NetworkManager.Inst.RunServerSocket();
#endif // #if UNITY_EDITOR
	}

	/** 제거 되었을 경우 */
	public override void OnDestroy() {
		base.OnDestroy();

#if UNITY_EDITOR
		CE20NetworkManager.Inst.StopServerSocket();
#endif // #if UNITY_EDITOR
	}

	/** 상태를 갱신한다 */
	public override void OnUpdate(float a_fDeltaTime) {
		base.OnUpdate(a_fDeltaTime);

		// UI 처리자를 갱신한다
		m_oPlayUIsHandler.OnUpdate(a_fDeltaTime);
		m_oMatchingUIsHandler.OnUpdate(a_fDeltaTime);
	}

	/** 상태를 갱신한다 */
	public override void OnLateUpdate(float a_fDeltaTime) {
		base.OnLateUpdate(a_fDeltaTime);

		// UI 처리자를 갱신한다
		m_oPlayUIsHandler.OnLateUpdate(a_fDeltaTime);
		m_oMatchingUIsHandler.OnLateUpdate(a_fDeltaTime);
	}

	/** 매칭에 성공했을 경우 */
	public void OnMatchingSuccess(int a_nPlayerOrder) {
		m_eState = EState.PLAY;
		m_oEngine.Play(a_nPlayerOrder);

		this.SetIsDirtyUpdateState(true);
	}

	/** 상태를 갱신한다 */
	protected override void UpdateState() {
		base.UpdateState();

		// 객체를 갱신한다 {
		m_oPlayUIsInfo.m_oUIs.SetActive(m_eState == EState.PLAY);
		m_oPlayUIsInfo.m_oObjs.SetActive(m_eState == EState.PLAY);

		m_oMatchingUIsInfo.m_oUIs.SetActive(m_eState == EState.MATCHING);
		m_oMatchingUIsInfo.m_oObjs.SetActive(m_eState == EState.MATCHING);
		// 객체를 갱신한다 }

		m_oPlayUIsHandler.SetIsDirtyUpdateState(true);
		m_oMatchingUIsHandler.SetIsDirtyUpdateState(true);
	}
	#endregion // 함수
}
