using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Poordooytify.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool InActive { get; set; }

    }
}