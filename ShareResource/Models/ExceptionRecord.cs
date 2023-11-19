using System;

namespace ShareResource.Models
{
    public class ExceptionRecord
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
    }
}