using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CalendarFive.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace CalendarFive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public bool Post([FromBody]Content val)
        {
            bool state = false;
            using (var context = new DbGoogleContext())
            {
               
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                  ClientId = "491606904456-nr7nscjo3l75na40kq939t35fd55i9o7.apps.googleusercontent.com",
                  ClientSecret = "xhc5pCiEoW54ch7aF_J8Ah69",
                },
                new[] { CalendarService.Scope.Calendar },
               "user",
                CancellationToken.None).Result;

                try
                {
                    var service = new CalendarService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Calendar API Realtor",
                    });

                    CalendarListResource.ListRequest cal = service.CalendarList.List();
                    cal.MaxResults = 100;
                    var calresult = cal.Execute().Items;



                     foreach (CalendarListEntry entry in calresult)
                     {
                         EventsResource.ListRequest request = service.Events.List(entry.Id);
                         request.TimeMin = Convert.ToDateTime("03/01/2015");
                         request.ShowDeleted = false;
                         request.SingleEvents = true;
                         request.MaxResults = 200;
                         request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                         
                         Events events = request.Execute();
                     }


                     
                     
                    
                    DateTime sDate = val.Start;

                    DateTime eDate = val.End;


                   
                    var myEvent = new Event
                    {
                        Id = val.EventId,
                        Summary = val.Title,
                        Description = val.Description,
                        Location = "Thailand",
                       
                        Start = new EventDateTime()
                        {


                            DateTime = sDate,
                            TimeZone = "Asia/Colombo",
                        },
                        End = new EventDateTime()
                        {
                            DateTime = eDate,
                            TimeZone = "Asia/Colombo",
                        },
                        Attendees = new EventAttendee[] {
        new EventAttendee() { Email = "oumw.udesh@gmail.com.com" },
        new EventAttendee() { Email = "fathimabushra9551@gmail.com" },
                        },
                        Reminders = new Event.RemindersData()
                        {
                            UseDefault = false,
                            Overrides = new EventReminder[] 
                            {
            new EventReminder() { Method = "email", Minutes = 24 * 60 },
            new EventReminder() { Method = "popup", Minutes = 10 },
                            }
                        }


                    };
                    String calendarId = "oumw.udesh@gmail.com";
                    EventsResource.InsertRequest singleEvent = service.Events.Insert(myEvent, calendarId);
                    singleEvent.SendNotifications = true; 
                    singleEvent.Execute();
                  

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                context.Content.Add(val);
                context.SaveChanges();
                state = true;
            }

            return state;
        }



        //GET Api/values
        [HttpGet]
        public List<Content> Get()
        {
            using (var context = new DbGoogleContext())
            {
                return context.Content.ToList();
            }
        }



        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValues(int id, Content vals)
        {
             using (var context = new DbGoogleContext())
            {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(

                   new ClientSecrets
                   {
                       ClientId = "491606904456-nr7nscjo3l75na40kq939t35fd55i9o7.apps.googleusercontent.com",
                       ClientSecret = "xhc5pCiEoW54ch7aF_J8Ah69",
                   },
                   new[] { CalendarService.Scope.Calendar },
                   "user",
                    
            CancellationToken.None).Result;
           

            try
            {
                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar API Realtor",
                });

                CalendarListResource.ListRequest cal = service.CalendarList.List();
                cal.MaxResults = 100;

                var calresult = cal.Execute().Items;


                // List<EventDetails> ed = new List<EventDetails>();

                foreach (CalendarListEntry entry in calresult)
                {
                    EventsResource.ListRequest request = service.Events.List(entry.Id);
                    request.TimeMin = Convert.ToDateTime("03/01/2019");
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 200;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                    // List events.
                    Events eventss = request.Execute();
                }






                var eid = new DbGoogleContext().Content.Where(d => d.Id == id).FirstOrDefault().EventId;

                String calendarId = "oumw.udesh@gmail.com";
                String eventId = eid;
                // var eventslist = service.Events.List(calendarId);
                var events = service.Events.Get(calendarId, eventId).Execute();
              
                DateTime ssDate = vals.Start;

                DateTime esDate = vals.End;
                Event ev = new Event
                {
                    Summary = vals.Title,
                    Id = vals.EventId,
                    Description = vals.Description,
                    Start = new EventDateTime()
                    {


                        DateTime = ssDate,
                        TimeZone = "Asia/Colombo",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = esDate,
                        TimeZone = "Asia/Colombo",
                    }
                    
                };
                //string FoundEventID = events.Id;
                
                var updateCal = service.Events.Patch(ev, calendarId, eventId);
                    updateCal.SendNotifications = true;
                    updateCal.Execute();

                    context.Entry(vals).State = EntityState.Modified;
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Content(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                }
            catch (Exception e)
            {
                throw e;
            }






            if (id != vals.Id)
            {
                return BadRequest();
            }
           
                
            }
            

            return NoContent();
        }


        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
            using (var context = new DbGoogleContext())
            {
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(

                   new ClientSecrets
                   {
                       ClientId = "491606904456-nr7nscjo3l75na40kq939t35fd55i9o7.apps.googleusercontent.com",
                       ClientSecret = "xhc5pCiEoW54ch7aF_J8Ah69",
                   },
                   new[] { CalendarService.Scope.Calendar },
                   "user",

            CancellationToken.None).Result;
                try
                {

                    var service = new CalendarService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Calendar API Realtor",
                    });

                    CalendarListResource.ListRequest cal = service.CalendarList.List();
                    cal.MaxResults = 100;

                    var calresult = cal.Execute().Items;


                    

                    foreach (CalendarListEntry entry in calresult)
                    {
                        EventsResource.ListRequest request = service.Events.List(entry.Id);
                        request.TimeMin = Convert.ToDateTime("03/01/2019");
                        request.ShowDeleted = false;
                        request.SingleEvents = true;
                        request.MaxResults = 200;
                        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                        Events eventss = request.Execute();
                       
                    }
                    var eid = new DbGoogleContext().Content.Where(d => d.Id == id).FirstOrDefault().EventId;
                    String calendarId = "oumw.udesh@gmail.com";
                    String eventId = eid;
                    var delCal = service.Events.Delete(calendarId, eventId);
                    delCal.SendNotifications = true;
                    delCal.Execute();

                    var removes = context.Content.Where(s => s.Id == id).FirstOrDefault();
                    context.Content.Remove(removes);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
                
            }
            
        }

        private bool Content(int id)
        {
            throw new NotImplementedException();
        }


       

    }




}
