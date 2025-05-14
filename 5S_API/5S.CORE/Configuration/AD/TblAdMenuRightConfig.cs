using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdMenuRightConfig : IEntityTypeConfiguration<TblAdMenuRight>
    {
        public void Configure(EntityTypeBuilder<TblAdMenuRight> builder)
        {
            builder.HasKey(x => new { x.MenuId, x.RightId });
        }
    }
}
