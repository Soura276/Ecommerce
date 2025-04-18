﻿namespace Catalog.Core.Specs
{
    public class Pagination<T> where T : class
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public long Count { get; }
        public IReadOnlyList<T> Data { get; }

        public Pagination(int pageIndex, int pageSize, long count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
    }
}
