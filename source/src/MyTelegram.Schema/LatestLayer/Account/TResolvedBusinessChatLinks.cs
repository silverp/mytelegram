﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/account.resolvedBusinessChatLinks" />
///</summary>
[TlObject(0x9a23af21)]
public sealed class TResolvedBusinessChatLinks : IResolvedBusinessChatLinks
{
    public uint ConstructorId => 0x9a23af21;
    public BitArray Flags { get; set; } = new BitArray(32);
    public MyTelegram.Schema.IPeer Peer { get; set; }
    public string Message { get; set; }
    public TVector<MyTelegram.Schema.IMessageEntity>? Entities { get; set; }
    public TVector<MyTelegram.Schema.IChat> Chats { get; set; }
    public TVector<MyTelegram.Schema.IUser> Users { get; set; }

    public void ComputeFlag()
    {
        if (Entities?.Count > 0) { Flags[0] = true; }

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Flags);
        writer.Write(Peer);
        writer.Write(Message);
        if (Flags[0]) { writer.Write(Entities); }
        writer.Write(Chats);
        writer.Write(Users);
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Flags = reader.ReadBitArray();
        Peer = reader.Read<MyTelegram.Schema.IPeer>();
        Message = reader.ReadString();
        if (Flags[0]) { Entities = reader.Read<TVector<MyTelegram.Schema.IMessageEntity>>(); }
        Chats = reader.Read<TVector<MyTelegram.Schema.IChat>>();
        Users = reader.Read<TVector<MyTelegram.Schema.IUser>>();
    }
}