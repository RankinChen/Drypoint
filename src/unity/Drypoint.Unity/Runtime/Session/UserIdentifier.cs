using Drypoint.Unity.Dependency;
using Drypoint.Unity.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Drypoint.Unity.Runtime.Session
{
    public class UserIdentifier : IUserIdentifier, IScopedDependency
    {

        public long UserId { get; set; }

        public long? RoleId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// </summary>
        protected UserIdentifier()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId">用户角色Id</param>
        /// <param name="userId">用户Id</param>
        public UserIdentifier(long? roleId, long userId)
        {
            RoleId = roleId;
            UserId = userId;
        }

        /// <summary>
        /// 格式  用户Id@角色Id
        /// </summary>
        /// <param name="userIdentifierString"></param>
        /// <returns></returns>
        public static UserIdentifier Parse(string userIdentifierString)
        {
            if (userIdentifierString.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userIdentifierString), "userAtTenant can not be null or empty!");
            }

            var splitted = userIdentifierString.Split('@');
            if (splitted.Length == 1)
            {
                return new UserIdentifier(null, splitted[0].To<long>());

            }

            if (splitted.Length == 2)
            {
                return new UserIdentifier(splitted[1].To<long>(), splitted[0].To<long>());
            }

            throw new ArgumentException("userAtTenant is not properly formatted", nameof(userIdentifierString));
        }

        /// <summary>
        /// 格式 用户Id@格式Id
        /// </summary>
        /// <returns></returns>
        public string ToUserIdentifierString()
        {
            if (RoleId == null)
            {
                return UserId.ToString();
            }

            return UserId + "@" + RoleId.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserIdentifier))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (UserIdentifier)obj;

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return RoleId == other.RoleId && UserId == other.UserId;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = RoleId.HasValue ? hash * 23 + RoleId.GetHashCode() : hash;
            hash = hash * 23 + UserId.GetHashCode();
            return hash;
        }
        public static bool operator ==(UserIdentifier left, UserIdentifier right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(UserIdentifier left, UserIdentifier right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return ToUserIdentifierString();
        }
    }
}
