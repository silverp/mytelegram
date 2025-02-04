﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Channels;

///<summary>
/// Update the <a href="https://corefork.telegram.org/api/colors">accent color and background custom emoji »</a> of a channel.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOOSTS_REQUIRED The specified channel must first be <a href="https://corefork.telegram.org/api/boost">boosted by its users</a> in order to perform this action.
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// See <a href="https://corefork.telegram.org/method/channels.updateColor" />
///</summary>
[TlObject(0xd8aa3671)]
public sealed class RequestUpdateColor : IRequest<MyTelegram.Schema.IUpdates>
{
    public uint ConstructorId => 0xd8aa3671;
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    public BitArray Flags { get; set; } = new BitArray(32);

    ///<summary>
    /// Whether to change the accent color emoji pattern of the profile page; otherwise, the accent color and emoji pattern of messages will be changed.
    /// See <a href="https://corefork.telegram.org/type/true" />
    ///</summary>
    public bool ForProfile { get; set; }

    ///<summary>
    /// Channel whose accent color should be changed.
    /// See <a href="https://corefork.telegram.org/type/InputChannel" />
    ///</summary>
    public MyTelegram.Schema.IInputChannel Channel { get; set; }

    ///<summary>
    /// <a href="https://corefork.telegram.org/api/colors">ID of the accent color palette »</a> to use (not RGB24, see <a href="https://corefork.telegram.org/api/colors">here »</a> for more info); if not set, the default palette is used.
    ///</summary>
    public int? Color { get; set; }

    ///<summary>
    /// Custom emoji ID used in the accent color pattern.
    ///</summary>
    public long? BackgroundEmojiId { get; set; }

    public void ComputeFlag()
    {
        if (ForProfile) { Flags[1] = true; }
        if (/*Color != 0 && */Color.HasValue) { Flags[2] = true; }
        if (/*BackgroundEmojiId != 0 &&*/ BackgroundEmojiId.HasValue) { Flags[0] = true; }
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(Flags);
        writer.Write(Channel);
        if (Flags[2]) { writer.Write(Color.Value); }
        if (Flags[0]) { writer.Write(BackgroundEmojiId.Value); }
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        Flags = reader.ReadBitArray();
        if (Flags[1]) { ForProfile = true; }
        Channel = reader.Read<MyTelegram.Schema.IInputChannel>();
        if (Flags[2]) { Color = reader.ReadInt32(); }
        if (Flags[0]) { BackgroundEmojiId = reader.ReadInt64(); }
    }
}
