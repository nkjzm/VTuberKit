using UnityEngine;
using Live2D.Cubism.Core;

namespace VTuberKit
{
    /// <summary>
    /// 口パクを行うクラス
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SimpleLipSyncer : MonoBehaviour
    {
        AudioSource audioSource = null;

        [SerializeField]
        CubismParameter MouthOpenParameter = null;

        float velocity = 0.0f;
        float currentVolume = 0.0f;

        [SerializeField]
        float Power = 20f;

        [SerializeField, Range(0f, 1f)]
        float Threshold = 0.1f;

        void Start()
        {
            // 空の Audio Sourceを取得
            audioSource = GetComponent<AudioSource>();

            // Audio Source の Audio Clip をマイク入力に設定
            // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数
            audioSource.clip = Microphone.Start(null, true, 1, 44100);
            // マイクが Ready になるまで待機（一瞬）
            while (Microphone.GetPosition(null) <= 0) { }
            // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）
            audioSource.Play();
            audioSource.loop = true;
        }

        private void LateUpdate()
        {
            float targetVolume = GetAveragedVolume() * Power;
            targetVolume = targetVolume < Threshold ? 0 : targetVolume;
            currentVolume = Mathf.SmoothDamp(currentVolume, targetVolume, ref velocity, 0.05f);

            if (MouthOpenParameter == null)
            {
                Debug.LogError("MouthOpenParameterが設定されていません");
                return;
            }
            // CubismParameterの更新はLateUpdate()内で行う必要がある点に注意
            MouthOpenParameter.Value = Mathf.Clamp01(currentVolume);
        }

        float GetAveragedVolume()
        {
            float[] data = new float[256];
            float a = 0;
            audioSource.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 255.0f;
        }
    }
}