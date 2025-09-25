using ToggleHub.Domain;

namespace ToggleHub.Application.DTOs;

public class PagedListDto<T> 
{
    public PagedListDto(IEnumerable<T> data, int total = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        Data = data;
        Total = total;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public IEnumerable<T> Data { get; set; }
    public int Total { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
    public bool HasNextPage => PageIndex + 1 < TotalPages;
    public bool HasPreviousPage => PageIndex > 0;
}