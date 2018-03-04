using System;

namespace ucubot.Model
{
    public class LessonSignalDto
    {
        public string UserId { get; set; }
        public int Type { get; set; }
        public DateTime Timestamp { get; set; }
        public int Id { get; set; }
       
    }
}