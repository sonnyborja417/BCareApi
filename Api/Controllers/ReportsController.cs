using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Reports")]
    public class ReportsController : Controller
    {
        private bcareContext context;

        public ReportsController(bcareContext context)
        {
            this.context = context;
        }

        public class ChartData
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public int index { get; set; }
        }

        public class StackData
        {
            public string Item1 { get; set; }
            public int Item2 { get; set; }
            public int Item3 { get; set; }
            public int Item4 { get; set; }
            public int Item5 { get; set; }
            public int Item6 { get; set; }
            
        }

        [HttpGet("Monthly/Diagnosis/{date}")]
        public IActionResult MonthlyDiagnosis([FromRoute] DateTime date)
        {
            var output = new List<ChartData>();

            var dFrom = new DateTime(date.Year, date.Month, 1);
            var dTo = dFrom.AddMonths(1);

            var grouping = this.context.Apointment.Where(d=>d.ApointmentDate >= dFrom && d.ApointmentDate < dTo && d.Diagnosy != null)?.GroupBy(d => d.Diagnosy.ToLower());
            if (grouping != null)
            {
                foreach (var group in grouping)
                {
                    output.Add(new ChartData { Name = group.Key, Count = group.Count() });
                }
            }
            
            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Table = output
                }
            });
        }

        [HttpGet("Monthly/Appointments")]
        public IActionResult MonthlyAppointments()
        {
            var output = new List<ChartData>();
          
            var grouping = this.context.Apointment.OrderBy(d=>d.ApointmentDate).GroupBy(d => d.ApointmentDate.ToString("yyyy-MM"));
            foreach (var group in grouping)
            {
                output.Add(new ChartData { Name = group.Key.ToString(), Count = group.Count() });
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Table = output
                }
            });
        }

        [HttpGet("Monthly/StackDiagnosis")]
        public IActionResult MonthlyGroupedDiagnosis()
        {
            var output = new List<StackData>();            

            var data = this.context.Apointment.Where(d=>d.Diagnosy != null).OrderBy(d => d.ApointmentDate);
            var grouping = data.GroupBy(d => d.ApointmentDate.ToString("yyyy-MM"));
            foreach (var group in grouping)
            {
                //output.Add(new ChartData { Name = group.Key.ToString(), Count = group.Count() });
                var dataDiag = data.Where(d => d.ApointmentDate.ToString("yyyy-MM") == group.Key);
                var diagnosisGrouping = dataDiag.GroupBy(e=>e.Diagnosy.ToLower());
                foreach(var dia in diagnosisGrouping)
                {
                    var month = group.Key;
                    var fever = dataDiag.Where(d => d.Diagnosy.ToLower() == "fever").Count();
                    var flu = dataDiag.Where(d => d.Diagnosy.ToLower() == "flu").Count();
                    var diarrhea = dataDiag.Where(d => d.Diagnosy.ToLower() == "diarrhea").Count();
                    var migraine = dataDiag.Where(d => d.Diagnosy.ToLower() == "migraine").Count();
                    var others = dataDiag.Where(d => d.Diagnosy.ToLower() != "fever" && d.Diagnosy.ToLower() != "flu" && d.Diagnosy.ToLower() != "diarrhea" && d.Diagnosy.ToLower() != "migraine").Count();

                    output.Add(new StackData { Item1 = month, Item2 = fever, Item3 = flu, Item4 = diarrhea, Item5 = migraine, Item6 = others});
                }
            }

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Table = output
                }
            });
        }

        [HttpGet("Monthly/Summary/{date}")]
        public IActionResult MonthlySummary([FromRoute] DateTime date)
        {
            var dFrom = new DateTime(date.Year, date.Month, 1);
            var dTo = dFrom.AddMonths(1);

            var appointments = this.context.Apointment
                .Include(e => e.FkPatient)
                .Include(e => e.Diagnosis)                
                .Where(e => (e.ApointmentDate >= dFrom && e.ApointmentDate < dTo) )
                .ToList();

            var data = new
            {
                Total = appointments.Count(),
                Closed = appointments.Where(a=>a.IsClosed).Count(),
                PendingApproval = appointments.Where(a => a.IsApproved == false && a.IsRejected == false &&  a.IsClosed == false).Count(),
                Rejected = appointments.Where(a => a.IsRejected  ).Count()
            };                                

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Table = data                    
                }
            });
        }

        [HttpGet("Monthly/Rejected/{date}")]
        public IActionResult MonthlyRejected([FromRoute] DateTime date)
        {
            var dFrom = new DateTime(date.Year, date.Month, 1);
            var dTo = dFrom.AddMonths(1);

            var appointments = this.context.Apointment
                .Include(e => e.FkPatient)
                .Include(e => e.Diagnosis)
                .Where(e => (e.ApointmentDate >= dFrom && e.ApointmentDate < dTo) && e.IsRejected )
                .ToList();            

            return Ok(new ResponseResult<object>
            {
                Status = ResponseResultStatus.Ok,
                Body = new
                {
                    Table = appointments
                }
            });
        }
    }
}