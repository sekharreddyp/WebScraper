using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using RazorClassLibrary.Components;
using WebScrapeApp.Services;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //var Services = new ServiceCollection();
            //Services.AddWindowsFormsBlazorWebView();
            //Services.AddBlazorWebView();
            //Services.AddInteractiveServerComponents();
            //Services.AddScoped<WebScrapeService>();
            //Services.AddScoped<SeleniumService>();

            InitializeComponent();

            //blazorWebView1.HostPage = @"wwwroot/index.html";
            //blazorWebView1.Services = Services.BuildServiceProvider();
            //blazorWebView1.RootComponents.Add<App>("#app");

        }
    }
}
