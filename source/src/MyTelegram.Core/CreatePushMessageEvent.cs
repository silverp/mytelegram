﻿namespace MyTelegram.Core;

public record CreatePushMessageEvent(
    Peer ToPeer,
    byte[] Data,
    int Pts,
    long OnlyPushToThisAuthKeyId,
    PtsType PtsType /*,Dictionary<string,string> OtherData*/);