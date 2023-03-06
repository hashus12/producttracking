using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<Product>> GetAll();
        IDataResult<List<Product>> GetAllByListId(int id);
        IDataResult<List<Product>> GetAllByListName(string listName);
        IDataResult<List<Product>> GetByAmount(decimal min, decimal max);
        IDataResult<Product> GetById(int productId);
        IResult Add(Product product);
        IResult Update(Product product);

    }
}
