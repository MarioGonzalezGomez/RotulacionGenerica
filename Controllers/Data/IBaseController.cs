using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Controllers.Data;
public interface IBaseController<T>
{
    public List<T> GetAll();
    public T GetById(int id);
    public T Post(T t);
    public T Put(T t);
    public T Delete(T t);
}
