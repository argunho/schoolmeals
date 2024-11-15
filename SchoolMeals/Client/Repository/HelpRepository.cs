using Microsoft.JSInterop;
using SchoolMeals.Client.Intefice;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Repository
{
    public class HelpRepository : IHelpRepository
    {
        private readonly CultureInfo _ci;

        public HelpRepository()
        {
            _ci = new CultureInfo("sv-SE");
        }


        // Return number of week by date
        public int ReturnWeek(DateTime date)
        {
            // Gets the Calendar instance associated with a CultureInfo.
            //CultureInfo _ci = new CultureInfo("sv-SE");
            Calendar _cal = _ci.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule _cwr = _ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek _dow = _ci.DateTimeFormat.FirstDayOfWeek;

            return _cal.GetWeekOfYear(date, _cwr, _dow);
        }

    }
}
