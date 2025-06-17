namespace PLX5S.BUSINESS.Dtos.Auth
{
    public class LoginDto
    {
        //console.log('đây là', info.name,info.operatingSystem, info.model, info.manufacturer, info.osVersion);

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ? DeviceId { get; set; }
        public string ? DeviceName { get; set; }

        public string ? OperatingSystem { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? osVersion { get; set; }

    }
}
