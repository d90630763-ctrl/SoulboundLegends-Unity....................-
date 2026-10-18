using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Authority
{
    public abstract class AuthorityManagedSystem : MonoBehaviour, IAuthoritySystem
    {
        [Header("Master Authority")]
        [SerializeField] private string systemId = "";
        [SerializeField] private List<string> capabilities = new();

        protected MasterAuthorityClient Authority { get; private set; }
        protected virtual string DefaultSystemId => "system.unnamed";
        public string SystemId => string.IsNullOrWhiteSpace(systemId) ? DefaultSystemId : systemId;
        public IEnumerable<string> Capabilities => capabilities;

        protected virtual void Awake() { if (string.IsNullOrWhiteSpace(systemId)) systemId = DefaultSystemId; }
        public virtual void OnAuthorityReady(MasterAuthorityClient authority) => Authority = authority;
        public virtual void OnAuthoritySnapshot(PlayerSnapshot snapshot) { }

        protected IEnumerator Authorize(string capability, string action, Dictionary<string, object> payload, Action<AuthorityResponse> completed)
        {
            if (Authority == null)
            {
                completed?.Invoke(new AuthorityResponse { code = AuthorityResultCode.Unauthorized, systemId = SystemId, action = action, message = "Master Authority is not attached." });
                yield break;
            }
            yield return Authority.Authorize(SystemId, capability, action, payload, completed);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(systemId)) systemId = DefaultSystemId;
        }
#endif
    }
}