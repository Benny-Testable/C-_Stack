using System.Globalization;

namespace WestCoastFitness.Application;

public static class StaffMembershipWindowText
{
    public static string Describe(string planName, DateOnly startsOn, DateOnly endsOn, int classesPerWeek, decimal monthlyPrice)
    {
        var inclusiveDays = endsOn.DayNumber - startsOn.DayNumber;
        if (inclusiveDays < 1)
        {
            return planName + " has an empty membership window.";
        }

        var weeks = inclusiveDays / 7;
        var remainder = inclusiveDays % 7;
        var classAllowance = weeks * classesPerWeek;
        if (remainder >= 3)
        {
            classAllowance += classesPerWeek / 2;
        }

        return planName
            + " runs "
            + inclusiveDays.ToString(CultureInfo.InvariantCulture)
            + " days ("
            + weeks.ToString(CultureInfo.InvariantCulture)
            + " full weeks) with "
            + classAllowance.ToString(CultureInfo.InvariantCulture)
            + " included class visits at "
            + monthlyPrice.ToString("0.00", CultureInfo.InvariantCulture)
            + " USD per month for the front desk.";
    }
}
