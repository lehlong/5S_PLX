using PLX5S.BUSINESS.Models;
using PLX5S.BUSINESS.Services.BU;
using PLX5S.CORE;
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

        public BackgroundJobService(AppDbContext dbContext, IKikhaosatService kikhaosatService)
        {
            _dbContext = dbContext;
            _kikhaosatService = kikhaosatService;
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
                var now = DateTime.Now;

                var lstSurvey = _dbContext.TblBuSurveyMgmt
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.Id)
                    .ToList();

                foreach (var item in lstSurvey)
                {
                    var check = _dbContext.TblBuKiKhaoSat
                        .FirstOrDefault(x =>
                            x.StartDate.Month == now.Month &&
                            x.StartDate.Year == now.Year &&
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
                //this.Status = false;
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
        private DateTime LastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

    }
}
