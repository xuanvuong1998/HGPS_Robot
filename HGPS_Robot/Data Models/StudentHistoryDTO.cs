using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class StudentHistoryDTO
    {
        public string Student_id { get; set; }
        public int LessonHistory_id { get; set; }
        public string UserAccountFullName { get; set; }
        public int TotalPointAwarded { get; set; }
        public string ResultInBinaryString { get; set; }
    }
}
