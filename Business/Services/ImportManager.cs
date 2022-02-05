using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Common.Enums;
using Timesheet.Common.Interfaces;
using Timesheet.Common.Models;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class ImportManager : IImportManager
    {
        private TimesheetContext _context;
        public ImportManager(TimesheetContext context)
        {
            _context = context;
        }
        public List<TimesheetImport> ConvertTimesheets(byte[] source)
        {
            List<TimesheetImport> import = new List<TimesheetImport>();
            using (MemoryStream stream = new MemoryStream(source))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    IEnumerable<ExcelWorksheet> wss = excelPackage.Workbook.Worksheets;
                    foreach (var ws in wss)
                    {
                        Dictionary<int, string> mappedColumns = new Dictionary<int, string>();
                        int startRow = ws.Dimension.Start.Row;
                        for (int i = ws.Dimension.Start.Column; i <= ws.Dimension.End.Column; i++)
                        {
                            mappedColumns.Add(i, ws.Cells[startRow, i].Value.ToString());
                        }

                        for (int i = startRow + 1; i <= ws.Dimension.End.Row; i++)
                        {
                            var timesheet = new Common.Timesheet();
                            TimesheetImport timesheetImport = new TimesheetImport(timesheet);
                            timesheet.CreateTime = DateTime.Now;
                            string fullname = string.Empty;
                            string firstname = string.Empty;
                            string lastname = string.Empty;

                            bool isActive = false;
                            foreach (var col in mappedColumns)
                            {
                                if (col.Value.Trim() == "Účast")
                                    isActive = ws.Cells[i, col.Key].Value.ToString().Trim() == "1";
                            }
                            //Neúčast - vynechání řádku
                            if (!isActive)
                                continue;

                            foreach (var col in mappedColumns)
                            {
                                if (col.Value.Trim() == "Člen")
                                    fullname = ws.Cells[i, col.Key].Value?.ToString().Trim() ?? "";
                            }
                            lastname = fullname.Substring(0, fullname.IndexOf(' ')).Trim();
                            firstname = fullname.Substring(fullname.IndexOf(' ') + 1).Trim();
                            //Není jméno ani příjmení - PersonMissing
                            if (firstname == string.Empty && lastname == string.Empty)
                                timesheetImport.AddError(TimesheetImportError.PersonMissing);
                            var person = _context.Person.FirstOrDefault(x => x.Name == firstname && x.Surname == lastname);
                            if (person != null)
                                _context.Attach(person).State = EntityState.Detached;
                            if (person == null)
                                timesheetImport.AddError(TimesheetImportError.PersonUndefined);
                            timesheet.Person = person ?? new Person { Name = firstname, Surname = lastname, CreateTime = DateTime.Now, JobId = 1, PaidFromId = _context.Finance.Select(x => x.Id).FirstOrDefault(), SectionId = _context.Section.Select(x => x.Id).FirstOrDefault() };
                            timesheet.PersonId = person?.Id ?? 0;

                            foreach (var col in mappedColumns)
                            {
                                string value = string.Empty;
                                switch (col.Value.Trim())
                                {
                                    case "Začátek":
                                        value = ws.Cells[i, col.Key].Value?.ToString() ?? "";
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.DateTimeFromMissing);
                                        timesheet.DateTimeFrom = DateTime.Parse(value);
                                        break;
                                    case "Konec":
                                        value = ws.Cells[i, col.Key].Value?.ToString() ?? "";
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.DateTimeToMissing);
                                        timesheet.DateTimeTo = DateTime.Parse(value);
                                        break;
                                    case "Název události":
                                        timesheet.Name = ws.Cells[i, col.Key].Value.ToString();
                                        break;
                                    case "Pozice RT":
                                        value = ws.Cells[i, col.Key].Value?.ToString().ToLower() ?? "";
                                        if (string.IsNullOrEmpty(value))
                                            timesheetImport.AddError(TimesheetImportError.JobMissing);
                                        var job = _context.Job.FirstOrDefault(x => x.Name == value);
                                        if (job != null)
                                            _context.Attach(job).State = EntityState.Detached;
                                        if (job == null)
                                            timesheetImport.AddError(TimesheetImportError.JobUndefined);
                                        timesheet.Job = job ?? new Job { Name = ws.Cells[i, col.Key].Value.ToString(), CreateTime = DateTime.Now };
                                        break;
                                }
                            }
                            if (!IsUnique(timesheet))
                                timesheetImport.AddError(TimesheetImportError.TimesheetNotUnique);

                            timesheet.CalculateReward();
                            import.Add(timesheetImport);
                        }

                    }
                }
            }
            return import;
        }

        public async Task Import(List<TimesheetImport> imports, bool overrideErrors)
        {
            var defaultList = imports;
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!overrideErrors)
                {
                    imports = imports.Where(x => x.Success).ToList();
                }
                else
                {
                    imports = imports.Where(x => x.CanPassErrors).ToList();
                    var people = new List<Person>();
                    var jobs = new List<Job>();
                    //Hledání undefined jobs/people
                    foreach (var import in imports)
                    {
                        if (import.Errors.Contains(TimesheetImportError.PersonUndefined) && !people.Any(x => x.FullName == import.Entity.Person.FullName))
                        {
                            people.Add(import.Entity.Person);
                        }
                        if (import.Errors.Contains(TimesheetImportError.JobUndefined) && !jobs.Any(x => x.Name == import.Entity.Job.Name))
                        {
                            jobs.Add(import.Entity.Job);
                        }
                    }
                    //Nahrazování undefined people
                    foreach (var person in people)
                    {
                        _context.Person.Add(person);
                        foreach (var item in imports.Where(x => x.Entity.Person.FullName == person.FullName).ToList())
                        {
                            item.Entity.Person = person;
                        }
                    }
                    //Nahrazování undefined jobs
                    foreach (var job in jobs)
                    {
                        _context.Job.Add(job);
                        foreach (var item in imports.Where(x => x.Entity.Job.Name == job.Name).ToList())
                        {
                            item.Entity.Job = job;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                //Samotné ukládání timesheetu do DB
                //await _context.Timesheet.AddRangeAsync(TimesheetImport.Select(x => x.Timesheet));
                foreach (var timesheet in imports.Select(x => x.Entity))
                {
                    //_context.Timesheet.Add(timesheet);
                    _context.Entry(timesheet).State = EntityState.Added;
                    if (_context.Entry(timesheet.Person).State != EntityState.Detached)
                        _context.Entry(timesheet.Person).State = EntityState.Detached;
                    if (_context.Entry(timesheet.Job).State != EntityState.Detached)
                        _context.Entry(timesheet.Job).State = EntityState.Detached;
                    await _context.SaveChangesAsync();
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                imports = defaultList;
                throw e;
            }
        }

        private bool IsUnique(Common.Timesheet timesheet)
        {
            var ts = _context.Timesheet
                .Include(x => x.Person)
                .FirstOrDefault(x => x.DateTimeFrom == timesheet.DateTimeFrom && x.DateTimeTo == timesheet.DateTimeTo && x.Person.Name == timesheet.Person.Name && x.Person.Surname == timesheet.Person.Surname);
            return ts == null;
        }

        public List<PersonImport> ConvertPeople(byte[] source)
        {
            using var memoryStream = new MemoryStream(source);
            using var streamReader = new StreamReader(memoryStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                Delimiter = ";"
            };
            using var csvReader = new CsvReader(streamReader, config);
            csvReader.Context.RegisterClassMap<PersonImportMap>();

            var records = csvReader.GetRecords<PersonImportDto>().ToList();
            var imports = new List<PersonImport>();

            foreach (var record in records)
            {
                List<PersonImportError> errors = new List<PersonImportError>();

                DateTime dateBirth = DateTime.MinValue;
                try
                {
                    dateBirth = DateTime.Parse(record.DateBirth, new CultureInfo("cs-CZ"));
                }
                catch (Exception ex)
                {
                    errors.Add(PersonImportError.DateBirthBadFormat);
                }

                bool isActive = false;
                if (record.IsActive == "Y" || record.IsActive == "N")
                    isActive = record.IsActive == "A";
                else if (string.IsNullOrEmpty(record.IsActive))
                    errors.Add(PersonImportError.IsActiveMissing);
                else
                    errors.Add(PersonImportError.IsActiveBadFormat);

                bool hasTax = false;
                if (record.HasTax == "Y" || record.HasTax == "N")
                    hasTax = record.HasTax == "A";
                else if (string.IsNullOrEmpty(record.HasTax))
                    errors.Add(PersonImportError.HasTaxMissing);
                else
                    errors.Add(PersonImportError.HasTaxBadFormat);

                Job job = null;
                if (string.IsNullOrEmpty(record.Job))
                    errors.Add(PersonImportError.JobMissing);
                else
                {
                    var jobExists = _context.Job.Where(x => x.Name.ToLower() == record.Job.ToLower()).Any();
                    if (jobExists)
                        job = _context.Job.Where(x => x.Name.ToLower() == record.Job.ToLower()).AsNoTracking().FirstOrDefault();
                    else
                        errors.Add(PersonImportError.JobUndefined);
                }

                Finance paidFrom = null;
                if (string.IsNullOrEmpty(record.PaidFrom))
                    errors.Add(PersonImportError.PaidFromMissing);
                else
                {
                    var financeExists = _context.Finance.Where(x => x.Name.ToLower() == record.PaidFrom.ToLower()).Any();
                    if (financeExists)
                        paidFrom = _context.Finance.Where(x => x.Name.ToLower() == record.PaidFrom.ToLower()).AsNoTracking().FirstOrDefault();
                    else
                        errors.Add(PersonImportError.PaidFromUndefined);
                }

                Section section = null;
                if (string.IsNullOrEmpty(record.Section))
                    errors.Add(PersonImportError.SectionMissing);
                else
                {
                    var sectionExists = _context.Section.Where(x => x.Name.ToLower() == record.Section.ToLower()).Any();
                    if (sectionExists)
                        section = _context.Section.Where(x => x.Name.ToLower() == record.Section.ToLower()).AsNoTracking().FirstOrDefault();
                    else
                        errors.Add(PersonImportError.SectionUndefined);
                }

                var person = new Person()
                {
                    Name = record.Name,
                    Surname = record.Surname,
                    DateBirth = dateBirth,
                    BankAccount = record.BankAccount,
                    BankCode = record.BankCode,
                    Street = record.Street,
                    HouseNumber = record.HouseNumber,
                    State = record.State,
                    PostalCode = record.PostalCode,
                    IdentityDocument = record.IdentityDocument,
                    IsActive = isActive,
                    HasTax = hasTax,
                    Section = section,
                    PaidFrom = paidFrom,
                    Job = job
                };

                if (string.IsNullOrEmpty(person.Name))
                    errors.Add(PersonImportError.NameMissing);
                if (string.IsNullOrEmpty(person.Surname))
                    errors.Add(PersonImportError.SurnameMissing);
                var isSamePerson = _context.Person.Where(
                    x => x.Name.ToLower() == person.Name.ToLower()
                    && x.Surname.ToLower() == person.Surname.ToLower()
                    && x.DateBirth == person.DateBirth).Any();
                if (isSamePerson)
                    errors.Add(PersonImportError.PersonNotUnique);

                var import = new PersonImport(person, errors);
                imports.Add(import);
            }
            return imports;
        }
        public async Task Import(List<PersonImport> imports, bool overrideErrors)
        {
            var defaultList = imports;
            try
            {
                if (!overrideErrors)
                {
                    imports = imports.Where(x => x.Success).ToList();
                }
                else
                {
                    imports = imports.Where(x => x.CanPassErrors).ToList();
                }

                imports.ForEach(x =>
                {
                    x.Entity.CreateTime = DateTime.Now;
                    _context.Entry(x.Entity).State = EntityState.Added;
                    x.Entity.Job = _context.Job.FirstOrDefault();
                    x.Entity.PaidFrom = _context.Finance.FirstOrDefault();
                    x.Entity.Section = _context.Section.FirstOrDefault();
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                imports = defaultList;
                throw e;
            }
        }
    }
}
