using System.Drawing;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Utilities;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Events.Commands;
using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class Events
{
    [Inject] protected IEventsService EventsService { get; set; } = default!;

    private EntityServerTableContext<EventDto, Guid, EventViewModel> Context { get; set; } = default!;
    private EntityTable<EventDto, Guid, EventViewModel> _table = default!;

    private MudColorPicker ColorPicker { get; set; } = default!;

    public IEnumerable<MudColor> CustomPalette { get; set; } = new MudColor[]
    {
        "#D1AC00", "#FB62F6", "#FF6666", "#ab3dff"
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<EventDto, Guid, EventViewModel>(
            entityName: L["Event"],
            entityNamePlural: L["Events"],
            entityResource: TravaloudResource.Events,
            fields: [new EntityField<EventDto>(@event => @event.Name, L["Name"], "Name")],
            enableAdvancedSearch: false,
            idFunc: @event => @event.Id,
            searchFunc: async filter => (await EventsService
                    .SearchAsync(filter.Adapt<SearchEventsRequest>()))
                .Adapt<PaginationResponse<EventDto>>(),
            createFunc: async @event =>
            {
                if (!string.IsNullOrEmpty(@event.ImageInBytes))
                {
                    @event.Image = new FileUploadRequest()
                    {
                        Data = @event.ImageInBytes, Extension = @event.ImageExtension ?? string.Empty,
                        Name = $"{@event.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (@event.BackgroundColor != null)
                {
                    @event.BackgroundColor = GenerateRgba(@event.BackgroundColor);
                    @event.ShortDescription = @event.Description;

                    if (@event.BackgroundColor == "0, 0, 0")
                        @event.BackgroundColor = null;
                }

                await EventsService.CreateAsync(@event.Adapt<CreateEventRequest>());
                @event.ImageInBytes = string.Empty;
            },
            getDetailsFunc: async (id) =>
            {
                var @event = await EventsService.GetAsync(id);

                if (!string.IsNullOrEmpty(@event.BackgroundColor))
                    @event.BackgroundColor = GenerateHex(@event.BackgroundColor);
                return @event.Adapt<EventViewModel>();
            },
            updateFunc: async (id, @event) =>
            {
                if (!string.IsNullOrEmpty(@event.ImageInBytes))
                {
                    @event.DeleteCurrentImage = true;
                    @event.Image = new FileUploadRequest()
                    {
                        Data = @event.ImageInBytes, Extension = @event.ImageExtension ?? string.Empty,
                        Name = $"{@event.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (@event.BackgroundColor != null)
                {
                    @event.BackgroundColor = GenerateRgba(@event.BackgroundColor);
                    @event.ShortDescription = @event.Description;
                    if (@event.BackgroundColor == "0, 0, 0")
                        @event.BackgroundColor = null;
                }

                await EventsService.UpdateAsync(id, @event.Adapt<UpdateEventRequest>());
                @event.ImageInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await EventsService.DeleteAsync(id)
        );
    }

    private string? _searchName;

    private string SearchName
    {
        get => _searchName ?? string.Empty;
        set
        {
            _searchName = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private static string GenerateRgba(string backgroundColor)
    {
        var color = ColorTranslator.FromHtml(backgroundColor);
        int r = Convert.ToInt16(color.R);
        int g = Convert.ToInt16(color.G);
        int b = Convert.ToInt16(color.B);
        return $"{r}, {g}, {b}";
    }

    private static string GenerateHex(string backgroundColor)
    {
        var rgb = backgroundColor.Split(',').Select(int.Parse).ToArray();

        var color = System.Drawing.Color.FromArgb(rgb[0], rgb[1], rgb[2]);

        return ColorTranslator.ToHtml(color);
    }

    public void ClearImageInBytes()
    {
        if (Context.AddEditModal?.RequestModel == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentImageFlag()
    {
        if (Context.AddEditModal?.RequestModel == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.ImagePath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                Context.AddEditModal.RequestModel.ImageExtension = fileUploadDetails.Extension;
                Context.AddEditModal.RequestModel.ImageInBytes = fileUploadDetails.FileInBytes;
                Context.AddEditModal.ForceRender();
            }
        }
    }
}

public class EventViewModel : UpdateEventRequest
{
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public MudColor? MudBackgroundColor { get; set; }
}