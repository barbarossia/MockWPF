using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.Common
{
    public class PagedList<T> : List<T>
    {
        public int StartIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int PageCount
        {
            get
            {
                if ((TotalCount % PageSize) > 0)
                    return (TotalCount / PageSize) + 1;
                else
                    return TotalCount / PageSize;
            }
        }

        private PagedList(IEnumerable<T> pagedList, int startIndex, int pageSize, int totalCount)
            : base(pagedList)
        {
            StartIndex = startIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public PagedList(IQueryable<T> dataSource, int startIndex, int pageSize)
            : base(dataSource.Skip(startIndex).Take(pageSize))
        {
            StartIndex = startIndex;
            PageSize = pageSize;
            TotalCount = dataSource.Count();
        }

        public PagedList<TResult> Transform<TResult>(Func<T, TResult> selector)
        {
            return new PagedList<TResult>(this.Select(selector), StartIndex, PageSize, TotalCount);
        }
    }
}
