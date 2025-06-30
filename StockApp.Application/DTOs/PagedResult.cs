using System;
using System.Collections.Generic;

namespace StockApp.Application.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public IEnumerable<T> Items => Data; // Alias for compatibility
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
        
        public int TotalCount => TotalRecords;
        public int CurrentPage => PageNumber;

        public PagedResult()
        {
        }

        public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }
    }
}