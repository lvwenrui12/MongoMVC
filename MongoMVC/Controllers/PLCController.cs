using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MongoMVC.Controllers
{
    public class PLCController : Controller
    {
        // GET: PLC
        public ActionResult Index()
        {
            return View();
        }

        // GET: PLC/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PLC/Create
        public ActionResult Create()
        {
            string strPost = HttpHelper.GetPostVaule(Request);

            return View();
        }

        // POST: PLC/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PLC/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PLC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PLC/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PLC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
