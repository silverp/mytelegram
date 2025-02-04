﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.starsRevenueAdsAccountUrl" />
///</summary>
[TlObject(0x394e7f21)]
public sealed class TStarsRevenueAdsAccountUrl : IStarsRevenueAdsAccountUrl
{
    public uint ConstructorId => 0x394e7f21;
    public string Url { get; set; }

    public void ComputeFlag()
    {

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Url);
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Url = reader.ReadString();
    }
}