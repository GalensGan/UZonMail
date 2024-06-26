﻿using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Controllers.Emails.Models
{
    public class SendingGroupStatusInfo
    {

        public int Id { get; set; }        
        public double TotalCount { get; set; }
        public int SentCount { get; set; }
        public int SuccessCount { get; set; }
        public SendingGroupStatus Status { get; set; }

        public SendingGroupStatusInfo(SendingGroup group)
        {
            Id = group.Id;           
            TotalCount = group.TotalCount;
            SentCount = group.SentCount;
            SuccessCount = group.SuccessCount;
            Status = group.Status;
        }

    }
}
