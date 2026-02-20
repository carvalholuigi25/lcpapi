using System.Runtime.Serialization;

namespace lcpapi.Models.Enums;

public enum EUserPrivacy {
    [EnumMember(Value = "locked")]
    locked = 0,
    [EnumMember(Value = "all")]
    all = 1
}