﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/reactionsNotifySettings" />
///</summary>
[TlObject(0x56e34970)]
public sealed class TReactionsNotifySettings : IReactionsNotifySettings
{
    public uint ConstructorId => 0x56e34970;
    public BitArray Flags { get; set; } = new BitArray(32);
    public MyTelegram.Schema.IReactionNotificationsFrom? MessagesNotifyFrom { get; set; }
    public MyTelegram.Schema.IReactionNotificationsFrom? StoriesNotifyFrom { get; set; }
    public MyTelegram.Schema.INotificationSound Sound { get; set; }
    public bool ShowPreviews { get; set; }

    public void ComputeFlag()
    {
        if (MessagesNotifyFrom != null) { Flags[0] = true; }
        if (StoriesNotifyFrom != null) { Flags[1] = true; }

    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Flags);
        if (Flags[0]) { writer.Write(MessagesNotifyFrom); }
        if (Flags[1]) { writer.Write(StoriesNotifyFrom); }
        writer.Write(Sound);
        writer.Write(ShowPreviews);
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Flags = reader.ReadBitArray();
        if (Flags[0]) { MessagesNotifyFrom = reader.Read<MyTelegram.Schema.IReactionNotificationsFrom>(); }
        if (Flags[1]) { StoriesNotifyFrom = reader.Read<MyTelegram.Schema.IReactionNotificationsFrom>(); }
        Sound = reader.Read<MyTelegram.Schema.INotificationSound>();
        ShowPreviews = reader.Read();
    }
}