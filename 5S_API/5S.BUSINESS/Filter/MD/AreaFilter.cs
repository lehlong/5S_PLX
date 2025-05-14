using Common;
namespace PLX5S.BUSINESS.Filter.MD
{
    public class AreaFilter : BaseFilter
    {
        public int? PartnerId { get; set; }
    }

    public class AreaGetAllFilter : BaseMdFilter
    {
        public int? PartnerId { get; set; }
    }
}
