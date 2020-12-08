using System.Collections.Generic;

namespace PCE_Web.Controllers
{
    public interface IService<T> where T : class
    {
        IEnumerable<T> Get();
    }

}