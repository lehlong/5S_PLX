using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Models;
using PLX5S.BUSINESS.Dtos.AD;
using System.Text.Json;
using Dtos.AD;
using Microsoft.Extensions.Logging;

namespace Services.AD
{
    public interface IFirebaseNotificationService
    {
        Task<FirebaseNotificationResponseDto> SendNotificationAsync(FirebaseNotificationDto notification);
        //Task<FirebaseNotificationResponseDto> SendToTopicAsync(string topic, string title, string body);
        Task<FirebaseNotificationResponseDto> SendToTopicAsync(string topic, string title, string body, object? data);
        Task<FirebaseNotificationResponseDto> SendToTokenAsync(string token, string title, string body);
        Task<FirebaseNotificationResponseDto> TestConnectionAsync();
        Task<FirebaseNotificationResponseDto> SendToTopicTestAsync(string title, string body);
    }

    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        private readonly FirebaseSettings _firebaseSettings;
        private readonly ILogger<FirebaseNotificationService> _logger;
        private FirebaseApp? _firebaseApp;

        public FirebaseNotificationService(IOptions<FirebaseSettings> firebaseSettings, ILogger<FirebaseNotificationService> logger)
        {
            _firebaseSettings = firebaseSettings.Value;
            _logger = logger;
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    GoogleCredential credential;
                    if (!string.IsNullOrEmpty(_firebaseSettings.CredentialPath) &&
                        File.Exists(_firebaseSettings.CredentialPath))
                    {
                        credential = GoogleCredential.FromFile(_firebaseSettings.CredentialPath);
                    }
                    else
                    {
                        credential = GoogleCredential.FromJson(JsonSerializer.Serialize(new
                        {
                            type = "service_account",
                            project_id = _firebaseSettings.ProjectId,
                            private_key_id = _firebaseSettings.PrivateKeyId,
                            private_key = _firebaseSettings.PrivateKey,
                            client_email = _firebaseSettings.ClientEmail,
                            client_id = _firebaseSettings.ClientId,
                            auth_uri = _firebaseSettings.AuthUri,
                            token_uri = _firebaseSettings.TokenUri,
                            auth_provider_x509_cert_url = _firebaseSettings.AuthProviderX509CertUrl,
                            client_x509_cert_url = _firebaseSettings.ClientX509CertUrl
                        }));
                    }

                    _firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential,
                        ProjectId = _firebaseSettings.ProjectId
                    });
                }
                else
                {
                    _firebaseApp = FirebaseApp.DefaultInstance;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khởi tạo Firebase: {ex.Message}");
            }
        }

        public async Task<FirebaseNotificationResponseDto> SendToTopicTestAsync(string title, string body)
        {
            var data = new DataFireBase()
            {
                content = body
            };
            return await SendToTopicAsync("test", "Đồng chí có công việc cần xử lý", body, data);
        }
        public async Task<FirebaseNotificationResponseDto> SendNotificationAsync(FirebaseNotificationDto notification)
        {
            try
            {
                if (_firebaseApp == null)
                {
                    var errorMessage = "Firebase chưa được khởi tạo";
                    _logger?.LogError(errorMessage);
                    return new FirebaseNotificationResponseDto
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }

                var message = new Message
                {
                    Notification = new Notification
                    {
                        Title = notification.Title,
                        Body = notification.Body
                    },
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ChannelId = "default"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default"
                        }
                    }
                };

                // Add custom data if provided
                if (notification.Data != null)
                {
                    try
                    {
                        if (notification.Data is Dictionary<string, string> dict)
                        {
                            message.Data = dict;
                        }
                        else
                        {
                            var json = JsonSerializer.Serialize(notification.Data);
                            var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                            if (dataDict != null)
                            {
                                message.Data = dataDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "");
                            }
                        }
                    }
                    catch (Exception dataEx)
                    {
                        _logger?.LogWarning(dataEx, "⚠️ Failed to serialize notification data");
                    }
                }

                // Set target (token or topic)
                if (!string.IsNullOrEmpty(notification.Token))
                {
                    message.Token = notification.Token;
                }
                else if (!string.IsNullOrEmpty(notification.Topic))
                {
                    message.Topic = notification.Topic;
                }

                var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
                var response = await messaging.SendAsync(message);

                _logger?.LogInformation("✅ Notification sent successfully. MessageId: {MessageId}", response);

                return new FirebaseNotificationResponseDto
                {
                    Success = true,
                    Message = "Thông báo đã được gửi thành công",
                    MessageId = response
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Failed to send notification: {Error}", ex.Message);
                return new FirebaseNotificationResponseDto
                {
                    Success = false,
                    Message = $"Lỗi gửi thông báo: {ex.Message}"
                };
            }
        }


        //public async Task<FirebaseNotificationResponseDto> SendToTopicAsync(string topic, string title, string body)
        //{
        //    var notification = new FirebaseNotificationDto
        //    {
        //        Topic = topic,
        //        Title = title,
        //        Body = body
        //    };
        //    return await SendNotificationAsync(notification);
        //}

        public async Task<FirebaseNotificationResponseDto> SendToTopicAsync(string topic, string title, string body, object? data)
        {
            var notification = new FirebaseNotificationDto
            {
                Topic = topic,
                Title = title,
                Body = body,
                Data = data
            };
            return await SendNotificationAsync(notification);
        }

        public async Task<FirebaseNotificationResponseDto> SendToTokenAsync(string token, string title, string body)
        {
            var notification = new FirebaseNotificationDto
            {
                Token = token,
                Title = title,
                Body = body
            };
            return await SendNotificationAsync(notification);
        }

        public async Task<FirebaseNotificationResponseDto> TestConnectionAsync()
        {
            try
            {
                if (_firebaseApp == null)
                {
                    return new FirebaseNotificationResponseDto
                    {
                        Success = false,
                        Message = "Firebase chưa được khởi tạo"
                    };
                }

                // Test bằng cách gửi thông báo đến topic test
                var message = new Message
                {
                    Topic = "test",
                    Notification = new Notification
                    {
                        Title = "Test Connection",
                        Body = "Firebase connection test"
                    }
                };

                var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
                var response = await messaging.SendAsync(message);

                return new FirebaseNotificationResponseDto
                {
                    Success = true,
                    Message = "Kết nối Firebase thành công",
                    MessageId = response
                };
            }
            catch (Exception ex)
            {
                return new FirebaseNotificationResponseDto
                {
                    Success = false,
                    Message = $"Lỗi kết nối Firebase: {ex.Message}"
                };
            }
        }
    }
}
