using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SchoolMeals.Client.Helpers;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Pages.Authorized.Components
{
    public partial class ControlPanel
    {
        //[Inject] IHelpRepository repository { get; set; }
        [Parameter] public bool Admin { get; set; }
        [Parameter] public bool DisplayMenu { get; set; }
        [Parameter] public EventCallback<bool> HidePanel { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AutheticationState { get; set; }

        public School school = new School();
        public bool Access { get; set; }
        private int menuId { get; set; } = 0;
        private bool dropedDown { get; set; } = false;

        List<ControlPanelMenu> Menu;

        // Control panel menu list
        protected override async Task OnParametersSetAsync()
        {
            if (DisplayMenu)
            {
                var auth = await AutheticationState;
                var user = auth.User;
                if (user.Identity.IsAuthenticated)
                {
                    school = await Http.GetFromJsonAsync<School>("Users/GetUsersSchool/" + user.Identity.Name);
                    if (school != null && school != new School())
                    {
                        Access = true;
                    }
                }
            }
            else if(dropedDown)
                await DropdownMenu(0);

            Menu = new List<ControlPanelMenu>()
                    {
                        new ControlPanelMenu(){Id = 1, Name = "Lägga till ny", Visible = true, Submenus = new List<ControlPanelSubmenu>(){
                            new ControlPanelSubmenu(){Id = 1, Name = "Skolan", Link = "/new/school", Access = (!Access || Admin) },
                            new ControlPanelSubmenu(){Id = 2, Name = "Måltid", Link = $"/school/{school.Id}/new/meal",  Access = Access },
                            new ControlPanelSubmenu(){Id = 3, Name = "Recept", Link = $"/school/{school.Id}/new/recipe",  Access = Access }
                        }},
                        new ControlPanelMenu(){Id = 2, Name = "Samlings listor", Visible = Access, Submenus = new List<ControlPanelSubmenu>(){
                            new ControlPanelSubmenu(){Id = 1, Name = "Måltider", Link = $"/school/{school.Id}/meals" },
                            new ControlPanelSubmenu(){Id = 2, Name = "Recept", Link = $"/school/{school.Id}/recipes" }
                        }}
                    };
        }


        // Dropdown method
        private async Task DropdownMenu(int id)
        {
            if (menuId > 0 && menuId != id)
            {
                menuId = 0;
                await Task.Delay(1000);
                menuId = id;
            }
            else if (dropedDown)
            {
                dropedDown = false;
                menuId = 0;
                DisplayMenu = false;
                await HidePanel.InvokeAsync(false);
            }
            else
            {
                dropedDown = true;
                menuId = id;
            }
        }
    }

    public class ControlPanelMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public List<ControlPanelSubmenu> Submenus { get; set; }
    }
    public class ControlPanelSubmenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string ImgUrl { get; set; }
        public bool Access { get; set; } = true;
        public ControlPanelMenu Menu { get; set; }
    }
}
