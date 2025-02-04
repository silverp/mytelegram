﻿namespace MyTelegram.Domain.Sagas.Events;

public class UpdatePinnedBoxPtsCompletedEvent(
    long peerId,
    int pts) : AggregateEvent<UpdatePinnedMessageSaga, UpdatePinnedMessageSagaId>
{
    public long PeerId { get; } = peerId;
    public int Pts { get; } = pts;
}
