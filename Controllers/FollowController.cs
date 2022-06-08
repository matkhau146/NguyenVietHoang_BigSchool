using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using TruongNgocThanh_BigSchool.Models;

namespace TruongNgocThanh_BigSchool.Controllers
{
    public class FollowController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Follow(Following follow)
        {
            var userID = User.Identity.GetUserId();
            if (userID== null)
            {
                return BadRequest("Please login first!");
            }
            if (userID == follow.FolloweeId)
            {
                return BadRequest("Can not follow myself!");
            }
            BigSchoolContext context = new BigSchoolContext();
            //Kiem tra xem ma userID da duoc theo doi chua
            Following find = context.Followings.FirstOrDefault(p=>p.FollowerId == userID && p.FolloweeId ==follow.FolloweeId);
            if (find != null)
            {
                //return BadRequest("The already following exists!");
                context.Followings.Remove(context.Followings.SingleOrDefault(p => p.FollowerId == userID &&
                p.FolloweeId == follow.FolloweeId));
                context.SaveChanges();
                return Ok("cancel");
            }
            follow.FollowerId = userID;
            context.Followings.Add(follow);
            context.SaveChanges();

            return Ok();
        }
    }
}