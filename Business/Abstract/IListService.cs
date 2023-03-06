using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IListService
    {
        IResult Add(List list);
        IDataResult<List<List>> GetAll();
        IDataResult<List> GetById(int listId);
    }
}
