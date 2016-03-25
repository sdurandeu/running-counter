namespace RunningCounter.Models
{
    using Identity;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Activity
    {
        public int Id { get; set; }

        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime Date { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Kilometers { get; set; }

        [JsonIgnore]
        [Required]
        public virtual User User { get; set; }
    }
}