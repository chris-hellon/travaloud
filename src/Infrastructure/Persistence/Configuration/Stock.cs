using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travaloud.Domain.Stock;

namespace Travaloud.Infrastructure.Persistence.Configuration;
//
// public class BarcodeConfig : IEntityTypeConfiguration<Barcode>
// {
//     public void Configure(EntityTypeBuilder<Barcode> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Barcodes", SchemaNames.Stock);
//     }
// }
//
// public class CategoryConfig : IEntityTypeConfiguration<Category>
// {
//     public void Configure(EntityTypeBuilder<Category> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Categories", SchemaNames.Stock);
//     }
// }
//
// public class CategoryTypeConfig : IEntityTypeConfiguration<CategoryType>
// {
//     public void Configure(EntityTypeBuilder<CategoryType> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("CategoryTypes", SchemaNames.Stock);
//     }
// }
//
// public class OrderConfig : IEntityTypeConfiguration<Order>
// {
//     public void Configure(EntityTypeBuilder<Order> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Orders", SchemaNames.Stock);
//     }
// }
//
// public class OrderProductConfig : IEntityTypeConfiguration<OrderProduct>
// {
//     public void Configure(EntityTypeBuilder<OrderProduct> builder)
//     {
//         builder.ToTable("OrderProducts", SchemaNames.Stock);
//     }
// }
//
// public class OrderSupplierNoteConfig : IEntityTypeConfiguration<OrderSupplierNote>
// {
//     public void Configure(EntityTypeBuilder<OrderSupplierNote> builder)
//     {
//         builder.ToTable("OrderSupplierNotes", SchemaNames.Stock);
//     }
// }
//
// public class ProductConfig : IEntityTypeConfiguration<Product>
// {
//     public void Configure(EntityTypeBuilder<Product> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Products", SchemaNames.Stock);
//     }
// }
//
// public class ProductBarcodeConfig : IEntityTypeConfiguration<ProductBarcode>
// {
//     public void Configure(EntityTypeBuilder<ProductBarcode> builder)
//     {
//         builder.ToTable("ProductBarcodes", SchemaNames.Stock);
//     }
// }
//
// public class ReasonConfig : IEntityTypeConfiguration<Reason>
// {
//     public void Configure(EntityTypeBuilder<Reason> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Reasons", SchemaNames.Stock);
//     }
// }
//
// public class ReportConfig : IEntityTypeConfiguration<Report>
// {
//     public void Configure(EntityTypeBuilder<Report> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Reports", SchemaNames.Stock);
//     }
// }
//
// public class ReportProductConfig : IEntityTypeConfiguration<ReportProduct>
// {
//     public void Configure(EntityTypeBuilder<ReportProduct> builder)
//     {
//         builder.ToTable("ReportProducts", SchemaNames.Stock);
//     }
// }
//
// public class ReturnConfig : IEntityTypeConfiguration<Return>
// {
//     public void Configure(EntityTypeBuilder<Return> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Returns", SchemaNames.Stock);
//     }
// }
//
// public class SupplierOfferConfig : IEntityTypeConfiguration<SupplierOffer>
// {
//     public void Configure(EntityTypeBuilder<SupplierOffer> builder)
//     {
//         builder.ToTable("SupplierOffers", SchemaNames.Stock);
//     }
// }
//
// public class SupplierOfferConditionConfig : IEntityTypeConfiguration<SupplierOfferCondition>
// {
//     public void Configure(EntityTypeBuilder<SupplierOfferCondition> builder)
//     {
//         builder.ToTable("SupplierOfferConditions", SchemaNames.Stock);
//     }
// }
//
// public class SupplierOfferRewardConfig : IEntityTypeConfiguration<SupplierOfferReward>
// {
//     public void Configure(EntityTypeBuilder<SupplierOfferReward> builder)
//     {
//         builder.ToTable("SupplierOfferRewards", SchemaNames.Stock);
//     }
// }
//
// public class SupplierProductConfig : IEntityTypeConfiguration<SupplierProduct>
// {
//     public void Configure(EntityTypeBuilder<SupplierProduct> builder)
//     {
//         builder.ToTable("SupplierProducts", SchemaNames.Stock);
//     }
// }
//
// public class TransferConfig : IEntityTypeConfiguration<Transfer>
// {
//     public void Configure(EntityTypeBuilder<Transfer> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Transfers", SchemaNames.Stock);
//     }
// }
//
// public class UnitConfig : IEntityTypeConfiguration<Unit>
// {
//     public void Configure(EntityTypeBuilder<Unit> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Units", SchemaNames.Stock);
//     }
// }
//
// public class WasteConfig : IEntityTypeConfiguration<Waste>
// {
//     public void Configure(EntityTypeBuilder<Waste> builder)
//     {
//         builder.IsMultiTenant();
//         builder.ToTable("Wastes", SchemaNames.Stock);
//     }
//}