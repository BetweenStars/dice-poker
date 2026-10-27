using System.Threading.Tasks;

public interface IBootstrapInitializable
{
    Task InitializeOnBootstrap();
}