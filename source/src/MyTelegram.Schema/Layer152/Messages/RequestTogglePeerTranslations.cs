﻿// <auto-generated/>
// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
///See <a href="https://core.telegram.org/method/messages.togglePeerTranslations" />
///</summary>
[TlObject(0xe47cb579)]
public sealed class RequestTogglePeerTranslations : IRequest<IBool>
{
    public uint ConstructorId => 0xe47cb579;
    public BitArray Flags { get; set; } = new BitArray(32);
    public bool Disabled { get; set; }

    ///<summary>
    ///See <a href="https://core.telegram.org/type/InputPeer" />
    ///</summary>
    public MyTelegram.Schema.IInputPeer Peer { get; set; }

    public void ComputeFlag()
    {
        if (Disabled) { Flags[0] = true; }

    }

    public void Serialize(BinaryWriter bw)
    {
        ComputeFlag();
        bw.Write(ConstructorId);
        bw.Serialize(Flags);
        Peer.Serialize(bw);
    }

    public void Deserialize(BinaryReader br)
    {
        Flags = br.Deserialize<BitArray>();
        if (Flags[0]) { Disabled = true; }
        Peer = br.Deserialize<MyTelegram.Schema.IInputPeer>();
    }
}
