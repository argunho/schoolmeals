using Microsoft.JSInterop;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Intefice
{
    public interface IHelpRepository
    {
        int ReturnWeek(DateTime date);

        //string DayOfWeek(DateTime date);
    }
}
