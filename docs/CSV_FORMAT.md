# CSV Import/Export Format Documentation

This document describes the CSV format used for importing and exporting data in the GSC Tracking application.

## Expense Export Format

When exporting expenses, the CSV file will contain the following columns:

| Column Name        | Type     | Description                                   | Example              |
|-------------------|----------|-----------------------------------------------|----------------------|
| Expense ID        | Integer  | Unique identifier for the expense             | 1                    |
| Job ID            | Integer  | ID of the associated job                      | 5                    |
| Type              | String   | Type of expense (Parts, Labor, Service)       | Parts                |
| Description       | String   | Description of the expense                    | Oil filter           |
| Amount            | Decimal  | Cost amount                                   | 15.99                |
| Date              | DateTime | Date of the expense                           | 2025-01-15T00:00:00Z |
| Receipt Reference | String   | Optional receipt reference number             | REC-001              |
| Created At        | DateTime | When the expense was created                  | 2025-01-15T10:30:00Z |
| Updated At        | DateTime | When the expense was last updated             | 2025-01-15T10:30:00Z |

### Example Export CSV

```csv
Expense ID,Job ID,Type,Description,Amount,Date,Receipt Reference,Created At,Updated At
1,5,Parts,Oil filter,15.99,2025-01-15T00:00:00Z,REC-001,2025-01-15T10:30:00Z,2025-01-15T10:30:00Z
2,5,Labor,Oil change service,45.00,2025-01-15T00:00:00Z,,2025-01-15T10:30:00Z,2025-01-15T10:30:00Z
3,6,Service,Engine diagnostic,125.00,2025-01-16T00:00:00Z,REC-002,2025-01-16T14:20:00Z,2025-01-16T14:20:00Z
```

## Expense Import Format

When importing expenses, the CSV file should contain the following columns (in order):

| Column Name        | Type     | Required | Validation                                    | Example              |
|-------------------|----------|----------|-----------------------------------------------|----------------------|
| Job ID            | Integer  | Yes      | Must be greater than 0; job must exist        | 5                    |
| Type              | String   | Yes      | Must be one of: Parts, Labor, Service         | Parts                |
| Description       | String   | Yes      | Max 500 characters                            | Oil filter           |
| Amount            | Decimal  | Yes      | Must be greater than 0                        | 15.99                |
| Date              | DateTime | Yes      | Valid date format (yyyy-MM-dd or MM/dd/yyyy)  | 2025-01-15           |
| Receipt Reference | String   | No       | Max 200 characters                            | REC-001              |

### Example Import CSV

```csv
Job ID,Type,Description,Amount,Date,Receipt Reference
5,Parts,Oil filter,15.99,2025-01-15,REC-001
5,Labor,Oil change service,45.00,2025-01-15,
6,Service,Engine diagnostic,125.00,2025-01-16,REC-002
```

### Import Validation Rules

1. **Job ID**: Must reference an existing job in the system
2. **Type**: Case-insensitive; valid values are "Parts", "Labor", or "Service"
3. **Description**: Cannot be empty
4. **Amount**: Must be a positive number with up to 2 decimal places
5. **Date**: Accepts various date formats including:
   - ISO 8601: `2025-01-15` or `2025-01-15T00:00:00Z`
   - US format: `1/15/2025` or `01/15/2025`
6. **Receipt Reference**: Optional; leave empty or provide a reference number

### Import Error Handling

When importing, the API will:
- Validate each row individually
- Continue processing valid rows even if some rows fail
- Return a detailed result showing:
  - Number of successful imports
  - Number of failed imports
  - List of errors with line numbers and descriptions

Example error response:
```json
{
  "successCount": 2,
  "errorCount": 1,
  "errors": [
    {
      "lineNumber": 3,
      "message": "Job with ID 999 not found",
      "rawData": "999,Parts,Missing part,10.00,2025-01-15"
    }
  ]
}
```

## Job Export Format

When exporting jobs with estimate/invoice data, the CSV file will contain:

