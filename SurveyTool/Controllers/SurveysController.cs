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
    public class SurveysController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SurveysController(ApplicationDbContext db)
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
        public ActionResult Create()
        {
            var survey = new Survey
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1)
                };

            return View(survey);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Survey survey, string action)
        {
            if (ModelState.IsValid)
            {
                survey.Questions.ForEach(q => q.CreatedOn = q.ModifiedOn = DateTime.Now);
                _db.Surveys.Add(survey);
                _db.SaveChanges();
                TempData["success"] = "The survey was successfully created!";
                return RedirectToAction("Edit", new {id = survey.Id});
            }
            else
            {
                TempData["error"] = "An error occurred while attempting to create this survey.";
                return View(survey);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var survey = _db.Surveys.Include("Questions").Single(x => x.Id == id);
            survey.Questions = survey.Questions.OrderBy(q => q.Priority).ToList();
            return View(survey);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Survey model)
        {
            
            foreach (var question in model.Questions)
            {
                question.SurveyId = model.Id;

                if (question.Id == 0)
                {
                    question.CreatedOn = DateTime.Now;
                    question.ModifiedOn = DateTime.Now;
                    _db.Entry(question).State = EntityState.Added;
                }
                else
                {
                    question.ModifiedOn = DateTime.Now;
                    _db.Entry(question).State = EntityState.Modified;
                    _db.Entry(question).Property(x => x.CreatedOn).IsModified = false;
                }
            }

            _db.Entry(model).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Edit", new {id = model.Id});
        }

        [HttpPost]
        public ActionResult Delete(Survey survey)
        {
            _db.Entry(survey).State = EntityState.Deleted;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateOption(Survey survey, int id)
        {
            System.Diagnostics.Debug.WriteLine("here");
            if (ModelState.IsValid)
            {
                survey.Questions.ForEach(q => q.CreatedOn = q.ModifiedOn = DateTime.Now);
                _db.Surveys.Add(survey);
                _db.SaveChanges();
                TempData["success"] = "The survey was successfully created!";
                id = survey.Questions[id].Id;
                //var question = _db.Questions.Include("Options").Single(x => x.Id == id);
                //question.Options = question.Options.OrderBy(q => q.Priority).ToList();
                System.Diagnostics.Debug.WriteLine("about to return");
                System.Diagnostics.Debug.WriteLine(id);
                return RedirectToAction("EditOption", new { id });
                // return View(question);
                //
            }
            else
            {
                TempData["error"] = "An error occurred while attempting to create this survey.";
                System.Diagnostics.Debug.WriteLine("here");
                System.Diagnostics.Debug.WriteLine(ModelState);
                return View("~/Views/Survey/Edit.cshtml", survey);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditOption(Survey model, int id, int priority)
        {
            int Id = -1;
            int pos = -1;
            int count = 0;
            foreach (var question in model.Questions)
            {
                question.SurveyId = model.Id;

                if (question.Id == 0)
                {
                    question.CreatedOn = DateTime.Now;
                    question.ModifiedOn = DateTime.Now;
                    _db.Entry(question).State = EntityState.Added;
                }
                else
                {
                    question.ModifiedOn = DateTime.Now;
                    _db.Entry(question).State = EntityState.Modified;
                    _db.Entry(question).Property(x => x.CreatedOn).IsModified = false;
                }

                
            }
            
            _db.Entry(model).State = EntityState.Modified;
            _db.SaveChanges();

            foreach (var question in model.Questions)
            {
 

                if (question.Priority.Equals(priority))
                {
                    pos = count;
                }
                count++;
            }
            Id = model.Questions[pos].Id;
            return RedirectToAction("EditOption", new { id = Id });

        } 

        public ActionResult EditOption(int id)
        {
            //System.Diagnostics.Debug.WriteLine(id);
            var question = _db.Questions.Include("Options").Single(x => x.Id == id);
            question.Options = question.Options.OrderBy(q => q.Priority).ToList();
            System.Diagnostics.Debug.WriteLine(question.ModifiedOn);
            return View(question);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveOption(Question model, int id)
        {

            foreach (var option in model.Options)
            {
                option.QuestionId = model.Id;

                if (option.Id == 0)
                {
                   
                    _db.Entry(option).State = EntityState.Added;
                }
                else
                {
                  
                    _db.Entry(option).State = EntityState.Modified;
                }
            }
            model.ModifiedOn = DateTime.Now;
            _db.Entry(model).State = EntityState.Modified;
            _db.Entry(model).Property(x => x.CreatedOn).IsModified = false;
            //_db.Entry(model).Property(x => x.Answers).IsModified = false;
            _db.Entry(model).Property(x => x.IsActive).IsModified = false;
            _db.Entry(model).Property(x => x.Priority).IsModified = false;
            _db.Entry(model).Property(x => x.Body).IsModified = false;
            _db.Entry(model).Property(x => x.Type).IsModified = false;
            _db.Entry(model).Property(x => x.SurveyId).IsModified = false;
            _db.Entry(model).Property(x => x.Title).IsModified = false;
            _db.Entry(model).Property(x => x.IsRequired).IsModified = false;
            _db.SaveChanges();
            return RedirectToAction("EditOption", new { id = model.Id });

            //if (ModelState.IsValid)
            //{
            //    survey.Questions.ForEach(q => q.CreatedOn = q.ModifiedOn = DateTime.Now);
            //    _db.Surveys.Add(survey);
            //    _db.SaveChanges();
            //    TempData["success"] = "The survey was successfully created!";
            //    return RedirectToAction("Edit", new { id = survey.Id });
            //}
            //else
            //{
            //    TempData["error"] = "An error occurred while attempting to create this survey.";
            //    return View(survey);
            //}

        }
    }
}
