using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Models.ViewModel
{
    public class NationalParkAndTrailVM
    {
        public IEnumerable<Trail> TrailList { get; set; }
        public IEnumerable<NationalPark> NationalParkList { get; set; }
    }
}
