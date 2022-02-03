namespace Timesheet.Common.Enums
{
    public enum PersonImportError
    {
        DateBirthMissing,
        DateBirthBadFormat,
        IsActiveMissing,
        IsActiveBadFormat,
        HasTaxMissing,
        HasTaxBadFormat,
        NameMissing,
        SurnameMissing,
        SectionMissing,
        SectionUndefined,
        PaidFromMissing,
        PaidFromUndefined,
        JobMissing,
        JobUndefined,
        PersonNotUnique
    }
}
