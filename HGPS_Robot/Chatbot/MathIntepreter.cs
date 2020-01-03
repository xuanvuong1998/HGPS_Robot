using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class MathIntepreter
    {        
        public static bool IsOperator(string word)
        {
            string[] ops = { "*", "/", "+", "-" };

            foreach (var item in ops)
            {
                if (item == word) return true;
            }
            return false;            
        }

        public static bool IsContainOperator(string query)
        {
            foreach (var item in query.Split(' '))
            {
                if (IsOperator(item)) return true;
            }

            return false;
        }
        
        // Make sure word is correct format first
        public static double ParseNumber(string word)
        {
            return double.Parse(word, NumberStyles.AllowThousands);
        }

        public static bool IsNumber(string word)
        {
            double num;
            bool res = double.TryParse(word, NumberStyles.AllowThousands
                    , CultureInfo.InvariantCulture, out num);

            return res;
            
        }
        public static bool IsContainOperand(string query)
        {
            var words = query.Split(' ');
            int numberOccurCnt = 0;
            foreach (var word in words)
            {
                if (IsNumber(word))
                {
                    numberOccurCnt++;
                }
            }

            return numberOccurCnt >= 2;
        }

        public static bool IsContainMathOperation(string query)
        {
            return (IsContainOperand(query) && IsContainOperator(query));
        }
                        

        public static string ProcessOperation(string query)
        {            
            string[] words = query.Split(' ');

            int i;
            string que = "";
            for(i = 0; i < words.Length; i++)
            {
                if (IsOperator(words[i]))
                {
                    if (i == 0 || !IsNumber(words[i - 1])) return "invalid";                                        
                    bool orderFlag = true; // true: operator, false: operand                    
                    double tmp = ParseNumber(words[i - 1]);
                    words[i - 1] = "" + tmp;
                    que = words[i - 1];
                    while (i < words.Length && (IsNumber(words[i]) || IsOperator(words[i])))
                    {
                        if (orderFlag) // required operator
                        {
                            if (!IsOperator(words[i])) return "invalid";                            
                        }
                        else // required operand
                        {
                            if (!IsNumber(words[i])) return "invalid";

                            // in case number with commas

                            double tmp2 = ParseNumber(words[i]);
                            words[i] = "" + tmp2;
                        }
                        que += words[i];
                        i++;
                        orderFlag = !orderFlag;
                    }
                    break;
                }
            }

            if (que == "") return "invalid";    

            var res = new DataTable().Compute(que, null).ToString();

            if (res.Contains(".")) 
            {
                res = res.Substring(0, Math.Min(res.IndexOf(".") + 3, res.Length));
            }
            if (res == "∞") res = "Infinity";            
            return res;
        }
        
    }
}
