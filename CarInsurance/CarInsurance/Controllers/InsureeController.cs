using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;
using DocuSign.eSign.Model;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

      
        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,DUI,CoverageType,SpeedingTickets,CarYear,CarMake,CarModel,Quote")]Insuree insuree)
        {
            //redirects user to the error page if the values they enter are null or empty
            if (ModelState.IsValid)
            {
                //set up vars 
                DateTime Today = DateTime.Today;
                int Year = Convert.ToInt32(Today.Year);
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;

                //start with a base of $50/month
                    insuree.Quote = 50;

                //if the user is 18 and under, add $100 to monthly total
                if (age < 18 || age == 18)
                {
                    insuree.Quote += 100;
                }

                //if the user is between 19 and 25, add $50 to monthly total
                else if (age > 19 && age < 25 || age == 25 || age == 19)
                {
                    insuree.Quote += 50;
                }

                //if the user is over 25, add $25 to monthly total
                else if (age > 25)
                {
                    insuree.Quote += 25;
                }

                //if the car's year is before 2000, add $25 to monthly total
                //if the car's year is after 2015, add $25 to monthly total
                if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
                {
                    insuree.Quote += 25;
                }

                //if the car's Make is a Porsche, add $25 to the price
                //if the car's Make is a Porsche and its model is a 911 Carrera, add an additional $25 
                if (insuree.CarMake == "Porsche" || insuree.CarMake == "porsche" && insuree.CarModel == "911 Carrera")
                {
                    if (insuree.CarMake == "Porsche" || insuree.CarMake == "porsche")
                    {
                        insuree.Quote += 25;
                    }
                    insuree.Quote += 50;
                }

                //add $10 to monthly total for every speeding ticket the user has
                if (insuree.SpeedingTickets > 0)
                {
                    insuree.Quote += 10 * insuree.SpeedingTickets;
                }

                //if the user has ever had a DUI, add 25% to the total
                if (insuree.DUI)
                {
                    insuree.Quote *= 1.25m;
                }

                //If it's full coverage, add 50% to the total
                if (insuree.CoverageType)
                {
                    insuree.Quote *= 1.5m;
                }

                Console.WriteLine("Your quote is: " + insuree.Quote);
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Insuree", insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
