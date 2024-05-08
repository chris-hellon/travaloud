﻿using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Color = MudBlazor.Color;

namespace Travaloud.Admin.Components.EntityTable;

public record EntityField<TEntity>(Func<TEntity, object?> ValueFunc, string DisplayName, string SortLabel = "", Type? Type = null, RenderFragment<TEntity>? Template = null, Func<TEntity, MudColor?>? Color = null)
{
    /// <summary>
    /// A function that returns the actual value of this field from the supplied entity.
    /// </summary>
    public Func<TEntity, object?> ValueFunc { get; init; } = ValueFunc;

    /// <summary>
    /// The string that's shown on the UI for this field.
    /// </summary>
    public string DisplayName { get; init; } = DisplayName;

    /// <summary>
    /// The string that's sent to the api as property to sort on for this field.
    /// This is only relevant when using server side sorting.
    /// </summary>
    public string SortLabel { get; init; } = SortLabel;

    /// <summary>
    /// The type of the field. Default is string, but when boolean, it shows as a checkbox.
    /// </summary>
    public Type? Type { get; init; } = Type;

    /// <summary>
    /// When supplied this template will be used for this field in stead of the default template.
    /// For an example on how to do this, see <see cref="Personal.AuditLogs"/>.
    /// </summary>
    public RenderFragment<TEntity>? Template { get; init; } = Template;

    public bool CheckedForSearch { get; set; } = true;
}