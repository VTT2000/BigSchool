using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class AttendancesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend(Course attedanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            
            if(context.Attendances.Any(p=>p.Attendee == userID && p.CourseId == attedanceDto.Id))
            {
                return BadRequest("The attendance already exists!");
            }
            var attendace = new Attendance() { CourseId = attedanceDto.Id, Attendee = User.Identity.GetUserId() };
            context.Attendances.Add(attendace);
            context.SaveChanges();
            return Ok();
        }
    }
}
