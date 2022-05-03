using GroupCalendar.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GroupCalendar.Data.Remote.Model
{


    public partial class EventModel : ObservableObject
    {
        [JsonProperty("color")]
        private CustomColorModel color = new CustomColorModel(255, 100, 100);

        [JsonProperty("confirmedUsers", NullValueHandling = NullValueHandling.Ignore)]
        private List<string> confirmedUsers = new List<string>();

        [JsonProperty("description")]
        private string? description;

        [JsonProperty("end")]
        private DateTimeOffset end;

        [JsonProperty("id")]
        private Guid id = Guid.NewGuid();

        [JsonProperty("name")]
        private string name = "";

        [JsonProperty("recurrenceId", NullValueHandling = NullValueHandling.Ignore)]
        private Guid? recurrenceId;

        [JsonProperty("requireConfirmation")]
        private bool requireConfirmation;

        [JsonProperty("start")]
        private DateTimeOffset start;



        public EventModel() { }



        #region Properties
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

        [JsonIgnore]
        public CustomColorModel Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public List<string> ConfirmedUsers
        {
            get { return confirmedUsers; }
            set
            {
                confirmedUsers = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public string? Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public DateTimeOffset End
        {
            get { return end; }
            set
            {
                end = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public DateTimeOffset Start
        {
            get { return start; }
            set
            {
                start = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool RequireConfirmation
        {
            get { return requireConfirmation; }
            set
            {
                requireConfirmation = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Guid? RecurrenceId
        {
            get { return recurrenceId; }
            set
            {
                recurrenceId = value;
                OnPropertyChanged();
            }
        }
        #endregion

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
