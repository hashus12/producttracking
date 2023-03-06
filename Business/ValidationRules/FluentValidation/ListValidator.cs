using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    internal class ListValidator : AbstractValidator<List>
    {
        public ListValidator()
        {
            RuleFor(l => l.ListName).NotEmpty();
            RuleFor(l => l.ListName).MinimumLength(2);
            RuleFor(l => l.CompletedDate).NotEmpty();
            RuleFor(l => l.CreatedDate).NotEmpty();
        }
    }
  
}
