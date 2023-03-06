using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class List : IEntity
    {
        public int ListId { get; set; }
        public string ListName { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string CompletedDate { get; set; }
    }
}
