using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// =======================================
/// 🔊 AudioManager（ScriptableObject対応版）
/// ---------------------------------------
/// ・SoundData.asset から音声リストを読み込む
/// ・BGM / SE を管理・再生
/// ・プールによる軽量なSE再生
/// ・自動生成シングルトン & シーン常駐
/// =======================================
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // どのシーンにも存在しなければ自動生成
                GameObject obj = new GameObject("AudioManager");
                _instance = obj.AddComponent<AudioManager>();
            }
            return _instance;
        }
    }

    [Header("サウンドデータ（ScriptableObject）")]
    [SerializeField] private SoundData soundData;

    [Header("SEプールサイズ")]
    [SerializeField] private int sePoolSize = 10;

    private AudioSource bgmSource;
    private List<AudioSource> sePool = new();

    private bool initialized = false;

    private void Awake()
    {
        // 重複生成防止
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (!initialized)
            Initialize();
    }

    /// <summary>
    /// 初期化処理（BGMとSEプール生成）
    /// </summary>
    private void Initialize()
    {
        initialized = true;

        // SoundData が未設定なら Resources から読み込む（保険）
        if (soundData == null)
        {
            soundData = Resources.Load<SoundData>("SoundData");
            if (soundData == null)
            {
                Debug.LogError("SoundData.asset が見つかりません。Resourcesフォルダに配置してください。");
                return;
            }
        }

        // BGM AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = (float)PlayerPrefs.GetInt("bgmVolume",10) * 0.1f;

        // SE AudioSource プール
        for (int i = 0; i < sePoolSize; i++)
        {
            AudioSource seSource = gameObject.AddComponent<AudioSource>();
            seSource.playOnAwake = false;
            seSource.volume = (float)PlayerPrefs.GetInt("seVolume",10) * 0.1f;
            sePool.Add(seSource);
        }
    }

    // ===============================
    // 🎵 BGM制御
    // ===============================
    public void PlayBGM(int index, float volume = -1f)
    {
        if (soundData == null || soundData.bgmClips == null) return;
        if (index < 0 || index >= soundData.bgmClips.Length) return;

        bgmSource.clip = soundData.bgmClips[index];
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ===============================
    // 🔊 SE制御（プール利用）
    // ===============================
    public void PlaySE(int index, float volume = -1f)
    {
        if (soundData == null || soundData.seClips == null) return;
        if (index < 0 || index >= soundData.seClips.Length) return;

        AudioSource source = GetAvailableSESource();
        source.clip = soundData.seClips[index];
        source.Play();
    }

    private AudioSource GetAvailableSESource()
    {
        foreach (var source in sePool)
        {
            if (!source.isPlaying)
                return source;
        }
        return sePool[0]; // 全部使用中なら再利用
    }

    // ===============================
    // 🎚️ 音量調整
    // ===============================
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSEVolume(float volume)
    {
        foreach (var s in sePool)
            s.volume = volume;
    }
}
