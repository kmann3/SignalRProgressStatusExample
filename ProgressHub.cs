using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR_ProgressStatusExample
{
    public class ProgressInfo
    {
        public string Status { get; set; }
        public int Percent { get; set; }
    }

    public interface IProgressHub
    {
        Task ReportProgress(ProgressInfo info);
    }

    public class ProgressHub : Hub<IProgressHub>
    {
        public Task ReportProgress(ProgressInfo info)
        {
            return Clients.Client(this.Context.ConnectionId).ReportProgress(info);
        }
    }
}
