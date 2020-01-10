using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class StudentPositionHelper
    {
        public static List<TablePosition> TablePositions { get; set; }

        public static void LoadTablesInfo()
        {
            TablePositions = WebHelper.GetTablePositions();
        }

        public static string FindTablePosByStdId(string stdId)
        {
            return TablePositions.Where(x => x.Student_Id == stdId)
                .Select(x => x.TableName).FirstOrDefault();
        }
    }
}
