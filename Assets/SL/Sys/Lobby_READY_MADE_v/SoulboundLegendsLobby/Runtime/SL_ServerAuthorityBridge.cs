using System;
using System.Collections;
using UnityEngine;

namespace SoulboundLegends.Lobby
{
    public abstract class SL_ServerAuthorityBridge : MonoBehaviour
    {
        public virtual bool IsConnected => false;
        public virtual IEnumerator ValidateAccount(Action<bool> result) { result?.Invoke(IsConnected); yield break; }
        public virtual IEnumerator ValidateName(string requestedName, Action<SL_NameValidationResult> result)
        {
            result?.Invoke(new SL_NameValidationResult { accepted = !string.IsNullOrWhiteSpace(requestedName), message = "Local fallback validation." });
            yield break;
        }
        public virtual IEnumerator ValidateUnlock(string requirementID, int level, Action<bool> result) { result?.Invoke(true); yield break; }
        public virtual IEnumerator SavePlayerIdentity(string playerName, Action<bool> result) { result?.Invoke(true); yield break; }
        public virtual IEnumerator ValidatePurchase(string itemID, Action<bool> result) { result?.Invoke(false); yield break; }
        public virtual IEnumerator ValidateDraw(string drawID, Action<bool> result) { result?.Invoke(false); yield break; }
    }

    [Serializable]
    public struct SL_NameValidationResult
    {
        public bool accepted;
        public bool taken;
        public string message;
    }

    public sealed class SL_LocalServerBridge : SL_ServerAuthorityBridge
    {
        public override bool IsConnected => true;
        public override IEnumerator ValidateAccount(Action<bool> result) { yield return null; result?.Invoke(true); }
        public override IEnumerator ValidateName(string requestedName, Action<SL_NameValidationResult> result)
        {
            yield return null;
            result?.Invoke(new SL_NameValidationResult { accepted = !string.IsNullOrWhiteSpace(requestedName), taken = false, message = "Local test server accepted the name." });
        }
        public override IEnumerator ValidateUnlock(string requirementID, int level, Action<bool> result) { yield return null; result?.Invoke(true); }
        public override IEnumerator SavePlayerIdentity(string playerName, Action<bool> result) { yield return null; result?.Invoke(true); }
    }
}
