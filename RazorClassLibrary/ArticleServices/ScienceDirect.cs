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
    public class ScienceDirect : ServiceBase
    {
        public async Task RunAutomation(string SearchText)
        {
            LoggerService.Log($"ScienceDirect - ******* Start RunAutomation: {SearchText}*******");

            if (string.IsNullOrEmpty(SearchText))
            {
                throw new Exception("Search text is empty");
            }
            string username = "insblr001@outlook.com";
            string password = "Sekhar@123";

            string downloadDirectory = Path.Combine(DownloadRootPath, @"sciencedirect");
            // Ensure the directory exists
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            // Set Edge options for downloading files
            EdgeOptions options = new EdgeOptions();
            //options.AddArgument("--headless=new");
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("safebrowsing.enabled", true);

            // Set up ChromeDriver (you can use other browsers if you prefer)
            IWebDriver driver = new EdgeDriver(options);

            try
            {
                driver.Navigate().GoToUrl(ArticleUrls.ScienceDirect);

                // Maximize the browser window
                driver.Manage().Window.Maximize();

                Task.Delay(5000).Wait();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //IWebElement closeGuideButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='pendo-close-guide-6d2f92bc']")));
                //closeGuideButton.Click();

                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("qs")));
                IWebElement search = driver.FindElement(By.Id("qs"));
                search.SendKeys(SearchText);
                search.SendKeys(Keys.Enter);

                //handle cookie page
                //if cookie page displays close it else skip and proceed next steps
                try
                {
                    IWebElement cookieButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), 'Accept All Cookies')]")));
                    cookieButton.Click();
                }
                catch (NoSuchElementException)
                {
                    // Cookie button not found, continue with the next steps
                }

                // Wait for the search results to load
                IWebElement pageInfoDiv = driver.FindElement(By.XPath("//div[contains(text(), 'Displaying article') and contains(text(), 'on page')]"));

                // Extract and print the text content of the div
                string displayedText = pageInfoDiv.Text;
                Console.WriteLine("Extracted text: " + displayedText);

               
                
                // Pause the program for a few seconds to see the result
                Task.Delay(5000).Wait();
                LoggerService.Log($"ScienceDirect - ******* End RunAutomation: {SearchText}*******");

            }
            catch (Exception ex)
            {
                throw;
                //Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Close the browser after the automation
                driver.Quit();
            }
        }

        
    }
}
