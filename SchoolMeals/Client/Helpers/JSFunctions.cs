using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Helpers
{
    public static class JSFunctions
    {
        public static async ValueTask GoBack(this IJSRuntime js)
        {
            await js.InvokeVoidAsync("history.back");
        }
        public static async ValueTask<bool> Confirm(this IJSRuntime js, string message)
        {
            return await js.InvokeAsync<bool>("confirm", message);
        }

        public static async ValueTask ScrollToElement(this IJSRuntime js, string id)
        {
            await js.InvokeVoidAsync("scroll_to_element", id);
        }
    }
}
