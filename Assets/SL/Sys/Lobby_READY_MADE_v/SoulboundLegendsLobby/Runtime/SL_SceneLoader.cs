using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulboundLegends.Lobby
{
    public class SL_SceneLoader : MonoBehaviour
    {
        public IEnumerator Load(string sceneName, Action<float> progress, Action completed)
        {
            if (string.IsNullOrEmpty(sceneName)) { completed?.Invoke(); yield break; }
            AsyncOperation op;
            try { op = SceneManager.LoadSceneAsync(sceneName); }
            catch (Exception e) { Debug.LogError(e); completed?.Invoke(); yield break; }
            while (!op.isDone)
            {
                progress?.Invoke(Mathf.Clamp01(op.progress / 0.9f));
                yield return null;
            }
            progress?.Invoke(1f);
            completed?.Invoke();
        }
    }
}
