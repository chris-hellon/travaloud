namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class PartialModel
{
    public int? Index { get; set; }
    public string ListName { get; set; }

    public string PropertyId(string property)
    {
        return _idBase + property;
    }

    public string PropertyName(string property)
    {
        return _nameBase + property;
    }

    private string _idBase
    {
        get
        {
            return $"{ListName}_{Index}__";
        }
    }
    private string _nameBase
    {
        get
        {
            return $"{ListName}[{Index}].";
        }
    }

    public PartialModel()
    {
    }

    public PartialModel(int? index)
    {
        Index = index;
    }

    public PartialModel(string listName)
    {
        ListName = listName;
    }
}