using System.Data;
using System.Globalization;
using TaskPlanner.Domain.Entities;

namespace TaskPlanner.Infrastructure.Data.Mappers;

internal static class UserDataMapper
{
    public static User Map(IDataRecord record)
    {
        return new User
        {
            Id = ReadGuid(record, "Id"),
            Email = ReadString(record, "Email"),
            DisplayName = ReadString(record, "DisplayName"),
            PasswordHash = ReadString(record, "PasswordHash"),
            CreatedAtUtc = ReadDateTime(record, "CreatedAtUtc"),
            UpdatedAtUtc = ReadDateTime(record, "UpdatedAtUtc"),
            CreatedBy = ReadGuid(record, "CreatedBy"),
            UpdatedBy = ReadGuid(record, "UpdatedBy")
        };
    }

    private static string ReadString(IDataRecord record, string columnName)
    {
        return record.GetString(record.GetOrdinal(columnName));
    }

    private static Guid ReadGuid(IDataRecord record, string columnName)
    {
        var ordinal = record.GetOrdinal(columnName);
        var value = record.GetValue(ordinal);
        return value switch
        {
            Guid guid => guid,
            string text => Guid.Parse(text),
            byte[] bytes => new Guid(bytes),
            _ => Guid.Parse(Convert.ToString(value, CultureInfo.InvariantCulture)!)
        };
    }

    private static DateTime ReadDateTime(IDataRecord record, string columnName)
    {
        var ordinal = record.GetOrdinal(columnName);
        var value = record.GetValue(ordinal);
        if (value is DateTime dateTime)
        {
            return dateTime;
        }

        return DateTime.Parse(
            Convert.ToString(value, CultureInfo.InvariantCulture)!,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
    }
}
