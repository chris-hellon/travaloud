using Mapster;
using Microsoft.AspNetCore.Components;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.JobVacancies.Commands;
using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class JobVacancies
{
    [Inject] protected IJobVacanciesService JobVacanciesService { get; set; } = default!;

    private EntityServerTableContext<JobVacancyDto, DefaultIdType, UpdateJobVacancyRequest> Context { get; set; } = default!;
    private EntityTable<JobVacancyDto, DefaultIdType, UpdateJobVacancyRequest> _table = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<JobVacancyDto, DefaultIdType, UpdateJobVacancyRequest>(
            entityName: L["Job Vacancy"],
            entityNamePlural: L["Job Vacancies"],
            entityResource: TravaloudResource.JobVacancies,
            fields:
            [
                new EntityField<JobVacancyDto>(jobVacancy => jobVacancy.JobTitle, L["Job Title"], "JobTitle"),
                new EntityField<JobVacancyDto>(jobVacancy => jobVacancy.Location, L["Location"], "Location")
            ],
            enableAdvancedSearch: false,
            idFunc: jobVacancy => jobVacancy.Id,
            searchFunc: async filter => (await JobVacanciesService
                    .SearchAsync(filter.Adapt<SearchJobVacanciesRequest>()))
                .Adapt<PaginationResponse<JobVacancyDto>>(),
            createFunc: async jobVacancy =>
            {
                await JobVacanciesService.CreateAsync(jobVacancy.Adapt<CreateJobVacancyRequest>());
            },
            getDetailsFunc: async (id) =>
            {
                var jobVacancy = await JobVacanciesService.GetAsync(id);
                return jobVacancy.Adapt<UpdateJobVacancyRequest>();
            },
            updateFunc: async (id, jobVacancy) =>
            {
                await JobVacanciesService.UpdateAsync(id, jobVacancy.Adapt<UpdateJobVacancyRequest>());
            },
            exportAction: string.Empty,
            deleteFunc: async id => await JobVacanciesService.DeleteAsync(id)
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