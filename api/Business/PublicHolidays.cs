namespace Business;

// A day off: a public holiday, or a company day off (then it has a CompanyDayId).
public record DayOff(DateOnly Date, string Name, Guid? CompanyDayId = null);

// Danish public holidays. Most of them move with Easter.
public static class PublicHolidays
{
    public static List<DayOff> For(int year)
    {
        var easter = EasterSunday(year);
        var days = new List<DayOff>
        {
            new(new DateOnly(year, 1, 1), "New Year's Day"),
            new(easter.AddDays(-3), "Maundy Thursday"),
            new(easter.AddDays(-2), "Good Friday"),
            new(easter, "Easter Sunday"),
            new(easter.AddDays(1), "Easter Monday"),
            new(easter.AddDays(39), "Ascension Day"),
            new(easter.AddDays(49), "Whit Sunday"),
            new(easter.AddDays(50), "Whit Monday"),
            new(new DateOnly(year, 12, 25), "Christmas Day"),
            new(new DateOnly(year, 12, 26), "Boxing Day"),
        };

        // Store bededag was removed as a public holiday from 2024.
        if (year <= 2023)
            days.Add(new DayOff(easter.AddDays(26), "Store bededag"));

        return days.OrderBy(d => d.Date).ToList();
    }

    // The "anonymous Gregorian algorithm" for the date of Easter Sunday.
    private static DateOnly EasterSunday(int year)
    {
        int a = year % 19, b = year / 100, c = year % 100;
        int d = b / 4, e = b % 4, f = (b + 8) / 25, g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4, k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = (h + l - 7 * m + 114) % 31 + 1;
        return new DateOnly(year, month, day);
    }
}
