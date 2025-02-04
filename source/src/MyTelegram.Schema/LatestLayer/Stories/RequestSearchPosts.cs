﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.searchPosts" />
///</summary>
[TlObject(0x6cea116a)]
public sealed class RequestSearchPosts : IRequest<MyTelegram.Schema.Stories.IFoundStories>
{
    public uint ConstructorId => 0x6cea116a;
    public BitArray Flags { get; set; } = new BitArray(32);
    public string? Hashtag { get; set; }
    public MyTelegram.Schema.IMediaArea? Area { get; set; }
    public string Offset { get; set; }
    public int Limit { get; set; }

    public void ComputeFlag()
    {
        if (Hashtag != null) { Flags[0] = true; }
        if (Area != null) { Flags[1] = true; }

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Flags);
        if (Flags[0]) { writer.Write(Hashtag); }
        if (Flags[1]) { writer.Write(Area); }
        writer.Write(Offset);
        writer.Write(Limit);
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Flags = reader.ReadBitArray();
        if (Flags[0]) { Hashtag = reader.ReadString(); }
        if (Flags[1]) { Area = reader.Read<MyTelegram.Schema.IMediaArea>(); }
        Offset = reader.ReadString();
        Limit = reader.ReadInt32();
    }
}
