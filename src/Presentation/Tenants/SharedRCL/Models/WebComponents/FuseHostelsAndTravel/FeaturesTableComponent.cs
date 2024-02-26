using Travaloud.Application.Catalog.Properties.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class FeaturesTableComponent
{
    public string Title { get; private set; }
    public IEnumerable<PropertyFacilityDto> Features { get; private set; }
    public List<List<string>> FeaturesParsed { get; private set; }

    public FeaturesTableComponent()
    {

    }

    public FeaturesTableComponent(string title, IEnumerable<PropertyFacilityDto> features)
    {
        Title = title;
        Features = features;
        FeaturesParsed = new List<List<string>>();

        var featuresColumnsCount = 4;
        for (var i = 0; i < featuresColumnsCount; i++)
        {
            FeaturesParsed.Add(new List<string>());
        }

        if (features.Any())
        {
            var featuresParsedIndex = 0;
            for (var i = 0; i < features.Count(); i++)
            {
                FeaturesParsed[featuresParsedIndex].Add(features.ToList()[i].Title);
                featuresParsedIndex++;

                if (featuresParsedIndex == 4)
                    featuresParsedIndex = 0;
            }
        }
    }
}