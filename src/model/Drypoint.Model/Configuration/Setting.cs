using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Drypoint.Model.Common.Auditing;
using Drypoint.Model.Common;

namespace Drypoint.Model.Configuration
{
    [Table("DrypointSettings")]
    public class Setting : AuditedEntity<long>
    {
        public virtual long? UserId { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string Name { get; set; }

        [StringLength(2000)]
        public virtual string Value { get; set; }

        public Setting()
        {

        }

        public Setting(long? userId, string name, string value)
        {
            UserId = userId;
            Name = name;
            Value = value;
        }
    }
}
