﻿using MyTelegram.Messenger.Services.Caching;
using MyTelegram.Services.Extensions;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

public class UpdateMessagePinDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IQueryProcessor queryProcessor,
    IResponseCacheAppService responseCacheAppService) : DomainEventHandlerBase(objectMessageSender, commandBus,
    idGenerator, ackCacheService, responseCacheAppService),
    ISubscribeSynchronousTo<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId, UpdateMessagePinnedCompletedSagaEvent>,
    ISubscribeSynchronousTo<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId, UpdateParticipantMessagePinnedCompletedSagaEvent>,
    ISubscribeSynchronousTo<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllMessagesCompletedSagaEvent>,
    ISubscribeSynchronousTo<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllParticipantMessagesCompletedSagaEvent>
{
    public async Task HandleAsync(IDomainEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId, UpdateMessagePinnedCompletedSagaEvent> domainEvent, CancellationToken cancellationToken)
    {
        var update = CreateUpdate(domainEvent.AggregateEvent.Pts, domainEvent.AggregateEvent.PtsCount,
            domainEvent.AggregateEvent.MessageIds,
            domainEvent.AggregateEvent.ToPeer,
            domainEvent.AggregateEvent.Pinned
        );
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Chats = new(),
            Users = new(),
            Date = DateTime.UtcNow.ToTimestamp(),
        };
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, updates);

        var toPeer = domainEvent.AggregateEvent.ToPeer;
        if (toPeer.PeerType == PeerType.User)
        {
            toPeer = toPeer with { PeerId = domainEvent.AggregateEvent.RequestInfo.UserId };
        }

        await PushUpdatesToPeerAsync(toPeer, updates,
            excludeAuthKeyId: domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId,
            pts: domainEvent.AggregateEvent.Pts);
    }

    public Task HandleAsync(IDomainEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId, UpdateParticipantMessagePinnedCompletedSagaEvent> domainEvent, CancellationToken cancellationToken)
    {
        var updates = CreateUpdates(domainEvent.AggregateEvent.Pts, domainEvent.AggregateEvent.PtsCount,
            domainEvent.AggregateEvent.MessageIds, domainEvent.AggregateEvent.ToPeer,
            domainEvent.AggregateEvent.Pinned
            );
        // Console.WriteLine($"################################ Push update pinned message:{domainEvent.AggregateEvent.OwnerPeerId},pts={domainEvent.AggregateEvent.Pts}");
        return PushUpdatesToPeerAsync(domainEvent.AggregateEvent.OwnerPeerId.ToUserPeer(), updates,
           pts: domainEvent.AggregateEvent.Pts);
    }

    public async Task HandleAsync(IDomainEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllMessagesCompletedSagaEvent> domainEvent, CancellationToken cancellationToken)
    {
        var r = new TAffectedHistory
        {
            Pts = domainEvent.AggregateEvent.Pts,
            PtsCount = domainEvent.AggregateEvent.PtsCount,
            Offset = domainEvent.AggregateEvent.Offset,
        };
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, r, pts: r.Pts);

        var updates = CreateUpdates(domainEvent.AggregateEvent.Pts, domainEvent.AggregateEvent.PtsCount,
            domainEvent.AggregateEvent.MessageIds, domainEvent.AggregateEvent.ToPeer, false);

        var toPeer = domainEvent.AggregateEvent.ToPeer;
        if (toPeer.PeerType == PeerType.User)
        {
            toPeer = toPeer with { PeerId = domainEvent.AggregateEvent.RequestInfo.UserId };
        }

        await PushUpdatesToPeerAsync(toPeer, updates,
            domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId, pts: domainEvent.AggregateEvent.Pts);
    }

    public Task HandleAsync(IDomainEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllParticipantMessagesCompletedSagaEvent> domainEvent, CancellationToken cancellationToken)
    {
        var updates = CreateUpdates(domainEvent.AggregateEvent.Pts, domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds,
            domainEvent.AggregateEvent.ToPeer,
            false
            );

        return PushUpdatesToPeerAsync(domainEvent.AggregateEvent.OwnerPeerId.ToUserPeer(), updates, pts: domainEvent.AggregateEvent.Pts);
    }

    //private IUpdates CreateUnPinAllMessagesUpdates(int pts, int ptsCount, List<int> messageIds, Peer toPeer)
    //{
    //    IUpdate update;
    //    if (toPeer.PeerType == PeerType.Channel)
    //    {
    //        update = new TUpdatePinnedChannelMessages
    //        {
    //            Pinned = false,
    //            Pts = pts,
    //            PtsCount = ptsCount,
    //            Messages = new TVector<int>(messageIds),
    //            ChannelId = toPeer.PeerId
    //        };
    //    }
    //    else
    //    {
    //        update = new TUpdatePinnedMessages
    //        {
    //            Pinned = false,
    //            Pts = pts,
    //            PtsCount = ptsCount,
    //            Messages = new TVector<int>(messageIds),
    //            Peer = toPeer.ToPeer()
    //        };
    //    }
    //    var updates = new TUpdateShort
    //    {
    //        Update = update,
    //        Date = DateTime.UtcNow.ToTimestamp()
    //    };

    //    return updates;
    //}

    private IUpdates CreateUpdates(int pts, int ptsCount, List<int> messageIds, Peer toPeer, bool pinned)
    {
        var update = CreateUpdate(pts, ptsCount, messageIds, toPeer, pinned);
        var updates = new TUpdates()
        {
            Updates = new TVector<IUpdate>(update),
            Users = [],
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp()
        };

        return updates;
    }

    private IUpdate CreateUpdate(int pts, int ptsCount, List<int> messageIds, Peer toPeer, bool pinned)
    {
        IUpdate update;
        if (toPeer.PeerType == PeerType.Channel)
        {
            update = new TUpdatePinnedChannelMessages
            {
                Pinned = pinned,
                Pts = pts,
                PtsCount = ptsCount,
                Messages = new TVector<int>(messageIds),
                ChannelId = toPeer.PeerId
            };
        }
        else
        {
            update = new TUpdatePinnedMessages
            {
                Pinned = pinned,
                Pts = pts,
                PtsCount = ptsCount,
                Messages = new TVector<int>(messageIds),
                Peer = toPeer.ToPeer()
            };
        }

        return update;
    }
}