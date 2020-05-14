using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task OnGetProcessAsync([FromServices]IHubContext<NotificationHub, INotificationHub> hubContext)
        {

            for (int i = 0; i < 10; i++)
            {
                await hubContext.Clients.All.Notify("hey there! gotcha"+i);
                await Task.Delay(TimeSpan.FromSeconds(1));

            }


        }

        public async Task OnGetStreamAsync([FromServices]IHubContext<NotificationHub> hubContext)
        {
            await hubContext.Clients.All.SendAsync("initiatestream");
        }
    }
}
