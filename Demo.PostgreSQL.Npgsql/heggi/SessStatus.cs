using System;
using System.Runtime.Serialization;

namespace Demo.PostgreSQL.Npgsql.heggi
{
    /*
    [Flags]
    public enum SessStatus
    {
        [EnumMember(Value = "active")]
        Active,

        [EnumMember(Value = "stopreq")]
        Stopreq = 1,

        [EnumMember(Value = "stopping")]
        Stopping = 2,
        [EnumMember(Value = "aaa")]
        AAA = 4,
        [EnumMember(Value = "bbb")]
        BBB = 8,
    }*/
    public enum SessStatus
    {
        [EnumMember(Value = "active")]
        Active,

        [EnumMember(Value = "stopreq")]
        Stopreq,

        [EnumMember(Value = "stopping")]
        Stopping,
        [EnumMember(Value = "aaa")]
        AAA,
        [EnumMember(Value = "bbb")]
        BBB,
    }
}
