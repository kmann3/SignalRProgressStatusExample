using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR_ProgressStatusExample
{
    public class Progress
    {
        public string status { get; set; }
        public int percent { get; set; }
    }

    public interface IProgressHub
    {
        Task ReportProgress(Progress info);
    }

    public class ProgressHub : Hub<IProgressHub>
    {
        public Task ReportProgress(Progress info)
        {
            return Clients.Client(this.Context.ConnectionId).ReportProgress(info);
        }
    }
}
