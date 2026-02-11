namespace Ecom.API.Helper
{
    public class Pagination<T>where T : class
    {
        public Pagination(int pageNumber, int pageSize, int totalCount, IEnumerable<T> data)
        {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            this.totalCount = totalCount;
            Data = data;
        }

        public int pageNumber {  get; set; }
        public int pageSize{  get; set; }
        public int totalCount{  get; set; }
        public IEnumerable<T> Data { get; set; }

        

    }
}
