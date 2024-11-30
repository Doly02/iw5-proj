using Forms.Common.Models.Response;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages;

public partial class ResponseDetailPage : ComponentBase
{
    [Inject]
    private ResponseFacade ResponseFacade { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public Guid Id { get; set; }

    private ResponseDetailModel? Response { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Response = await ResponseFacade.GetByIdAsync(Id);
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/ResponseListPage");
    }
}