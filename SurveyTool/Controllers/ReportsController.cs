using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyTool.Models;

namespace SurveyTool.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var surveys = _db.Surveys.ToList();
            return View(surveys);
        }

        [HttpGet]
        public ActionResult Details(int id, int? departmentId, DateTime? startDate, DateTime? endDate)
        {
            var questions = new List<QuestionViewModel>();
            startDate = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            endDate = endDate ?? DateTime.Now;

            var survey = _db.Surveys.Single(s => s.Id == id);

            _db.Questions
               .Where(q => q.SurveyId == id)
               .OrderBy(q => q.Priority)
               .Select(q => new
                   {
                       q.Title,
                       q.Body,
                       q.Type,
                       Answers = _db.Answers.Where(a => a.QuestionId == q.Id &&
                                                        a.Response.CreatedOn >= startDate.Value &&
                                                        a.Response.CreatedOn <= endDate.Value),
                       Options = _db.Options.Where(a => a.QuestionId == q.Id)
                   })
               .ToList()
               .ForEach(r => questions.Add(new QuestionViewModel
                   {
                       Title = r.Title,
                       Body = r.Body,
                       Type = r.Type,
                       Answers = r.Answers.ToList(),
                       Options = r.Options.ToList(),
                       OptionsViewList = new List<OptionViewModel>()
                   }));

            foreach (QuestionViewModel q in questions)
            {
                if (q.Type.Equals("RadioBox"))
                {
                    var stat = from opt in q.Options
                               select new OptionViewModel
                               {
                                   Id = opt.Id,
                                   Text = opt.Text,
                                   Priority = opt.Priority
                               };
                               
                    
                    var answerCount = from ans in q.Answers
                                      group ans by ans.Value into grp
                                      select new { Key = Convert.ToInt32(grp.Key), Count = grp.Count() };

                    var res = from opt in stat
                              join ans in answerCount on opt.Id equals ans.Key into joined
                              from j in joined.DefaultIfEmpty()
                              select new OptionViewModel
                              {
                                  Id = opt.Id,
                                  Text = opt.Text,
                                  Priority = opt.Priority,
                                  NumSelected = j?.Count ?? 0
                              };
                    q.OptionsViewList = res.ToList();
                }
            }

            var vm = new ReportViewModel
                {
                    StartDate = startDate.Value,
                    EndDate = endDate.Value,
                    Survey = survey,
                    Responses = questions
                };

            return View(vm);
        }
    }
}
