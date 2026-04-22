using Dtos.AD;
using PLX5S.BUSINESS.Common.Enum;
using PLX5S.BUSINESS.Models;
using PLX5S.BUSINESS.Services.BU;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using PLX5S.CORE.Statics;
using Services.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BackgroundHangfire
{
    public class BackgroundJobService
    {
        private readonly AppDbContext _dbContext;
        private readonly IKikhaosatService _kikhaosatService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;

        public BackgroundJobService(AppDbContext dbContext, IKikhaosatService kikhaosatService, IFirebaseNotificationService firebaseNotificationService)
        {
            _dbContext = dbContext;
            _kikhaosatService = kikhaosatService;
            _firebaseNotificationService = firebaseNotificationService;
        }

        public async Task TestJobHangfire()
        {
            try
            {
                var ky = _dbContext.TblBuKiKhaoSat.FirstOrDefault();
                ky.Des = "backGroupJob hangfire thành công";
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task AutoStartKy()
        {
            try
            {
                var now = DateTime.Today;

                var lstSurvey = _dbContext.TblBuSurveyMgmt
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.Id)
                    .ToList();

                foreach (var item in lstSurvey)
                {
                    var check = _dbContext.TblBuKiKhaoSat
                        .FirstOrDefault(x =>
                            x.EndDate.Month == now.Month &&
                            x.EndDate.Year == now.Year &&
                            x.TrangThaiKi == "1" &&
                            x.SurveyMgmtId == item.Id);

                    if (check == null)
                    {
                        continue;
                    }

                    check.TrangThaiKi = "2"; // Cập nhật trạng thái
                    await _kikhaosatService.UpdateKhaoSatTrangThai(check);
                }

            }
            catch (Exception ex)
            {
            }
        }


        public async Task AutoCloseKy()
        {
            try
            {
                var now = DateTime.Now;

                // Xác định tháng/năm cần đóng
                var prevMonth = now.Month == 1 ? 12 : now.Month - 1;
                var prevYear = now.Month == 1 ? now.Year - 1 : now.Year;

                var lstSurvey = _dbContext.TblBuSurveyMgmt
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.Id)
                    .ToList();

                foreach (var item in lstSurvey)
                {
                    var check = _dbContext.TblBuKiKhaoSat
                        .FirstOrDefault(x =>
                            x.EndDate.Month == prevMonth &&
                            x.EndDate.Year == prevYear &&
                            x.SurveyMgmtId == item.Id);

                    if (check == null || check.TrangThaiKi == "1" || check.TrangThaiKi == "0")
                    {
                        continue;
                    }

                    check.TrangThaiKi = "0"; // Cập nhật trạng thái sang "đóng"
                    await _kikhaosatService.UpdateKhaoSatTrangThai(check);
                }

            }
            catch (Exception ex)
            {
                // log lỗi ra file/db thay vì bỏ trống
            }
        }


        public async Task AutoCreateKy()
        {
            try
            {
                var now = DateTime.Now;
                var lstSurvey = _dbContext.TblBuSurveyMgmt
                    //.Where(x => x.IsActive == true)
                    .OrderBy(x => x.Id)
                    .ToList();

                foreach (var item in lstSurvey)
                {
                    var check = _dbContext.TblBuKiKhaoSat
                        .FirstOrDefault(x =>
                            x.StartDate.Month == now.Month &&
                            x.StartDate.Year == now.Year &&
                            x.SurveyMgmtId == item.Id);

                    if (check != null)
                    {
                        continue;
                    }
                    var newKy = new KiKhaoSatModel();
                    var ky = _dbContext.TblBuKiKhaoSat
                        .FirstOrDefault(x =>
                            x.EndDate.Month == now.Month &&
                            x.EndDate.Year == now.Year &&
                            x.SurveyMgmtId == item.Id);


                    if (ky == null) continue;

                    newKy = await _kikhaosatService.getKyCopy(ky.Id);

                    // prev end
                    var prevEnd = ky.EndDate;

                    // StartDate = last day of this month
                    var newStart = LastDayOfMonth(prevEnd);

                    // EndDate = last day of next month
                    var next = prevEnd.AddMonths(1);
                    var newEnd = LastDayOfMonth(next);

                    newKy.KiKhaoSat.StartDate = newStart;
                    newKy.KiKhaoSat.EndDate = newEnd;

                    newKy.KiKhaoSat.Name = $"Tháng {newEnd.Month:D2}/{newEnd.Year}";
                    newKy.KiKhaoSat.Code = $"T{newEnd.Month:D2}{newEnd.Year}";
                    newKy.KiKhaoSat.TrangThaiKi = "1";

                    await _kikhaosatService.Insert(newKy);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public async Task NotifyCanhBaoDenDotCham()
        {
            try
            {
                var now = DateTime.Now;
                //var now = new DateTime(2026, 3, 3);
                var lastDay = LastDayOfMonth(now);
                var Notis = new List<TblBuNotification>();

                if (now.Day == 7)
                {
                    var lstUserATVSV = _dbContext.TblAdAccount.Where(x => x.ChucVuId == RoleIds.ATVSV).ToList();

                    var lstTokenNoti = _dbContext.TblBuUserTokenNoti.Where(x => lstUserATVSV.Select(x => x.UserName).Contains(x.UserName)).ToList();

                    var noti = _dbContext.TblBuNotification.FirstOrDefault(x => x.CreateDate.Value.Day == now.Day &&
                                                                                x.CreateDate.Value.Month == now.Month &&
                                                                                x.CreateDate.Value.Year == now.Year);
                    if(noti != null)
                    {
                        return;
                    }

                    foreach (var item in lstUserATVSV)
                    {
                        Notis.Add(new TblBuNotification
                        {
                            Code = Guid.NewGuid().ToString(),
                            Title = "Đến thời gian chấm điểm 5S",
                            Body = $"Kính báo chuẩn bị đến thời gian chấm điểm của An toàn vệ sinh viên, từ ngày 08-15/{now.Month}",
                            UserName = item.UserName,
                            IsRead = false,
                            IsSend = true,
                        });
                    }
                    _dbContext.TblBuNotification.AddRange(Notis);

                    _dbContext.SaveChanges();

                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNoti.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của An toàn vệ sinh viên");

                }

                if (now.Day == 15)
                {
                    var lstUserTT = _dbContext.TblAdAccount.Where(x => x.ChucVuId == RoleIds.CHT || x.ChucVuId == RoleIds.TK).ToList();

                    var lstTokenNotiTK = _dbContext.TblBuUserTokenNoti.Where(x => lstUserTT.Where(x => x.ChucVuId == RoleIds.TK).Select(x => x.UserName).Contains(x.UserName)).ToList();
                    var lstTokenNotiCHT = _dbContext.TblBuUserTokenNoti.Where(x => lstUserTT.Where(x => x.ChucVuId == RoleIds.CHT).Select(x => x.UserName).Contains(x.UserName)).ToList();

                    var noti = _dbContext.TblBuNotification.FirstOrDefault(x => x.CreateDate.Value.Day == now.Day &&
                                                                                x.CreateDate.Value.Month == now.Month &&
                                                                                x.CreateDate.Value.Year == now.Year);
                    if (noti != null)
                    {
                        return;
                    }


                    foreach (var item in lstUserTT)
                    {
                        var chucVu = item.ChucVuId == RoleIds.CHT ? "Cửa Hàng Trưởng" : "Trưởng Kho";
                        Notis.Add(new TblBuNotification
                        {
                            Code = Guid.NewGuid().ToString(),
                            Title = "Đến thời gian chấm điểm 5S",
                            Body = $"Kính báo chuẩn bị đến thời gian chấm điểm của {chucVu}, từ ngày 16-23/{now.Month}",
                            UserName = item.UserName,
                            IsRead = false,
                            IsSend = true,
                        });
                    }
                    _dbContext.TblBuNotification.AddRange(Notis);

                    _dbContext.SaveChanges();
                    
                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNotiTK.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của Trưởng Kho");
                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNotiCHT.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của Cửa Hàng Trưởng");
                }

                if (now.Day == 23)
                {
                    var lstUserATVSV = _dbContext.TblAdAccount.Where(x => x.ChucVuId == RoleIds.ATVSV).ToList();

                    var lstTokenNoti = _dbContext.TblBuUserTokenNoti.Where(x => lstUserATVSV.Select(x => x.UserName).Contains(x.UserName)).ToList();

                    var noti = _dbContext.TblBuNotification.FirstOrDefault(x => x.CreateDate.Value.Day == now.Day &&
                                                                                x.CreateDate.Value.Month == now.Month &&
                                                                                x.CreateDate.Value.Year == now.Year);
                    if (noti != null)
                    {
                        return;
                    }

                    var newNoti = new TblBuNotification
                    {
                        Code = Guid.NewGuid().ToString(),
                        Title = "Đến thời gian chấm điểm 5S",
                        Body = $"Kính báo chuẩn bị đến thời gian chấm điểm của An toàn vệ sinh viên, từ ngày 24-{lastDay.Day}/{now.Month}",
                    };
                    
                    _dbContext.TblBuNotification.Add(newNoti);
                    _dbContext.SaveChanges();

                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNoti.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của An toàn vệ sinh viên");
                }

                if (now.Day == lastDay.Day)
                {
                    var lstUserTT = _dbContext.TblAdAccount.Where(x => x.ChucVuId == RoleIds.CHT || x.ChucVuId == RoleIds.TK).ToList();

                    var lstTokenNotiTK = _dbContext.TblBuUserTokenNoti.Where(x => lstUserTT.Where(x => x.ChucVuId == RoleIds.TK).Select(x => x.UserName).Contains(x.UserName)).ToList();
                    var lstTokenNotiCHT = _dbContext.TblBuUserTokenNoti.Where(x => lstUserTT.Where(x => x.ChucVuId == RoleIds.CHT).Select(x => x.UserName).Contains(x.UserName)).ToList();

                    var noti = _dbContext.TblBuNotification.FirstOrDefault(x => x.CreateDate.Value.Day == now.Day &&
                                                                                x.CreateDate.Value.Month == now.Month &&
                                                                                x.CreateDate.Value.Year == now.Year);
                    if (noti != null)
                    {
                        return;
                    }


                    foreach (var item in lstUserTT)
                    {
                        var chucVu = item.ChucVuId == RoleIds.CHT ? "Cửa Hàng Trưởng" : "Trưởng Kho";
                        Notis.Add(new TblBuNotification
                        {
                            Code = Guid.NewGuid().ToString(),
                            Title = "Đến thời gian chấm điểm 5S",
                            Body = $"Kính báo chuẩn bị đến thời gian chấm điểm của {chucVu}, từ ngày 01-07/{now.Month}",
                            UserName = item.UserName,
                            IsRead = false,
                            IsSend = true,
                        });
                    }
                    _dbContext.TblBuNotification.AddRange(Notis);

                    _dbContext.SaveChanges();

                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNotiTK.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của Trưởng Kho");
                    await _firebaseNotificationService.SendToTokenListAsync(lstTokenNotiCHT.Select(x => x.Token).ToList(), "Đến thời gian chấm điểm 5S", $"huẩn bị đến thời gian chấm điểm của Cửa Hàng Trưởng");
                }

            }
            catch (Exception ex)
            {
                // log lỗi ra file/db thay vì bỏ trống
            }
        }



        private DateTime LastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

    }
}
