using BeatThat.SafeRefs;
using BeatThat.TransformPathExt;
using BeatThat.Pools;
using BeatThat.Controllers;
using System.Collections;
using System.Collections.Generic;
using BeatThat.Bindings;
using BeatThat.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace BeatThat.StateControllers
{
    /// <summary>
    /// Binds a StateController to another state controller so that any param set in the parent is synched locally.
    /// </summary>
    public class SyncStateFromParent : BindingBehaviour
	{
		public bool m_debug;

		[Tooltip("set params in this list if you want to sync ONLY a defined set of params")]
		public string[] m_includeParams;

		[Tooltip("set params in this list if you want to sync ALL params EXCEPT a defined set of params")]
		public string[] m_excludeParams;
			
		private bool ShouldSyncParam(string param)
		{
			if(m_includeParams != null && m_includeParams.Length > 0) {
				foreach(var n in m_includeParams) {
					if(n == param) {
						return true;
					}
				}
				return false;
			}

			if(m_excludeParams != null && m_excludeParams.Length > 0) {
				foreach(var n in m_excludeParams) {
					if(n == param) {
						return false;
					}
				}
			}

			return true;
		}

		private void SyncParam(StateParam p)
		{
			//			if(m_debug) {
			//				Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p);
			//			}

			if(!ShouldSyncParam(p.name)) {
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] ignoring param: " + p);
				}
				return;
			}

			switch(p.type) {
			case StateParamType.Float:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetFloat(p.name));
				}

				this.state.SetFloat(p.name, this.syncFrom.GetFloat(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;
			case StateParamType.Int:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetInt(p.name));
				}

				this.state.SetInt(p.name, this.syncFrom.GetInt(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;
			case StateParamType.Bool:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetBool(p.name));
				}

				this.state.SetBool(p.name, this.syncFrom.GetBool(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;
			case StateParamType.Trigger:
				if(this.syncFrom.GetBool(p.name)) {
					if(m_debug) {
						Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
							+ " (SET trigger) ");
					}


					this.state.SetTrigger(p.name, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				}
				else {
					if(m_debug) {
						Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
							+ " (CLEAR trigger) ");
					}

					this.state.SetBool(p.name, false, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				}
				break;
			}
		}

		private void SyncParam(StateParamUpdate p)
		{
//			if(m_debug) {
//				Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p);
//			}

			if(!ShouldSyncParam(p.name)) {
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] ignoring param: " + p);
				}
				return;
			}

			switch(p.type) {
			case StateParamUpdateType.Float:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetFloat(p.name));
				}

				this.state.SetFloat(p.name, this.syncFrom.GetFloat(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.Int:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetInt(p.name));
				}

				this.state.SetInt(p.name, this.syncFrom.GetInt(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.Bool:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " to value " + this.syncFrom.GetBool(p.name));
				}

				this.state.SetBool(p.name, this.syncFrom.GetBool(p.name), PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.TriggerSet:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " (SET trigger) ");
				}
					
				this.state.SetTrigger(p.name, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;

			case StateParamUpdateType.TriggerClear:
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] [" + this.Path() + "] sync param from parent: " + p 
						+ " (CLEAR trigger) ");
				}

				this.state.SetBool(p.name, false, PropertyEventOptions.SendOnChange, StateParamOptions.DontRequireParam); // TODO: maybe better to require and have an ignore list
				break;
			}
		}

		private UnityAction<StateParamUpdate> paramUpdatedAction { get { return m_paramUpdatedAction?? (m_paramUpdatedAction = this.SyncParam); } } 
		private UnityAction<StateParamUpdate> m_paramUpdatedAction;

		private static bool ContainsParam(string paramName, IList<Param> paramList)
		{
			foreach(var p in paramList) {
				if(p.param == paramName) {
					return true;
				}
			}
			return false;

		}

		private void SyncNonLocal()
		{
			if (this.syncFrom == null) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogWarning("[" + Time.frameCount + "] SyncStateFromParent on " + this.Path() + " has no parent to sync from");
				#endif
				return;
			}

			if(!this.state.isReady || !this.syncFrom.isReady) {
				StopAllCoroutines();
				StartCoroutine(SyncWhenReady());
				return;
			}

			using(var syncFromParams = ListPool<StateParam>.Get()) {
				this.syncFrom.GetParams(syncFromParams);
				using(var localParams = ListPool<Param>.Get()) {
					GetComponents<Param>(localParams);

					foreach(var p in syncFromParams) {
						if(ContainsParam(p.name, localParams)) {
							continue;
						}

						SyncParam(p);
					}
				}
			}
		}

		private IEnumerator SyncWhenReady()
		{
			while(this.state.isReady || !this.syncFrom.isReady) {
				yield return new WaitForEndOfFrame();
			}

			SyncNonLocal();
		}
	
		override protected void BindAll()
		{
			var fromState = this.syncFrom;
			if(fromState == null) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "::BindPresenter no parent state to bind to");
				return;
			}

			Bind(fromState.paramUpdated, this.paramUpdatedAction);
		}

		override protected void UnbindAll()
		{
			m_syncFrom.value = null;
		}

		private bool didStart { get; set; }
		void Start()
		{
			this.didStart = true;
			Activate();
		}

		void OnEnable()
		{
			if(!this.didStart) {
				return;
			}

			Activate();
		}

		void OnDisable()
		{
			Unbind();
		}

		private void Activate()
		{
			Bind();
			SyncNonLocal();
		}

		private StateController syncFrom 
		{ 
			get { 
				var ctl = m_syncFrom.value;
				if(ctl != null) {
					return ctl;
				}
				if(this.transform.parent == null) {
					return null;
				}
				m_syncFrom = new SafeRef<StateController>(this.transform.parent.GetComponentInParent<StateController>());
				return m_syncFrom.value;
			}
		}

		private SafeRef<StateController> m_syncFrom;

		private StateController state { get { return m_state?? (m_state = GetComponent<StateController>()); } }
		private StateController m_state;
	}
}




