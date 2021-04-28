using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;

namespace Portal.Models
{
    public class ImportManager
    {
        private TimesheetContext _context { get; set; }
        public ImportManager(TimesheetContext context)
        {
            _context = context;
        }
        public IList<TimesheetImport> ConvertPeople(byte[] source)
        {
            IList<TimesheetImport> import = new List<TimesheetImport>();
            using (MemoryStream stream = new MemoryStream(source))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    IEnumerable<ExcelWorksheet> wss;
                    if (excelPackage.Workbook.Worksheets.Count == 1)
                        wss = excelPackage.Workbook.Worksheets;
                    else
                        wss = excelPackage.Workbook.Worksheets.Where(x => x.Name == "Import");

                    foreach (var ws in wss)
                    {
                        Dictionary<int, string> mappedColumns = new Dictionary<int, string>();
                        int startRow = ws.Dimension.Start.Row;
                        for (int i = ws.Dimension.Start.Column; i <= ws.Dimension.End.Column; i++)
                        {
                            mappedColumns.Add(i, ws.Cells[startRow, i].Value.ToString());
                        }

                        for(int i = startRow+1; i <= ws.Dimension.End.Row; i++)
                        {
                            Timesheet.Entity.Entities.Timesheet timesheet = new Timesheet.Entity.Entities.Timesheet();
                            TimesheetImport timesheetImport = new TimesheetImport(timesheet);
                            timesheet.CreateTime = DateTime.Now;
                            string firstname = string.Empty;
                            string lastname = string.Empty;
                            foreach (var col in mappedColumns)
                            {
                                switch (col.Value)
                                {
                                    case "Jmeno":
                                        firstname = ws.Cells[i, col.Key].Value.ToString();
                                        break;
                                    case "Prijmeni":
                                        lastname = ws.Cells[i, col.Key].Value.ToString();
                                        break;
                                }
                            }
                            //Není jméno ani příjmení - PersonMissing
                            if (firstname == string.Empty && lastname == string.Empty)
                                timesheetImport.AddError(TimesheetImportError.PersonMissing);
                            var person = _context.Person.FirstOrDefault(x => x.Name == firstname && x.Surname == lastname);
                            if (person == null)
                                timesheetImport.AddError(TimesheetImportError.PersonUndefined);
                            timesheet.Person = person ?? new Person{ Name = firstname, Surname = lastname, CreateTime = DateTime.Now };

                            foreach (var col in mappedColumns)
                            {
                                string value = string.Empty;
                                switch (col.Value)
                                {
                                    case "Od":
                                        value = ws.Cells[i, col.Key].Value.ToString();
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.DateTimeFromMissing);
                                        timesheet.DateTimeFrom = DateTime.Parse(value);
                                        break;
                                    case "Do":
                                        value = ws.Cells[i, col.Key].Value.ToString();
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.DateTimeToMissing);
                                        timesheet.DateTimeTo = DateTime.Parse(value);
                                        break;
                                    case "Text":
                                        timesheet.Name = ws.Cells[i, col.Key].Value.ToString();
                                        break;
                                    case "Funkce":
                                        value = ws.Cells[i, col.Key].Value.ToString();
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.JobMissing);
                                        var job = _context.Job.FirstOrDefault(x => x.Name == value);
                                        if (job == null)
                                            timesheetImport.AddError(TimesheetImportError.JobUndefined);
                                        timesheet.Job = job ?? new Job{ Name = ws.Cells[i, col.Key].Value.ToString(), CreateTime = DateTime.Now };
                                        break;
                                }
                            }
                            if (!IsUnique(timesheet))
                                timesheetImport.AddError(TimesheetImportError.TimesheetNotUnique);
                            import.Add(timesheetImport);
                        }

                    }
                }
            }
            return import;
        }

        private bool IsUnique (Timesheet.Entity.Entities.Timesheet timesheet)
        {
            var ts = _context.Timesheet
                .Include(x => x.Person)
                .FirstOrDefault(x => x.DateTimeFrom == timesheet.DateTimeFrom && x.DateTimeTo == timesheet.DateTimeTo && x.Person.Name == timesheet.Person.Name && x.Person.Surname == timesheet.Person.Surname);
            return ts == null;
        }
    }
}
