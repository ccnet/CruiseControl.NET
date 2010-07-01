using System;

namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers
{
    [Flags]
    public enum WeekDay
    {
        None = 0,
        Monday = 1,
        Mon = 1,
        Thusday = 2,
        Tue = 2,
        Wednesday = 4,
        Wed = 4,
        Thursday = 8,
        Thu = 8,
        Friday = 16,
        Fri = 16,
        Saturday = 32,
        Sat = 32,
        Sunday = 64,
        Sun = 64,

        Weekend = Sat | Sun,
        Workweek = Mon | Tue | Wed | Thu | Fri,
        Any = Weekend | Workweek
    }
}