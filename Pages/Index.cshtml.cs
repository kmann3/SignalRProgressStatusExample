﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SignalR_ProgressStatusExample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        IHubContext<ProgressHub, IProgressHub> _progressHubContext;

        public IndexModel(ILogger<IndexModel> logger, IHubContext<ProgressHub, IProgressHub> progressHubContext)
        {
            _logger = logger;
            _progressHubContext = progressHubContext;
        }

        [BindProperty]
        public string ConnectionId { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Click the Progress button to see some async progress reporting, click the About menu item for an explanation and source code link";
        }

        public void OnPost()
        {
            // 10% report is done in js code...
            Thread.Sleep(1000);
            ReportAndSleep("Relaxing Splines", 20, ConnectionId, 1000);
            ReportAndSleep("Reset", 0, ConnectionId, 0);
        }

        private void ReportAndSleep(string message, int _pct, string connectionId, int sleepFor)
        {
            var info = new ProgressInfo() { message = message, pct = _pct };
            _progressHubContext.Clients.Client(connectionId).ReportProgress(info);
            Thread.Sleep(sleepFor);
        }
    }
}
