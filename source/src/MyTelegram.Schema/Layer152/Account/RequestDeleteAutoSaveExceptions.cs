﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
///See <a href="https://core.telegram.org/method/account.deleteAutoSaveExceptions" />
///</summary>
[TlObject(0x53bc0020)]
public sealed class RequestDeleteAutoSaveExceptions : IRequest<IBool>
{
    public uint ConstructorId => 0x53bc0020;

    public void ComputeFlag()
    {

    }

    public void Serialize(BinaryWriter bw)
    {
        ComputeFlag();
        bw.Write(ConstructorId);

    }

    public void Deserialize(BinaryReader br)
    {

    }
}
