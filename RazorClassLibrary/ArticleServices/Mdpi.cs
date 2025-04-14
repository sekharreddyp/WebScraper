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
    public class Mdpi
    {
        public async Task RunAutomation(string SearchText, string fromYear = null, string toYear = null, string username = null, string password = null)
        {
            LoggerService.Log($"MDPI - ******* Start RunAutomation: {SearchText}*******");

            if (string.IsNullOrEmpty(SearchText))
            {
                throw new Exception("Search text is empty");
            }
            if (string.IsNullOrEmpty(username))
            {
                username = "insblr001@outlook.com";
            }
            if (string.IsNullOrEmpty(password))
            {
                password = "Sekhar@123";
            }
            string downloadDirectory = @"C:\CustomDownloads\mdpi";
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
                // Navigate to the SAGE Journals website
                driver.Navigate().GoToUrl("https://www.mdpi.com/");

                // Maximize the browser window
                driver.Manage().Window.Maximize();

                Task.Delay(5000).Wait();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"container\"]/section/header/div[1]/div/div[3]/div/a[1]")));

                IWebElement signin = driver.FindElement(By.XPath("//*[@id=\"container\"]/section/header/div[1]/div/div[3]/div/a[1]"));
                signin.Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));

                //UA_SignInUpButton
                //IWebElement signin = driver.FindElement(By.ClassName("UA_SignInUpButton"));
                //signin.Click();
                //Task.Delay(1000).Wait();

                IWebElement email = driver.FindElement(By.Id("username"));
                email.SendKeys(username);
                IWebElement pass = driver.FindElement(By.Id("password"));
                pass.SendKeys(password);
                //IWebElement login = driver.FindElement(By.XPath("//button[contains(text(), 'Continue')]"));
                pass.SendKeys(Keys.Enter);

                wait.Until(d => d.FindElement(By.Id("q")));

                IWebElement search = driver.FindElement(By.Id("q"));
                search.SendKeys(SearchText);
                search.SendKeys(Keys.Enter);

                //IWebElement resultsPerPageDropdown = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"selectView\"]/div/div[3]/div[2]/div")));
                //resultsPerPageDropdown.Click();
                //Task.Delay(1000).Wait(); // Wait for the dropdown to open
                //// Find the last option in the dropdown
                //IWebElement lastOption200 = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"selectView\"]/div/div[3]/div[2]/div/div/ul/li[last()]")));
                //lastOption200.Click();

                if (!string.IsNullOrEmpty(fromYear) && !string.IsNullOrEmpty(toYear))
                {
                    IWebElement fromYearElement = driver.FindElement(By.XPath("//*[@id=\"refine_year_from\"]"));
                    fromYearElement.SendKeys(Keys.Control + "a");
                    fromYearElement.SendKeys(Keys.Delete);
                    fromYearElement.SendKeys(fromYear);
                    IWebElement toYearElement = driver.FindElement(By.XPath("//*[@id=\"refine_year_to\"]"));
                    toYearElement.SendKeys(Keys.Control + "a");
                    toYearElement.SendKeys(Keys.Delete);
                    toYearElement.SendKeys(toYear);
                    IWebElement searchButton = driver.FindElement(By.XPath("//*[@id=\"formRefine\"]/div[2]/div[2]/div[7]/a"));
                    searchButton.Click();
                    //Task.Delay(2000).Wait();
                }
                // Wait for the search results to load
                IWebElement pageInfoDiv = driver.FindElement(By.XPath("//div[contains(text(), 'Displaying article') and contains(text(), 'on page')]"));

                // Extract and print the text content of the div
                string displayedText = pageInfoDiv.Text;
                Console.WriteLine("Extracted text: " + displayedText);

                // Regular expression to extract the total number of pages
                Regex regex = new Regex(@"page (\d+) of (\d+)");
                Match match = regex.Match(displayedText);
                var totalPages = 0;
                if (match.Success)
                {
                    // Extract the total number of pages (the second captured group)
                    string totalPages1 = match.Groups[2].Value;
                    totalPages = int.Parse(totalPages1);

                    // Output the total number of pages
                    Console.WriteLine("Total pages: " + totalPages1);
                }
                LoggerService.Log($"MDPI - Total pages: {totalPages}");
                for (int i = 1; i <= totalPages; i++)
                {
                    try
                    {
                        IWebElement export = driver.FindElement(By.ClassName("export-options-show"));
                        export.Click();

                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        int previousHeight = 0;

                        while (true)
                        {
                            // Scroll down by a certain amount
                            js.ExecuteScript("window.scrollBy(0, 1000);");
                            Task.Delay(2000).Wait(); // Wait for the page to load new data

                            // Get the current scroll height
                            int currentHeight = (int)(long)js.ExecuteScript("return document.body.scrollHeight;");

                            // Check if the scroll height has not changed
                            if (currentHeight == previousHeight)
                            {
                                Console.WriteLine("No more data to load.");
                                break; // Exit the loop if no more data is loaded
                            }

                            previousHeight = currentHeight;
                        }

                        // Loop through all checkbox groups
                        var checkboxGroups = driver.FindElements(By.ClassName("article-list-checkbox"));
                        LoggerService.Log($"MDPI - Page: {i} Data {checkboxGroups.Count()}");

                        // Select all checkboxes
                        IWebElement selectCheckbox = driver.FindElement(By.Id("selectUnselectAll"));
                        selectCheckbox.Click();

                        IWebElement dropdown = driver.FindElement(By.XPath("//*[@id=\"exportArticles\"]/div/div[1]/div[2]/div/div[2]/div[2]/div/div"));
                        dropdown.Click(); // Open the dropdown

                        // Wait for the options to be visible and locate the option
                        IWebElement option = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"exportArticles\"]/div/div[1]/div[2]/div/div[2]/div[2]/div/div/div/ul/li[6]")));
                        option.Click();
                        //Task.Delay(3000).Wait();

                        IWebElement exportButton = driver.FindElement(By.Id("articleBrowserExport_top"));
                        exportButton.Click();
                        //Task.Delay(3000).Wait();

                        if (i < totalPages)
                        {
                            if (totalPages > 10)
                            {
                                IWebElement pagenumber = driver.FindElement(By.Id("pager-page-number"));
                                pagenumber.Clear();
                                pagenumber.SendKeys((i + 1).ToString());
                                pagenumber.SendKeys(Keys.Enter);
                            }
                            else
                            {
                                string nextButtonXPath = $"//*[@id='exportArticles']/div/div[3]/div/div[2]/div[2]/div/a[normalize-space(text())='{i + 1}']";
                                //string nextButtonXPath = $"//*[@id=\"exportArticles\"]/div/div[3]/div/div[2]/div[2]/div/a[{j + 1}]";
                                IWebElement nextButton = driver.FindElement(By.XPath(nextButtonXPath));
                                // Scroll the element into view
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", nextButton);

                                wait.Until(ExpectedConditions.ElementToBeClickable(nextButton));
                                nextButton.Click();
                            }
                        }


                        // Wait for the next page to load
                        //Task.Delay(1000).Wait();
                        await WriteToExcel(SearchText);
                    }
                    catch (Exception ex)
                    {
                        throw;
                        //i = i - 1;
                    }
                }

                SearchText = SearchText.Replace(" ", "_");
                string excelFilePath = Path.Combine(downloadDirectory, $"{SearchText}.xlsx");

                //remove duplecate emails from excel
                ExcelHelpers.RemoveDuplicateEmails(excelFilePath);
                // Pause the program for a few seconds to see the result
                Task.Delay(5000).Wait();
                LoggerService.Log($"MDPI - ******* End RunAutomation: {SearchText}*******");

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

        private async Task WriteToExcel(string SearchText)
        {
            string downloadDirectory = @"C:\CustomDownloads\mdpi";
            //remove spaces from the search text
            SearchText = SearchText.Replace(" ", "_");
            string excelFilePath = Path.Combine(downloadDirectory, $"{SearchText}.xlsx");

            // Get all .txt files from the download directory
            string[] txtFiles = Directory.GetFiles(downloadDirectory, "*.txt");

            DataTable datatable = new DataTable();
            char[] delimiter = new char[] { '\t' };

            foreach (string file in txtFiles)
            {
                using (StreamReader streamreader = new StreamReader(file))
                {
                    // Read the column headers from the first file
                    if (datatable.Columns.Count == 0)
                    {
                        string[] columnheaders = streamreader.ReadLine().Split(delimiter);
                        foreach (string columnheader in columnheaders)
                        {
                            datatable.Columns.Add(columnheader.Trim());
                        }
                    }

                    // Read the data rows
                    while (streamreader.Peek() > 0)
                    {
                        DataRow datarow = datatable.NewRow();
                        datarow.ItemArray = streamreader.ReadLine().Split(delimiter);
                        datatable.Rows.Add(datarow);
                    }
                }
            }

            // Append data to Excel file
            using (var workbook = File.Exists(excelFilePath) ? new XLWorkbook(excelFilePath) : new XLWorkbook())
            {
                //Get complete data
                //IXLWorksheet worksheet;
                //if (workbook.Worksheets.Count == 0)
                //{
                //    worksheet = workbook.Worksheets.Add("Sheet1");
                //}
                //else
                //{
                //    worksheet = workbook.Worksheet("Sheet1");
                //}

                //int startRow = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;

                //// Write the headers if the worksheet is empty
                //if (startRow == 1)
                //{
                //    for (int i = 0; i < datatable.Columns.Count; i++)
                //    {
                //        worksheet.Cell(startRow, i + 1).Value = datatable.Columns[i].ColumnName.Trim();
                //    }
                //    startRow++;
                //}

                //// Write the data
                //for (int i = 0; i < datatable.Rows.Count; i++)
                //{
                //    for (int j = 0; j < datatable.Columns.Count; j++)
                //    {
                //        worksheet.Cell(startRow + i, j + 1).Value = datatable.Rows[i][j].ToString();
                //    }
                //}

                // Check if the "data" worksheet exists, if not, create it
                IXLWorksheet dataWorksheet;
                if (workbook.Worksheets.Count == 0)
                {
                    dataWorksheet = workbook.Worksheets.Add("Sheet1");
                    dataWorksheet.Cell(1, 1).Value = "AUTHOR";
                    dataWorksheet.Cell(1, 2).Value = "EMAIL";
                    dataWorksheet.Cell(1, 3).Value = "JOURNAL";
                    dataWorksheet.Cell(1, 4).Value = "TITLE";
                }
                else
                {
                    dataWorksheet = workbook.Worksheet("Sheet1");
                }

                int dataStartRow = dataWorksheet.LastRowUsed()?.RowNumber() + 1 ?? 2;

                // Write the data to the "data" worksheet
                for (int i = 0; i < datatable.Rows.Count; i++)
                {
                    string[] authors = datatable.Rows[i]["AUTHOR"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] emails = datatable.Rows[i]["EMAIL"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < authors.Length; k++)
                    {
                        dataWorksheet.Cell(dataStartRow + i + k, 1).Value = authors[k].Trim();
                        if (k < emails.Length)
                        {
                            dataWorksheet.Cell(dataStartRow + i + k, 2).Value = emails[k].Trim();
                        }
                        dataWorksheet.Cell(dataStartRow + i + k, 3).Value = datatable.Rows[i]["JOURNAL"].ToString();
                        dataWorksheet.Cell(dataStartRow + i + k, 4).Value = datatable.Rows[i]["TITLE"].ToString();
                        dataStartRow++;
                    }
                }

                workbook.SaveAs(excelFilePath);
            }

            // Optionally, delete the .txt files after processing
            foreach (string file in txtFiles)
            {
                //File.Delete(file);
            }
        }

        //remove duplecate emails from excel


    }
}
