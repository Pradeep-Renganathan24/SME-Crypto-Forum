using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using MultiChainLib;
using System.Threading.Tasks;
using CryptoForum.Models;

namespace CryptoForum.Controllers
{
    public class HomeController : Controller
    {
        private CryptoForumEntities dbObj = new CryptoForumEntities();
        private static MultiChainClient client = new MultiChainClient("localhost", 8358, false, "multichainrpc", "Bm2B1x1GtZbH7Hutguem55wqw9PYyc61MeqqzBWTb24F", "admin");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [WebMethod]
        public async Task<ActionResult> GetWalletInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                string user = User.Identity.Name;
                var address = dbObj.tblNodeInfoes.Where(t => t.UserId == user).Select(t => t.Handshake).FirstOrDefault();
                //var test = await client.SendToAddressAsync(address, "BullCoin", 2);
                //var sendAssetToAddress = await client.SendAssetToAddressAsync(address, "BullCoin", 1);
                //test.AssertOk();
                var info = await client.GetAddressBalancesAsync(address);
                info.AssertOk();
                return Json(info.Result);
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        public async Task<ActionResult> GetStats()
        {
            var txs = await client.GetWalletInfoAsync();
            txs.AssertOk();
            var txCount = txs.Result.TxCount;
            var users = dbObj.tblNodeInfoes.GroupBy(t => t.UserId).Count();
            var posts = dbObj.tblPosts.GroupBy(t => t.PostId).Count();

            return Json(new { txCount = txCount, users = users, posts = posts }, JsonRequestBehavior.AllowGet);
        }

        [WebMethod]
        public async Task<ActionResult> PostCommentAsync(String comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                string user = User.Identity.Name;
                tblPost post = new tblPost();
                post.Comment = comment;
                post.UserId = user;
                post.TimeStamp = DateTime.Now;
                post.ParentId = null;
                dbObj.tblPosts.Add(post);
                dbObj.SaveChanges();                
                var address = dbObj.tblNodeInfoes.Where(t => t.UserId == user).Select(t => t.Handshake).FirstOrDefault();
                var test = await client.SendAssetFromAsync("16rM7qe49PXuXGjF6QheMKa8fzbDwxoHkbKu3j", address, "BullCoin", 5);
                test.AssertOk();
                return Json(true);
            }
            return Json(false);
        }

        [WebMethod]
        public ActionResult ListComments()
        {
            var list = dbObj.tblPosts.OrderByDescending(t => t.TimeStamp).Select(x => x).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}