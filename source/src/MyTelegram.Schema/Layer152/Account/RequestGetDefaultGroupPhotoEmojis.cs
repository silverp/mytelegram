﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
///See <a href="https://core.telegram.org/method/account.getDefaultGroupPhotoEmojis" />
///</summary>
[TlObject(0x915860ae)]
public sealed class RequestGetDefaultGroupPhotoEmojis : IRequest<MyTelegram.Schema.IEmojiList>
{
    public uint ConstructorId => 0x915860ae;
    public long Hash { get; set; }

    public void ComputeFlag()
    {

    }

    public void Serialize(BinaryWriter bw)
    {
        ComputeFlag();
        bw.Write(ConstructorId);
        bw.Write(Hash);
    }

    public void Deserialize(BinaryReader br)
    {
        Hash = br.ReadInt64();
    }
}
