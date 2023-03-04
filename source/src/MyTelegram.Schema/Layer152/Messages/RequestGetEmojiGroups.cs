﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
///See <a href="https://core.telegram.org/method/messages.getEmojiGroups" />
///</summary>
[TlObject(0x7488ce5b)]
public sealed class RequestGetEmojiGroups : IRequest<MyTelegram.Schema.Messages.IEmojiGroups>
{
    public uint ConstructorId => 0x7488ce5b;
    public int Hash { get; set; }

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
        Hash = br.ReadInt32();
    }
}
