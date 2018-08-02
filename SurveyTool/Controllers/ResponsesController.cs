using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyTool.Models;

namespace SurveyTool.Controllers
{
    [Authorize]
    public class ResponsesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ResponsesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult Index(int surveyId)
        {
            var responses = _db.Responses
                               .Include("Survey")
                               .Include("Answers")
                               .Include("Answers.Question")
                               .Where(x => x.SurveyId == surveyId)
                               .Where(x => x.CreatedBy == User.Identity.Name)
                               .OrderByDescending(x => x.CreatedOn)
                               .ThenByDescending(x => x.Id)
                               .ToList();

            return View(responses);
        }

        [HttpGet]
        public ActionResult Details(int surveyId, int id)
        {
            var response = _db.Responses
                              .Include("Survey")
                              .Include("Answers")
                              .Include("Answers.Question")
                              .Where(x => x.SurveyId == surveyId)
                              .Where(x => x.CreatedBy == User.Identity.Name)
                              .Single(x => x.Id == id);

            response.Answers = response.Answers.OrderBy(x => x.Question.Priority).ToList();
            return View(response);
        }

        [HttpGet]
        public ActionResult Create(int surveyId)
        {
            List<Question> questions = new List<Question>();
            var survey = _db.Surveys
                            .Where(s => s.Id == surveyId)
                            .Select(s => new
                                {
                                    Survey = s,
                                    Questions = s.Questions
                                                 .Where(q => q.IsActive)
                                                 .OrderBy(q => q.Priority)
                                })
                             .AsEnumerable()
                             .Select(x =>
                                 {
                                     x.Survey.Questions = x.Questions.ToList();
                                     return x.Survey;
                                 })
                             .Single();

            survey.Questions.
                Select(q => new
                {
                    q.Id,
                    q.SurveyId,
                    q.Title,
                    q.Body,
                    q.Type,
                    q.IsRequired,
                    q.IsActive,
                    q.CreatedOn,
                    q.ModifiedOn,
                    Options = _db.Options.Where(o => o.QuestionId == q.Id && o.IsActive == true).OrderBy(x => x.Priority)
                })
                .ToList()
                .ForEach(q => questions.Add(new Question
                {
                    Id = q.Id,
                    Title = q.Title,
                    Body = q.Body,
                    Type = q.Type,
                    SurveyId = q.SurveyId,
                    IsActive = q.IsActive,
                    IsRequired = q.IsRequired,
                    CreatedOn = q.CreatedOn,
                    ModifiedOn = q.ModifiedOn,
                    Options = q.Options.ToList()
                }));

            survey.Questions = questions;
            //foreach (var question in survey.Questions)
            //{
            //    if (question.Type.Equals("RadioBox"))
            //    {
            //        //System.Diagnostics.Debug.WriteLine("before " + question.Options.Count());
            //        //question.OptionString = BuildRadioBox(question.Options);
            //        foreach (var opt in _db.Options)
            //        {
            //            if (opt.QuestionId == question.Id)
            //            {
            //                question.Options.Add(opt);
            //                question.Options.RemoveAt(question.Options.Count() - 1);
            //                //System.Diagnostics.Debug.WriteLine(opt.Text);
            //                //System.Diagnostics.Debug.WriteLine("len " + question.Options.Count());
            //            }
                        
            //        }
            //        question.Options = question.Options.Where(x=>x.IsActive == true).OrderBy(x => x.Priority).ToList();
                    
            //    }
            //}
            //var options = _db.Options;
            //foreach (var opt in options)
            //{
            //    System.Diagnostics.Debug.WriteLine(opt.Text);

            //}

            //var sur = _db.Surveys
            //            .Include("Questions")
            //            .Include("Questions.Options")
            //            .Select(x=>x.);


            return View(survey);
        }

        [HttpPost]
        public ActionResult Create(int surveyId, string action, Response model)
        {
            model.Answers = model.Answers.Where(a => !String.IsNullOrEmpty(a.Value)).ToList();
            model.SurveyId = surveyId;
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            _db.Responses.Add(model);
            _db.SaveChanges();

            TempData["success"] = "Your response was successfully saved!";

            return action == "Next"
                       ? RedirectToAction("Create", new {surveyId})
                       : RedirectToAction("Index", "DashBoard");
        }

        [HttpPost]
        public ActionResult Delete(int surveyId, int id, string returnTo)
        {
            var response = new Response() { Id = id, SurveyId = surveyId };
            _db.Entry(response).State = EntityState.Deleted;
            _db.SaveChanges();
            return Redirect(returnTo ?? Url.RouteUrl("Root"));
        }
    }
}