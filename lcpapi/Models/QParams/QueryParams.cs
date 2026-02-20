using System.Runtime.Serialization;

namespace lcpapi.Models.QParams;

public class QueryParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
    }

    public string? SortBy { get; set; } = "id";
    public SortOrderEnum? SortOrder { get; set; } = SortOrderEnum.asc;
    public string? Search { get; set; }
}

public enum SortOrderEnum
{
    [EnumMember(Value = "asc")]
    asc,
    [EnumMember(Value = "desc")]
    desc
}