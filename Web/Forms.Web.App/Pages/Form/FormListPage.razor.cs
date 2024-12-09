using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common;
using Forms.Common.Models.Form;
using Forms.Common.Models.User;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Forms.Web.App.Pages
{
    public partial class FormListPage
    {
        [Inject]
        private FormFacade FormFacade { get; set; } = null!;
        
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        
        private readonly IMapper _mapper;
        private Guid CurrentUserId { get; set; }
        private bool IsAdmin { get; set; }
        private ICollection<FormListModel> Forms { get; set; } = new List<FormListModel>();


        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("Uživatel není přihlášen.");
            }
            
            var userIdClaim = user.FindFirst("Id");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("Nelze načíst ID uživatele.");
            }

            CurrentUserId = Guid.Parse(userIdClaim.Value);
            IsAdmin = user.Claims.Any(c => c.Type == "role" && c.Value == AppRoles.Admin);
            
            Forms = await FormFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }
    }
}