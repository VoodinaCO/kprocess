using KProcess.KL2.Business.Impl.Helpers;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Models.Inspection;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor)]
    [SettingUserContextFilter]
    public class ScheduleController : LocalizedController
    {
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;

        public ScheduleController(
            IApplicationUsersService applicationUsersService,
            IPrepareService prepareService,
            IReferentialsService referentialsService,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _prepareService = prepareService;
            _referentialsService = referentialsService;
        }

        // GET: Schedule
        public async Task<ActionResult> Index(bool partial = false)
        {
            var timeslots = await _prepareService.GetTimeslots();
            ViewBag.TimeslotExist = timeslots.Any();
            if (partial)
                return PartialView();
            return View();
        }

        public async Task<JsonResult> GetInspectionScheduleAsync()
        {
            var data = await GetSchedules();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<List<InspectionScheduleViewModel>> GetSchedules()
        {            
            var schedules = await _prepareService.GetInspectionSchedulesNonFilter();
            return schedules.Select(schedule =>
            {
                var startTime = schedule.Timeslot.StartTime;
                var endTime = schedule.Timeslot.EndTime < schedule.Timeslot.StartTime ? schedule.Timeslot.EndTime.Add(TimeSpan.FromDays(1)) : schedule.Timeslot.EndTime;
                var duration = endTime - startTime;
                return new InspectionScheduleViewModel
                {
                    Id = schedule.InspectionScheduleId,
                    StartTime = schedule.StartDate.Add(schedule.Timeslot.StartTime),
                    EndTime = duration != TimeSpan.Zero ? schedule.StartDate.Add(schedule.Timeslot.StartTime).Add(duration) : schedule.StartDate.Add(schedule.Timeslot.StartTime).Add(TimeSpan.FromDays(1)),
                    ProcessId = schedule.ProcessId,
                    ProcessLabel = schedule.Procedure.Label,
                    RecurrenceRule = schedule.RecurrenceRule,
                    RecurrenceException = schedule.RecurrenceException,
                    RecurrenceID = schedule.RecurrenceId,
                    IsAllDay = duration != TimeSpan.Zero ? false : true,
                    TimeslotId = schedule.TimeslotId,
                    Timeslot = new TimeslotViewModel
                    {
                        TimeslotId = schedule.TimeslotId,
                        Label = schedule.Timeslot.Label,
                        Description = schedule.Timeslot.Description,
                        StartTime = schedule.Timeslot.StartTime,
                        EndTime = schedule.Timeslot.EndTime,
                        Color = schedule.Timeslot.Color != null ? "#" + schedule.Timeslot.Color.Substring(3) : "#000000",
                        DisplayOrder = schedule.Timeslot.DisplayOrder
                    }
                };
            }).ToList();
        }

        public async Task<ActionResult> ManageTimeslots(bool partial = false)
        {
            var timeslots = await _prepareService.GetTimeslots();
            var model = timeslots.Select(timeslot => new TimeslotViewModel
            {
                TimeslotId = timeslot.TimeslotId,
                Label = timeslot.Label,
                Description = timeslot.Description,
                StartTime = timeslot.StartTime,
                StartTimeString = DateTime.Today.Add(timeslot.StartTime).ToString("HH:mm"),
                EndTime = timeslot.EndTime,
                EndTimeString = DateTime.Today.Add(timeslot.EndTime).ToString("HH:mm"),
                Color = timeslot.Color != null ? "#" + timeslot.Color.Substring(3) : "",
                DisplayOrder = timeslot.DisplayOrder,
                IsAllDay = timeslot.StartTime == TimeSpan.Zero && timeslot.EndTime == TimeSpan.Zero ? true : false,
            }).ToList();
            if (partial)
                return PartialView(model);
            return View(model);
        }

        public async Task<ActionResult> InsertTimeslot(CRUDModel<TimeslotViewModel> timeslot)
        {
            try
            {
                var newTimeslot = new Timeslot
                {
                    Label = timeslot.Value.Label,
                    StartTime = timeslot.Value.StartTime,
                    EndTime = timeslot.Value.EndTime,
                    Description = timeslot.Value.Description,
                    Color = timeslot.Value.Color != null ? "#FF" + timeslot.Value.Color.Remove(timeslot.Value.Color.Length - 2).Substring(1).ToUpper() : null,
                    DisplayOrder = timeslot.Value.DisplayOrder,
                    IsDeleted = false
                };
                Timeslot[] addTimeslot = { newTimeslot };
                var data = await _prepareService.SaveTimeslots(addTimeslot);
                var currentTimeSlot = data.FirstOrDefault();
                timeslot.Value.TimeslotId = currentTimeSlot.TimeslotId;
                timeslot.Value.StartTimeString = DateTime.Today.Add(timeslot.Value.StartTime).ToString("HH:mm");
                timeslot.Value.EndTimeString = DateTime.Today.Add(timeslot.Value.EndTime).ToString("HH:mm");
                return Json(timeslot.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> UpdateTimeslot(CRUDModel<TimeslotViewModel> timeslot)
        {
            try
            {
                ModelState.Remove("Value.IsAllDay");
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }
                var timeslots = await _prepareService.GetTimeslots(timeslot.Value.TimeslotId);
                var updateTimeslot = timeslots.FirstOrDefault();
                updateTimeslot.StartTracking();
                updateTimeslot.Label = timeslot.Value.Label;
                updateTimeslot.StartTime = timeslot.Value.StartTime;
                updateTimeslot.EndTime = timeslot.Value.EndTime;
                updateTimeslot.Description = timeslot.Value.Description;
                updateTimeslot.Color = timeslot.Value.Color != null ? "#FF" + timeslot.Value.Color.Remove(timeslot.Value.Color.Length - 2).Substring(1).ToUpper() : null;
                updateTimeslot.DisplayOrder = timeslot.Value.DisplayOrder;
                updateTimeslot.MarkAsModified();
                Timeslot[] editTimeslot = { updateTimeslot };
                var data = await _prepareService.SaveTimeslots(editTimeslot);

                return Json(timeslot.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> DeleteTimeslot(int key)
        {
            try
            {
                var timeslots = await _prepareService.GetTimeslots(key);
                var timeslotDelete = timeslots.FirstOrDefault();
                if (timeslotDelete != null)
                {
                    timeslotDelete.StartTracking();
                    timeslotDelete.IsDeleted = true;
                    timeslotDelete.DisplayOrder = null;
                    timeslotDelete.MarkAsModified();
                    Timeslot[] deleteTimeslot = { timeslotDelete };
                    await _prepareService.SaveTimeslots(deleteTimeslot);
                }
                //reorder DisplayOrder of not deleted timeslots
                timeslots = await _prepareService.GetTimeslots();
                var countReorder = 1;
                foreach (var timeslot in timeslots)
                {
                    timeslot.StartTracking();
                    timeslot.DisplayOrder = countReorder;
                    timeslot.MarkAsModified();
                    countReorder++;
                }
                await _prepareService.SaveTimeslots(timeslots.ToArray());
                return Json(timeslotDelete, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<string> SaveScheduleValidation(InspectionScheduleViewModel data)
        {
            var testInspectionSchedule = new InspectionSchedule
            {
                InspectionScheduleId = data.Id,
                ProcessId = data.ProcessId,
                StartDate = data.StartTime,
                Timeslot = (await _prepareService.GetTimeslots(data.TimeslotId)).FirstOrDefault(),
                RecurrenceRule = data.RecurrenceRule,
                RecurrenceException = data.RecurrenceException
            };
            var schedules = (await _prepareService.GetInspectionSchedulesNonFilter()).Where(_ => _.ProcessId == data.ProcessId);
            if (data.Id > 0)
                schedules = schedules.Where(s => s.InspectionScheduleId != data.Id);
            //Check validation if schedule data have same inspection in the same timeslot in the same day + recurrence
            var dataDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(testInspectionSchedule);
            foreach (var schedule in schedules)
            {
                var dates = RecurrenceHelper.GetRecurrenceDateTimeCollection(schedule);
                var intersect = dataDates.Intersect(dates);
                if (intersect.Any())
                    return "L'horaire pour l'inspection sélectionnée, le créneau horaire et la date existe déjà";
            }
            return string.Empty;
        }

        public async Task<JsonResult> SaveTimeslotValidation(TimeslotViewModel data)
        {
            var timeslots = await _prepareService.GetTimeslots();
            if (data.TimeslotId != 0)
                timeslots = timeslots.Where(t => t.TimeslotId != data.TimeslotId);
            string validationMessage = "";
            bool goodData = true;
            if ((data.StartTime == data.EndTime) && (data.StartTime != TimeSpan.Zero || data.EndTime != TimeSpan.Zero))
            {
                goodData = false;
                validationMessage = "Les heures de début et de fin ne peuvent pas être identiques";
                return Json(new { goodData, Message = validationMessage }, JsonRequestBehavior.AllowGet);
            }
            if (data.StartTime == null || data.EndTime == null)
            {
                goodData = false;
                validationMessage = "Les heures de début et de fin ne peuvent être nulles";
                return Json(new { goodData, Message = validationMessage }, JsonRequestBehavior.AllowGet);
            }
            //Overlap validation
            foreach (var timeslot in timeslots)
            {
                //timeslot
                var startTime = timeslot.StartTime;
                var end = timeslot.EndTime < timeslot.StartTime ? timeslot.EndTime.Add(TimeSpan.FromDays(1)) : timeslot.EndTime;
                var duration = end - startTime;
                var endTime = startTime + duration;

                var tsStart = DateTime.Today.Add(startTime);
                var tsEnd = DateTime.Today.Add(endTime);

                //data
                startTime = data.StartTime;
                end = data.EndTime < data.StartTime ? data.EndTime.Add(TimeSpan.FromDays(1)) : data.EndTime;
                duration = end - startTime;
                endTime = startTime + duration;
                var dataStart = DateTime.Today.Add(startTime);
                var dataEnd = DateTime.Today.Add(endTime);
                

                if ((dataStart < tsEnd && tsStart < dataEnd) || (dataStart.AddDays(-1) < tsEnd && tsStart < dataEnd.AddDays(-1)) || (dataStart.AddDays(1) < tsEnd && tsStart < dataEnd.AddDays(1)))
                {
                    goodData = false;
                    validationMessage = "La plage horaire ne peut pas déborder sur une autre plage horaire";
                    break;
                }
            }
            var result = new { goodData, Message = validationMessage };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> RowDropHandler(List<TimeslotViewModel> changed, int dropIndex)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                //RowDropModel dropDetails = (RowDropModel)ser.Deserialize(Request.Headers["rowDropDetails"], typeof(RowDropModel));
                if (changed != null)
                {
                    var timeslots = await _prepareService.GetTimeslots();
                    var items = timeslots.ToObservableCollection();
                    var oldIndex = items.IndexOf(items.FirstOrDefault(i => i.TimeslotId == changed.FirstOrDefault().TimeslotId));
                    items.Move(oldIndex, dropIndex);
                    var countReorder = 1;
                    foreach (var item in items)
                    {
                        item.StartTracking();
                        item.DisplayOrder = countReorder;
                        countReorder++;
                    }
                    Timeslot[] editTimeslots = items.ToArray();
                    await _prepareService.SaveTimeslots(editTimeslots);
                }
                else throw new ArgumentException("Veuillez sélectionner une ligne avant de changer de position");

                return Json(changed, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<JsonResult> GetTimeslots()
        {
            var timeslots = await _prepareService.GetTimeslots();
            var data = timeslots.Select(t => new { t.Label, t.TimeslotId, Color = t.Color != null ? "#" + t.Color.Substring(3) : "" }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProcesses()
        {
            var processes = await _prepareService.GetPublishedProcessesForInspection();
            var data = processes.Select(p => new { p.Label, Id = p.ProcessId }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSchedule(EditParams param)
        {
            if (param.added != null)
            {
                foreach (var add in param.added)
                {
                    var valid = await SaveScheduleValidation(add);
                    if (valid != "")
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, valid);
                    }
                    await InsertSchedule(add);
                }
            }
            if (param.changed != null)
            {
                foreach (var change in param.changed)
                {
                    var valid = await SaveScheduleValidation(change);
                    if (valid != "")
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, valid);
                    }
                    var schedules = await _prepareService.GetInspectionSchedulesNonFilter(change.Id);
                    await UpdateSchedule(schedules.FirstOrDefault(), change);
                }
            }
            if (param.deleted != null)
            {
                foreach (var delete in param.deleted)
                {
                    await DeleteSchedule(delete.Id);
                }
            }
            if(param.value != null)
            {
                var valid = await SaveScheduleValidation(param.value);
                if (valid != "")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, valid);
                }
                var schedules = await _prepareService.GetInspectionSchedulesNonFilter(param.value.Id);
                if (!schedules.Any())
                {
                    await InsertSchedule(param.value);
                }
                else
                {
                    await UpdateSchedule(schedules.FirstOrDefault(), param.value);
                }
            }
            if (param.action == "remove" || (param.key != null && param.value == null))
            {
                await DeleteSchedule(Convert.ToInt32(param.key));
            }
            var data = await GetSchedules();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public Task InsertSchedule(InspectionScheduleViewModel scheduleViewModel)
        {   
            var schedule = new InspectionSchedule
            {
                ProcessId = scheduleViewModel.ProcessId,
                StartDate = scheduleViewModel.StartTime.Date,
                TimeslotId = scheduleViewModel.TimeslotId,
                RecurrenceRule = scheduleViewModel.RecurrenceRule,
                RecurrenceException = scheduleViewModel.RecurrenceException,
                RecurrenceId = scheduleViewModel.RecurrenceID
            };
            schedule.MarkAsAdded();

            return _prepareService.SaveInspectionSchedule(schedule);
        }

        public Task UpdateSchedule(InspectionSchedule schedule, InspectionScheduleViewModel scheduleViewModel)
        {
            schedule.ProcessId = scheduleViewModel.ProcessId;
            schedule.StartDate = scheduleViewModel.StartTime.Date;
            schedule.TimeslotId = scheduleViewModel.TimeslotId;
            schedule.RecurrenceRule = scheduleViewModel.RecurrenceRule;
            schedule.RecurrenceException = scheduleViewModel.RecurrenceException;
            schedule.RecurrenceId = scheduleViewModel.RecurrenceID;
            schedule.MarkAsModified();

            return _prepareService.SaveInspectionSchedule(schedule);
        }

        public async Task DeleteSchedule(int scheduleId)
        {
            var schedules = await _prepareService.GetInspectionSchedulesNonFilter(scheduleId);
            var schedule = schedules.FirstOrDefault();
            schedule.MarkAsDeleted();
            await _prepareService.SaveInspectionSchedule(schedule);
        }

        //Needed by Syncfusion for Crud of Schedule
        public class EditParams
        {
            public string key { get; set; }
            public string action { get; set; }
            public List<InspectionScheduleViewModel> added { get; set; }
            public List<InspectionScheduleViewModel> changed { get; set; }
            public List<InspectionScheduleViewModel> deleted { get; set; }
            public InspectionScheduleViewModel value { get; set; }
        }
    }
}