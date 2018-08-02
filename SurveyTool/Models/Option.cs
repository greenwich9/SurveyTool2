using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SurveyTool.Models
{
    public class Option
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public int Priority { get; set; }

        public int QuestionId { get; set; }

        //public Question Question { get; set; }

        public bool IsActive { get; set; }

        
    }
}