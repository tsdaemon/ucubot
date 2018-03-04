using System;
using System.Text;

namespace ucubot.Model
{
    public enum LessonSignalType
    {
        BoringSimple = -1,
        Interesting = 0,
        BoringHard = 1
        
    }
    public static class SignalTypeUtils
    {
        public static LessonSignalType ConvertSlackMessageToSignalType(this string message)
        {
            switch (message)
            {
                case "simple":
                    return LessonSignalType.BoringSimple;
                case "interesting":
                    return LessonSignalType.Interesting;
                case "hard":
                    return LessonSignalType.BoringHard;
                default:
                    throw new CanNotParseSlackCommandException(message);
            }
        }

        public static LessonSignalType Decode(int code)
        {
            switch (code)
            {
                case 1:
                    return LessonSignalType.BoringHard;
                case 0:
                    return LessonSignalType.Interesting;
                case -1:
                    return  LessonSignalType.BoringSimple;
                default:
                    return LessonSignalType.Interesting;   
            }
        }
    }
}
