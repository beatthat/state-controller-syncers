using UnityEngine;
using UnityEngine.Events;
using BeatThat;

namespace BeatThat
{
	/// <summary>
	/// Binds a StateController to another state controller so that any param set in the parent is synched locally.
	/// </summary>
	public class SyncStateToParent : BindingBehaviour
	{
		public bool m_debug;
		[Tooltip("set params in this list if you want to sync ONLY a defined set of params")]
		public string[] m_includeParams;

		[Tooltip("set params in this list if you want to sync ALL params EXCEPT a defined set of params")]
		public string[] m_excludeParams;

		private bool ShouldSyncParam(StateParamUpdate p)
		{
			if(m_includeParams != null && m_includeParams.Length > 0) {
				foreach(var n in m_includeParams) {
					if(n == p.name) {
						return true;
					}
				}
				return false;
			}

			if(m_excludeParams != null && m_excludeParams.Length > 0) {
				foreach(var n in m_excludeParams) {
					if(n == p.name) {
						return false;
					}
				}
			}

			return true;
		}

		private void SyncParam(StateParamUpdate p)
		{
			if(!ShouldSyncParam(p)) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] ignoring param: " + p);
				}
				#endif
				return;
			}
				
			switch(p.type) {
			case StateParamUpdateType.Float:
				var f = this.state.GetFloat(p.name);

				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync parm to parent: " + p + " -> " + f);
				}
				#endif

				this.syncTo.SetFloat(p.name, f, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.Int:
				var i = this.state.GetInt(p.name);

				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync parm to parent: " + p + " -> " + i);
				}
				#endif

				this.syncTo.SetInt(p.name, i, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.Bool:
				var b = this.state.GetBool(p.name);
				
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync parm to parent: " + p + " -> " + b);
				}
				#endif

				this.syncTo.SetBool(p.name, b, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.TriggerSet:
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync parm to parent: " + p + " -> INVOKE");
				}
				#endif

				this.syncTo.SetTrigger(p.name, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;
			}
		}

		private UnityAction<StateParamUpdate> paramUpdatedAction { get { return m_paramUpdatedAction?? (m_paramUpdatedAction = this.SyncParam); } } 
		private UnityAction<StateParamUpdate> m_paramUpdatedAction;

		override protected void BindAll()
		{
			var fromState = this.state;
			if(fromState == null) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "::BindPresenter no parent state to bind to");
				return;
			}
			Bind(fromState.paramUpdated, this.paramUpdatedAction);
		}

		override protected void UnbindAll()
		{
			var sync = this.syncTo;
			if(sync == null) {
				return;
			}

			m_syncTo.value = null;
		}


		void OnEnable()
		{
			if(!this.didStart) {
				return;
			}
			Bind();
		}

		void OnDisable()
		{
			Unbind();
		}

		private bool didStart { get; set; }
		void Start()
		{
			this.didStart = true;
			Bind();
		}

		private StateController syncTo 
		{ 
			get { 
				var ctl = m_syncTo.value;
				if(ctl != null) {
					return ctl;
				}
				if(this.transform.parent == null) {
					return null;
				}
				m_syncTo = new SafeRef<StateController>(this.transform.parent.GetComponent<StateController>());
				return m_syncTo.value;
			}
		}
		private SafeRef<StateController> m_syncTo;

		private StateController state { get { return m_state?? (m_state = GetComponent<StateController>()); } }
		private StateController m_state;
	}
}