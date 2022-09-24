using System.Threading.Tasks;
using ScriptBeeWebApp.Data;

namespace ScriptBeeWebApp.Hubs;

public interface IFileWatcherClient
{
    Task ReceiveFileWatch(WatchedFile watchedFile);
}
