namespace RunningCounter.Models
{
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeKindAttribute : Attribute
    {
        private readonly DateTimeKind kind;

        public DateTimeKindAttribute(DateTimeKind kind)
        {
            this.kind = kind;
        }

        public DateTimeKind Kind
        {
            get
            {
                return this.kind;
            }
        }

        public static void Apply(object entity)
        {
            if (entity == null)
            {
                return;
            }

            var properties = entity.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<DateTimeKindAttribute>();
                if (attr == null)
                {
                    continue;
                }

                var dateTime = property.PropertyType == typeof(DateTime?) ? (DateTime?)property.GetValue(entity) : (DateTime)property.GetValue(entity);

                if (dateTime == null)
                {
                    continue;
                }

                property.SetValue(entity, DateTime.SpecifyKind(dateTime.Value, attr.Kind));
            }
        }
    }
}