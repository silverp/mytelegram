﻿namespace MyTelegram.Domain.Sagas.Events;

public class ClearHistoryPtsIncrementedEvent(
    long peerId,
    int messageId,
    int pts) : AggregateEvent<ClearHistorySaga, ClearHistorySagaId>
{
    public int MessageId { get; } = messageId;

    public long PeerId { get; } = peerId;
    public int Pts { get; } = pts;
}
