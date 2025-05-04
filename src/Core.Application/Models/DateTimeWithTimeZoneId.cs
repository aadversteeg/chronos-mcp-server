using System;

namespace Core.Application.Models
{
    public record DateTimeWithTimeZoneId(DateTime CurrentDateTime, TimeZoneId UsedTimezoneId);
}
