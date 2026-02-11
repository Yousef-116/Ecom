using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Sharing
{
    public class ProductParams
    {
        private const int MaxPageSize = 50;

        public int? CategoryId { get; set; }
        public string? Search { get; set; } 
        public string Sort { get; set; } = string.Empty;

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value <= 0)
                ? 20
                : (value > MaxPageSize ? MaxPageSize : value);
        }
    }


}
