using SurveyTool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyTool.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {

        private readonly ApplicationDbContext _db;

        public QuestionController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Question
        public ActionResult Index()
        {
            return View();
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
                return RedirectToAction("Edit", new { id });
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
        public ActionResult Edit(Survey model, int id)
        {
            //int Id = -1;
            //int count = 0;
            //foreach (var question in model.Questions)
            //{
            //    question.SurveyId = model.Id;

            //    if (question.Id == 0)
            //    {
            //        question.CreatedOn = DateTime.Now;
            //        question.ModifiedOn = DateTime.Now;
            //        _db.Entry(question).State = EntityState.Added;
            //    }
            //    else
            //    {
            //        question.ModifiedOn = DateTime.Now;
            //        _db.Entry(question).State = EntityState.Modified;
            //        _db.Entry(question).Property(x => x.CreatedOn).IsModified = false;
            //    }

            //    if (question.Priority.Equals(id))
            //    {
            //        Id = count;
            //    }
            //    count++;
            //}

            //_db.Entry(model).State = EntityState.Modified;
            //_db.SaveChanges();

            //return RedirectToAction("Edit", new { id = Id });

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
            return RedirectToAction("~/Views/Survey/Edit", new { id = model.Id });

        }

        public ActionResult Edit(int id)
        {
            System.Diagnostics.Debug.WriteLine(id);
            var question = _db.Questions.Include("Options").Single(x => x.Id == id);
            question.Options = question.Options.OrderBy(q => q.Priority).ToList();
            System.Diagnostics.Debug.WriteLine(question.ModifiedOn);
            return View(question);
            
        }
    }
}