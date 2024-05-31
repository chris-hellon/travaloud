using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travaloud.Domain.PointOfSale;

namespace Travaloud.Infrastructure.Persistence.Configuration;
//
// public class DealConfig : IEntityTypeConfiguration<Deal>
// {
//     public void Configure(EntityTypeBuilder<Deal> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Deals", SchemaNames.PointOfSale);
//     }
// }
//
// public class DealPriceTierConfig : IEntityTypeConfiguration<DealPriceTier>
// {
//     public void Configure(EntityTypeBuilder<DealPriceTier> builder)
//     {
//         builder.ToTable("DealPriceTiers", SchemaNames.PointOfSale);
//     }
// }
//
// public class FloorPlanConfig : IEntityTypeConfiguration<FloorPlan>
// {
//     public void Configure(EntityTypeBuilder<FloorPlan> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("FloorPlans", SchemaNames.PointOfSale);
//     }
// }
//
// public class FloorPlanTableConfig : IEntityTypeConfiguration<FloorPlanTable>
// {
//     public void Configure(EntityTypeBuilder<FloorPlanTable> builder)
//     {
//         builder.ToTable("FloorPlanTables", SchemaNames.PointOfSale);
//     }
// }
//
// public class MenuItemConfig : IEntityTypeConfiguration<MenuItem>
// {
//     public void Configure(EntityTypeBuilder<MenuItem> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("MenuItems", SchemaNames.PointOfSale);
//     }
// }
//
// public class MenuItemCategoryConfig : IEntityTypeConfiguration<MenuItemCategory>
// {
//     public void Configure(EntityTypeBuilder<MenuItemCategory> builder)
//     {
//         builder.ToTable("MenuItemCategory", SchemaNames.PointOfSale);
//     }
// }