namespace Travaloud.Domain.Stock;

public abstract class Product(string name, DefaultIdType categoryId, DefaultIdType unitId, bool packaged, int quantityPerPackage,
        string containerNoun, decimal volume, string? salesNoun, string? openNoun, decimal? saleVolume,
        DefaultIdType? saleUnitId, string? tags, bool? requiresVat, bool? trialProduct, bool? staffCreated,
        bool? weighedOnDelivery, string? imageUrl)
    : AuditableEntity, IAggregateRoot
{
    private string Name { get; set; } = name;
    private DefaultIdType CategoryId { get; set; } = categoryId;
    private DefaultIdType UnitId { get; set; } = unitId;
    private bool Packaged { get; set; } = packaged;
    private int QuantityPerPackage { get; set; } = quantityPerPackage;
    private string ContainerNoun { get; set; } = containerNoun;
    private decimal Volume { get; set; } = volume;
    private string? SalesNoun { get; set; } = salesNoun;
    private string? OpenNoun { get; set; } = openNoun;
    private decimal? SaleVolume { get; set; } = saleVolume;
    private DefaultIdType? SaleUnitId { get; set; } = saleUnitId;
    private string? Tags { get; set; } = tags;
    private bool? RequiresVAT { get; set; } = requiresVat;
    private bool? TrialProduct { get; set; } = trialProduct;
    private bool? StaffCreated { get; set; } = staffCreated;
    private bool? WeighedOnDelivery { get; set; } = weighedOnDelivery;
    private string? ImageUrl { get; set; } = imageUrl;

    public virtual Category Category { get; set; } = default!;
    public virtual Unit Unit { get; set; } = default!;
    public virtual Unit SaleUnit { get; set; } = default!;
    public virtual IEnumerable<ProductBarcode> Barcodes { get; set; } = default!;

    public Product Update(
        string? name,
        DefaultIdType? categoryId,
        DefaultIdType? unitId,
        bool? packaged,
        int? quantityPerPackage,
        string? containerNoun,
        decimal? volume,
        string? salesNoun,
        string? openNoun,
        decimal? saleVolume,
        DefaultIdType? saleUnitId,
        string? tags,
        bool? requiresVAT,
        bool? trialProduct,
        bool? staffCreated,
        bool? weighedOnDelivery,
        string? imageUrl)
    {
        if (name is not null && Name != name)
            Name = name;

        if (categoryId is not null && CategoryId != categoryId.Value)
            CategoryId = categoryId.Value;

        if (unitId is not null && UnitId != unitId.Value)
            UnitId = unitId.Value;

        if (packaged is not null && Packaged != packaged.Value)
            Packaged = packaged.Value;

        if (quantityPerPackage is not null && QuantityPerPackage != quantityPerPackage.Value)
            QuantityPerPackage = quantityPerPackage.Value;

        if (containerNoun is not null && ContainerNoun != containerNoun)
            ContainerNoun = containerNoun;

        if (volume is not null && Volume != volume.Value)
            Volume = volume.Value;

        if (salesNoun is not null && SalesNoun != salesNoun)
            SalesNoun = salesNoun;

        if (openNoun is not null && OpenNoun != openNoun)
            OpenNoun = openNoun;

        if (saleVolume is not null && SaleVolume != saleVolume.Value)
            SaleVolume = saleVolume.Value;

        if (saleUnitId is not null && SaleUnitId != saleUnitId.Value)
            SaleUnitId = saleUnitId.Value;

        if (tags is not null && Tags != tags)
            Tags = tags;

        if (requiresVAT is not null && RequiresVAT != requiresVAT.Value)
            RequiresVAT = requiresVAT.Value;

        if (trialProduct is not null && TrialProduct != trialProduct.Value)
            TrialProduct = trialProduct.Value;

        if (staffCreated is not null && StaffCreated != staffCreated.Value)
            StaffCreated = staffCreated.Value;

        if (weighedOnDelivery is not null && WeighedOnDelivery != weighedOnDelivery.Value)
            WeighedOnDelivery = weighedOnDelivery.Value;

        if (imageUrl is not null && ImageUrl != imageUrl)
            ImageUrl = imageUrl;

        return this;
    }
}