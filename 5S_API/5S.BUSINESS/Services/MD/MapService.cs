using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.MD;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE;
using FirebaseAdmin.Messaging;
using Google.Api;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PlX5S.BUSINESS.Services.MD
{
    public interface IMapService : IGenericService<TblMdMap, MapDto>
    {
        Task<IList<MapDto>> GetAll(BaseMdFilter filter);
        Task<Object> Insert(MapDto map);
        Task<List<MapDto>> GetNearbyStations(double lat, double lng);
        //Task<byte[]> Export(BaseMdFilter filter);
    }
    public class MapService(AppDbContext dbContext, IMapper mapper) : GenericService<TblMdMap, MapDto>(dbContext, mapper), IMapService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblMdMap.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.Id.ToString().Contains(filter.KeyWord));
                }
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive);
                }
                return await Paging(query, filter);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public async Task<Object> Insert(MapDto map)
        {
            try
            {
                // Lấy tất cả trạm xăng hiện có
                var allStations = _dbContext.TblMdMap.ToList();

                foreach (var station in allStations)
                {
                    double dist = GetDistance(
                        (double)station.ViDo,
                        (double)station.KinhDo,
                        (double)map.ViDo,
                        (double)map.KinhDo
                    );

                    if (dist < 50)
                    {
                        Status = false;
                        this.MessageObject.MessageDetail = "Trong bán kính 50m đã có trạm xăng khác.";
                        return null;
                    }
                }

                // =============== CHO INSERT ======================
                map.Id = Guid.NewGuid().ToString();
              
                _dbContext.TblMdMap.Add(_mapper.Map<TblMdMap>(map));

                Status = true;
                this.MessageObject.MessageDetail = "Thêm trạm xăng thành công.";
                return map;
            }
            catch (Exception ex)
            {
                Status = false;
                return null;
            }
        }


        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // bán kính Trái Đất (m)
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // khoảng cách tính bằng mét
        }

        public async Task<List<MapDto>> GetNearbyStations(double lat, double lng)
        {
            try
            {
                var allStations = _mapper.Map<List<MapDto>>(_dbContext.TblMdMap.ToList()); 

                var nearbyStations = new List<MapDto>();

                foreach (var station in allStations)
                {
                    double dist = GetDistance(
                        (double)station.ViDo,
                        (double)station.KinhDo,
                        lat,
                        lng
                    );

                    if (dist <= 2000) // 2km = 2000 mét
                    {
                        station.KhoangCach = (decimal?)dist;
                        nearbyStations.Add(station);
                    }
                }


                Status = true;
                this.MessageObject.MessageDetail = "Lấy danh sách trạm xăng thành công.";
                return nearbyStations;
            }
            catch (Exception ex)
            {
                Status = false;
                this.MessageObject.MessageDetail = "Có lỗi xảy ra: " + ex.Message;
                return null;
            }
        }

        //public async Task<byte[]> Export(BaseMdFilter filter)
        //{
        //    try
        //    {
        //        var query = _dbContext.TblMdMap.AsQueryable();
        //        if (!string.IsNullOrWhiteSpace(filter.KeyWord))
        //        {
        //            query = query.Where(x => x.Title.Contains(filter.KeyWord));
        //        }
        //        if (filter.IsActive.HasValue)
        //        {
        //            query = query.Where(x => x.IsActive == filter.IsActive);
        //        }
        //        var data = await base.GetAllMd(query, filter);
        //        int i = 1;
        //        //data.ForEach(x =>
        //        //{
        //        //    x.OrdinalNumber = i++;
        //        //});
        //        return await ExportExtension.ExportToExcel(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        Status = false;
        //        Exception = ex;
        //        return null;
        //    }
        //}

        public async Task<IList<MapDto>> GetAll(BaseMdFilter filter)
        {
            try
            {
                var query = _dbContext.TblMdMap.AsQueryable();
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive);
                }
                return await base.GetAllMd(query, filter);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
    }
}
