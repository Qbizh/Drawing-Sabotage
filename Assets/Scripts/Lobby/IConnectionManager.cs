
using System.Threading.Tasks;

public interface IConnectionManager
{
    public void Init();
    public Task<string> CreateLobby();
    public Task<bool> JoinLobby(string joinCode);
}
