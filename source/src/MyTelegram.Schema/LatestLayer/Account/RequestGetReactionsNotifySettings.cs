﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/method/account.getReactionsNotifySettings" />
///</summary>
[TlObject(0x6dd654c)]
public sealed class RequestGetReactionsNotifySettings : IRequest<MyTelegram.Schema.IReactionsNotifySettings>
{
    public uint ConstructorId => 0x6dd654c;

    public void ComputeFlag()
    {

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);

    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {

    }
}
