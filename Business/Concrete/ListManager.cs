using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class ListManager : IListService
    {
        IListDal _listDal;

        public ListManager(IListDal listDal)
        {
            _listDal = listDal;
        }

        [SecuredOperation("user")]
        [LogAspect(typeof(MongoDbLogger))]
        [ValidationAspect(typeof(ListValidator))]
        //[CacheRemoveAspect("IListService.Get")]
        public IResult Add(List list)
        {
            _listDal.Add(list);
            return new SuccessResult(Messages.ListAdded);
        }

        [SecuredOperation("admin")]
        [CacheAspect(10)]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List<List>> GetAll()
        {
            //Business codes
            return new SuccessDataResult<List<List>>(_listDal.GetAll());
        }

        //Select * from Listes where ListId = 3
        [CacheAspect(10)]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List> GetById(int listId)
        {
            return new SuccessDataResult<List>(_listDal.Get(c=>c.ListId == listId));
        }
    }
}
