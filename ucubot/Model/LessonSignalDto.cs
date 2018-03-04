using System;

namespace ucubot.Model
{
    public class LessonSignalDto
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}