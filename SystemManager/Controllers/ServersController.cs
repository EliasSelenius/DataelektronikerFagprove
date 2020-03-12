using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SystemManager.Models;

namespace SystemManager.Controllers
{
    public class ServersController : Controller
    {


        // GET: Servers
        public ActionResult Index()
        {
            return View(DbManager.GetServers());
        }

        public ActionResult InvokeService(int id, string service, string name) {
            WmiManager.InvokeOnService(DbManager.GetServer(id), service, name);
            return RedirectToAction("ServerDetails", new { id });
        }


        public ActionResult Reboot(int id) {
            WmiManager.RebootServer(DbManager.GetServer(id));
            return RedirectToAction("Index");
        }

        public ActionResult ServerDetails(int id) {
            var s = DbManager.GetServer(id);
            return View(WmiManager.GetServerInfo(s));
        }

        // GET: Servers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Servers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {

            bool r = DbManager.AddServer(collection["Domain"], collection["Username"], collection["Password"]);
            if (r) {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        // GET: Servers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Servers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {

            if (DbManager.RemoveServer(id)) {
                return RedirectToAction(nameof(Index));
            } else {
                return View();
            }
        }
    }
}