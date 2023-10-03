using Tcc2.Domain.Entities;

namespace Tcc2.Domain.Pagination;

public class Paginated<T> where T : IEntity
{
    private readonly int _pageIndex;
    private readonly int _pageSize;
    private readonly int _totalPages;
    private readonly IReadOnlyCollection<T> _items;

    public Paginated(int pageIndex, int pageSize, int totalItems, IReadOnlyCollection<T> items)
    {
        _pageIndex = pageIndex;
        _pageSize = pageSize;
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
        _totalPages = totalPages;
        _items = items;
    }

    public int PageIndex => _pageIndex;
    public int PageSize => _pageSize;
    public int TotalPages => _totalPages;
    public IReadOnlyCollection<T> Items => _items;
}
