using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public interface ILoadingAsyncInitializable
{
    void InitializeOnLoading();
    void OnReadyToStart();
}