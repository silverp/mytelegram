﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/starsTransactionPeerPremiumBot" />
///</summary>
[TlObject(0x250dbaf8)]
public sealed class TStarsTransactionPeerPremiumBot : IStarsTransactionPeer
{
    public uint ConstructorId => 0x250dbaf8;


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