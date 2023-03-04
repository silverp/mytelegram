﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Photos;

///<summary>
///See <a href="https://core.telegram.org/method/photos.uploadContactProfilePhoto" />
///</summary>
[TlObject(0xe14c4a71)]
public sealed class RequestUploadContactProfilePhoto : IRequest<MyTelegram.Schema.Photos.IPhoto>
{
    public uint ConstructorId => 0xe14c4a71;
    public BitArray Flags { get; set; } = new BitArray(32);
    public bool Suggest { get; set; }
    public bool Save { get; set; }

    ///<summary>
    ///See <a href="https://core.telegram.org/type/InputUser" />
    ///</summary>
    public MyTelegram.Schema.IInputUser UserId { get; set; }

    ///<summary>
    ///See <a href="https://core.telegram.org/type/InputFile" />
    ///</summary>
    public MyTelegram.Schema.IInputFile? File { get; set; }

    ///<summary>
    ///See <a href="https://core.telegram.org/type/InputFile" />
    ///</summary>
    public MyTelegram.Schema.IInputFile? Video { get; set; }
    public double? VideoStartTs { get; set; }

    ///<summary>
    ///See <a href="https://core.telegram.org/type/VideoSize" />
    ///</summary>
    public MyTelegram.Schema.IVideoSize? VideoEmojiMarkup { get; set; }

    public void ComputeFlag()
    {
        if (Suggest) { Flags[3] = true; }
        if (Save) { Flags[4] = true; }
        if (File != null) { Flags[0] = true; }
        if (Video != null) { Flags[1] = true; }
        if (VideoStartTs>0) { Flags[2] = true; }
        if (VideoEmojiMarkup != null) { Flags[5] = true; }
    }

    public void Serialize(BinaryWriter bw)
    {
        ComputeFlag();
        bw.Write(ConstructorId);
        bw.Serialize(Flags);
        UserId.Serialize(bw);
        if (Flags[0]) { File.Serialize(bw); }
        if (Flags[1]) { Video.Serialize(bw); }
        if (Flags[2]) { bw.Serialize(VideoStartTs.Value); }
        if (Flags[5]) { VideoEmojiMarkup.Serialize(bw); }
    }

    public void Deserialize(BinaryReader br)
    {
        Flags = br.Deserialize<BitArray>();
        if (Flags[3]) { Suggest = true; }
        if (Flags[4]) { Save = true; }
        UserId = br.Deserialize<MyTelegram.Schema.IInputUser>();
        if (Flags[0]) { File = br.Deserialize<MyTelegram.Schema.IInputFile>(); }
        if (Flags[1]) { Video = br.Deserialize<MyTelegram.Schema.IInputFile>(); }
        if (Flags[2]) { VideoStartTs = br.ReadDouble(); }
        if (Flags[5]) { VideoEmojiMarkup = br.Deserialize<MyTelegram.Schema.IVideoSize>(); }
    }
}
