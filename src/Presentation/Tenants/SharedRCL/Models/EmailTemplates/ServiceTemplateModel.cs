using Travaloud.Application.Catalog.Services.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

public class ServiceTemplateModel : EmailTemplateBaseModel
{
	public ServiceDetailsDto Service { get; set; }
	public ServiceTemplateModel()
	{
	}

	public ServiceTemplateModel(string tenantName, string? primaryBackgroundColor, string? secondaryBackgroundColor, string? headerBackgroundColor, string? textColor, string? logoImageUrl, ServiceDetailsDto service) : base(tenantName, primaryBackgroundColor, secondaryBackgroundColor, headerBackgroundColor, textColor, logoImageUrl)
	{
		Service = service;
	}
}