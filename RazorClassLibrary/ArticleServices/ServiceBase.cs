using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using SeleniumExtras.WaitHelpers;
using System.Text.RegularExpressions;
using System.Data;
using ClosedXML.Excel;
using System.Threading.Tasks;
using WebScrapeApp.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace WebScrapeApp.ArticleServices
{
    public class ServiceBase
    {
        public string DownloadRootPath = @"C:\CustomDownloads";


    }
    public static class ArticleUrls
    {
        public const string ScienceDirect = "https://www.sciencedirect.com/";
        public const string SageJournals = "https://journals.sagepub.com/";
        public const string mdpi = "https://www.mdpi.com/";
    }
}
