using Business;

namespace UnitTests;

public class PublicHolidaysTests
{
    [Fact]
    public void Public_holidays_2025_are_correct()
    {
        // Easter Sunday 2025 is 20 April.
        DateOnly[] expected =
        [
            new(2025, 1, 1),   // New Year's Day
            new(2025, 4, 17),  // Maundy Thursday
            new(2025, 4, 18),  // Good Friday
            new(2025, 4, 20),  // Easter Sunday
            new(2025, 4, 21),  // Easter Monday
            new(2025, 5, 29),  // Ascension Day
            new(2025, 6, 8),   // Whit Sunday
            new(2025, 6, 9),   // Whit Monday
            new(2025, 12, 25), // Christmas Day
            new(2025, 12, 26), // Boxing Day
        ];

        Assert.Equal(expected, PublicHolidays.For(2025).Select(d => d.Date));
    }

    [Fact]
    public void Store_bededag_is_a_holiday_until_2023()
    {
        Assert.Contains(PublicHolidays.For(2023), d => d.Date == new DateOnly(2023, 5, 5));
        Assert.Equal(10, PublicHolidays.For(2024).Count);
    }
}
