using Microsoft.AspNetCore.Components;
using SchoolMeals.Client.Helpers;
using SchoolMeals.Client.Intefice;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Pages.Views
{
    public partial class SchoolView
    {
        [Inject] IHelpRepository _help { get; set; }
        [Inject] SingletonService singleton { get; set; }

        [Parameter] public string municipality { get; set; }
        [Parameter] public string place { get; set; }
        [Parameter] public string name { get; set; }

        public School school = new School();
        private bool Loading { get; set; } = true;
        private bool ReadMore { get; set; }
        private bool HideText { get; set; }
        private string Text { get; set; }
        public string EditLink { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //var link = municipality + "/" + place + "/" + name;
            school = await Http.GetFromJsonAsync<School>($"School/GetByParameters/{municipality}/{place}/{name}");
            if (school != null && school != new School())
            {
                Text = CutText(school.Text);
                EditLink = "edit-school/school/" + school.Id;
                singleton.Param = school.Municipality.Name;
            }

            Loading = false;
        }

        public string CutText(string text)
        {
            int maxLength = 400;
            HideText = false;
            var currentText = text.Substring(0, text.LastIndexOf(" "));
            if (text != null && currentText.Length > maxLength)
            {
                ReadMore = true;
                currentText = text.Substring(0, maxLength);
                return currentText.Substring(0, currentText.LastIndexOf(" ")) + " ...";
            }

            return text;
        }

        private void ReadHideText()
        {
            HideText = !HideText;
            if (HideText)
            {
                Text = school.Text;
            }
            else
            {
                Text = CutText(school.Text);
            }
        }
    }
}
