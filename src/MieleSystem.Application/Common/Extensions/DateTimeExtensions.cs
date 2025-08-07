namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Extensões para manipulação de datas.
/// </summary>
public static class DateTimeExtensions
{
    public static bool IsWeekend(this DateTime date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public static DateTime GetFirstDayOfMonth(this DateTime date) => new(date.Year, date.Month, 1);

    public static string ToBrazilianDate(this DateTime date) => date.ToString("dd/MM/yyyy");

    public static string ToMonthYear(this DateTime date) => date.ToString("MM/yyyy");

    public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);

    public static DateTime ToDateTime(this DateOnly date) => date.ToDateTime(new TimeOnly(0, 0));
}
