using DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace FluentAPI.EntityConfigurations
{
    public class CourseConfiguration : EntityTypeConfiguration<Course>
    {
        public CourseConfiguration()
        {
            // Table overrides
            // Primary Keys overrides
            // Property configurations
            Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

            Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);


            // Relationships
            HasRequired(c => c.Author)
            .WithMany(a => a.Courses)
            .HasForeignKey(c => c.AuthorId)
            .WillCascadeOnDelete(false);

            HasRequired(c => c.Cover)
            .WithRequiredPrincipal(c => c.Course);

            HasMany(c => c.Tags)
            .WithMany(t => t.Courses)
            .Map(m =>
            {
                m.ToTable("CourseTags");
                m.MapLeftKey("CourseId");
                m.MapRightKey("TagId");
            });
        }
    }
}