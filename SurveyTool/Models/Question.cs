using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace SurveyTool.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        public int SurveyId { get; set; }

        public string Title { get; set; }
        
        public string Type { get; set; }

        public string Body { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }

        public string IsRequired { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public List<Answer> Answers { get; set; }

        public List<Option> Options { get; set; }

        //public string OptionString { get; set; }

        public string ToJson()
        {
            var js = JsonSerializer.Create(new JsonSerializerSettings());
            var jw = new StringWriter();
            js.Serialize(jw, this);
            return jw.ToString();
        }

        public Question()
        {
            Options = new List<Option>();
        }
    }
}