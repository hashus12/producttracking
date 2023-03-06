using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{

    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        IListService _listService;
        IListDal _listDal;

        public ProductManager(IProductDal productDal,IListService listService, IListDal listDal)
        {
            _productDal = productDal;
            _listService = listService;
            _listDal = listDal;
        }


        //Claim
        [SecuredOperation("user")]
        [LogAspect(typeof(MongoDbLogger))]
        [ValidationAspect(typeof(ProductValidator))]
        //[CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Product product)
        {

            //Aynı isimde ürün eklenemez
            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName));

            if (result != null)
            {
                return result;
            }

            _productDal.Add(product);

            return new SuccessResult(Messages.ProductAdded);

        }

        [SecuredOperation("admin")]
        [CacheAspect(10)] //key,value
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List<Product>> GetAll()
        {
            if (DateTime.Now.Hour == 2)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }

            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAllByListId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.ListId == id));
        }

        public IDataResult<List<Product>> GetAllByListName(string listName)
        {
            List List = _listDal.Get(l => l.ListName == listName);
            return GetAllByListId(List.ListId);
            
        }

        [CacheAspect(10)]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        [CacheAspect(10)]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List<Product>> GetByAmount(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.Amount >= min && p.Amount <= max));
        }


        [SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        [LogAspect(typeof(MongoDbLogger))]
        public IResult Update(Product product)
        {
            var result = _productDal.GetAll(p => p.ListId == product.ListId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfListError);
            }
            throw new NotImplementedException();
        }


        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        [LogAspect(typeof(MongoDbLogger))]
        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }


    }
}
