using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private ILoadingAsyncInitializable[] _initializables;

    private void Awake()
    {
        ResourceManager.Instance.ReleaseSceneCache();
    }
    private async void Start()
    {
        //TODO : fade out
        await ScreenFader.Instance.FadeOutAsync(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Loading Scene"));

        //이전씬 언로드
        if (!string.IsNullOrEmpty(GameManager.Instance.CurrentScene))
        {
            await SceneManager.UnloadSceneAsync(GameManager.Instance.CurrentScene).ToUniTask();
        }

        //데이터 프리로드
        switch (GameManager.Instance.NextScene)
        {
            case "Stage Scene":
                await ResourceManager.Instance.PreloadStageAsync();
                break;
            case "Home Scene":
                await ResourceManager.Instance.PreloadHomeAsync();
                break;
            case "Title Scene": break;
        }

        //다음씬 로드
        await SceneManager.LoadSceneAsync(GameManager.Instance.NextScene, LoadSceneMode.Additive).ToUniTask();

        Scene targetScene = SceneManager.GetSceneByName(GameManager.Instance.NextScene);
        SceneManager.SetActiveScene(targetScene);

        GameManager.Instance.CurrentScene = GameManager.Instance.NextScene;

        //await 뭐 비동기 초기화 함수
        _initializables = FindObjectsByType<MonoBehaviour>()
        .Where(mb => mb.gameObject.scene == targetScene)
        .OfType<ILoadingAsyncInitializable>()
        .ToArray();

        foreach (var i in _initializables) { i.InitializeOnLoading(); }

        //페이드 인
        await ScreenFader.Instance.FadeInAsync(1f);

        foreach (var i in _initializables) { i.OnReadyToStart(); }

        //로딩 씬 파괴
        await SceneManager.UnloadSceneAsync("Loading Scene").ToUniTask();
    }
}