using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SoulboundLegends.InGameCommerce
{
    public sealed class InGameCommerceClient : MonoBehaviour
    {
        [Header("Server")]
        [SerializeField] private string oracleApiBaseUrl = "https://YOUR-ORACLE-SERVER";
        [SerializeField] private string sessionToken;
        [Header("Security")]
        [SerializeField] private bool requireHttps = true;

        public string OracleApiBaseUrl => oracleApiBaseUrl;
        public void SetSessionToken(string token) => sessionToken = token;

        public void BuyShopItem(string merchantId, string productId, int quantity, Action<CommerceResponse> callback)
        {
            var req = new ShopPurchaseRequest { merchantId = merchantId, productId = productId, quantity = quantity, idempotencyKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") };
            StartCoroutine(Post("/api/commerce/shop/buy", JsonUtility.ToJson(req), callback));
        }

        public void CreateAuction(string itemInstanceId, long startingBid, long buyoutPrice, int durationSeconds, Action<CommerceResponse> callback)
        {
            var req = new AuctionListRequest { itemInstanceId = itemInstanceId, startingBid = startingBid, buyoutPrice = buyoutPrice, durationSeconds = durationSeconds, idempotencyKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") };
            StartCoroutine(Post("/api/commerce/auction/list", JsonUtility.ToJson(req), callback));
        }

        public void PlaceBid(string auctionId, long bidAmount, Action<CommerceResponse> callback)
        {
            var req = new AuctionBidRequest { auctionId = auctionId, bidAmount = bidAmount, idempotencyKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") };
            StartCoroutine(Post("/api/commerce/auction/bid", JsonUtility.ToJson(req), callback));
        }

        public void FinalizeAuction(string auctionId, Action<CommerceResponse> callback)
        {
            var req = new AuctionFinalizeRequest { auctionId = auctionId, idempotencyKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") };
            StartCoroutine(Post("/api/commerce/auction/finalize", JsonUtility.ToJson(req), callback));
        }

        private IEnumerator Post(string path, string json, Action<CommerceResponse> callback)
        {
            if (string.IsNullOrWhiteSpace(oracleApiBaseUrl)) { callback?.Invoke(new CommerceResponse { success = false, error = "Server URL is missing." }); yield break; }
            if (requireHttps && !oracleApiBaseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) { callback?.Invoke(new CommerceResponse { success = false, error = "HTTPS is required." }); yield break; }
            using var request = new UnityWebRequest(oracleApiBaseUrl.TrimEnd('/') + path, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrWhiteSpace(sessionToken)) request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
            yield return request.SendWebRequest();
            callback?.Invoke(new CommerceResponse { success = request.result == UnityWebRequest.Result.Success, error = request.error, rawJson = request.downloadHandler?.text });
        }
    }
}
