using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #region all-time-loaded
    private Dictionary<string, AudioClip> _sfxCache = new();
    private Dictionary<CharacterId, CharacterAsset> _characterCache = new();
    private Dictionary<string, EpisodeAsset> _episodeCache = new();


    private AsyncOperationHandle<IList<AudioClip>> _sfxHandle = new();
    private AsyncOperationHandle<IList<CharacterAsset>> _characterHandle = new();
    private AsyncOperationHandle<IList<EpisodeAsset>> _episodeHandle = new();
    #endregion

    #region temp-loaded
    private AsyncOperationHandle<AudioClip> _playingBGMHandle = new();

    private Dictionary<string, Sprite> _spriteCache = new();
    private Dictionary<string, AsyncOperationHandle<Sprite>> _spriteHandles = new();
    #endregion

    #region preload-bootstrap
    private async UniTask PreloadSFX()
    {
        _sfxHandle = Addressables.LoadAssetsAsync<AudioClip>(
            "SFX",
            clip =>
            {
                _sfxCache[clip.name] = clip;
            });

        await _sfxHandle.Task;
    }
    private async UniTask PreloadCharacter()
    {
        _characterHandle = Addressables.LoadAssetsAsync<CharacterAsset>(
        "Character",
        asset =>
        {
            _characterCache[asset.id] = asset;
        });

        await _characterHandle.Task;
    }
    private async UniTask PreloadEpisode()
    {
        _episodeHandle = Addressables.LoadAssetsAsync<EpisodeAsset>(
        "Episode",
        asset =>
        {
            _episodeCache[asset.Key] = asset;
        });

        await _episodeHandle.Task;
    }

    public async Task PreloadOnBootstrap()
    {
        await UniTask.WhenAll(
            PreloadSFX(),
            PreloadCharacter(),
            PreloadEpisode()
        );

        Debug.Log("Resource Manager Initialize Complete");
    }
    #endregion

    #region preload-stage
    private async UniTask PreloadBgImg()
    {
        string key = DataManager.Instance.Progress.Key;

        if (!_episodeCache.TryGetValue(key, out var episode))
        {
            Debug.LogWarning($"Episode Cache Not Found : {key}");
            return;
        }

        //Debug.Log(episode);

        var handle = episode.backgroundSprite.LoadAssetAsync<Sprite>();
        Sprite sprite = await handle.Task;

        //Debug.Log(sprite.name);

        _spriteHandles["background"] = handle;
        _spriteCache["background"] = sprite;
    }
    private async UniTask PreloadCharacterSprites()
    {
        string key = DataManager.Instance.Progress.Key;

        if (!_episodeCache.TryGetValue(key, out var episode))
        {
            Debug.LogWarning($"Episode Cache Not Found : {key}");
            return;
        }

        HashSet<CharacterId> charIds = new();

        foreach (var spNode in episode.spriteNodes)
        {
            charIds.Add(spNode.charId);
        }

        foreach (var charId in charIds)
        {
            foreach (var portraits in _characterCache[charId].PortraitSprites)
            {
                var handle = portraits.sprite.LoadAssetAsync<Sprite>();
                var portrait = await handle.Task;

                string saveKey = $"{charId}_{portraits.id}";

                _spriteHandles[saveKey] = handle;
                _spriteCache[saveKey] = portrait;
            }
        }
    }
    public async UniTask PreloadStageAsync()
    {
        //Debug.Log("Preload on loading start");
        try
        {
            await UniTask.WhenAll(
                PreloadCharacterSprites(),
                PreloadBgImg()
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Preload on loading errer : {e}");
            return;
        }

        Debug.Log("[Resource Manager] Preload Stage Resource Complete");
    }
    #endregion

    #region preload-home
    private List<BGMDefinition> _playlistCache = new();
    private AsyncOperationHandle<IList<BGMDefinition>> _playlistHandle = new();

    public async UniTask PreloadHomeBgm()
    {
        var labels = new List<object> { "BGM", "Home" };

        _playlistCache.Clear();

        _playlistHandle = Addressables.LoadAssetsAsync<BGMDefinition>(
        labels,
        data => _playlistCache.Add(data),
        Addressables.MergeMode.Intersection);

        await _playlistHandle.Task;
    }
    public async UniTask PreloadHomeAsync()
    {
        try
        {
            await UniTask.WhenAll(
                PreloadHomeBgm()
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Preload Home Resources Error : {e}");
            return;
        }

        Debug.Log("[Resource Manager] Preload Home Resouce Complete.");
    }
    #endregion

    #region load-on-scene
    public async UniTask<AudioClip> LoadBGMClip(AssetReferenceT<AudioClip> clipRef)
    {
        if (_playingBGMHandle.IsValid())
        {
            Addressables.Release(_playingBGMHandle);
        }

        _playingBGMHandle = clipRef.LoadAssetAsync<AudioClip>();

        await _playingBGMHandle.Task;

        return _playingBGMHandle.Result;
    }
    #endregion

    #region release
    private void ReleaseSprite()
    {
        foreach (var handle in _spriteHandles)
        {
            Addressables.Release(handle.Value);
        }

        _spriteHandles.Clear();
        _spriteCache.Clear();
    }
    private void ReleaseBGMDefinition()
    {
        if (_playingBGMHandle.IsValid()) { Addressables.Release(_playingBGMHandle); }

        AudioManager.Instance.StopBGM();

        //home 브금 언로드
        if (_playlistHandle.IsValid()) { Addressables.Release(_playlistHandle); }
        _playlistCache.Clear();
    }
    public void ReleaseSceneCache()
    {
        ReleaseSprite();
        ReleaseBGMDefinition();
    }
    #endregion

    #region get
    public List<BGMDefinition> GetPlaylist() => _playlistCache;

    public Sprite GetBackGroundSprite()
    {
        if (!_spriteCache.TryGetValue("background", out var sprite))
        {
            Debug.LogWarning("Background sprite not found at cache");
            return null;
        }

        return sprite;
    }
    public EpisodeAsset GetEpisodeAsset(string key)
    {
        return _episodeCache[key];
    }

    public AudioClip GetSFXClip(string key)
    {
        return _sfxCache[key];
    }

    public Sprite GetCharacterPortraitSprite(CharacterId charId, string spriteId)
    {
        string key = $"{charId}_{spriteId}";

        if (_spriteCache.TryGetValue(key, out var sprite)) return sprite;

        return null;
    }

    public CharacterAsset GetCharacterAsset(CharacterId id)
    {
        return _characterCache[id];
    }
    #endregion
}
