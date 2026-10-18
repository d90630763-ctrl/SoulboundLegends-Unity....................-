using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SoulboundLegends.Commerce
{
    public sealed class CommerceBackendClient
    {
        private readonly string baseUrl;
        private readonly Func<string> accessTokenProvider;

        public CommerceBackendClient(string baseUrl, Func<string> accessTokenProvider)
        {
            this.baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');
            this.accessTokenProvider = accessTokenProvider;
        }

        public IEnumerator VerifyGooglePurchase(GooglePurchaseEvidence evidence, Action<CommerceVerifyResult> completed)
        {
            string json = JsonUtility.ToJson(evidence);
            using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/api/commerce/googleplay/verify", "POST"))
            {
                byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                string token = accessTokenProvider == null ? null : accessTokenProvider();
                if (!string.IsNullOrEmpty(token))
                    request.SetRequestHeader("Authorization", "Bearer " + token);

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    completed?.Invoke(new CommerceVerifyResult
                    {
                        accepted = false,
                        message = request.error
                    });
                    yield break;
                }

                CommerceVerifyResult result = JsonUtility.FromJson<CommerceVerifyResult>(request.downloadHandler.text);
                completed?.Invoke(result ?? new CommerceVerifyResult { accepted = false, message = "Invalid server response." });
            }
        }
    }
}
