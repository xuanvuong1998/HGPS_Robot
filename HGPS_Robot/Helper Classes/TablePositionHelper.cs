using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class TablePositionHelper
    {
        public static List<TablePosition> TablePositions { get; set; } = new List<TablePosition>();
        public static List<string> ChosenStudentList { get; set; } = new List<string>(); //  List of chosen students for asking a question
        public static string LatestChosenStudent {get; set;} // Latest PICKED UP student for asking

        private static Random rand = new Random();

        public static void LoadTablesInfo()
        {
            TablePositions = WebHelper.GetTablePositions();
        }

        public static void DeleteChosenStudentList()
        {
            ChosenStudentList.Clear();
            LatestChosenStudent = null;
        }

        /// <summary>
        /// Get all students member by group number
        /// </summary>
        /// <param name="gnum"></param>
        /// <returns></returns>
        public static List<string> GetMembersByGroupNumber(int gnum)
        {
            return TablePositions.Where(x => x.GroupNumber == gnum)
                .Select(x => x.Student_Id).ToList();
        }

        private static bool IsLeftGroup(string tableNum)
        {
            return int.Parse(tableNum.Substring(1)) % 2 == 1;
        }


        public static List<string> GetLeftGroupMembers()
        {
            List<string> list = new List<string>();

            foreach (var table in TablePositions)
            {
                if (IsLeftGroup(table.TableName))
                {
                    list.Add(table.Student_Id);
                }
            }
            return list;
        }

        public static List<string> GetRightGroupMembers()
        {
            List<string> list = new List<string>();

            foreach (var table in TablePositions)
            {
                if (IsLeftGroup(table.TableName) == false)
                {
                    list.Add(table.Student_Id);
                }
            }
            return list;
        }

        public static int GetGroupQuantity()
        {
            return TablePositions.Select(x => x.GroupNumber).Distinct().Count();
        }

        public static void DeleteAllPositions()
        {
            WebHelper.DeleteAllTablePositions();
        }

        public static string FindTablePosByStdId(string stdId)
        {
            return TablePositions.Where(x => x.Student_Id == stdId)
                .Select(x => x.TableName).FirstOrDefault();
        }
        
        public static void RandomStudentForAsking()
        {
            string rdmStd = null;

            if (ChosenStudentList.Count == TablePositions.Count) ChosenStudentList.Clear();

            do
            {
                rdmStd = TablePositions[rand.Next(TablePositions.Count)].Student_Id;
            } while (ChosenStudentList.Contains(rdmStd));

            ChosenStudentList.Add(rdmStd);

            LatestChosenStudent = rdmStd;
        }
        
    }
}
