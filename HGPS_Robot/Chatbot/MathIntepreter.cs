using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class MathIntepreter
    {        
        private static bool IsOperator(string word)
        {
            string[] ops = { "*", "/", "+", "-" };

            foreach (var item in ops)
            {
                if (item == word) return true;
            }
            return false;            
        }

        private static bool IsContainOperator(string query)
        {
            string ops = "+-*/";
            foreach (var op in ops)
            {
                if (query.Contains(ops)) return true;
            }

            return false;
        }
        
        private static bool IsNumber(string word)
        {
            foreach (var c in word)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
        private static bool IsContainOperand(string query)
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
            if (query[query.Length - 1] == '?')
            {
                query = query.Remove(query.Length - 1);
            }

            string[] words = query.Split(' ');

            int i;
            string que = "";
            for(i = 0; i < words.Length; i++)
            {
                if (IsOperator(words[i]))
                {
                    if (i == 0 || !IsNumber(words[i - 1])) return "invalid";                                        
                    bool orderFlag = true; // true: operator, false: operand
                    que = words[i - 1];                    
                    while(i < words.Length && (IsNumber(words[i]) || IsOperator(words[i])))
                    {
                        if (orderFlag) // required operator
                        {
                            if (!IsOperator(words[i])) return "invalid";                            
                        }
                        else // required operand
                        {
                            if (!IsNumber(words[i])) return "invalid";                            
                        }
                        que += words[i];
                        i++;
                        orderFlag = !orderFlag;
                    }
                    break;
                }
            }

            if (que == "") return "invalid";    

            var res = new DataTable().Compute(que, null);

            return res.ToString();
        }
        
    }
}
