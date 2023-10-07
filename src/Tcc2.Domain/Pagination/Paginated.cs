using Tcc2.Domain.Entities;

namespace Tcc2.Domain.Pagination;

public class Paginated<T>
{
    private readonly int _pageIndex;
    private readonly int _pageSize;
    private readonly int _totalPages;
    private readonly IReadOnlyCollection<T> _items;

    public Paginated(
        int pageIndex, 
        int pageSize, 
        IReadOnlyCollection<T> items, 
        int? totalItems = null, 
        int? totalPages = null)
    {
        if (totalItems is null && totalPages is null)
        {
            throw new ArgumentException(
                $"Arguments {nameof(totalItems)} and {nameof(totalPages)} cannot be null at the same time");
        }

        _pageIndex = pageIndex;
        _pageSize = pageSize;
        totalPages ??= (int)Math.Ceiling((decimal)totalItems! / pageSize);
        _totalPages = (int)totalPages!;
        _items = items;
    }

    public int PageIndex => _pageIndex;
    public int PageSize => _pageSize;
    public int TotalPages => _totalPages;
    public IReadOnlyCollection<T> Items => _items;
}
