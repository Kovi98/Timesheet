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
        public IList<Timesheet.Entity.Entities.Timesheet> ConvertPeople(byte[] source)
        {
            IList<Timesheet.Entity.Entities.Timesheet> import = new List<Timesheet.Entity.Entities.Timesheet>();
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
                            //Není jméno ani příjmení - vynechání řádku
                            if (firstname == string.Empty && lastname == string.Empty)
                                continue;

                            timesheet.Person = _context.Person.First(x => x.Name == firstname && x.Surname == lastname) ?? new Person() { Name = firstname, Surname = lastname };

                            foreach (var col in mappedColumns)
                            {
                                switch (col.Value)
                                {
                                    case "Od":
                                        timesheet.DateTimeFrom = DateTime.Parse(ws.Cells[i, col.Key].Value.ToString());
                                        break;
                                    case "Do":
                                        timesheet.DateTimeTo = DateTime.Parse(ws.Cells[i, col.Key].Value.ToString());
                                        break;
                                    case "Text":
                                        timesheet.Name = ws.Cells[i, col.Key].Value.ToString();
                                        break;
                                    case "Funkce":
                                        timesheet.Job = _context.Job.First(x => x.Name == ws.Cells[i, col.Key].Value.ToString()) ?? new Job() { Name = ws.Cells[i, col.Key].Value.ToString() };
                                        break;
                                }
                            }

                            if (IsUnique(timesheet))
                                import.Add(timesheet);
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
                .First(x => x.DateTimeFrom == timesheet.DateTimeFrom && x.DateTimeTo == timesheet.DateTimeTo && x.Person.FullName == timesheet.Person.FullName);
            return ts == null;
        }
    }
}
