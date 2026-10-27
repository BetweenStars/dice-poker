using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    private async void Start()
    {
        await Addressables.InitializeAsync().ToUniTask();

        DataManager.Instance.LoadAllSettings();
        await ResourceManager.Instance.PreloadOnBootstrap();

        Debug.Log("BootStrap Initialize Async Complete");

        await SceneManager.LoadSceneAsync("Title Scene", LoadSceneMode.Additive).ToUniTask();
        GameManager.Instance.CurrentScene = "Title Scene";

        Destroy(gameObject);
    }
}
