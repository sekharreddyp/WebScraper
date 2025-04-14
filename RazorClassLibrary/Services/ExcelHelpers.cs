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
            var rowsToDelete = new List<int>(); // Collect row numbers to delete

            foreach (var cell in emailCells)
            {
                var email = cell.GetValue<string>()?.Trim(); // Trim whitespace
                if (string.IsNullOrEmpty(email) || !uniqueEmails.Add(email))
                {
                    rowsToDelete.Add(cell.Address.RowNumber); // Collect the row number for deletion
                }
            }

            LoggerService.Log($"MDPI - Duplicate Emails: {rowsToDelete.Count}");

            // Delete rows in reverse order to avoid index shifting
            foreach (var rowNumber in rowsToDelete.OrderByDescending(r => r))
            {
                worksheet.Row(rowNumber).Delete();
            }

            var newFilePath = Path.Combine(Path.GetDirectoryName(FilePath),
                                           Path.GetFileNameWithoutExtension(FilePath) + "_Cleaned" + Path.GetExtension(FilePath));
            workbook.SaveAs(newFilePath);
        }
    }


}

