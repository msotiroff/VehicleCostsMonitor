namespace VehicleCostsMonitor.Web.Infrastructure.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using Interfaces;

    public class PaginatedList<T> : IPaginatedList, IEnumerable<T>
    {
        private IEnumerable<T> data;
        
        public PaginatedList(IEnumerable<T> items, int pageIndex, int totalPages)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;

            this.data = items;
        }

        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public IEnumerator<T> GetEnumerator() => this.data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
