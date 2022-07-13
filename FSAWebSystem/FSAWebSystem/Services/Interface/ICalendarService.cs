﻿using FSAWebSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FSAWebSystem.Services.Interface
{
    public interface ICalendarService
    {
        public Task<FSACalendarHeader> GetFSACalendarHeader(int month, int year);

        public Task<FSACalendarDetail> GetCalendarDetail(DateTime date); 
        public List<SelectListItem> GetListMonth();
		public List<SelectListItem> GetListYear();

        public Task AddCalendar(FSACalendarHeader fsaCalendar);
    }
}
