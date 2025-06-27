using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Dtos.AD
{
    public class FirebaseNotificationDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public string? Token { get; set; }

        public string? Topic { get; set; }

        public object? Data { get; set; }
    }

    public class FirebaseNotificationResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? MessageId { get; set; }
    }

    public class SendToTopicRequest
    {
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DataFireBase? Data { get; set; }
    }

    public class SendToTopicTestRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class DataFireBase
    {
        public DateTime date { get; set; } = DateTime.Now;
        public string? content { get; set; }
    }

    public class SendToTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
