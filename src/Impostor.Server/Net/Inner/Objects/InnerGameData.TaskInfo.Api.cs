using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Server.Events.Player;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            public async ValueTask SetCompleteAsync()
            {
                if (Complete)
                {
                    return;
                }

                Complete = true;

                using var writer = _game.StartRpc(_player.NetId, RpcCalls.CompleteTask);
                writer.Write(Id);
                await _game.FinishRpcAsync(writer);

                // Notify plugins.
                Api.Net.IClientPlayer? target = _game.GetClientPlayer(_player.OwnerId);
                if (target == null)
                {
                    return;
                }

                await _eventManager.CallAsync(new PlayerCompletedTaskEvent(_game, target, _player, this));
            }
        }
    }
}
