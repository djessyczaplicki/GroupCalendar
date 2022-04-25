using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GroupCalendar.Data.Remote.Model
{


    public partial class EventModel
    {
        [JsonProperty("color")]
        public CustomColorModel Color { get; set; } = new CustomColorModel(255, 100, 100);

        [JsonProperty("confirmedUsers", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ConfirmedUsers { get; set; } = new List<string>();

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("end")]
        public DateTimeOffset End { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("recurrenceId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? RecurrenceId { get; set; }

        [JsonProperty("requireConfirmation")]
        public bool RequireConfirmation { get; set; }

        [JsonProperty("start")]
        public DateTimeOffset Start { get; set; }

        public EventModel() { }


        [JsonIgnore]
        public DateTimeOffset StartTime
        {
            get { return Start; }
            set { Start = ChangeTime(Start, value); }
        }

        [JsonIgnore]
        public DateTimeOffset Date
        {
            get { return Start; }
            set { Start = ChangeDate(Start, value); End = ChangeDate(End, value); }
        }

        [JsonIgnore]
        public DateTimeOffset EndTime
        {
            get { return End; }
            set { End = ChangeTime(End, value); }
        }





        private DateTimeOffset ChangeDate(DateTimeOffset dateTimeOffset, DateTimeOffset newDateTimeOffset)
        {
            var changedDateTimeOffset = new DateTimeOffset(dateTimeOffset.DateTime, TimeZoneInfo.Local.BaseUtcOffset);
            changedDateTimeOffset = changedDateTimeOffset.AddYears(newDateTimeOffset.Year - dateTimeOffset.Year);
            changedDateTimeOffset = changedDateTimeOffset.AddMonths(newDateTimeOffset.Month - dateTimeOffset.Month);
            changedDateTimeOffset = changedDateTimeOffset.AddDays(newDateTimeOffset.Day - dateTimeOffset.Day);
            return changedDateTimeOffset;
        }

        private DateTimeOffset ChangeTime(DateTimeOffset dateTimeOffset, DateTimeOffset newDateTimeOffset)
        {
            var changedDateTimeOffset = new DateTimeOffset(dateTimeOffset.DateTime, TimeZoneInfo.Local.BaseUtcOffset);
            changedDateTimeOffset = changedDateTimeOffset.AddHours(newDateTimeOffset.Hour - dateTimeOffset.Hour);
            changedDateTimeOffset = changedDateTimeOffset.AddMinutes(newDateTimeOffset.Minute - dateTimeOffset.Minute);
            changedDateTimeOffset = changedDateTimeOffset.AddSeconds(newDateTimeOffset.Second - dateTimeOffset.Second);
            return changedDateTimeOffset;
        }

        public EventModel(string name, string? description, CustomColorModel color, DateTimeOffset start, DateTimeOffset end, bool requireConfirmation = false, Guid? recurrenceId = null)
        {
            Name = name ?? "";
            Description = description;
            Color = color ?? new CustomColorModel(255, 100, 100);
            Start = start;
            End = end;
            RequireConfirmation = requireConfirmation;
            RecurrenceId = recurrenceId;
        }

        public EventModel(CustomColorModel color, List<string> confirmedUsers, string? description, DateTimeOffset end, string name,
            Guid? recurrenceId, bool requireConfirmation, DateTimeOffset start)
        {
            Color = color ?? throw new ArgumentNullException(nameof(color));
            ConfirmedUsers = confirmedUsers ?? throw new ArgumentNullException(nameof(confirmedUsers));
            Description = description;
            End = end;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RecurrenceId = recurrenceId;
            RequireConfirmation = requireConfirmation;
            Start = start;
        }


    }





}
