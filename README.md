# SignalRProgressStatusExample
This is an example for how to use SignalR to communicate a status and progress of an event.
The goal is a.) to use modern SignalR procedures and b.) be as simple as possible.

In this example we'll have a fake "migration" project where we'll show the use a progress bar to show how far, overall, the migration has left as well as a status label to show what action is currently being taken.

Most of this was taken and updated from: https://github.com/endintiers/SignalR-AspNetCore-ProgressDemo/blob/master/AsyncPOST.md

## Begin

- Create a ASP NET Core 3.1 project. Name it something, such as "MyExampleProject". Go with all defaults.
- Install Signal by going to the Package Manager and running the following command: -- IS THIS EVEN NEEDED? (TBI)
  ```CSharp
  Install-Package Microsoft.AspNet.SignalR
  ```
  - OR you can use the Nuget manager and search for "".
  
- Create a class file named "ProgressHub.cs" in the root of your project (right-click on the project -> Add New -> Class Project).
  - Insert the following code:
  ```CSharp
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
    ```
  - Note: There's two classes here in the namespace and an interface. 
- Now add the following to Startup.Ccs
```CSharp
public void ConfigureServices(IServiceCollection services)
{
  //... 
  services.AddSignalR();
}
```
- Now add the following endpoint / route in the same file inside of the Configure(IApplicationBuilder app, IHostingEnvironment env) method:
```CSharp
app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapHub<ProgressHub>("/myhub"); // Add this line
        });
  ```
- Now go to wwroot older and expand it. Right-click on lib and select "Add" and then select "Client-Side Library..." and type in "microsoft-signalr" using the provider "unpkg". It will likely have some version info that goes along with it such as.. "@microsoft/signalr@latest" or something like that. We now open Pages/Shared/_Layout.cshtml and add a reference to this new package. The easiest way to do this is to open the _Layout.cshtml file and drag the wwwroot/lib/microsoft-signalr/signalr.js in it in the script section.
- Now we open /Index.cshtml and add the following into it at the bottom:
```JavaScript
@section Scripts {
    <script>
        var connection = new signalR.HubConnectionBuilder().withUrl("/myhub").build();
        connection.on("reportprogress", info => {
            console.log(info.message + ' - ' + info.pct + '%');
            reportProgress(info);
        });
        connection.start();

        function reportProgress(info) {
            if (info.pct < 1 || info.message.toLowerCase() == 'reset') {
                $('#pbar').css('width', '0%')
                    .attr('aria-valuenow', 0).text('');
                $('#progressButton').button('reset');
            }
            else {
                $('#pbar').css('width', info.pct + '%')
                    .attr('aria-valuenow', info.pct).text(info.message);
                $('#progressButton').button('loading');
            }
        }

        function getConnectionIdAndReportStart() {
            $('#connectionId').val(connection.connectionId);
            var info = { message: 'Starting Out', pct: 10 };
            connection.invoke('reportprogress', info);
        }
    </script>
}
```
- 