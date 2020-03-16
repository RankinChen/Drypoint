using Drypoint.Model.Common;
using Drypoint.Unity.Extensions;
using DrypointException;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Drypoint.Model.Auditing
{
    [Table("DrypointAuditLogs")]
    public class AuditLog : Entity<long>
    {
        public virtual long? UserId { get; set; }

        public virtual string ServiceName { get; set; }
        
        public virtual string MethodName { get; set; }

        public virtual string Parameters { get; set; }

        public virtual string ReturnValue { get; set; }

        public virtual DateTime ExecutionTime { get; set; }

        public virtual int ExecutionDuration { get; set; }

        public virtual string ClientIpAddress { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string BrowserInfo { get; set; }

        public virtual string Exception { get; set; }

        public virtual long? ImpersonatorUserId { get; set; }

        public virtual int? ImpersonatorTenantId { get; set; }

        public virtual string CustomData { get; set; }

        public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        {
            var exceptionMessage = GetClearException(auditInfo.Exception);
            return new AuditLog
            {
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(256),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(256),
                Parameters = auditInfo.Parameters.TruncateWithPostfix(1024),
                ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(1024),
                ExecutionTime = auditInfo.ExecutionTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(64),
                ClientName = auditInfo.ClientName.TruncateWithPostfix(128),
                BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(512),
                Exception = exceptionMessage.TruncateWithPostfix(2000),
                ImpersonatorUserId = auditInfo.ImpersonatorUserId,
                ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
                CustomData = auditInfo.CustomData.TruncateWithPostfix(2000)
            };
        }

        public override string ToString()
        {
            return string.Format(
                "AUDIT LOG: {0}.{1} is executed by user {2} in {3} ms from {4} IP address.",
                ServiceName, MethodName, UserId, ExecutionDuration, ClientIpAddress
                );
        }

        public static string GetClearException(Exception exception)
        {
            var clearMessage = "";
            switch (exception)
            {
                case null:
                    return null;

                case ValidationException validationException:
                    clearMessage = "There are " + validationException.ValidationErrors.Count + " validation errors:";
                    foreach (var validationResult in validationException.ValidationErrors)
                    {
                        var memberNames = "";
                        if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                        {
                            memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                        }

                        clearMessage += "\r\n" + validationResult.ErrorMessage + memberNames;
                    }
                    break;

                case UserFriendlyException userFriendlyException:
                    clearMessage =
                        $"UserFriendlyException.Code:{userFriendlyException.Code}\r\nUserFriendlyException.Details:{userFriendlyException.Details}";
                    break;
            }

            return exception + (clearMessage.IsNullOrWhiteSpace() ? "" : "\r\n\r\n" + clearMessage);
        }
    }
}
