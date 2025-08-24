using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCMS.MathHouse
{
    public class QuestionResult
    {
        public string QuestionText { get; set; }
        public int CorrectAnswer { get; set; }
        public int UserAnswer { get; set; }
        public bool IsCorrect => UserAnswer == CorrectAnswer;
    }

}