| Column Name      | Type     | Description                                | Example              |
|-----------------|----------|--------------------------------------------|--------------------|
| Job ID          | Integer  | Unique identifier for the job              | 1                  |
| Customer ID     | Integer  | ID of the customer                         | 10                 |
| Customer Name   | String   | Name of the customer                       | John Doe           |
| Equipment Type  | String   | Type of equipment                          | Lawn Mower         |
| Equipment Model | String   | Model of the equipment                     | Honda HRX217       |
| Description     | String   | Job description                            | Annual maintenance |
| Status          | String   | Job status (Quote, InProgress, etc.)       | Completed          |
| Date Received   | DateTime | When the job was received                  | 2025-01-10         |
| Date Completed  | DateTime | When the job was completed (if applicable) | 2025-01-15         |
| Estimate Amount | Decimal  | Estimated cost                             | 150.00             |
| Actual Amount   | Decimal  | Actual invoiced amount                     | 145.50             |
| Total Cost      | Decimal  | Total expenses for the job                 | 85.25              |
| Profit Margin   | Decimal  | Profit margin (Actual - Total Cost)        | 60.25              |
| Created At      | DateTime | When the job was created                   | 2025-01-10         |
| Updated At      | DateTime | When the job was last updated              | 2025-01-15         |

### Example Job Export CSV

```csv
Job ID,Customer ID,Customer Name,Equipment Type,Equipment Model,Description,Status,Date Received,Date Completed,Estimate Amount,Actual Amount,Total Cost,Profit Margin,Created At,Updated At
1,10,John Doe,Lawn Mower,Honda HRX217,Annual maintenance,Completed,2025-01-10,2025-01-15,150.00,145.50,85.25,60.25,2025-01-10,2025-01-15
2,11,Jane Smith,Chainsaw,Stihl MS271,Chain replacement,InProgress,2025-01-12,,,75.00,35.50,,,2025-01-12,2025-01-12
```

## API Endpoints

### Export Expenses
```
GET /api/export/expenses
GET /api/export/expenses?jobId=5
```

Returns a CSV file with all expenses (or filtered by job).

### Export Jobs
```
GET /api/export/jobs
GET /api/export/jobs?status=Completed
```

Returns a CSV file with all jobs (or filtered by status).

### Import Expenses
```
POST /api/import/expenses
Content-Type: multipart/form-data
```

Upload a CSV file to import expenses. Returns an import result with success/error counts.

## Tips for Creating Import Files

1. **Use a template**: Export existing data to get a properly formatted CSV template
2. **Keep headers**: The first row must contain the column headers exactly as shown
3. **Date formats**: Use ISO format (YYYY-MM-DD) for consistency
4. **Decimal separator**: Use a period (.) as the decimal separator
5. **Empty values**: Leave cells empty for optional fields (don't use "null" or "N/A")
6. **Encoding**: Save the file as UTF-8 encoding
7. **Line endings**: Unix (LF) or Windows (CRLF) line endings are both supported
8. **File size**: Maximum file size is 10 MB

## Common Issues

| Issue | Solution |
|-------|----------|
| "Job with ID X not found" | Verify the Job ID exists in the system before importing |
| "Invalid expense type" | Use exactly "Parts", "Labor", or "Service" (case-insensitive) |
| "Amount must be greater than 0" | Ensure amounts are positive numbers |
| "Date is required" | Provide a valid date in the Date column |
| "Description is required" | Ensure the Description field is not empty |
| CSV parsing error | Check that all commas are properly escaped in text fields |

## Character Encoding and Special Characters

- The CSV files use UTF-8 encoding
- Text fields containing commas should be enclosed in double quotes
- Double quotes within text fields should be escaped by doubling them (`""`)
- Example: `"Description with ""quoted"" text and, comma"`

## Tab-Separated Values (TSV)

While the system primarily uses comma-separated values (CSV), you can use tab-separated values by:
1. Ensuring your file uses tabs as delimiters
2. Saving with a .csv extension (the parser will auto-detect tabs)
3. Following the same column order and validation rules

---

For questions or issues with CSV import/export, please refer to the application documentation or contact support.
