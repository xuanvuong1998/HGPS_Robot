using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class StudentGroup
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<string> Member { get; set; }
        
        public int Rank { get; set; }
        
    }
}
