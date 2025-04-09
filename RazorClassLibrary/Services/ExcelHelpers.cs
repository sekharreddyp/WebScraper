using ClosedXML.Excel;

namespace WebScrapeApp.Services;

public static class ExcelHelpers
{
    public static void RemoveDuplicateEmails(string FilePath)
    {
        using (var workbook = new XLWorkbook(FilePath))
        {
            var worksheet = workbook.Worksheet("Sheet1");

            // Ensure the email column exists
            var emailColumn = worksheet.Column(2);
            if (emailColumn == null)
            {
                throw new Exception("Email column not found in the worksheet.");
            }

            // Get all used cells in the email column, skipping the header
            var emailCells = emailColumn.CellsUsed().Skip(1);
            LoggerService.Log($"MDPI - Total emails extracted: {emailCells.Count()}");
            var uniqueEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Case-insensitive comparison
            var rowsToDelete = new List<IXLRow>();

            foreach (var cell in emailCells)
            {
                var email = cell.GetValue<string>()?.Trim(); // Trim whitespace
                if (string.IsNullOrEmpty(email) || !uniqueEmails.Add(email))
                {
                    rowsToDelete.Add(cell.WorksheetRow()); // Mark the row for deletion
                }
            }

            LoggerService.Log($"MDPI - Duplecate Emails: {rowsToDelete.Count()}");
            // Delete rows with duplicate emails
            foreach (var row in rowsToDelete)
            {
                row.Delete();
            }

            workbook.Save();
        }
    }
}

