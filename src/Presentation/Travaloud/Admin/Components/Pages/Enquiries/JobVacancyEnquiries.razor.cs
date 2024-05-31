using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Commands;
using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Enquiries;

public partial class JobVacancyEnquiries
{
    [Inject] protected IJobVacancyResponsesService JobVacancyResponsesService { get; set; } = default!;

    private EntityServerTableContext<JobVacancyResponseDto, DefaultIdType, JobVacancyResponseViewModel> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<JobVacancyResponseDto, DefaultIdType, JobVacancyResponseViewModel> _table = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<JobVacancyResponseDto, DefaultIdType, JobVacancyResponseViewModel>(
            entityName: L["Job Vacancy Enquiry"],
            entityNamePlural: L["Job Vacancy Enquiries"],
            entityResource: TravaloudResource.Enquiries,
            fields:
            [
                new EntityField<JobVacancyResponseDto>(enquiry => enquiry.JobVacancy.JobTitle, L["Job Title"], "JobVacancy.JobTitle"),
                new EntityField<JobVacancyResponseDto>(enquiry => enquiry.JobVacancy.Location, L["Location"], "JobVacancy.Location"),
                new EntityField<JobVacancyResponseDto>(enquiry => $"{enquiry.FirstName} {enquiry.LastName}", L["Name"], "FirstName"),
                new EntityField<JobVacancyResponseDto>(enquiry => enquiry.Email, L["Email Address"], "Email"),
                new EntityField<JobVacancyResponseDto>(enquiry => enquiry.CreatedOn, L["Submit Date"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: enquiry => enquiry.Id,
            searchFunc: async filter => (await JobVacancyResponsesService
                    .SearchAsync(filter.Adapt<SearchJobVacancyResponsesRequest>()))
                .Adapt<PaginationResponse<JobVacancyResponseDto>>(),
            createAction: string.Empty,
            updateAction: string.Empty,
            exportAction: string.Empty,
            deleteAction: string.Empty
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
}

public class JobVacancyResponseViewModel : CreateJobVacancyResponseRequest
{
    public JobVacancyDto JobVacancy { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = default!;
}