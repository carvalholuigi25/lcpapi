using System.Runtime.Serialization;

namespace lcpapi.Models.Enums;

public enum EUsersRoles {
    [EnumMember(Value = "user")]
    user,
    [EnumMember(Value = "guest")]
    guest,
    [EnumMember(Value = "moderator")]
    moderator,
    [EnumMember(Value = "admin")]
    admin
}