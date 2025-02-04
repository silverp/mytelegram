﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/method/account.createBusinessChatLink" />
///</summary>
[TlObject(0x8851e68e)]
public sealed class RequestCreateBusinessChatLink : IRequest<MyTelegram.Schema.IBusinessChatLink>
{
    public uint ConstructorId => 0x8851e68e;
    public MyTelegram.Schema.IInputBusinessChatLink Link { get; set; }

    public void ComputeFlag()
    {

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Link);
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Link = reader.Read<MyTelegram.Schema.IInputBusinessChatLink>();
    }
}
