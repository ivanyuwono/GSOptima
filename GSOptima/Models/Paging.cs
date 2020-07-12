
using System.Linq;
using System.Linq.Expressions;


namespace GSOptima.ViewModels
{

    public class TableHeader
    {
        public PagingAttribute pagingAttribute;
        public string sortBy;
        public string columnText;
        public string dataBreakpoint;

    }
    public class PagingAttribute
    {
        public int recordsTotal { get; set; }
        public int currentPage { get; set; }
        public int totalPage { get; set; }
        public int recordPerPage { get; set; }
        public int filteredRecord { get; set; }
        public string sorting { get; set; }
        public string filter { get; set; }
        public string formID { get; set; }
        public string url { get; set; }
        public string divName { get; set; }
    }
    public class Paging<T>
    {

        public IQueryable<T> data { get; set; }
        public PagingAttribute attribute { get; set;}
        //public int recordsTotal { get; set; }
        //public int currentPage { get; set; }
        //public int totalPage { get; set; }
        //public int recordPerPage { get; set; }
        //public int filteredRecord { get; set; }
        //public string sorting { get; set; }
        //public string filter { get; set; }

        public Paging()
        {
            attribute = new PagingAttribute();
        }
        public void OrderBy(string propertyName)
        {
            
            var methodName = "";
            if (propertyName.Contains("DESC"))
            {
                methodName = "OrderByDescending";
                propertyName = propertyName.Substring(0, propertyName.Length - 5);
            }
            else
                methodName = "OrderBy";


            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);



            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == methodName && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { data, lambda });

            //return (IOrderedQueryable<TSource>)result;
            data = (IQueryable<T>)result;
        }

        public void DoPaging(int page_index)
        {

            data = data.Skip((page_index - 1) * attribute.recordPerPage).Take(attribute.recordPerPage);
            attribute.currentPage = page_index;
            attribute.filteredRecord = data.Count();
        }
    }

}
