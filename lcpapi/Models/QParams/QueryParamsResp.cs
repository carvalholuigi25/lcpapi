namespace lcpapi.Models;

public class QueryParamsResp<T> where T : class   
{
    public int TotalCount { get; set; } = 0;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public IList<T>? Data { get; set; } = new List<T>();
}
